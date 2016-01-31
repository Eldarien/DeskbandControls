using Deskband.Communication;
using Deskband.Console;
using Deskband.Controls;
using Deskband.Settings;
using Deskband.UIProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband
{
    public class App : IDisposable
    {
        private Band _band;
        private SettingsManager _settingsManager;
        private ConsoleHandler _consoleHandler;
        private ControlHost _controlHost;
        private MenuProvider _menu;

        private FloatingForm _floatingForm;

        private string _miStop;
        private string _miPlayPause;
        private string _Prev;
        private string _Next;
        private string _Random;
        private string _ToggleStopAfterCurrent;

        private string _miCopyTitle;
        private string _miCopyArtist;
        private string _miOpenContainingFolder;
        private string _miSearchInInternet;
        private string _miCopyArtistAndTitle;
        private string _miConsole;

        public App(
            Band band,
            SettingsManager settingsManager,
            ConsoleHandler consoleHandler,
            ControlHost controlHost,
            MenuProvider menu
            )
        {
            _band = band;
            _settingsManager = settingsManager;
            _consoleHandler = consoleHandler;
            _controlHost = controlHost;
            _menu = menu;
        }

        public void Dispose()
        {
            _floatingForm.Dispose();
        }

        public void Run()
        {
            _band.Close += OnClose;

            _settingsManager.LoadSettings();

            _controlHost.OnApplySettings += OnApplySettings;
            _controlHost.OnPlaybackState += OnPlaybackState;
            _controlHost.OnFoobarShowHide += OnFoobarShowHide;

            _floatingForm = new FloatingForm(_settingsManager);

            // DoubleClick
            _band.DoubleClick += OnActivateFoobar;
            _floatingForm.DoubleClick += OnActivateFoobar;

            // Console
            _consoleHandler.OnConsoleToggle += (s, e) => _menu.SetItemCheckedState(_miConsole, e.Value);

            // ContextMenu
            _miStop = _menu.AddItem("", "Stop", _controlHost.Controller.FoobarActions.Stop);
            _miPlayPause = _menu.AddItem("", "Play / Pause", _controlHost.Controller.FoobarActions.PlayPause);
            _Prev = _menu.AddItem("", "Previous", _controlHost.Controller.FoobarActions.Previuos);
            _Next = _menu.AddItem("", "Next", _controlHost.Controller.FoobarActions.Next);
            _Random = _menu.AddItem("", "Random", _controlHost.Controller.FoobarActions.Random);
            _ToggleStopAfterCurrent = _menu.AddItem("", "Toggle Stop After Current", _controlHost.Controller.FoobarActions.ToggleStopAfterCurrent);

            _menu.AddSeparator("");

            _miCopyArtistAndTitle = _menu.AddItem("", "Copy Artist and Title", OnCopyArtistAndTitle);
            _miCopyTitle = _menu.AddItem("", "Copy Title", OnCopyTitle);
            _miCopyArtist = _menu.AddItem("", "Copy Artist", OnCopyArtist);
            _miOpenContainingFolder = _menu.AddItem("", "Open Containing Folder", OnOpenContainingFolderClick);
            _miSearchInInternet = _menu.AddItem("", "Search in Internet", OnSearchInInternetClick);

            _menu.AddSeparator("");

            _miConsole = _menu.AddItem("", "Console", _consoleHandler.ToggleConsole);
            _menu.AddItem("", "Settings", OnSettingsMenuItemClick);

            // Startup complete
            _consoleHandler.AddLine("Deskband Controls started");


            _controlHost.ApplySettings();
        }

        private void ShowHide(bool show)
        {
            if (show)
            {
                if (_settingsManager.Settings.General.FloatingMode)
                    ShowFloatingWindow();
                else
                    ShowBand();
            }
            else
            {
                if (_settingsManager.Settings.General.FloatingMode)
                    HideFloatingWindow();
                else
                    HideBand();
            }
        }

        private void OnPlaybackState(object sender, ValueEventArgs<bool> e)
        {
            if (_settingsManager.Settings.General.HideIfNotPlaying && !e.Value)
                ShowHide(false);
            else
                ShowHide(true);

            var stopped = _controlHost.Controller.Stopped;

            _menu.SetItemEnabledState(_miStop, !stopped);
            _menu.SetItemEnabledState(_ToggleStopAfterCurrent, !stopped);

            _menu.SetItemEnabledState(_miCopyArtistAndTitle, !stopped);
            _menu.SetItemEnabledState(_miCopyArtist, !stopped);
            _menu.SetItemEnabledState(_miCopyTitle, !stopped);
            _menu.SetItemEnabledState(_miOpenContainingFolder, !stopped);
            _menu.SetItemEnabledState(_miSearchInInternet, !stopped);
        }

        private void OnFoobarShowHide(object sender, ValueEventArgs<bool> e)
        {
            if (_settingsManager.Settings.General.HideIfFoobar2000IsNotRunning)
            {
                ShowHide(e.Value);
                if (e.Value)
                {
                    OnPlaybackState(sender, new ValueEventArgs<bool>(false));
                }
            }
        }

        private void OnApplySettings(object sender, EventArgs e)
        {
            if (_floatingForm.Controls.Contains(_controlHost))
                _floatingForm.Controls.Remove(_controlHost);
            if (_band.Controls.Contains(_controlHost))
                _band.Controls.Remove(_controlHost);

            if (_settingsManager.Settings.General.FloatingMode)
            {
                _floatingForm.Controls.Add(_controlHost);

                ShowFloatingWindow();
                HideBand();

                _floatingForm.LoadSettings();
            }
            else
            {
                _band.Controls.Add(_controlHost);

                HideFloatingWindow();
                ShowBand();
            }
        }

        private void HideBand()
        {
            if (_band.MinSize.Width != 0)
            {
                _band.MinSize = new System.Drawing.Size(0, 0);
                _band.ExecBandInfoChangedCommand();
            }
        }

        private void ShowBand()
        {
            int bandSize = _settingsManager.Settings.General.BandSize;

            if (_band.MinSize.Width != bandSize)
            {
                _band.MinSize = new System.Drawing.Size(bandSize, 0);
                _band.ExecBandInfoChangedCommand();
            }
        }

        private void HideFloatingWindow()
        {
            if (_floatingForm.Visible)
                _floatingForm.Hide();
        }

        private void ShowFloatingWindow()
        {
            if (!_floatingForm.Visible)
                _floatingForm.Show();
        }

        private void OnClose(object sender, EventArgs e)
        {
            _settingsManager.SaveSettings();
        }

        private void OnSettingsMenuItemClick()
        {
            var wnd = new SettingsWindow();
            var context = new SettingsViewModel(wnd, _settingsManager);
            wnd.DataContext = context;
            wnd.Show();

            context.OnClose += (s, ea) => wnd.Close();
            context.OnApply += (s, ea) => { _controlHost.ApplySettings(); _settingsManager.SaveSettings(); };
        }

        private void OnOpenContainingFolderClick()
        {
            _controlHost.Controller.FoobarActions.FilePath(0);
        }

        private void OnSearchInInternetClick()
        {
            _controlHost.Controller.FoobarActions.FormatString(FormatStringIndex.InternetSearch, _settingsManager.Settings.General.InternetSearchFormat);
        }

        private void OnCopyArtist()
        {
            _controlHost.Controller.FoobarActions.FormatString(FormatStringIndex.CopyArtist, "%artist%");
        }

        private void OnCopyTitle()
        {
            _controlHost.Controller.FoobarActions.FormatString(FormatStringIndex.CopyTitle, "%title%");
        }

        private void OnCopyArtistAndTitle()
        {
            _controlHost.Controller.FoobarActions.FormatString(FormatStringIndex.CopyArtistAndTitle, "%artist% - %title%");
        }

        private void OnActivateFoobar(object sender, EventArgs e)
        {
            _controlHost.Controller.FoobarActions.ActivateFoobar();
        }
    }
}
