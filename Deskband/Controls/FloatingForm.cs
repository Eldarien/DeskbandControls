using Deskband.Common;
using Deskband.Common.Extensions;
using Deskband.Native;
using Deskband.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband.Controls
{
    public partial class FloatingForm : Form
    {
        private bool dragging;
        private Point dragAt = Point.Empty;

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

        public FloatingForm()
        {
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

        private void FloatingForm_Load(object sender, EventArgs e)
        {
            WinApi.SetWindowPos(Handle, new IntPtr(-1), 0, 0, 0, 0, WinApi.SWP_NOMOVE | WinApi.SWP_NOSIZE);
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
                SettingsManager.Instance.Settings.FloatingWindow.X = Location.X;
                SettingsManager.Instance.Settings.FloatingWindow.Y = Location.Y;
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

        public void LoadSettings()
        {
            var fwSettings = SettingsManager.Instance.Settings.FloatingWindow;
            Location = new Point(fwSettings.X, fwSettings.Y);
            Size = new Size(fwSettings.Width, fwSettings.Height);
            Opacity = fwSettings.Opacity;
            BackColor = fwSettings.Color.AsDrawingColor();

            if (fwSettings.UseBackgroundImage)
            {
                BackgroundImage = ImageHelpers.GetImageFromFile(fwSettings.BackgroundImage);
                BackgroundImageLayout = fwSettings.StretchBackgroundImage ? ImageLayout.Stretch : ImageLayout.None;
            }
            else
            {
                BackgroundImage = ImageHelpers.Empty;
                BackgroundImageLayout = ImageLayout.None;
            }

            TransparencyKey = fwSettings.UseTransparencyKey ? Color.Fuchsia : Color.Empty;
        }
    }
}