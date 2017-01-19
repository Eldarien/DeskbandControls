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

        public TooltipProvider(Band band, IConfigurationProvider config, ModuleContainer moduleContainer)
        {
            _band = band;
            _config = config;
            _moduleContainer = moduleContainer;
        }

        private TooltipForm _form;

        public void ShowTooltip(Guid moduleId, TooltipInfo ti)
        {
            _form = new TooltipForm();
            _form.TopMost = true;
            _form.MinimizeBox = false;
            _form.MaximizeBox = false;
            _form.ControlBox = false;
            _form.ShowInTaskbar = false;
            _form.FormBorderStyle = ti.UseBorderlessWindow ? FormBorderStyle.None : FormBorderStyle.SizableToolWindow;
            _form.Text = null;
            _form.BackColor = ti.BackgroundColor;
            _form.Width = ti.Width;
            _form.Height = ti.Height;

            var screen = Screen.FromControl(_form);
            

            var cfg = _config.GetConfiguration(Guid.Empty, ConfigurationModel.Default);
            var layoutMode = cfg.GeneralSettings.DisplayMode == DisplayMode.Deskband ? _band.GetTaskbarSizeInfo().Mode : cfg.FloatingWindowSettings.Mode;

            if (layoutMode == LayoutMode.Horizontal || cfg.GeneralSettings.DisplayMode == DisplayMode.FloatingWindow)
            {
                // Horizontal deskband || floating window
                
                if (cfg.GeneralSettings.DisplayMode == DisplayMode.Deskband)
                {
                    // horizontal center is module center
                    _form.Left = ti.X - _form.Width / 2;
                    _form.Top = screen.WorkingArea.Top == 0
                        ? screen.WorkingArea.Height - _form.Height
                        : screen.WorkingArea.Top;
                }
                else
                {
                    // horizontal center is floating window center
                    _form.Left = _moduleContainer.GetScreenRectangle().Left + _moduleContainer.Width / 2 - ti.Width / 2;
                    var top = cfg.FloatingWindowSettings.Y - ti.Height;
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
                _form.Top = ti.Y - _form.Height / 2;
            }

            ti.DrawAction(_form);
            _form.Show();
        }

        public void HideTooltip()
        {
            if (_form != null)
            {
                _form.Hide();
                _form.Dispose();
                _form = null;
            }
        }
    }
}
