using Deskband.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband.AutoUpdater
{
    public partial class NotificationForm : Form
    {
        private Label infoMessageLabel;

        public string VersionTitle
        {
            set { infoMessageLabel.Text = value; }
        }

        public NotificationForm()
        {
            InitializeComponent();

            string fontName = Environment.OSVersion.Version.Major < 6 ? "Tahoma" : "Segoe UI";
            Font = new System.Drawing.Font(fontName, 9);

            BackColor = SystemColors.Info;
            ForeColor = SystemColors.InfoText;

            Width = 300;
            Height = 80;

            var wa = Screen.PrimaryScreen.WorkingArea;

            StartPosition = FormStartPosition.Manual;

            Left = wa.X + wa.Width - Width - 5;
            Top = wa.Y + wa.Height - Height - 5;

            infoMessageLabel = new Label();
            Controls.Add(infoMessageLabel);
            infoMessageLabel.Left = 5;
            infoMessageLabel.Top = 5;
            infoMessageLabel.Width = Width - 10;

            FormBorderStyle = FormBorderStyle.None;
            ControlBox = false;
            ShowInTaskbar = false;
            TopLevel = true;

            Load += NotificationForm_Load;
        }

        private void NotificationForm_Load(object sender, EventArgs e)
        {
            WinApi.SetWindowPos(Handle, new IntPtr(-1), 0, 0, 0, 0, WinApi.SWP_NOMOVE | WinApi.SWP_NOSIZE);
        }

        //private void btnClose_Click(object sender, EventArgs e)
        //{
        //    Close();
        //}
    }
}