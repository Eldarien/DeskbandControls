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
            _form.FormBorderStyle = ti.UseBorderlessWindow ? FormBorderStyle.None : FormBorderStyle.Sizable;
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
            var layoutMode = cfg.GeneralSettings.DisplayMode == DisplayMode.Deskband ? _band.GetTaskbarSizeInfo().Mode : cfg.FloatingWindowSettings.Mode;
            if (layoutMode == LayoutMode.Horizontal || cfg.GeneralSettings.DisplayMode == DisplayMode.FloatingWindow)
            {
                // Horizontal deskband || floating window

                if (cfg.GeneralSettings.DisplayMode == DisplayMode.Deskband)
                {
                    // horizontal center is module center
                    _form.Left = position.X - _form.Width / 2;
                    _form.Top = screen.WorkingArea.Top == 0
                        ? screen.WorkingArea.Height - _form.Height
                        : screen.WorkingArea.Top;
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
                _form.Left = screen.WorkingArea.Left == 0
                    ? screen.WorkingArea.Right - _form.Width
                    : screen.WorkingArea.Left;
                _form.Top = position.Y - _form.Height / 2;
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

            if (_form != null)
            {
                DoHide();
            }
        }

        private void DoHide()
        {
            if (_form != null)
            {
                _form.Hide();
                _form.Dispose();
                _form = null;

                _ti.DestroyAction?.Invoke();
                _ti = null;
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
