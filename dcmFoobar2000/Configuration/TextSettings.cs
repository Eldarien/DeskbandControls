using Deskband.Core.Common;
using Deskband.Core.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;

namespace dcmFoobar2000.Configuration
{
    [SettingsCommand("Remove Text", nameof(ConfigurationModel.RemoveText))]
    public class TextSettings : SettingsObject
    {
        public override string ToString()
        {
            return Name ?? "Text";
        }

        [Browsable(true)]
        public override string Name { get; set; }

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

        [Category("Font"), DisplayName("Font Name"), TypeConverter(typeof(FontNameStringConverter))]
        public string FontName { get; set; }

        [Category("Font"), DisplayName("Font Size")]
        public int FontSize { get; set; }

        [Category("Font"), DisplayName("Font Style Italic"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool FontStyleItalic { get; set; }

        [Category("Font"), DisplayName("Font Style Bold")]
        public bool FontStyleBold { get; set; }

        [Category("Font"), DisplayName("Font Color")]
        public Color FontColor { get; set; }

        [Category("Text")]
        public string Format { get; set; }

        [Category("Text"), DisplayName("Enable Scroll")]
        public bool EnableScroll { get; set; }

        [Category("Text"), DisplayName("Align To Right")]
        public bool AlightToRight { get; set; }

        [Category("Text"), DisplayName("Stopped Text")]
        public string StoppedText { get; set; }
    }
}