using Deskband.Core.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace dcmFoobar2000.Configuration
{
    public class TooltipSettings
    {
        [Category("Visibility"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool Enabled { get; set; }

        [Category("Size")]
        public int Width { get; set; }

        [Category("Size")]
        public int Height { get; set; }

        [Category("Window"), DisplayName("Background Color"), TypeConverter(typeof(ColorHexConverter))]
        public Color BackgroundColor { get; set; }

        [Category("Window"), DisplayName("Borderless Window"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool UseBorderlessWindow { get; set; }

        [Browsable(false), SettingsNode("Album Art")]
        public AlbumArtSettings AlbumArt { get; set; } = new AlbumArtSettings { Visible = true, X = 5, Y = 5, Width = 100, Height = 100 };

        [Browsable(false), SettingsNodeList("Texts")]
        [SettingsCommand("Add Text", nameof(TooltipSettings.AddText))]
        public List<TooltipTextSettings> Texts { get; set; } = new List<TooltipTextSettings>();

        public static object AddText(TooltipSettings model)
        {
            return SettingsObject.AddCollectionItemCommandHelper(model.Texts, "Tooltip Text", name => new TooltipTextSettings { Name = name, Format = "%title%" });
        }

        public static object RemoveText(TooltipSettings model, TooltipTextSettings item)
        {
            return SettingsObject.RemoveCollectionItemCommandHelper(model.Texts, item);
        }
    }
}
