using Deskband.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband.Common
{
    public static class ImageHelpers
    {
        private static readonly ImageConverter _imageConverter = new ImageConverter();

        public static Bitmap Empty = Resources.Transparent_1x1;

        public static Bitmap GetImageFromByteArray(byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length == 0)
                return Empty;

            try
            {
                Bitmap bm = (Bitmap)_imageConverter.ConvertFrom(byteArray);

                if (bm != null && (bm.HorizontalResolution != (int)bm.HorizontalResolution ||
                                   bm.VerticalResolution != (int)bm.VerticalResolution))
                {
                    // Correct a strange glitch that has been observed in the test program when converting
                    //  from a PNG file image created by CopyImageToByteArray() - the dpi value "drifts"
                    //  slightly away from the nominal integer value
                    bm.SetResolution((int)(bm.HorizontalResolution + 0.5f),
                                     (int)(bm.VerticalResolution + 0.5f));
                }
                return bm;
            }
            catch
            {
            }
            return Empty;
        }

        public static Image GetImageFromFile(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
                return Empty;

            try
            {
                return GetImageFromByteArray(File.ReadAllBytes(fileName));
            }
            catch
            {
                //MessageBox.Show(String.Format("Unable to load image file:\n{0}\nAdditional information:\n{1}",
                //    fileName, ex.Message), "Deskband Controls", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return Empty;
        }

        public static Image HQResize(Image image, int width, int height)
        {
            var result = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var g = Graphics.FromImage(result))
            {
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.DrawImage(image, 0, 0, result.Width, result.Height);
            }

            return result;
        }
    }
}