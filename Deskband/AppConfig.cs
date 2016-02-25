using Deskband.Configuration;
using Deskband.Console;
using Deskband.Controls;
using Deskband.Core.Interfaces;
using Deskband.Extensions;
using Deskband.Settings;
using Deskband.UI;
using Ninject;
using Ninject.Planning.Bindings.Resolvers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
            kernel.Bind<ConfigurationProvider, IConfigurationProvider>().To<ConfigurationProvider>();
            kernel.Bind<ConsoleHandler, IConsole>().To<ConsoleHandler>();
            kernel.Bind<IMenuProvider>().To<MenuProvider>();
            kernel.Bind<ModuleContainer, IModuleContainer>().To<ModuleContainer>();
            kernel.Bind<FloatingForm>().ToSelf();


            kernel.Bind<SettingsManager>().ToSelf();
            kernel.Bind<ControlHost>().ToSelf();


            kernel.Bind<App>().ToSelf();

            LoadModules(kernel);

            return kernel;
        }

        private static void LoadModules(IKernel kernel)
        {
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var directory = new DirectoryInfo(location);
            var files = directory.GetFiles("dcm*.dll", SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                var assemblyName = AssemblyName.GetAssemblyName(file.FullName);
                var assembly = AppDomain.CurrentDomain.Load(assemblyName);
                var types = assembly.GetTypes().TypesImplementingInterface<IModule>();
                foreach (var t in types)
                {
                    kernel.Bind<IModule>().To(t);
                }
            }
        }
    }
}
