using Deskband.Common;
using Deskband.Communication;
using Deskband.Controls;
using Deskband.Core.WinApi;
using Deskband.Settings;
using DeskbandBridge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband
{
    public class Controller
    {
        public event EventHandler OnApplySettings;

        public event EventHandler<ValueEventArgs<bool>> OnFoobarShowHide;

        public event EventHandler<ValueEventArgs<bool>> OnPlaybackState;

        private readonly MessageForm _messageForm;
        private readonly FoobarActions _foobarActions;

        public FoobarActions FoobarActions { get { return _foobarActions; } }

        private readonly List<AeroLabel> _labels;
        private readonly List<AeroButton> _buttons;
        private readonly List<MediaTrackbar> _trackbars;
        private readonly List<AlbumArtPicture> _albumArts;

        private bool _stopped = true;
        private bool _paused;
        private bool _stop_after_current;

        public bool Stopped { get { return _stopped; } }

        private SettingsManager _settings;

        public Controller(
            SettingsManager settings,
            MessageForm messageForm,
            List<AeroLabel> labels,
            List<AeroButton> buttons,
            List<MediaTrackbar> trackbars,
            List<AlbumArtPicture> albumArts)
        {
            _settings = settings;

            _messageForm = messageForm;
            _foobarActions = new FoobarActions(_messageForm.Handle);

            _messageForm.OnThemeChanged += OnThemeChanged;

            _messageForm.OnFoobarShow += OnFoobarShow;
            _messageForm.OnFoobarHide += OnFoobarHide;
            _messageForm.OnTrackLength += OnTrackLength;
            _messageForm.OnTrackTime += OnTrackTime;
            _messageForm.OnTrackText += OnTrackText;
            _messageForm.OnPauseState += OnPauseState;
            _messageForm.OnStop += OnStop;
            _messageForm.OnTrackVolume += OnTrackVolume;
            _messageForm.OnStopAfterCurrentState += OnStopAfterCurrentState;
            _messageForm.OnAlbumArt += OnAlbumArt;
            _messageForm.OnFilePath += OnFilePath;
            _messageForm.OnVersion += OnVersion;

            _labels = labels;
            _buttons = buttons;
            _trackbars = trackbars;
            _albumArts = albumArts;
        }

        private void FireApplySettings()
        {
            if (OnApplySettings != null)
                OnApplySettings(this, EventArgs.Empty);
        }

        private void FirePlaybackState(bool state)
        {
            if (OnPlaybackState != null)
                OnPlaybackState(this, new ValueEventArgs<bool>(state));

            if (_settings.Settings.General.HideIfFoobar2000IsNotRunning && !_foobarActions.IsFoobarStarted)
            {
                if (OnFoobarShowHide != null)
                    OnFoobarShowHide(this, new ValueEventArgs<bool>(false));
            }
        }

        private void UpdateButtonIcons()
        {
            bool isPlaying = !_stopped && !_paused;

            foreach (var b in _buttons.Where(x => x.Kind == Enums.ButtonKindType.PlayPause))
            {
                b.ShowAdditionalImage = isPlaying;
                b.Refresh();
            }
            foreach (var b in _buttons.Where(x => x.Kind == Enums.ButtonKindType.StopAfterCurrent))
            {
                b.ShowAdditionalImage = _stop_after_current;
                b.Refresh();
            }
        }

        private void UpdateTexts()
        {
            for (int i = 0; i < _labels.Count(); i++)
            {
                _foobarActions.FormatString(i, _labels[i].Format);
            }
        }

        private void ClearTexts()
        {
            _labels.ForEach(x =>
            {
                x.Text = x.StoppedText;
            });
        }

        private void UpdatePosition(int pos, int? range = null)
        {
            foreach (var tb in _trackbars.Where(x => x.Kind == Enums.TrackbarKindType.Position))
            {
                if (range != null)
                    tb.Range = range.Value;
                tb.Position = pos;
            }
        }

        private void UpdateAlbumArt(System.Drawing.Image img, bool stub)
        {
            foreach (var a in _albumArts)
            {
                a.SetImage(img, stub, _settings.Settings.AlbumArt.DoNotShowStubImage);
            }
        }

        // Main events

        private void OnFoobarShow(object sender, EventArgs e)
        {
            if (OnFoobarShowHide != null)
                OnFoobarShowHide(sender, new ValueEventArgs<bool>(true));
        }

        private void OnFoobarHide(object sender, EventArgs e)
        {
            if (OnFoobarShowHide != null)
                OnFoobarShowHide(sender, new ValueEventArgs<bool>(false));
        }

        private void OnVersion(object sender, ValueEventArgs<string> e)
        {
            var version = e.Value;
            if (version != FB2KConstants.DeskbandControlsVersion)
            {
                var msg = String.Format("Plugin version mismatch! Expected \"{0}\" but found \"{1}\".\r\nPlease update plugin to expected version.", FB2KConstants.DeskbandControlsVersion, version);
                MessageBox.Show(msg, FB2KConstants.DeskbandControlsTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _messageForm.Lock();
                _foobarActions.SetVersion(true);
            }
            else
            {
                _foobarActions.SetVersion(false);
            }
        }

        private void OnThemeChanged(object sender, EventArgs e)
        {
            FireApplySettings();
        }

        private void OnTrackLength(object sender, ValueEventArgs<double> e)
        {
            _stopped = false;
            _paused = false;
            UpdateButtonIcons();
            UpdatePosition(0, (int)e.Value);
            UpdateAlbumArt(null, true);
            UpdateTexts();

            FirePlaybackState(true);
        }

        private void OnTrackTime(object sender, ValueEventArgs<double> e)
        {
            UpdateTexts();
            UpdatePosition((int)e.Value);

            // If we receive 'time' message and current state is stopped
            // it means that we have no track info, ask for it.
            if (_stopped)
                _foobarActions.ResendLastState();
        }

        private void OnTrackText(object sender, TrackTextEventArgs e)
        {
            if (e.Index >= 0)
            {
                if (e.Index < _labels.Count())
                {
                    var lbl = _labels[e.Index];
                    lbl.Text = e.Text;
                }
            }
            else
            {
                OnFormatStringIndex(e.Index, e.Text);
            }
        }

        private void OnPauseState(object sender, ValueEventArgs<bool> e)
        {
            _paused = e.Value;
            UpdateButtonIcons();
            UpdateTexts();

            FirePlaybackState(!e.Value);
        }

        private void OnStop(object sender, EventArgs e)
        {
            _paused = false;
            _stopped = true;
            UpdateButtonIcons();
            UpdatePosition(0);
            UpdateAlbumArt(null, true);
            ClearTexts();

            FirePlaybackState(false);
        }

        private void OnTrackVolume(object sender, ValueEventArgs<float> e)
        {
            //volume_in_percent = pow(10, (volume_in_db / 30)) * 100
            int volume = (int)(Math.Pow(10.0, (e.Value / 30.0)) * 100.0);

            foreach (var tb in _trackbars.Where(x => x.Kind == Enums.TrackbarKindType.Volume))
            {
                tb.Position = volume;
            }
        }

        private void OnStopAfterCurrentState(object sender, ValueEventArgs<bool> e)
        {
            _stop_after_current = e.Value;
            UpdateButtonIcons();
        }

        private void OnAlbumArt(object sender, ValueEventArgs<Tuple<byte[], bool>> e)
        {
            var img = ImageHelpers.GetImageFromByteArray(e.Value.Item1);
            var stub = e.Value.Item2;
            UpdateAlbumArt(img, stub);
        }

        private void OnFilePath(object sender, TrackTextEventArgs e)
        {
            if (e.Text.StartsWith("file://"))
            {
                var args = String.Format("/select,\"{0}\"", e.Text);
                Shell32.ShellExecute(IntPtr.Zero, "open", "explorer.exe", args, null, WinApiTypes.SW_SHOWNORMAL);
            }
        }

        private void OnFormatStringIndex(int index, string text)
        {
            switch (index)
            {
                case FormatStringIndex.InternetSearch:
                    {
                        var url = _settings.Settings.General.InternetSearchUrl.Replace("%q%", Uri.EscapeDataString(text));

                        //var url = String.Format("https://www.google.com/search?q={0}", Uri.EscapeDataString(text));
                        Shell32.ShellExecute(IntPtr.Zero, "open", url, null, null, WinApiTypes.SW_SHOWNORMAL);
                    }
                    break;

                case FormatStringIndex.CopyArtistAndTitle:
                case FormatStringIndex.CopyTitle:
                case FormatStringIndex.CopyArtist:
                    {
                        System.Windows.Forms.Clipboard.SetText(text);
                    }
                    break;
            }
        }

        public void UpdateControlHandlers()
        {
            _buttons.ForEach(x =>
            {
                if (x.Kind == Enums.ButtonKindType.PlayPause)
                    x.Click += (s, e) => _foobarActions.PlayPause();
                else if (x.Kind == Enums.ButtonKindType.Stop)
                    x.Click += (s, e) => _foobarActions.Stop();
                else if (x.Kind == Enums.ButtonKindType.Previous)
                    x.Click += (s, e) => _foobarActions.Previuos();
                else if (x.Kind == Enums.ButtonKindType.Next)
                    x.Click += (s, e) => _foobarActions.Next();
                else if (x.Kind == Enums.ButtonKindType.StopAfterCurrent)
                    x.Click += (s, e) => _foobarActions.ToggleStopAfterCurrent();
                else if (x.Kind == Enums.ButtonKindType.Random)
                    x.Click += (s, e) => _foobarActions.Random();
            });

            foreach (var tb in _trackbars.Where(x => x.Kind == Enums.TrackbarKindType.Position))
            {
                tb.OnPositionChanged += (s, p) => _foobarActions.Seek(p.Position);
            }

            foreach (var tb in _trackbars.Where(x => x.Kind == Enums.TrackbarKindType.Volume))
            {
                tb.OnPositionChanged += (s, p) =>
                    {
                        // volume_in_db = 30 * log10 (volume_in_percent / 100)
                        float vdb = (float)(30.0 * Math.Log10((float)p.Position / 100.0));
                        if (vdb < -100.0f) vdb = -100.0f;
                        _foobarActions.Volume(vdb);
                    };
            }

            if (_stopped)
            {
                _foobarActions.ResendLastNonTrackState();
                ClearTexts();
                FirePlaybackState(false);
            }
            else
            {
                _foobarActions.ResendLastState();
            }
        }

        public void SetVolumeDelta(int delta)
        {
            foreach (var tb in _trackbars.Where(x => x.Kind == Enums.TrackbarKindType.Volume))
            {
                tb.SetDelta(delta);
            }
        }
    }
}