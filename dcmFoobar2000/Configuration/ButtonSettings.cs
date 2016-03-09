using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace dcmFoobar2000.Configuration
{
    public class ButtonSettings
    {
        public bool Visible { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Icon1Path { get; set; }
        public string Icon2Path { get; set; }
        public Color ColorizeColor { get; set; }
    }
}
