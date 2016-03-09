using Deskband.Core.Interfaces;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace dcmWeather
{
    public class WeatherModule : IModule
    {
        internal static readonly string ModuleName = "Weather";
        internal static readonly Guid ModuleId = Guid.Parse("{B362F04C-3015-45E1-9A1B-FB2424FE9878}");

        public Guid Id { get { return ModuleId; } }
        public string Name { get { return ModuleName; } }

        private IConsole _console;
        private IMenuProvider _menuProvider;

        public WeatherModule(IConsole console, IMenuProvider menuProvider)
        {
            _console = console;
            _menuProvider = menuProvider;
        }

        public void Initialize(IKernel kernel)
        {
            _console.AddLine("Hello console, I am a weather plugin!");

            var gid = _menuProvider.AddItem(ModuleId, null, "Weather Menu Entry", null);
            _menuProvider.AddItem(ModuleId, gid, "Submenu test 1", null);
            _menuProvider.AddItem(ModuleId, gid, "Submenu test 2", null);

            _menuProvider.AddItem(ModuleId, null, "-", null);
        }

        public void Dispose()
        {
        }

        public void ApplyConfiguration()
        {
        }

        public void DoubleClick()
        {
            _console.AddDebugLine("Weather plugin was double-clicked!");
        }
    }
}
