using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deskband.Configuration;

namespace Deskband.Settings
{
    public class SettingsModel
    {
        public ConfigurationModel GlobalSettings { get; set; }
        //public IEnumerable<ModuleSettings> Modules { get; set; }
        public IEnumerable<ModuleSettingsModel> ModulesSettings { get; set; }
    }

    public class ModuleSettingsModel
    {
        public object SettingsObject { get; set; }
        public string Name { get; set; }
    }
}
