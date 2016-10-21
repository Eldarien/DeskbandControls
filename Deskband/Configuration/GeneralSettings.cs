using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Deskband.Configuration
{
    public class GeneralSettings
    {
        [DisplayName("Deskband Mode")]
        public DisplayMode DisplayMode { get; set; }

        public static GeneralSettings GetDefault()
        {
            return new GeneralSettings
            {
                DisplayMode = DisplayMode.Deskband
            };
        }
    }
}