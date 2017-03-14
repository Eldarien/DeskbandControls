using Deskband.Configuration;
using Deskband.Core.Common;
using Deskband.Core.Interfaces;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Deskband.UI
{
    public class TooltipProvider : ITooltipProvider
    {
        readonly Band _band;
        readonly IConfigurationProvider _config;
        readonly ModuleContainer _moduleContainer;
        readonly ISizeProvider _sizeProvider;

        public TooltipProvider(Band band, IConfigurationProvider config, ModuleContainer moduleContainer, ISizeProvider sizeProvider)
        {
            _band = band;
            _config = config;
            _moduleContainer = moduleContainer;
            _sizeProvider = sizeProvider;
        }

        private Guid _moduleId;
        private TooltipInfo _ti;
        private TooltipForm _form;
        private int _borderDelta;

        public void ShowTooltip(Guid moduleId, TooltipInfo ti)
        {
            _moduleId = moduleId;
            _ti = ti;

            _form = new TooltipForm();
            _form.TopMost = true;
            _form.MinimizeBox = false;
            _form.MaximizeBox = false;
            _form.ControlBox = false;
            _form.ShowInTaskbar = false;
            var borderStyle = Environment.OSVersion.Version.Major < 10
                ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle; // win 10 has no Aero and sizeble border looks ugly
            _form.FormBorderStyle = ti.UseBorderlessWindow ? FormBorderStyle.None : borderStyle;
            _form.Text = null;
            _form.BackColor = ti.BackgroundColor;

            _form.Width = _sizeProvider.MakeValue(_ti.Width);
            _form.Height = _sizeProvider.MakeValue(_ti.Height);
            _borderDelta = _form.Width - _form.ClientRectangle.Width;
            _form.Width = _form.Width + _borderDelta;
            _form.Height = _form.Height + _borderDelta;

            // Disable resizing
            _form.MinimumSize = _form.Size;
            _form.MaximumSize = _form.Size;

            SetPosition(_ti.Rect);

            _ti.CreateAction(_form);
            _form.Show();
        }

        private void SetPosition(Rectangle rc)
        {
            var position = new Point(rc.Left + rc.Width / 2, rc.Top + rc.Height / 2);

            var screen = Screen.FromControl(_form);
            var cfg = _config.GetConfiguration(Guid.Empty, ConfigurationModel.Default);
            var taskbarInfo = _band.GetTaskbarSizeInfo();
            var layoutMode = cfg.GeneralSettings.DisplayMode == DisplayMode.Deskband ? taskbarInfo.Mode : cfg.FloatingWindowSettings.Mode;
            if (layoutMode == LayoutMode.Horizontal || cfg.GeneralSettings.DisplayMode == DisplayMode.FloatingWindow)
            {
                // Horizontal deskband || floating window

                if (cfg.GeneralSettings.DisplayMode == DisplayMode.Deskband)
                {
                    // horizontal center is module center
                    _form.Left = position.X - _form.Width / 2;
                    int maxLeft = screen.Bounds.Width - _form.Width;
                    if (_form.Left > maxLeft) _form.Left = maxLeft;
                   
                    _form.Top = taskbarInfo.Rect.Top <= 0
                        ? taskbarInfo.Rect.Height
                        : screen.Bounds.Height - taskbarInfo.Rect.Height - _form.Height;
                }
                else
                {
                    // horizontal center is floating window center
                    _form.Left = _moduleContainer.GetScreenRectangle().Left + _moduleContainer.Width / 2 - _form.Width / 2;
                    var top = cfg.FloatingWindowSettings.Y - _form.Height;
                    _form.Top = top < screen.WorkingArea.Top
                        ? cfg.FloatingWindowSettings.Y + _moduleContainer.Height
                        : top;
                }
            }
            else
            {
                // Vertical deskband

                _form.Left = taskbarInfo.Rect.Left <= 0
                    ? taskbarInfo.Rect.Width
                    : screen.Bounds.Width - taskbarInfo.Rect.Width - _form.Width;
                

                _form.Top = position.Y - _form.Height / 2;
                int maxTop = screen.Bounds.Height - _form.Height;
                if (_form.Top > maxTop) _form.Top = maxTop;
            }
        }

        public void HandleMove(Rectangle rc)
        {
            if (_form != null)
            {
                SetPosition(rc);
            }
        }

        private bool _cursorOverForm;

        public void RequestHideTooltip()
        {
            if (_cursorOverForm) return;

            DoHide();
        }

        private readonly object _dohide_locker = new object();
        private void DoHide()
        {
            lock (_dohide_locker)
            {
                if (_form != null)
                {
                    _form.Close();
                    _form = null;
                }
                if (_ti != null)
                {
                    _ti.DestroyAction?.Invoke();
                    _ti = null;
                }
            }
        }

        public void HandleMousePoint(Point globalPoint)
        {
            if (_form != null)
            {
                var bounds = new Rectangle(_form.Bounds.Location, _form.Bounds.Size);
                bounds.Inflate(2, 2);
                _cursorOverForm = bounds.Contains(globalPoint) || _moduleContainer.LocateModuleAtPoint(_moduleContainer.PointToClient(globalPoint)) == _moduleId;
                if (!_cursorOverForm)
                {
                    DoHide();
                }
            }
        }
    }
}
