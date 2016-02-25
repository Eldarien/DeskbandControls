using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Deskband.Configuration
{
    public class FloatingWindowSettings
    {
        public int X { get; set; }
        public int Y { get; set; }
        public double Opacity { get; set; }
        public Color Color { get; set; }
        public bool UseBackgroundImage { get; set; }
        public string BackgroundImagePath { get; set; }
        public bool StretchBackgroundImage { get; set; }
        public bool UseTransparencyKey { get; set; }

        public static FloatingWindowSettings GetDefault()
        {
            return new FloatingWindowSettings
            {
                Opacity = 1.0,
                Color = Color.Tan
            };
        }
    }
}
