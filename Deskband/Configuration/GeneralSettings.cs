using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms.Design;

namespace Deskband.Configuration
{
    public class GeneralSettings
    {
        [DisplayName("Deskband Mode")]
        public DisplayMode DisplayMode { get; set; }

        [DisplayName("Draw Borders")]
        public bool DrawBorders { get; set; }

        public static GeneralSettings GetDefault()
        {
            return new GeneralSettings
            {
                DisplayMode = DisplayMode.Deskband
            };
        }
    }
}