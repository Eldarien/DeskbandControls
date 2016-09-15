using Deskband.Core.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Deskband.Configuration
{
    public class FloatingWindowSettings
    {
        [Category("Position")]
        public int X { get; set; }

        [Category("Position")]
        public int Y { get; set; }

        public double Opacity { get; set; }
        public Color Color { get; set; }

        [DisplayName("Use Background Image"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool UseBackgroundImage { get; set; }

        [DisplayName("Background Image Path")]
        public string BackgroundImagePath { get; set; }

        [DisplayName("Stretch Background Image"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool StretchBackgroundImage { get; set; }

        [DisplayName("Use Transparency Key"), TypeConverter(typeof(YesNoBooleanConverter))]
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