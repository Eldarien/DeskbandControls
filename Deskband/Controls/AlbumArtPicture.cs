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

        public new Image Image
        {
            get { return base.Image; }
            set
            {
                var image = value;
                if (image == null)
                {
                    image = ImageHelpers.GetImageFromFile(StubImagePath);
                    if (image == ImageHelpers.Empty)
                        image = Resources.NoCoverArt;
                }
                base.Image = ImageHelpers.HQResize(image, Width, Height);
            }
        }

        public static AlbumArtPicture Create(AlbumArtModel model)
        {
            var pic = new AlbumArtPicture();

            pic.Visible = model.Visible;
            pic.Location = new Point(model.X, model.Y);
            pic.Size = new Size(model.Width, model.Height);
            pic.StubImagePath = model.StubImagePath;
            pic.Image = null;

            return pic;
        }
    }
}