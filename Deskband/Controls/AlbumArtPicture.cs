using Deskband.Common;
using Deskband.Native;
using Deskband.Properties;
using Deskband.Settings.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband.Controls
{
    public class AlbumArtPicture : PictureBox
    {
        public AlbumArtPicture()
        {
            SizeMode = PictureBoxSizeMode.CenterImage;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WinApi.WM_NCHITTEST)
            {
                m.Result = (IntPtr)WinApi.HTTRANSPARENT;
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        public string StubImagePath { get; set; }

        public bool PreserveAspectRatio { get; set; }

        public new Image Image
        {
            get { return base.Image; }
            set { base.Image = ImageHelpers.HQResize(value, Width, Height, PreserveAspectRatio); }
        }

        public void SetImage(Image image, bool stub, bool doNotShowStub)
        {
            if (image == null)
            {
                stub = true;
                image = ImageHelpers.GetImageFromFile(StubImagePath);
                if (image == ImageHelpers.Empty)
                    image = Resources.NoCoverArt;
            }

            if (stub && doNotShowStub)
            {
                image = ImageHelpers.Empty;
            }

            Image = image;
        }

        public static AlbumArtPicture Create(AlbumArtModel model)
        {
            var pic = new AlbumArtPicture();

            pic.Visible = model.Visible;
            pic.Location = new Point(model.X, model.Y);
            pic.Size = new Size(model.Width, model.Height);
            pic.PreserveAspectRatio = model.PreserveAspectRatio;
            pic.StubImagePath = model.StubImagePath;
            pic.SetImage(null, true, model.DoNotShowStubImage);

            return pic;
        }
    }
}