using Deskband.Core.Interfaces;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using dcmWeather.Configuration;
using Deskband.Core.Configuration;

namespace dcmWeather
{
    public class WeatherModule : IModule
    {
        internal static readonly string ModuleName = "Weather monitor";
        internal static readonly Guid ModuleId = Guid.Parse("{B362F04C-3015-45E1-9A1B-FB2424FE9878}");

        public Guid Id { get { return ModuleId; } }
        public string Name { get { return ModuleName; } }

        private IConsole _console;
        private IMenuProvider _menuProvider;
        private IConfigurationProvider _config;

        public WeatherModule(IConsole console, IMenuProvider menuProvider, IConfigurationProvider config)
        {
            _console = console;
            _menuProvider = menuProvider;
            _config = config;
        }

        public void Initialize(IKernel kernel)
        {
            var gid = _menuProvider.AddItem(ModuleId, null, "Weather Menu Entry", null);
            _menuProvider.AddItem(ModuleId, gid, "Submenu test 1", null);
            _menuProvider.AddItem(ModuleId, gid, "Submenu test 2", null);
            _menuProvider.AddItem(ModuleId, null, "-", null);

            _console.AddLine("Weather plugin initilized");
        }

        public void Dispose()
        {
        }

        public void ApplyConfiguration()
        {
        }

        public void DoubleClick()
        {
            _console.AddLine("Weather plugin was double-clicked!");
        }

        public void MouseWheel(int delta)
        {
            _console.AddLine($"Weather plugin was scrolled with delta {delta}");
        }

        public ConfigurationObjectBase GetConfiguration()
        {
            return _config.GetConfiguration(ModuleId, ConfigurationModel.Default);
        }
    }
}
