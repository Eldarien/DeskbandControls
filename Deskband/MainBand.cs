using Deskband.Common;
using Deskband.Common.Extensions;
using Deskband.Communication;
using Deskband.Controls;
using Deskband.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Deskband
{
    [ComVisible(true)]
    [Guid(MainBand.ClassGuid)]
    [BandObject(MainBand.ClassTitle)]
    public class MainBand : BandObject
    {
        public const String ClassGuid = "9690ED28-CD24-4534-B380-77103A4E7774";

        public const String ClassTitle = "Deskband Controls";

        protected override string GetClassGuidString()
        {
            return ClassGuid;
        }

        private List<IDisposable> _disposeList = new List<IDisposable>();

        private T Disposable<T>(T disposable) where T : IDisposable
        {
            _disposeList.Add(disposable);
            return disposable;
        }

        private ControlHost _host;
        private FloatingForm _floatingForm;

        private MenuItem _miStop;
        private MenuItem _miPlayPause;
        private MenuItem _Prev;
        private MenuItem _Next;
        private MenuItem _Random;
        private MenuItem _ToggleStopAfterCurrent;

        private MenuItem _miCopyArtistAndTitle;
        private MenuItem _miCopyTitle;
        private MenuItem _miCopyArtist;
        private MenuItem _miOpenContainingFolder;
        private MenuItem _miSearchInInternet;
        private MenuItem _miSettings;

        public MainBand()
        {
            // Entry Point

            Title = MainBand.ClassTitle;
            BackColor = Color.Transparent;

            AssemblyResolver.Initialize();
            SettingsManager.Instance.LoadSettings();

            _host = Disposable(new ControlHost());
            _host.OnApplySettings += OnApplySettings;
            _host.OnPlaybackState += OnPlaybackState;

            _floatingForm = Disposable(new FloatingForm());

            // DoubleClick
            DoubleClick += OnActivateFoobar;
            _floatingForm.DoubleClick += OnActivateFoobar;

            // ContextMenu
            var contextMenu = Disposable(new ContextMenu());

            _miStop = contextMenu.MenuItems.Add("Stop", (s, ea) => _host.Controller.FoobarActions.Stop());
            _miPlayPause = contextMenu.MenuItems.Add("Play / Pause", (s, ea) => _host.Controller.FoobarActions.PlayPause());
            _Prev = contextMenu.MenuItems.Add("Previous", (s, ea) => _host.Controller.FoobarActions.Previuos());
            _Next = contextMenu.MenuItems.Add("Next", (s, ea) => _host.Controller.FoobarActions.Next());
            _Random = contextMenu.MenuItems.Add("Random", (s, ea) => _host.Controller.FoobarActions.Random());
            _ToggleStopAfterCurrent = contextMenu.MenuItems.Add("Toggle Stop After Current", (s, ea) => _host.Controller.FoobarActions.ToggleStopAfterCurrent());

            contextMenu.MenuItems.Add("-");
            _miCopyArtistAndTitle = contextMenu.MenuItems.Add("Copy Artist and Title", OnCopyArtistAndTitle);
            _miCopyTitle = contextMenu.MenuItems.Add("Copy Title", OnCopyTitle);
            _miCopyArtist = contextMenu.MenuItems.Add("Copy Artist", OnCopyArtist);
            _miOpenContainingFolder = contextMenu.MenuItems.Add("Open Containing Folder", OnOpenContainingFolderClick);
            _miSearchInInternet = contextMenu.MenuItems.Add("Search in Internet", OnSearchInInternetClick);

            contextMenu.MenuItems.Add("-");
            _miSettings = contextMenu.MenuItems.Add("Settings", OnSettingsMenuItemClick);
            this.ContextMenu = contextMenu;
            _floatingForm.ContextMenu = contextMenu;

            _host.ApplySettings();
        }

        private void OnPlaybackState(object sender, ValueEventArgs<bool> e)
        {
            var settings = SettingsManager.Instance.Settings;
            if (settings.General.HideIfNotPlaying && !e.Value)
            {
                if (settings.General.FloatingMode)
                    HideFloatingWindow();
                else
                    HideBand();
            }
            else
            {
                if (settings.General.FloatingMode)
                    ShowFloatingWindow();
                else
                    ShowBand();
            }

            var stopped = _host.Controller.Stopped;

            _miStop.Enabled = !stopped;
            _ToggleStopAfterCurrent.Enabled = !stopped;

            _miCopyArtistAndTitle.Enabled = !stopped;
            _miCopyArtist.Enabled = !stopped;
            _miCopyTitle.Enabled = !stopped;
            _miOpenContainingFolder.Enabled = !stopped;
            _miSearchInInternet.Enabled = !stopped;
        }

        private void OnApplySettings(object sender, EventArgs e)
        {
            var settings = SettingsManager.Instance.Settings;

            if (_floatingForm.Controls.Contains(_host))
                _floatingForm.Controls.Remove(_host);
            if (Controls.Contains(_host))
                Controls.Remove(_host);

            if (settings.General.FloatingMode)
            {
                _floatingForm.Controls.Add(_host);

                ShowFloatingWindow();
                HideBand();

                _floatingForm.LoadSettings();
            }
            else
            {
                Controls.Add(_host);

                HideFloatingWindow();
                ShowBand();
            }
        }

        private void HideBand()
        {
            if (MinSize.Width != 0)
            {
                MinSize = new Size(0, 0);
                ExecBandInfoChangedCommand();
            }
        }

        private void ShowBand()
        {
            int bandSize = SettingsManager.Instance.Settings.General.BandSize;

            if (MinSize.Width != bandSize)
            {
                MinSize = new Size(bandSize, 0);
                ExecBandInfoChangedCommand();
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

        protected override void OnClose()
        {
            SettingsManager.Instance.SaveSettings();

            for (int i = _disposeList.Count - 1; i >= 0; i--)
            {
                _disposeList[i].Dispose();
                _disposeList[i] = null;
            }
        }

        private void OnSettingsMenuItemClick(object sender, EventArgs e)
        {
            var wnd = new Deskband.Settings.SettingsWindow();
            var context = new Deskband.Settings.SettingsViewModel(wnd);
            wnd.DataContext = context;
            wnd.Show();

            context.OnClose += (s, ea) => wnd.Close();
            context.OnApply += (s, ea) => { _host.ApplySettings(); SettingsManager.Instance.SaveSettings(); };
        }

        private void OnOpenContainingFolderClick(object sender, EventArgs e)
        {
            _host.Controller.FoobarActions.FilePath(0);
        }

        private void OnSearchInInternetClick(object sender, EventArgs e)
        {
            _host.Controller.FoobarActions.FormatString(FormatStringIndex.InternetSearch, SettingsManager.Instance.Settings.General.InternetSearchFormat);
        }

        private void OnCopyArtist(object sender, EventArgs e)
        {
            _host.Controller.FoobarActions.FormatString(FormatStringIndex.CopyArtist, "%artist%");
        }

        private void OnCopyTitle(object sender, EventArgs e)
        {
            _host.Controller.FoobarActions.FormatString(FormatStringIndex.CopyTitle, "%title%");
        }

        private void OnCopyArtistAndTitle(object sender, EventArgs e)
        {
            _host.Controller.FoobarActions.FormatString(FormatStringIndex.CopyArtistAndTitle, "%artist% - %title%");
        }

        private void OnActivateFoobar(object sender, EventArgs e)
        {
            _host.Controller.FoobarActions.ActivateFoobar();
        }
    }
}