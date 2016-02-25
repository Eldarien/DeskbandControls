using Deskband.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.Configuration
{
    public class ConfigurationModel : ConfigurationObjectBase
    {
        public DisplayMode DisplayMode { get; set; }
        public List<ModuleSettings> ModulesSettings { get; set; } = new List<ModuleSettings>();
        public FloatingWindowSettings FloatingWindowSettings { get; set; } = FloatingWindowSettings.GetDefault();

        public static ConfigurationModel GetDefault(IEnumerable<IModule> modules)
        {
            return new ConfigurationModel
            {
                ModuleId = Band.ModuleId,
                DisplayMode = DisplayMode.Deskband,
                ModulesSettings = modules.Select(x => new ModuleSettings
                {
                    Id = x.Id,
                    Width = 100,
                    Height = 30
                }).ToList()
            };
        }
    }
}
