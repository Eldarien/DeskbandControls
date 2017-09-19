using Deskband.Core.WinApi;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Deskband.Core.Common;

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
        public bool EnableStubImage { get; set; }

        public new Image Image
        {
            get { return base.Image; }
            private set { base.Image = value; }
        }

        private Image _stubImage = ImageHelpers.Empty;
        public void SetStubImage(Image image)
        {
            if (_stubImage != null && _stubImage != ImageHelpers.Empty)
            {
                _stubImage.Dispose();
                _stubImage = null;
            }

            _stubImage = image != null
                  ? ImageHelpers.HQResize(image, Width, Height, PreserveAspectRatio)
                  : ImageHelpers.Empty;
        }

        public void SetImage(Image image)
        {
            var oldImage = Image;
            Image = image != null
                  ? ImageHelpers.HQResize(image, Width, Height, PreserveAspectRatio)
                  : (EnableStubImage
                        ? (_stubImage != null ? new Bitmap(_stubImage) : ImageHelpers.Empty)
                        : ImageHelpers.Empty);

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
            
            if (_stubImage != null && _stubImage != ImageHelpers.Empty)
            {
                _stubImage.Dispose();
                _stubImage = null;
            }

            base.Dispose(disposing);
        }
    }
}