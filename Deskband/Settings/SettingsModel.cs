using Deskband.Configuration;
using Deskband.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.Settings
{
    public class SettingsModel
    {
        //[SettingsNode("Global Settings")]
        //public ConfigurationModel GlobalSettings { get; set; }

        //[SettingsNodeList("Modules Settings")]
        //public IEnumerable<object> ModulesSettings { get; set; }

        [SettingsNodeList("Settings")]
        public List<object> SettingsModels { get; set; }
    }
}