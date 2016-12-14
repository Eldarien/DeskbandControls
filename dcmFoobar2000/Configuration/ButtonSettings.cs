using Deskband.Core.Common;
using Deskband.Core.Configuration;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace dcmFoobar2000.Configuration
{
    public class ButtonSettings
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
        public int Height { get; set; }

        [Category("Icon"), DisplayName("Icon Path"), Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string Icon1Path { get { return _icon1Path; } set { _icon1Path = PathHelpers.TryPlaceEnvVars(value); } }
        private string _icon1Path;

        [Category("Icon"), DisplayName("Secondary Icon Path"), Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string Icon2Path { get { return _icon2Path; } set { _icon2Path = PathHelpers.TryPlaceEnvVars(value); } }
        private string _icon2Path;

        [Category("Icon"), DisplayName("Colorize Color"), TypeConverter(typeof(ColorHexConverter))]
        public Color ColorizeColor { get; set; }
    }
}