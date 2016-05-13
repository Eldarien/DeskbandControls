using Deskband.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deskband.Core.Interfaces;

namespace Deskband.Configuration
{
    public class ConfigurationModel : ConfigurationObjectBase
    {
        public override string ToString()
        {
            return "Deskband Controls";
        }

        public DisplayMode DisplayMode { get; set; }
        public List<ModuleSettings> ModulesSettings { get; set; } = new List<ModuleSettings>();
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
