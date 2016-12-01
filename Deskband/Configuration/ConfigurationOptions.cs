using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.Configuration
{
    public class ConfigurationOptions
    {
        public string ConfigurationDirectory { get; set; }

        public static ConfigurationOptions GetDefault()
        {
            return new ConfigurationOptions();
        }
    }
}
