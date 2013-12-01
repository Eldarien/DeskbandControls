using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Deskband.Common.Extensions
{
    public static class MediaColorExtensions
    {
        public static System.Drawing.Color AsDrawingColor(this Color color)
        {
            return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}