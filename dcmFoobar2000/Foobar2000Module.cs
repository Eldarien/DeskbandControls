using dcmFoobar2000.Code;
using Deskband.Core.Interfaces;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace dcmFoobar2000
{
    public class Foobar2000Module : IModule
    {
        internal static readonly string ModuleName = "Foobar2000";
        internal static readonly Guid ModuleId = Guid.Parse("{FB3F7AB3-A9F4-4C39-8C13-D9CD1110B579}");

        public Guid Id { get { return ModuleId; } }
        public string Name { get { return ModuleName; } }

        private IConsole _console;
        private IMenuProvider _menuProvider;
        private Controller _controller;

        public Foobar2000Module(IConsole console, IMenuProvider menuProvider)
        {
            _console = console;
            _menuProvider = menuProvider;
        }

        public void Initialize(IKernel kernel)
        {
            _console.AddLine("Hello console, I am a foobar2000 plugin!");

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
    }
}
