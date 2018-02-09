using Deskband.Core.Configuration;
using System.ComponentModel;

namespace Deskband.Configuration
{
    public class GeneralSettings
    {
        [DisplayName("Display Mode"), TypeConverter(typeof(EnumDescriptionConverter<DisplayMode>))]
        public DisplayMode DisplayMode { get; set; }

        [DisplayName("Deskband Mode"), TypeConverter(typeof(EnumDescriptionConverter<DeskbandMode>))]
        public DeskbandMode DeskbandMode { get; set; }

        [DisplayName("Draw Borders"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool DrawBorders { get; set; }

        public static GeneralSettings Default =>
            new GeneralSettings
            {
                DisplayMode = DisplayMode.Deskband,
                DeskbandMode = DeskbandMode.Docked
            };
    }
}