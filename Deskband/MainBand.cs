using Deskband.Common;
using Deskband.Common.Extensions;
using Deskband.Communication;
using Deskband.Console;
using Deskband.Controls;
using Deskband.Native;
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

        private IntPtr _taskbarWindowHandle;

        private List<IDisposable> _disposeList = new List<IDisposable>();

        private T Disposable<T>(T disposable) where T : IDisposable
        {
            _disposeList.Add(disposable);
            return disposable;
        }

        private ControlHost _host;
        private FloatingForm _floatingForm;

        private ConsoleHandler _consoleHandler;

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
        private MenuItem _miConsole;
        private MenuItem _miSettings;

        public MainBand()
        {
            // Entry Point
            _taskbarWindowHandle = WinApi.FindWindow("Shell_TrayWnd", null);

            Title = MainBand.ClassTitle;
            BackColor = Color.Transparent;

            AssemblyResolver.Initialize();
            SettingsManager.Instance.LoadSettings();

            _host = Disposable(new ControlHost());
            _host.OnApplySettings += OnApplySettings;
            _host.OnPlaybackState += OnPlaybackState;
            _host.OnFoobarShowHide += OnFoobarShowHide;

            _floatingForm = Disposable(new FloatingForm());

            // DoubleClick
            DoubleClick += OnActivateFoobar;
            _floatingForm.DoubleClick += OnActivateFoobar;

            // Console
            _consoleHandler = new ConsoleHandler();
            _consoleHandler.OnConsoleToggle += (s, e) => _miConsole.Checked = e.Value;

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
            _miConsole = contextMenu.MenuItems.Add("Console", (s, e) => _consoleHandler.ToggleConsole());
            _miSettings = contextMenu.MenuItems.Add("Settings", OnSettingsMenuItemClick);
            this.ContextMenu = contextMenu;
            _floatingForm.ContextMenu = contextMenu;

            // Startup complete
            _consoleHandler.AddLine("Deskband Controls started");


            _host.ApplySettings();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WinApi.WM_NCHITTEST)
            {
                int x = ((int)m.LParam).LowWord();
                int y = ((int)m.LParam).HighWord();

                var point = new WinApi.POINT { X = x, Y = y };
                if (WinApi.ScreenToClient(_taskbarWindowHandle, ref point))
                {
                    WinApi.RECT r;
                    WinApi.GetWindowRect(_taskbarWindowHandle, out r);
                    bool isHorizontal = (r.right - r.left) > (r.bottom - r.top);

                    if (isHorizontal && point.Y == 0 || !isHorizontal && point.X == 0)
                    {
                        m.Result = (IntPtr)WinApi.HTTRANSPARENT;
                        return;
                    }
                }
            }

            base.WndProc(ref m);
        }

        private void ShowHide(bool show)
        {
            var settings = SettingsManager.Instance.Settings;
            if (show)
            {
                if (settings.General.FloatingMode)
                    ShowFloatingWindow();
                else
                    ShowBand();
            }
            else
            {
                if (settings.General.FloatingMode)
                    HideFloatingWindow();
                else
                    HideBand();
            }
        }

        private void OnPlaybackState(object sender, ValueEventArgs<bool> e)
        {
            var settings = SettingsManager.Instance.Settings;

            if (settings.General.HideIfNotPlaying && !e.Value)
                ShowHide(false);
            else
                ShowHide(true);

            var stopped = _host.Controller.Stopped;

            _miStop.Enabled = !stopped;
            _ToggleStopAfterCurrent.Enabled = !stopped;

            _miCopyArtistAndTitle.Enabled = !stopped;
            _miCopyArtist.Enabled = !stopped;
            _miCopyTitle.Enabled = !stopped;
            _miOpenContainingFolder.Enabled = !stopped;
            _miSearchInInternet.Enabled = !stopped;
        }

        private void OnFoobarShowHide(object sender, ValueEventArgs<bool> e)
        {
            var settings = SettingsManager.Instance.Settings;
            if (settings.General.HideIfFoobar2000IsNotRunning)
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
            var wnd = new SettingsWindow();
            var context = new SettingsViewModel(wnd);
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