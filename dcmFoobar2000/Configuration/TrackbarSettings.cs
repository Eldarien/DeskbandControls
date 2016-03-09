using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace dcmFoobar2000.Configuration
{
    public class TrackbarSettings
    {
        public bool Visible { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Heigth { get; set; }
        public Color Color { get; set; }
        public Color BackgroundColor { get; set; }
        public bool UseBackgroundColor { get; set; }
        public bool HideBorders { get; set; }
    }
}
