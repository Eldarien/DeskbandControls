using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace dcmFoobar2000.Configuration
{
    public class PlaylistSettings
    {
        [DisplayName("Number Of Items Before Current")]
        public int NumberOfItemsBeforeCurrent { get; set; }

        [DisplayName("Number Of Items After Current")]
        public int NumberOfItemsAfterCurrent { get; set; }

        public string Format { get; set; }
    }
}
