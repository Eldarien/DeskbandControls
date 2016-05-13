using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using Deskband.Core.Common;
using Deskband.Core.Configuration;
using Newtonsoft.Json;

namespace dcmFoobar2000.Configuration
{
    [SettingsCommand("Remove Text", nameof(ConfigurationModel.RemoveText))]
    public class TextSettings : SettingsObject
    {
        [Browsable(true)]
        public override string Name { get; set; }

        public bool Visible { get; set; }

        [Category("Position and Size")]
        public int X { get; set; }

        [Category("Position and Size")]
        public int Y { get; set; }

        [Category("Position and Size")]
        public int Width { get; set; }

        [Category("Position and Size")]
        public int Height { get; set; }

        public string Format { get; set; }

        [DisplayName("Font Name"), TypeConverter(typeof(FontNameStringConverter))]
        public string FontName { get; set; }

        [DisplayName("Font Size")]
        public int FontSize { get; set; }

        [DisplayName("Font Style Italic"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool FontStyleItalic { get; set; }

        [DisplayName("Font Style Bold")]
        public bool FontStyleBold { get; set; }

        [DisplayName("Font Color")]
        public Color FontColor { get; set; }

        [DisplayName("Enable Scroll")]
        public bool EnableScroll { get; set; }

        [DisplayName("Align To Right")]
        public bool AlightToRight { get; set; }

        [DisplayName("Stopped Text")]
        public string StoppedText { get; set; }
    }
}