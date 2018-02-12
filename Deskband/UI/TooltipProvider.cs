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
        readonly Timer _showTimer;

        public TooltipProvider(Band band, IConfigurationProvider config, ModuleContainer moduleContainer, ISizeProvider sizeProvider)
        {
            _band = band;
            _config = config;
            _moduleContainer = moduleContainer;
            _sizeProvider = sizeProvider;
            _showTimer = new Timer();
            _showTimer.Tick += ShowTimer_Tick;
        }

        private Guid _moduleId;
        private TooltipInfo _ti;
        private TooltipForm _form;
        private int _borderDelta;
        private bool _disabled;

        public void ShowTooltip(Guid moduleId, TooltipInfo ti)
        {
            if (_disabled) return;

            _moduleId = moduleId;
            _ti = ti;

            CreateForm();

            _showTimer.Interval = _ti.ShowDelay > 0 ? _ti.ShowDelay : 1;
            _showTimer.Enabled = true;
        }

        private void ShowTimer_Tick(object sender, EventArgs e)
        {
            _showTimer.Enabled = false;

            lock (_dohide_locker)
            {
                if (_ti == null) return;

                _form.Show();
            }
        }

        public void DisableTooltip()
        {
            _disabled = true;
            _showTimer.Enabled = false;
        }

        public void EnableTooltip()
        {
            _disabled = false;
        }

        private void CreateForm()
        {
            _form = new TooltipForm();
            _form.TopMost = true;
            _form.MinimizeBox = false;
            _form.MaximizeBox = false;
            _form.ControlBox = false;
            _form.ShowInTaskbar = false;
            var borderStyle = Environment.OSVersion.Version.Major < 10
                ? FormBorderStyle.Sizable : FormBorderStyle.FixedSingle; // win 10 has no Aero and sizeble border looks ugly
            _form.FormBorderStyle = _ti.UseBorderlessWindow ? FormBorderStyle.None : borderStyle;
            _form.Text = null;

            _form.BackColor = Color.FromArgb(0xFF, _ti.BackgroundColor.R, _ti.BackgroundColor.G, _ti.BackgroundColor.B);

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
        }

        private void SetPosition(Rectangle rc)
        {
            var cfg = _config.GetConfiguration(Guid.Empty, ConfigurationModel.Default);
            var position = new Point(rc.Left + rc.Width / 2, rc.Top + rc.Height / 2);
            var screen = Screen.FromControl(_form);
            var taskbarInfo = _band.GetTaskbarSizeInfo();
            var layoutMode = cfg.GeneralSettings.IsDeskband ? taskbarInfo.Mode : cfg.FloatingWindowSettings.Mode;
            if (layoutMode == LayoutMode.Horizontal || cfg.GeneralSettings.DisplayMode == DisplayMode.FloatingWindow)
            {
                // Horizontal deskband || floating window

                if (cfg.GeneralSettings.IsDeskband)
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
                if (_form.Top < 0) _form.Top = 0;
            }
        }

        public void HandleMove(Rectangle rc)
        {
            if (_form != null)
            {
                SetPosition(rc);
            }
        }

        private bool _needToKeepOpen;

        public void RequestHideTooltip()
        {
            if (_needToKeepOpen) return;

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
            bool mouseOverFrom = false;
            lock (_dohide_locker)
            {
                if (_form != null && _ti != null && _ti.KeepOpenOnMouseOver)
                {
                    var bounds = new Rectangle(_form.Bounds.Location, _form.Bounds.Size);
                    bounds.Inflate(2, 2);
                    mouseOverFrom = bounds.Contains(globalPoint);
                }

                _needToKeepOpen = mouseOverFrom || _ti != null && _moduleContainer.LocateModuleAtPoint(_moduleContainer.PointToClient(globalPoint)) == _moduleId;
            }
            if (!_needToKeepOpen)
            {
                _showTimer.Enabled = false;
                DoHide();
            }
        }
    }
}
