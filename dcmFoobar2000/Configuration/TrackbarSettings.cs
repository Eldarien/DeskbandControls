using Deskband.Core.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace dcmFoobar2000.Configuration
{
    public class TrackbarSettings
    {
        [Category("Visibility"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool Visible { get; set; }

        [Category("Position")]
        public int X { get; set; }

        [Category("Position")]
        public int Y { get; set; }

        [Category("Size")]
        public int Width { get; set; }

        [Category("Size")]
        public int Heigth { get; set; }

        [Category("Colors")]
        public Color Color { get; set; }

        [Category("Colors"), DisplayName("Background Color")]
        public Color BackgroundColor { get; set; }

        [Category("Colors"), DisplayName("Use Background Color"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool UseBackgroundColor { get; set; }

        [DisplayName("Hide Borders"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool HideBorders { get; set; }
    }
}