using Deskband.Configuration;
using Deskband.Console;
using Deskband.Core.Interfaces;
using Deskband.Settings;
using Deskband.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Deskband
{
    public class App
    {
        private Band _band;
        private IEnumerable<IModule> _modules;

        // Services

        private ConfigurationProvider _config;

        private SettingsManager _settingsManager;
        private ConsoleHandler _consoleHandler;

        //private ControlHost _controlHost;
        private IMenuProvider _menu;

        private ISizeProvider _sp;
        private ModuleContainer _moduleContainer;

        private FloatingForm _floatingForm;

        public App(
            Band band,
            IEnumerable<IModule> modules,
            ConfigurationProvider config,
            FloatingForm floatingForm,

            SettingsManager settingsManager,
            ConsoleHandler consoleHandler,
            //ControlHost controlHost,

            IMenuProvider menu,
            ISizeProvider sp,
            ModuleContainer moduleContainer
            )
        {
            _band = band;
            _modules = modules;
            _config = config;
            _floatingForm = floatingForm;

            _settingsManager = settingsManager;
            _consoleHandler = consoleHandler;
            //_controlHost = controlHost;
            _menu = menu;
            _sp = sp;
            _moduleContainer = moduleContainer;
        }

        public void Run()
        {
            _config.Load();

            _band.Close += OnClose;
            _moduleContainer.Resize += OnResize;

            ApplyConfiguration();

            //

            // DPI
            _band.DPIChanged += (s, e) => ApplyConfiguration();

            // DoubleClick
            _band.MouseDoubleClick += (s, e) => HandleDoubleClick(e.Location);
            _floatingForm.MouseDoubleClick += (s, e) => HandleDoubleClick(e.Location);

            // Console
            var miConsole = _menu.AddItem(Guid.Empty, null, "Console", _consoleHandler.ToggleConsole);
            _consoleHandler.OnConsoleToggle += (s, e) => _menu.SetItemCheckedState(miConsole, e.Value);

            // Settings
            _menu.AddItem(Guid.Empty, null, "Settings", OnSettingsMenuItemClick);
            _menu.AddItem(Guid.Empty, null, "New Settings", () =>
            {
                var cfg = _config.GetConfiguration(Guid.Empty, ConfigurationModel.GetDefault(_modules));
                cfg.ModulesSettings.ForEach(m => m.SetName(_modules.Where(x => x.Id == m.Id).Select(x => x.Name).FirstOrDefault()));
                var sm = new SettingsModel
                {
                    GlobalSettings = cfg,
                    ModulesSettings = _modules.Select(x => x.GetConfiguration())
                };

                var sf = new SettingsForm(_config, _consoleHandler, sm);
                sf.OnApply += (s, e) => { ApplyConfiguration(); _config.Save(); };
                sf.Show();
            });

            //_settingsManager.LoadSettings();

            //_controlHost.OnApplySettings += OnApplySettings;
            //_controlHost.OnPlaybackState += OnPlaybackState;
            //_controlHost.OnFoobarShowHide += OnFoobarShowHide;

            // Console
            //_consoleHandler.OnConsoleToggle += (s, e) => _menuProvider.SetItemCheckedState(_miConsole, e.Value);

            // ContextMenu
            /*
            _miStop = _menuProvider.AddItem("", "Stop", _controlHost.Controller.FoobarActions.Stop);
            _miPlayPause = _menuProvider.AddItem("", "Play / Pause", _controlHost.Controller.FoobarActions.PlayPause);
            _Prev = _menuProvider.AddItem("", "Previous", _controlHost.Controller.FoobarActions.Previuos);
            _Next = _menuProvider.AddItem("", "Next", _controlHost.Controller.FoobarActions.Next);
            _Random = _menuProvider.AddItem("", "Random", _controlHost.Controller.FoobarActions.Random);
            _ToggleStopAfterCurrent = _menuProvider.AddItem("", "Toggle Stop After Current", _controlHost.Controller.FoobarActions.ToggleStopAfterCurrent);

            _menuProvider.AddSeparator("");

            _miCopyArtistAndTitle = _menuProvider.AddItem("", "Copy Artist and Title", OnCopyArtistAndTitle);
            _miCopyTitle = _menuProvider.AddItem("", "Copy Title", OnCopyTitle);
            _miCopyArtist = _menuProvider.AddItem("", "Copy Artist", OnCopyArtist);
            _miOpenContainingFolder = _menuProvider.AddItem("", "Open Containing Folder", OnOpenContainingFolderClick);
            _miSearchInInternet = _menuProvider.AddItem("", "Search in Internet", OnSearchInInternetClick);

            _menuProvider.AddSeparator("");

            _miConsole = _menuProvider.AddItem("", "Console", _consoleHandler.ToggleConsole);
            _menuProvider.AddItem("", "Settings", OnSettingsMenuItemClick);
            */

            //_controlHost.ApplySettings();
        }

        private void OnClose(object sender, EventArgs e)
        {
            _config.Save();

            //_settingsManager.SaveSettings();
        }

        private void OnResize(object sender, EventArgs e)
        {
            _consoleHandler.AddDebugLine(String.Format("Module container resized: {0}x{1}", _moduleContainer.Size.Width, _moduleContainer.Size.Height));

            _band.MinSize = new Size(_band.Visible ? _moduleContainer.Size.Width : 10, 0); // 10px reserve for accessing context menu
            _band.ExecBandInfoChangedCommand();

            _floatingForm.Size = _moduleContainer.Size;
        }

        private void ApplyConfiguration()
        {
            var cfg = _config.GetConfiguration(Guid.Empty, ConfigurationModel.GetDefault(_modules));
            cfg.ModulesSettings.RemoveAll(ms => !_modules.Any(m => m.Id == ms.Id));
            _config.UpdateConfiguration(cfg);

            _band.Controls.Clear();
            _floatingForm.Controls.Clear();

            if (cfg.DisplayMode == DisplayMode.Deskband)
            {
                _floatingForm.Hide();
                _band.Visible = true;
                _band.Controls.Add(_moduleContainer.AsControl());
            }
            else if (cfg.DisplayMode == DisplayMode.FloatingWindow)
            {
                _band.Visible = false;
                _floatingForm.Show();
                _floatingForm.Controls.Add(_moduleContainer.AsControl());
            }
            _floatingForm.ApplyConfiguration();

            foreach (var ms in cfg.ModulesSettings)
            {
                _moduleContainer.PositionModules(ms.Id, _sp.MakeSize(ms.Width, ms.Height), _sp.MakePoint(ms.Left, ms.Top));
            }

            foreach (var m in _modules)
            {
                m.ApplyConfiguration();
            }
        }

        private void HandleDoubleClick(Point location)
        {
            var moduleId = _moduleContainer.LocateModuleAtPoint(location);
            if (moduleId != null)
            {
                var module = _modules.First(m => m.Id == moduleId.Value);
                module.DoubleClick();
            }
        }

        //private void OnApplySettings(object sender, EventArgs e)
        //{
        //    if (_floatingForm.Controls.Contains(_controlHost))
        //        _floatingForm.Controls.Remove(_controlHost);
        //    if (_band.Controls.Contains(_controlHost))
        //        _band.Controls.Remove(_controlHost);

        //    if (_settingsManager.Settings.General.FloatingMode)
        //    {
        //        _floatingForm.Controls.Add(_controlHost);

        //        ShowFloatingWindow();
        //        HideBand();

        //        _floatingForm.LoadSettings();
        //    }
        //    else
        //    {
        //        _band.Controls.Add(_controlHost);

        //        HideFloatingWindow();
        //        ShowBand();
        //    }
        //}

        private void OnSettingsMenuItemClick()
        {
            var wnd = new SettingsWindow();
            var context = new SettingsViewModel(wnd, _settingsManager);
            wnd.DataContext = context;
            wnd.Show();

            context.OnClose += (s, ea) => wnd.Close();
            //context.OnApply += (s, ea) => { _controlHost.ApplySettings(); _settingsManager.SaveSettings(); };
        }
    }
}