using Deskband.Console;
using Deskband.Controls;
using Deskband.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deskband.Core.Interfaces;
using Deskband.UI;
using Deskband.Configuration;
using System.Drawing;

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
            _moduleContainer = moduleContainer;
        }

        public void Run()
        {
            _config.Load();

            _band.Close += OnClose;
            _moduleContainer.Resize += OnResize;

            ApplyConfiguration();



            // Console
            var miConsole = _menu.AddItem("", "Console", _consoleHandler.ToggleConsole);
            _consoleHandler.OnConsoleToggle += (s, e) => _menu.SetItemCheckedState(miConsole, e.Value);

            // Settings
            _menu.AddItem("", "Settings", OnSettingsMenuItemClick);


            //_settingsManager.LoadSettings();

            //_controlHost.OnApplySettings += OnApplySettings;
            //_controlHost.OnPlaybackState += OnPlaybackState;
            //_controlHost.OnFoobarShowHide += OnFoobarShowHide;



            // DoubleClick
            //_band.DoubleClick += OnActivateFoobar;
            //_floatingForm.DoubleClick += OnActivateFoobar;

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

            // Startup complete
            _consoleHandler.AddLine("Deskband Controls ready!");


            //_controlHost.ApplySettings();
        }

        private void OnClose(object sender, EventArgs e)
        {
            _config.Save();

            //_settingsManager.SaveSettings();
        }

        private void OnResize(object sender, EventArgs e)
        {
            _consoleHandler.AddLine(String.Format("Module container resized: {0}x{1}", _moduleContainer.Size.Width, _moduleContainer.Size.Height));

            _band.MinSize = new Size(_band.Visible ? _moduleContainer.Size.Width : 10, 0); // 10px reserve for accessing context menu
            _band.ExecBandInfoChangedCommand();

            _floatingForm.Size = _moduleContainer.Size;
        }

        private void ApplyConfiguration()
        {
            var cfg = _config.GetConfiguration(Band.ModuleId, ConfigurationModel.GetDefault(_modules));
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
                _moduleContainer.SetModuleSize(ms.Id, new Size(ms.Width, ms.Height));
            }

            foreach (var m in _modules)
            {
                m.ApplyConfiguration();
            }
        }

        //private void OnPlaybackState(object sender, ValueEventArgs<bool> e)
        //{
        //    if (_settingsManager.Settings.General.HideIfNotPlaying && !e.Value)
        //        ShowHide(false);
        //    else
        //        ShowHide(true);

        //    var stopped = _controlHost.Controller.Stopped;

        //    _menuProvider.SetItemEnabledState(_miStop, !stopped);
        //    _menuProvider.SetItemEnabledState(_ToggleStopAfterCurrent, !stopped);

        //    _menuProvider.SetItemEnabledState(_miCopyArtistAndTitle, !stopped);
        //    _menuProvider.SetItemEnabledState(_miCopyArtist, !stopped);
        //    _menuProvider.SetItemEnabledState(_miCopyTitle, !stopped);
        //    _menuProvider.SetItemEnabledState(_miOpenContainingFolder, !stopped);
        //    _menuProvider.SetItemEnabledState(_miSearchInInternet, !stopped);
        //}

        //private void OnFoobarShowHide(object sender, ValueEventArgs<bool> e)
        //{
        //    if (_settingsManager.Settings.General.HideIfFoobar2000IsNotRunning)
        //    {
        //        ShowHide(e.Value);
        //        if (e.Value)
        //        {
        //            OnPlaybackState(sender, new ValueEventArgs<bool>(false));
        //        }
        //    }
        //}

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

        //private void OnOpenContainingFolderClick()
        //{
        //    _controlHost.Controller.FoobarActions.FilePath(0);
        //}

        //private void OnSearchInInternetClick()
        //{
        //    _controlHost.Controller.FoobarActions.FormatString(FormatStringIndex.InternetSearch, _settingsManager.Settings.General.InternetSearchFormat);
        //}

        //private void OnCopyArtist()
        //{
        //    _controlHost.Controller.FoobarActions.FormatString(FormatStringIndex.CopyArtist, "%artist%");
        //}

        //private void OnCopyTitle()
        //{
        //    _controlHost.Controller.FoobarActions.FormatString(FormatStringIndex.CopyTitle, "%title%");
        //}

        //private void OnCopyArtistAndTitle()
        //{
        //    _controlHost.Controller.FoobarActions.FormatString(FormatStringIndex.CopyArtistAndTitle, "%artist% - %title%");
        //}

        //private void OnActivateFoobar(object sender, EventArgs e)
        //{
        //    _controlHost.Controller.FoobarActions.ActivateFoobar();
        //}
    }
}
