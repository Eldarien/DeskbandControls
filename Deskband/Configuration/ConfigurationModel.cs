using Deskband.Core.Configuration;
using Deskband.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Deskband.Configuration
{
    public class ConfigurationModel : ConfigurationObjectBase
    {
        [DisplayName("Deskband Mode")]
        public DisplayMode DisplayMode { get; set; }

        [Browsable(false)]
        [SettingsNodeList("Modules Settings")]
        public List<ModuleSettings> ModulesSettings { get; set; } = new List<ModuleSettings>();

        [Browsable(false)]
        [SettingsNode("Floating Window")]
        public FloatingWindowSettings FloatingWindowSettings { get; set; } = FloatingWindowSettings.GetDefault();

        public static ConfigurationModel GetDefault(IEnumerable<IModule> modules)
        {
            return new ConfigurationModel
            {
                ModuleId = Guid.Empty,
                DisplayMode = DisplayMode.Deskband,
                ModulesSettings = modules.Select(x => new ModuleSettings
                {
                    Id = x.Id,
                    Width = 260,
                    Height = 30
                }).ToList()
            };
        }
    }
}