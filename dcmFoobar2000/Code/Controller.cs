using Deskband.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace dcmFoobar2000.Code
{
    public class Controller : IDisposable
    {
        private IConfigurationProvider _config;
        private IMenuProvider _menu;
        private IModuleContainer _container;
        private Actions _actions;

        private ConfigurationModel _cfg;

        public Controller(
            IConfigurationProvider config,
            IMenuProvider menu,
            IModuleContainer container,
            Actions actions)
        {
            _config = config;
            _menu = menu;
            _container = container;
            _actions = actions;
        }

        public void ApplyConfiguration()
        {
            _cfg = _config.GetConfiguration(Foobar2000Module.ModuleId, ConfigurationModel.Default);
            _config.UpdateConfiguration(_cfg);

            RegisterMenu();
            RegisterControls();
        }

        public void Dispose()
        {
            _config.UpdateConfiguration(_cfg);
        }

        private string _miStop;
        private string _miPlayPause;
        private string _miPrev;
        private string _miNext;
        private string _miRandom;
        private string _miToggleStopAfterCurrent;
        private string _miCopyArtistAndTitle;
        private string _miCopyTitle;
        private string _miCopyArtist;
        private string _miOpenContainingFolder;
        private string _miSearchInInternet;

        private void RegisterMenu()
        {
            var group = Foobar2000Module.ModuleName;
            _menu.ClearGroup(group);

            _miStop = _menu.AddItem(group, "Stop", _actions.Stop);
            _miPlayPause = _menu.AddItem(group, "Play / Pause", _actions.PlayPause);
            _miPrev = _menu.AddItem(group, "Previous", _actions.Prev);
            _miNext = _menu.AddItem(group, "Next", _actions.Next);
            _miRandom = _menu.AddItem(group, "Random", _actions.Random);
            _miToggleStopAfterCurrent = _menu.AddItem(group, "Toggle Stop After Current", _actions.ToggleStopAfterCurrent);

            _menu.AddSeparator(group);

            _miCopyArtistAndTitle = _menu.AddItem(group, "Copy Artist and Title", _actions.CopyArtistAndTitle);
            _miCopyTitle = _menu.AddItem(group, "Copy Title", _actions.CopyTitle);
            _miCopyArtist = _menu.AddItem(group, "Copy Artist", _actions.CopyArtist);
            _miOpenContainingFolder = _menu.AddItem(group, "Open Containing Folder", _actions.OpenContainingFolder);
            _miSearchInInternet = _menu.AddItem(group, "Search in Internet", _actions.SearchInInternet);
        }

        private void RegisterControls()
        {
            _container.ClearControls(Foobar2000Module.ModuleId);

            var btn = new System.Windows.Forms.Button();
            btn.Size = new System.Drawing.Size(150, 20);
            btn.Text = "button test";
            btn.Click += (s, e) => _container.Hide(Foobar2000Module.ModuleId);
            _container.AddControl(Foobar2000Module.ModuleId, btn);
        }
    }
}
