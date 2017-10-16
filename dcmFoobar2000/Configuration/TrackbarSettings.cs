using Deskband.Core.Configuration;
using System.ComponentModel;
using System.Drawing;

namespace dcmFoobar2000.Configuration
{
    public class TrackbarSettings
    {
        public TrackbarSettings()
        {
            Color = Color.White;
        }

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

        [Category("Colors"), TypeConverter(typeof(ColorHexConverter))]
        public Color Color { get; set; }

        [Category("Colors"), DisplayName("Background Color"), TypeConverter(typeof(ColorHexConverter))]
        public Color BackgroundColor { get; set; } = Color.Transparent;

        [DisplayName("Hide Borders"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool HideBorders { get; set; }

        [Category("Padding"), DisplayName("Top")]
        public int PaddingTop { get; set; }

        [Category("Padding"), DisplayName("Bottom")]
        public int PaddingBottom { get; set; }
    }
}