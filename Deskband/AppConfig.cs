using Deskband.Console;
using Deskband.Controls;
using Deskband.Settings;
using Deskband.UIProviders;
using Ninject;
using Ninject.Planning.Bindings.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband
{
    public static class AppConfig
    {
        public static IKernel InitializeKernel(Band band)
        {
            var kernel = new StandardKernel(new NinjectSettings { DefaultScopeCallback = c => band });
            kernel.Components.Remove<IMissingBindingResolver, SelfBindingResolver>();

            kernel.Bind<Band>().ToConstant(band);
            kernel.Bind<SettingsManager>().ToSelf();
            kernel.Bind<ConsoleHandler>().ToSelf();
            kernel.Bind<ControlHost>().ToSelf();
            kernel.Bind<MenuProvider>().ToSelf();
            kernel.Bind<App>().ToSelf();

            return kernel;
        }
    }
}
