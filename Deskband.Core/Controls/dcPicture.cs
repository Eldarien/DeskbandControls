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
            _stubImage = image != null
                  ? ImageHelpers.HQResize(image, Width, Height, PreserveAspectRatio)
                  : ImageHelpers.Empty;
        }

        public void SetImage(Image image)
        {
            Image = image != null
                  ? ImageHelpers.HQResize(image, Width, Height, PreserveAspectRatio)
                  : (EnableStubImage ? _stubImage : ImageHelpers.Empty);
        }

        //    if (image == null)
        //    {
        //        stub = true;
        //        image = ImageHelpers.GetImageFromFile(StubImagePath);
        //        if (image == ImageHelpers.Empty)
        //            image = Resources.NoCoverArt;
        //    }

        //    if (stub && doNotShowStub)
        //    {
        //        image = ImageHelpers.Empty;
        //    }

        //    Image = image;
        //}

        //public static dcPicture Create(AlbumArtModel model)
        //{
        //    var pic = new dcPicture();

        //    pic.Visible = model.Visible;
        //    pic.Location = new Point(model.X, model.Y);
        //    pic.Size = new Size(model.Width, model.Height);
        //    pic.PreserveAspectRatio = model.PreserveAspectRatio;
        //    pic.StubImagePath = model.StubImagePath;
        //    pic.SetImage(null, true, model.DoNotShowStubImage);

        //    return pic;
        //}
    }
}