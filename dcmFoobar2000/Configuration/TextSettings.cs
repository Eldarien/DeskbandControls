using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace dcmFoobar2000.Configuration
{
    public class TextSettings
    {
        public bool Visible { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Format { get; set; }
        public string FontName { get; set; }
        public int FontSize { get; set; }
        public bool FontStyleItalic { get; set; }
        public bool FontStyleBold { get; set; }
        public Color FontColor { get; set; }
        public bool EnableScroll { get; set; }
        public bool AlightToRight { get; set; }
        public string StoppedText { get; set; }
    }
}
