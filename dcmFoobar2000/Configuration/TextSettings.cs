using Deskband.Core.Configuration;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.VisualStyles;

namespace dcmFoobar2000.Configuration
{
    [SettingsCommand("Remove Text", nameof(ConfigurationModel.RemoveText))]
    public class TextSettings : TextSettingsBase { }

    [SettingsCommand("Remove Text", nameof(TooltipSettings.RemoveText))]
    public class TooltipTextSettings : TextSettingsBase { }

    public class TextSettingsBase : SettingsObject
    {
        public override string ToString()
        {
            return Name ?? "Text";
        }

        [Browsable(true)]
        public override string Name { get; set; }

        [Category("Visibility"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool Visible { get; set; } = true;

        [Category("Position")]
        public int X { get; set; }

        [Category("Position")]
        public int Y { get; set; }

        [Category("Size")]
        public int Width { get; set; } = 100;

        [Category("Size")]
        public int Height { get; set; } = 16;

        [Category("Font"), DisplayName("Font Name"), TypeConverter(typeof(FontNameStringConverter))]
        public string FontName { get; set; } = "Segoe UI";

        [Category("Font"), DisplayName("Font Size")]
        [TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(5, 30)]
        public int FontSize { get; set; } = 8;

        [Category("Font"), DisplayName("Font Style Italic")]
        [TypeConverter(typeof(YesNoBooleanConverter))]
        public bool FontStyleItalic { get; set; }

        [Category("Font"), DisplayName("Font Style Bold")]
        [TypeConverter(typeof(YesNoBooleanConverter))]
        public bool FontStyleBold { get; set; }

        [Category("Font"), DisplayName("Font Color"), TypeConverter(typeof(ColorHexConverter))]
        public Color FontColor { get; set; } = Color.White;

        [Category("Font"), DisplayName("Display Shadow")]
        [TypeConverter(typeof(YesNoBooleanConverter))]
        public bool DisplayShadow { get; set; }

        [Category("Font"), DisplayName("Shadow Color"), TypeConverter(typeof(ColorHexConverter))]
        public Color ShadowColor { get; set; } = Color.Black;

        [Category("Font"), DisplayName("Shadow Offset")]
        public int ShadowOffset { get; set; } = 2;

        [Category("Font"), DisplayName("Background Color"), TypeConverter(typeof(ColorHexConverter))]
        public Color BackgroundColor { get; set; } = Color.Transparent;

        [Category("Text")]
        public string Format { get; set; }

        [Category("Text"), DisplayName("Paused Format")]
        public string PausedFormat { get; set; } = "";

        [Category("Text"), DisplayName("Stopped Text")]
        public string StoppedText { get; set; } = "";

        [Category("Text"), DisplayName("Horizontal Align")]
        public HorizontalAlign HorizontalAlign { get; set; }

        [Category("Scroll"), DisplayName("Enable Scroll"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool EnableScroll { get; set; } = true;

        [Category("Scroll"), DisplayName("Scroll Speed"), TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(10, 1000)]
        public int ScrollSpeed { get; set; } = 100;

        [Category("Scroll"), DisplayName("Scroll Step"), TypeConverter(typeof(NumericUpDownTypeConverter))]
        [Editor(typeof(NumericUpDownTypeEditor), typeof(UITypeEditor)), MinMax(1, 10)]
        public int ScrollStep { get; set; } = 2;

        [Category("Scroll"), DisplayName("Scroll Separator")]
        public string ScrollSeparator { get; set; } = " **** ";
    }
}