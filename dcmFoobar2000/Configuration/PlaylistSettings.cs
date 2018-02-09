using Deskband.Core.Configuration;
using System.ComponentModel;

namespace dcmFoobar2000.Configuration
{
    public class PlaylistSettings
    {
        [DisplayName("Number Of Items Before Current")]
        public int NumberOfItemsBeforeCurrent { get; set; }

        [DisplayName("Number Of Items After Current")]
        public int NumberOfItemsAfterCurrent { get; set; }

        public string Format { get; set; }

        [DisplayName("Cascaded Menu"), TypeConverter(typeof(YesNoBooleanConverter))]
        public bool CascadedMenu { get; set; }
    }
}
