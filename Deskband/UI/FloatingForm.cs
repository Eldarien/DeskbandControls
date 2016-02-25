using Deskband.Common;
using Deskband.Configuration;
using Deskband.Core.Interfaces;
using Deskband.Core.WinApi;
using Deskband.Extensions;
using Deskband.Settings;
using System;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband.UI
{
    public partial class FloatingForm : Form
    {
        private IConfigurationProvider _config;

        private bool dragging;
        private Point dragAt = Point.Empty;

        public FloatingForm(IConfigurationProvider config)
        {
            _config = config;
            InitializeComponent();

            FormBorderStyle = FormBorderStyle.None;

            //FormBorderStyle = FormBorderStyle.SizableToolWindow;
            ControlBox = false;

            ShowInTaskbar = false;
            TopLevel = true;

            Load += FloatingForm_Load;
            MouseDown += FloatingForm_MouseDown;
            MouseUp += FloatingForm_MouseUp;
            MouseMove += FloatingForm_MouseMove;
            Move += FloatingForm_Move;
        }

        public void Pick(Control control, int x, int y)
        {
            dragging = true;
            dragAt = new Point(x, y);
            control.Capture = true;
        }

        public void Drop(Control control)
        {
            dragging = false;
            control.Capture = false;
        }

        private void FloatingForm_Load(object sender, EventArgs e)
        {
            User32.SetWindowPos(Handle, new IntPtr(-1), 0, 0, 0, 0, WinApiTypes.SWP_NOMOVE | WinApiTypes.SWP_NOSIZE);
        }

        private void FloatingForm_MouseDown(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left)
            //{
            //    WinApi.ReleaseCapture();
            //    WinApi.SendMessage(Handle, WinApi.WM_NCLBUTTONDOWN, (IntPtr)WinApi.HT_CAPTION, IntPtr.Zero);
            //}
            Pick((Control)sender, e.X, e.Y);
        }

        private void FloatingForm_MouseUp(object sender, MouseEventArgs e)
        {
            Drop((Control)sender);
        }

        private void FloatingForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Left = e.X + Left - dragAt.X;
                Top = e.Y + Top - dragAt.Y;
            }
            else dragAt = new Point(e.X, e.Y);
        }

        private void FloatingForm_Move(object sender, EventArgs e)
        {
            if (Visible)
            {
                var cfg = _config.GetConfiguration<ConfigurationModel>(Band.ModuleId, null);
                if (cfg != null)
                {
                    cfg.FloatingWindowSettings.X = Location.X;
                    cfg.FloatingWindowSettings.Y = Location.Y;
                    _config.UpdateConfiguration(cfg);
                }
            }
        }

        //private void ApplyGlass()
        //{
        //    var mg = new WinApi.MARGINS();
        //    mg.m_Buttom = -1;
        //    mg.m_Left = -1;
        //    mg.m_Right = -1;
        //    mg.m_Top = -1;
        //    WinApi.DwmExtendFrameIntoClientArea(Handle, ref mg);

        //    BackColor = Color.Transparent;
        //    TransparencyKey = BackColor;
        //}

        public void ApplyConfiguration()
        {
            var cfg = _config.GetConfiguration<ConfigurationModel>(Band.ModuleId, null);

            Location = new Point(cfg.FloatingWindowSettings.X, cfg.FloatingWindowSettings.Y);
            Opacity = cfg.FloatingWindowSettings.Opacity;

            // Ignore Alpha part of color for form background to prevent crush
            var backColor = cfg.FloatingWindowSettings.Color; //fwSettings.Color.AsDrawingColor();
            BackColor = Color.FromArgb(0xFF, backColor.R, backColor.G, backColor.B);

            if (cfg.FloatingWindowSettings.UseBackgroundImage)
            {
                BackgroundImage = ImageHelpers.GetImageFromFile(cfg.FloatingWindowSettings.BackgroundImagePath);
                BackgroundImageLayout = cfg.FloatingWindowSettings.StretchBackgroundImage ? ImageLayout.Stretch : ImageLayout.None;
            }
            else
            {
                BackgroundImage = ImageHelpers.Empty;
                BackgroundImageLayout = ImageLayout.None;
            }

            TransparencyKey = cfg.FloatingWindowSettings.UseTransparencyKey ? Color.Fuchsia : Color.Empty;
        }
    }
}