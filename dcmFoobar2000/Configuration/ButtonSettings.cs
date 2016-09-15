using Deskband.Core.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

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

        [Category("Icon"), DisplayName("Icon Path")]
        public string Icon1Path { get; set; }

        [Category("Icon"), DisplayName("Secondary Icon Path")]
        public string Icon2Path { get; set; }

        [Category("Icon"), DisplayName("Colorize Color")]
        public Color ColorizeColor { get; set; }
    }
}