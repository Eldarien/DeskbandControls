using Deskband.Core.Common;
using Deskband.Core.Configuration;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace Deskband.Configuration
{
    public class FloatingWindowSettings
    {
        [Category("Position")]
        public int X { get; set; }

        [Category("Position")]
        public int Y { get; set; }

        [DisplayName("Mode")]
        public LayoutMode Mode { get; set; }

        public double Opacity { get; set; }

        [Category("Background"), TypeConverter(typeof(ColorHexConverter))]
        public Color Color { get; set; }

        [Category("Background"), DisplayName("Image Path"), Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string BackgroundImagePath { get { return _backgroundImagePath; } set { _backgroundImagePath = PathHelpers.TryPlaceEnvVars(value); } }
        private string _backgroundImagePath;

        [Category("Background"), DisplayName("Stretch Image"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool StretchBackgroundImage { get; set; }

        [Category("Background"), DisplayName("Use Transparency Key Color (Fuchsia)"), TypeConverter(typeof(YesNoBooleanConverter))]
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