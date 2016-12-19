using Deskband.Core.Common;
using Deskband.Core.WinApi;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Deskband.UI
{
    public class ControlsContainer : UserControl
    {
        public int Offset { get; set; }
        public new Image BackgroundImage { get; set; }

        public ControlsContainer()
        {
            BackColor = Color.Transparent;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (!ImageHelpers.IsNullOrEmpty(BackgroundImage))
            {
                if (BackgroundImageLayout == ImageLayout.Stretch)
                    e.Graphics.DrawImage(BackgroundImage, ClientRectangle);
                else
                    e.Graphics.DrawImage(BackgroundImage, new Point(0, 0));
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WinApiTypes.WM_NCHITTEST)
            {
                m.Result = (IntPtr)WinApiTypes.HTTRANSPARENT;
            }
            else
            {
                base.WndProc(ref m);
            }
        }
    }
}
