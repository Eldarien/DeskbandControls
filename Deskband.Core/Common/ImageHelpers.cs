using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Deskband.Core.Properties;

namespace Deskband.Core.Common
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
                    // from a PNG file image created by CopyImageToByteArray() - the dpi value "drifts"
                    // slightly away from the nominal integer value
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

        public static Image HQResize(Image image, int width, int height, bool preserveAspect)
        {
            if (preserveAspect)
            {
                double originalRatio = (double)image.Width / (double)image.Height;
                double resultRatio = (double)width / (double)height;
                if (originalRatio > resultRatio)
                    height = (int)(width / originalRatio);
                else
                    width = (int)(height * originalRatio);
            }

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

        public static Image Colorize(Image image, Color color)
        {
            if (color == Color.Transparent || color == Color.White ||
                color.A == 255 && color.R == 0 && color.G == 0 && color.B == 0)
                return image;

            var result = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            result.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            float R = color.R / 255;
            float G = color.G / 255;
            float B = color.B / 255;

            float[][] coeff = {
                new float[] { R, 0, 0, 0, 0 },
                new float[] { 0, G, 0, 0, 0 },
                new float[] { 0, 0, B, 0, 0 },
                new float[] { 0, 0, 0, 1, 0 },
                new float[] { 0, 0, 0, 0, 1 }};

            ColorMatrix cm = new ColorMatrix(coeff);
            using (var ia = new ImageAttributes())
            {
                ia.SetColorMatrix(new ColorMatrix(coeff));

                using (var g = Graphics.FromImage(result))
                {
                    g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                        0, 0, image.Width, image.Height, GraphicsUnit.Pixel, ia);
                }
            }

            return result;
        }
    }
}