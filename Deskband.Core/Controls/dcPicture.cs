using Deskband.Core.Common;
using Deskband.Core.WinApi;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Deskband.Core.Controls
{
    public class dcPicture : PictureBox
    {
        public dcPicture()
        {
            SizeMode = PictureBoxSizeMode.CenterImage;
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

        public bool PreserveAspectRatio { get; set; }

        public new Image Image
        {
            get { return base.Image; }
            private set { base.Image = value; }
        }

        public void SetImage(Image image)
        {
            var oldImage = Image;
            Image = image != null
                  ? ImageHelpers.HQResize(image, Width, Height, PreserveAspectRatio)
                  : ImageHelpers.Empty;

            if (oldImage != null && oldImage != ImageHelpers.Empty)
            {
                oldImage.Dispose();
                oldImage = null;
            }
        }

        protected override void Dispose(bool disposing)
        {
            var oldImage = Image;
            Image = ImageHelpers.Empty;

            if (oldImage != null && oldImage != ImageHelpers.Empty)
            {
                oldImage.Dispose();
                oldImage = null;
            }
            
            base.Dispose(disposing);
        }
    }
}