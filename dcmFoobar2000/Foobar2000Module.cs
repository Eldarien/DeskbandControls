using dcmFoobar2000.Code;
using Deskband.Core.Interfaces;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using dcmFoobar2000.Configuration;
using Deskband.Core.Configuration;

namespace dcmFoobar2000
{
    public class Foobar2000Module : IModule
    {
        internal static readonly string ModuleName = "foobar2000 plugin";
        internal static readonly Guid ModuleId = Guid.Parse("{FB3F7AB3-A9F4-4C39-8C13-D9CD1110B579}");

        public Guid Id { get { return ModuleId; } }
        public string Name { get { return ModuleName; } }

        private IConsole _console;
        private IMenuProvider _menuProvider;
        private IConfigurationProvider _config;
        private Controller _controller;

        public Foobar2000Module(IConsole console, IMenuProvider menuProvider, IConfigurationProvider config)
        {
            _console = console;
            _menuProvider = menuProvider;
            _config = config;
        }

        public void Initialize(IKernel kernel)
        {
            kernel.Bind<MessageForm>().ToSelf();
            kernel.Bind<Foobar2000Actions>().ToSelf();
            kernel.Bind<Controller>().ToSelf();

            _controller = kernel.Get<Controller>();
        }

        public void Dispose()
        {
            _controller.Dispose();
        }

        public void ApplyConfiguration()
        {
            _controller.ApplyConfiguration();
        }

        public void DoubleClick()
        {
            _controller.DoubleClick();
        }

        public void MouseWheel(int delta)
        {
            _controller.MouseWheel(delta);
        }

        public void MousePoint(Point localPoint, Point globalPoint, Rectangle moduleScreenRectangle)
        {
            _controller.ShowTooltip(localPoint, globalPoint, moduleScreenRectangle);
        }

        public void MousePointOut()
        {
            _controller.HideTooltip();
        }

        public ConfigurationObjectBase GetConfiguration()
        {
            return _config.GetConfiguration(Foobar2000Module.ModuleId, ConfigurationModel.Default);
        }
    }
}
