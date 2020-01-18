using dcmFoobar2000.Configuration;
using dcmFoobar2000.Properties;
using Deskband.Core.Common;
using Deskband.Core.Controls;
using Deskband.Core.Extensions;
using Deskband.Core.Interfaces;
using Deskband.Core.WinApi;
using DeskbandBridge;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace dcmFoobar2000.Code
{
    public class Controller : IDisposable
    {
        private Guid _id;
        private ISizeProvider _sp;
        private IConsole _console;
        private IConfigurationProvider _config;
        private IMenuProvider _menu;
        private IModuleContainer _mcontainer;
        private Foobar2000Actions _actions;
        private ILastActiveWindowActivator _lastActiveWindowActivator;
        private ITooltipProvider _tooltipProvider;

        private ConfigurationModel _cfg;

        private MessageForm _messageForm;

        private DisposableContainer _disposable;

        private Timer _hideTimer;
        private const int _hideTimerInitialInterval = 1000;
        private const int _hideTimerRegularInterval = 300;

        private AlbumArtAccessor _aaAccessor;
        private Timer _aaStubTimer;
        private const int _aaStubTimerInterval = 1500;

        private Timer _visualizationTimer;

        private bool _eventsInitialized;

        private bool _stopped = true;
        private bool _paused;
        private bool _stop_after_current;
        private float _volume;
        private float _volume_step;

        // Controls

        private dcPicture _picAlbumArt;

        private dcButton _btnStop;
        private dcButton _btnPlayPause;
        private dcButton _btnPrev;
        private dcButton _btnNext;
        private dcButton _btnRandom;
        private dcButton _btnStopAC;

        private dcTrackbar _trbPosition;
        private dcTrackbar _trbVolume;

        private dcLevelbar _levelRight;
        private dcLevelbar _levelLeft;

        private List<dcLabel> _labels = new List<dcLabel>();

        private DisposableContainer _controls;

        public Controller(
            ISizeProvider sp,
            IConsole console,
            IConfigurationProvider config,
            IMenuProvider menu,
            IModuleContainer mcontainer,
            Foobar2000Actions actions,
            ILastActiveWindowActivator lastActiveWindowActivator,
            ITooltipProvider tooltipProvider,
            MessageForm messageForm
            )
        {
            _id = Foobar2000Module.ModuleId;
            _sp = sp;
            _console = console;
            _config = config;
            _menu = menu;
            _mcontainer = mcontainer;
            _actions = actions;
            _lastActiveWindowActivator = lastActiveWindowActivator;
            _tooltipProvider = tooltipProvider;
            _messageForm = messageForm;

            _disposable = new DisposableContainer();

            _hideTimer = _disposable.Add(new Timer());
            _hideTimer.Interval = _hideTimerInitialInterval;
            _hideTimer.Tick += (s, e) => HandleHideTimerTick();

            _aaAccessor = _disposable.Add(new AlbumArtAccessor());
            _aaStubTimer = _disposable.Add(new Timer());
            _aaStubTimer.Interval = _aaStubTimerInterval;
            _aaStubTimer.Tick += (s, e) => HandleAlbumArtStubTick();

            _visualizationTimer = _disposable.Add(new Timer());
            _visualizationTimer.Interval = 12;
            _visualizationTimer.Tick += (s, e) => HandleVisualizationTimerTick();
            _visualizationTimer.Enabled = true;

            _controls = new DisposableContainer();
        }

        public void ApplyConfiguration()
        {
            _cfg = _config.GetConfiguration(Foobar2000Module.ModuleId, ConfigurationModel.Default);
            _config.UpdateConfiguration(_cfg);

            RegisterMenu();
            RegisterControls();

            if (!_eventsInitialized)
            {
                InitMessageFormEvents();
                _eventsInitialized = true;
            }

            UpdateAlbumArt();

            _actions.Init(_stopped, _cfg);

            if (_stopped)
            {
                HandleStop();
            }
        }

        public void Dispose()
        {
            _config.UpdateConfiguration(_cfg);

            _messageForm.Lock();
            _controls.Dispose();
            _disposable.Dispose();

        }

        private T CreateControl<T>() where T : Control, new()
        {
            return AddControl(new T());
        }

        private T AddControl<T>(T control) where T : Control
        {
            return _controls.Add(control);
        }

        private void RemoveAndDestroyControl<T>(T control) where T : Control
        {
            _controls.DisposeRemoveItem(control);
        }

        private void AddControlToModuleContainer(Control control)
        {
            _mcontainer.AddControl(Foobar2000Module.ModuleId, control);
        }

        private Guid _miStop;
        private Guid _miPlayPause;
        private Guid _miPrev;
        private Guid _miNext;
        private Guid _miRandom;
        private Guid _miToggleStopAC;
        private Guid _miCopyArtistAndTitle;
        private Guid _miCopyTitle;
        private Guid _miCopyArtist;
        private Guid _miOpenContainingFolder;
        private Guid _miSearchInInternet;

        private void RegisterMenu()
        {
            var m = _cfg.Menu;

            //var group = Foobar2000Module.ModuleName;
            //_menu.ClearGroup(group);
            _menu.ClearByModule(_id);

            if (m.Enabled)
            {
                _miStop = m.Stop ? _menu.AddItem(_id, null, "Stop", () => { _actions.Stop(); _lastActiveWindowActivator.Activate(); }) : Guid.Empty;
                _miPlayPause = m.PlayPause ? _menu.AddItem(_id, null, "Play / Pause", () => { _actions.PlayPause(); _lastActiveWindowActivator.Activate(); }) : Guid.Empty;
                _miPrev = m.Previous ? _menu.AddItem(_id, null, "Previous", () => { _actions.Prev(); _lastActiveWindowActivator.Activate(); }) : Guid.Empty;
                _miNext = m.Next ? _menu.AddItem(_id, null, "Next", () => { _actions.Next(); _lastActiveWindowActivator.Activate(); }) : Guid.Empty;
                _miRandom = m.Random ? _menu.AddItem(_id, null, "Random", () => { _actions.Random(); _lastActiveWindowActivator.Activate(); }) : Guid.Empty;
                _miToggleStopAC = m.StopAfterCurrent ? _menu.AddItem(_id, null, "Stop After Current", () => { _actions.ToggleStopAfterCurrent(); _lastActiveWindowActivator.Activate(); }) : Guid.Empty;

                if (m.Stop || m.PlayPause || m.Previous || m.Next || m.Random || m.StopAfterCurrent)
                {
                    _menu.AddItem(_id, null, "-", null);
                }

                _miCopyArtistAndTitle = m.CopyArtistAndTitle ? _menu.AddItem(_id, null, "Copy Artist and Title", CopyArtistAndTitle) : Guid.Empty;
                _miCopyTitle = m.CopyTitle ? _menu.AddItem(_id, null, "Copy Title", CopyTitle) : Guid.Empty;
                _miCopyArtist = m.CopyArtist ? _menu.AddItem(_id, null, "Copy Artist", CopyArtist) : Guid.Empty;
                _miOpenContainingFolder = m.OpenContainingFolder ? _menu.AddItem(_id, null, "Open Containing Folder", OpenContainingFolder) : Guid.Empty;
                _miSearchInInternet = m.SearchInInternet ? _menu.AddItem(_id, null, "Search in Internet", SearchInInternet) : Guid.Empty;

                if (m.CopyArtistAndTitle || m.CopyTitle || m.CopyArtist || m.OpenContainingFolder || m.SearchInInternet)
                {
                    _menu.AddItem(_id, null, "-", null);
                }
            }
        }

        private void RegisterControls()
        {
            _mcontainer.ClearControls(Foobar2000Module.ModuleId);
            _controls.DisposeRemoveItems();

            _picAlbumArt = CreateAlbumArt(_cfg.AlbumArt);
            AddControlToModuleContainer(_picAlbumArt);

            _btnStop = CreateButton(_cfg.Buttons.BtnStop, Resources.Icon_Stop, null,
                () => { if (_stopped) HandleStop(); else _actions.Stop(); _lastActiveWindowActivator.Activate(); });
            AddControlToModuleContainer(_btnStop);

            _btnPlayPause = CreateButton(_cfg.Buttons.BtnPlayPause, Resources.Icon_Play, Resources.Icon_Pause,
                () => { _actions.PlayPause(); _lastActiveWindowActivator.Activate(); });
            AddControlToModuleContainer(_btnPlayPause);

            _btnPrev = CreateButton(_cfg.Buttons.BtnPrev, Resources.Icon_Prev, null,
                () => { _actions.Prev(); _lastActiveWindowActivator.Activate(); });
            AddControlToModuleContainer(_btnPrev);

            _btnNext = CreateButton(_cfg.Buttons.BtnNext, Resources.Icon_Next, null,
                () => { _actions.Next(); _lastActiveWindowActivator.Activate(); });
            AddControlToModuleContainer(_btnNext);

            _btnRandom = CreateButton(_cfg.Buttons.BtnRandom, Resources.Icon_Random, null,
                () => { _actions.Random(); _lastActiveWindowActivator.Activate(); });
            AddControlToModuleContainer(_btnRandom);

            _btnStopAC = CreateButton(_cfg.Buttons.BtnStopAC, Resources.Icon_StopAfterCurrentOff, Resources.Icon_StopAfterCurrentOn,
                () => { _actions.ToggleStopAfterCurrent(); _lastActiveWindowActivator.Activate(); });
            AddControlToModuleContainer(_btnStopAC);

            _trbPosition = CreateTrackbar(_cfg.PositionBar, true,
                p => _actions.Seek(p), () => _lastActiveWindowActivator.Activate());
            AddControlToModuleContainer(_trbPosition);

            _trbVolume = CreateTrackbar(_cfg.VolumeBar, false,
                p => SetVolume(p), () => _lastActiveWindowActivator.Activate());
            AddControlToModuleContainer(_trbVolume);

            _levelLeft = CreateLevelbar(_cfg.PeakMeter, true);
            AddControlToModuleContainer(_levelLeft);

            _levelRight = CreateLevelbar(_cfg.PeakMeter, false);
            AddControlToModuleContainer(_levelRight);

            _labels.Clear();
            foreach (var text in _cfg.Texts)
            {
                var label = CreateLabel(text);
                _labels.Add(label);
                AddControlToModuleContainer(label);
            }
        }

        private Image MakeColorizedImageFromIcon(Icon icon, Color color)
        {
            using (Image image = icon.ToBitmap())
            {
                return ImageHelpers.Colorize(image, color);
            }
        }

        private dcButton CreateButton(ButtonSettings settings, Icon icon1, Icon icon2, Action action)
        {
            var btn = CreateControl<dcButton>();
            btn.Visible = settings.Visible;
            btn.Location = _sp.MakePoint(settings.X, settings.Y);
            btn.Size = _sp.MakeSize(settings.Width, settings.Height);

            Image image1 = null;
            if (settings.Icon1Path != null)
            {
                image1 = ImageHelpers.GetImageFromFile(PathHelpers.ResolvePath(settings.Icon1Path, _cfg.PathToFoobar2000));
            }
            if (icon1 != null && image1 == null)
            {
                image1 = MakeColorizedImageFromIcon(icon1, settings.ColorizeColor);
            }
            if (image1 != null)
            {
                btn.SetImage(image1);
                if (image1 != ImageHelpers.Empty)
                {
                    image1.Dispose();
                    image1 = null;
                }
            }

            Image image2 = null;
            if (settings.Icon2Path != null)
            {
                image2 = ImageHelpers.GetImageFromFile(PathHelpers.ResolvePath(settings.Icon2Path, _cfg.PathToFoobar2000));
            }
            if (icon2 != null && image2 == null)
            {
                image2 = MakeColorizedImageFromIcon(icon2, settings.ColorizeColor);
            }
            if (image2 != null)
            {
                btn.SetImage2(image2);
                if (image2 != ImageHelpers.Empty)
                {
                    image2.Dispose();
                    image2 = null;
                }
            }

            btn.Click += (s, e) => action();
            return btn;
        }

        private dcPicture CreateAlbumArt(AlbumArtSettings settings)
        {
            var aa = CreateControl<dcPicture>();
            aa.Visible = settings.Visible;
            aa.Location = _sp.MakePoint(settings.X, settings.Y);
            aa.Size = _sp.MakeSize(settings.Width, settings.Height);
            aa.PreserveAspectRatio = settings.PreserveAspectRatio;
            return aa;
        }

        private dcTrackbar CreateTrackbar(TrackbarSettings settings, bool changeOnMouseUp, Action<int> action, Action mouseUpAction)
        {
            var trb = CreateControl<dcTrackbar>();
            trb.Visible = settings.Visible;
            trb.Location = _sp.MakePoint(settings.X, settings.Y);
            trb.Size = _sp.MakeSize(settings.Width, settings.Heigth);
            trb.ForeColor = settings.Color;
            trb.BackgroundColor = settings.BackgroundColor;
            trb.HideBorders = settings.HideBorders;
            trb.Range = 100;
            trb.Position = 0;
            trb.PaddingTop = settings.PaddingTop;
            trb.PaddingBottom = settings.PaddingBottom;
            trb.ChangeOnMouseUp = changeOnMouseUp;
            trb.OnPositionChanged += (s, e) => action(e.Value);
            trb.MouseUp += (s, e) => mouseUpAction();
            return trb;
        }

        private dcLabel CreateLabel(TextSettingsBase settings)
        {
            var fs = FontStyles.Regular;
            if (settings.FontStyleBold) fs = fs | FontStyles.Bold;
            if (settings.FontStyleItalic) fs = fs | FontStyles.Italic;
            var lbl = new dcLabel(_sp.DPI, new FontConfiguration(settings.FontName, settings.FontSize, fs));
            AddControl(lbl);
            lbl.Visible = settings.Visible;
            lbl.Location = _sp.MakePoint(settings.X, settings.Y);
            lbl.Size = _sp.MakeSize(settings.Width, settings.Height);
            lbl.ForeColor = settings.FontColor;
            lbl.TextAlign = settings.HorizontalAlign;
            lbl.ShadowColor = settings.ShadowColor;
            lbl.ShadowOffset = settings.ShadowOffset;
            lbl.BackgroundColor = settings.BackgroundColor;
            lbl.EnableScrolling = settings.EnableScroll;
            lbl.ScrollSpeed = settings.ScrollSpeed;
            lbl.ScrollStep = settings.ScrollStep;
            lbl.ScrollSeparator = settings.ScrollSeparator;

            if (settings.DetectHttpLinks)
            {
                lbl.HandleClicks = true;
                lbl.Click += Lbl_Click;
            }
            return lbl;
        }

        private dcLevelbar CreateLevelbar(LevelbarSettings settings, bool leftChannel)
        {
            var lvb = CreateControl<dcLevelbar>();
            lvb.Visible = settings.Visible;
            lvb.Location = leftChannel
                ? _sp.MakePoint(settings.LeftChannelX, settings.LeftChannelY)
                : _sp.MakePoint(settings.RightChannelX, settings.RightChannelY);
            lvb.Size = _sp.MakeSize(settings.Width, settings.Heigth);
            lvb.PrimarySegmentColor = settings.PrimarySegmentColor;
            lvb.SecondarySegmentColor = settings.SecondarySegmentColor;
            lvb.BackgroundColor = settings.BackgroundColor;
            lvb.InactiveSegmentColor = settings.InactiveSegmentColor;
            lvb.SegmentsCount = settings.SegmentsCount;
            lvb.TransitionPoint = settings.TransitionPoint;
            lvb.StripedSegments = settings.StripedSegments;
            lvb.SegmentSpaceRatio = settings.SegmentSpaceRatio;
            lvb.Range = 100;
            lvb.Position = 0;
            lvb.PaddingTop = settings.PaddingTop;
            lvb.PaddingBottom = settings.PaddingBottom;

            return lvb;
        }

        private void Lbl_Click(object sender, EventArgs e)
        {
            var lbl = (dcLabel)sender;
            var urlMatch = Regex.Match(lbl.Text, @"((http|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (urlMatch.Success)
            {
                Shell32.ShellExecute(IntPtr.Zero, "open", urlMatch.Value, null, null, WinApiTypes.SW_SHOWNORMAL);
            }
        }

        private void UpdateButtonIcons()
        {
            bool isPlaying = !_stopped && !_paused;

            _btnPlayPause.ShowAdditionalImage = isPlaying;
            _btnPlayPause.Refresh();

            _btnStopAC.ShowAdditionalImage = _stop_after_current;
            _btnStopAC.Refresh();
        }

        private void HandleAlbumArtStubTick()
        {
            _aaStubTimer.Enabled = false;
            _aaAccessor.SetBitmap(null, true);
            UpdateAlbumArt();
        }

        private void ResolveAlbumArtImage(dcPicture pic, AlbumArtSettings aaSettings)
        {
            if (pic == null)
                return;

            var aaData = _aaAccessor.GetBitmapData();
            Image image = aaData.Bitmap;
            bool dispose = false;
            if (aaSettings.DoNotShowStubImage && aaData.IsStub)
            {
                image = null;
            }
            if (image == null)
            {
                if (aaSettings.StubImagePath != null)
                {
                    image = ImageHelpers.GetImageFromFile(PathHelpers.ResolvePath(aaSettings.StubImagePath, _cfg.PathToFoobar2000));
                    dispose = true;
                }
            }
            if (image == null || image == ImageHelpers.Empty)
            {
                image = Resources.Image_NoCoverArt;
            }
            pic.SetImage(image); // pic creates its own copy of image
            if (dispose)
            {
                image.Dispose();
            }
        }

        private void UpdateAlbumArt()
        {
            ResolveAlbumArtImage(_picAlbumArt, _cfg.AlbumArt);
            ResolveAlbumArtImage(_tooltipAlbumArt, _cfg.Tooltip.AlbumArt);
            SetTooltipLabelsImage();
        }

        private void UpdatePosition(int pos, int? range = null)
        {
            if (range != null)
                _trbPosition.Range = range.Value;
            _trbPosition.Position = pos;
        }

        private void UpdateTexts()
        {
            for (int i = 0; i < _labels.Count; i++)
            {
                var index = i;
                _actions.FormatString(_cfg.Texts[i].GetFormatFromPausedState(_paused),
                    s => { if (_labels.TryGetElementAt(index, out var lbl)) lbl.Text = s; });
            }

            for (int i = 0; i < _tooltipLabels.Count; i++)
            {
                var index = i;
                _actions.FormatString(_cfg.Tooltip.Texts[i].GetFormatFromPausedState(_paused),
                    s => { if (_tooltipLabels.TryGetElementAt(index, out var lbl)) lbl.Text = s; });
            }
        }

        private void UpdateStoppedTexts()
        {
            _cfg.Texts.Zip(_labels, (settings, lbl) => new { settings, lbl }).ToList().ForEach(x =>
            {
                _actions.FormatString(x.settings.StoppedText, s => x.lbl.Text = s);
            });
        }

        private void ResetTextsScrollPosition()
        {
            _labels.ForEach(x => x.ResetScrollPosition());
        }

        private void InitMessageFormEvents()
        {
            _messageForm.OnThemeChanged += (s, e) => ApplyConfiguration();
            _messageForm.OnFoobarShow += (s, e) => ShowOrHide(true);
            _messageForm.OnFoobarHide += (s, e) => ShowOrHide(false);
            _messageForm.OnTrackLength += (s, e) => HandleTrackLength(e.Value);
            _messageForm.OnTrackTime += (s, e) => HandleTrackTime(e.Value);
            _messageForm.OnPauseState += (s, e) => HandlePauseState(e.Value);
            _messageForm.OnStop += (s, e) => HandleStop();
            _messageForm.OnTrackVolume += (s, e) => HandleVolume(e.Value.Item1, e.Value.Item2);
            _messageForm.OnStopAfterCurrentState += (s, e) => HandleStopAfterCurrent(e.Value);
            _messageForm.OnAlbumArt += (s, e) => HandleAlbumArt(e.Value.Item1, e.Value.Item2);
            _messageForm.OnVersion += (s, e) => HandleVersion(e.Value);
            _messageForm.OnPlaylist += (s, e) => HandlePlaylist(e.CurrentIndex, e.Playlist);
            _messageForm.OnVisualizationData += (s, e) => HandleVisualizationData(e.ChannelCount, e.Samples);
        }

        private void ShowOrHide(bool state)
        {
            if (_cfg.HideIfFoobar2000IsNotRunning)
            {
                if (state)
                {
                    HandlePlaybackState_Ex(!_stopped);
                }
                else
                {
                    _mcontainer.Hide(Foobar2000Module.ModuleId);
                }
            }
        }

        private void HandleTrackLength(double length)
        {
            _stopped = false;
            _paused = false;
            UpdateButtonIcons();
            UpdatePosition(0, (int)length);
            UpdateTexts();
            ResetTextsScrollPosition();

            HandlePlaybackState(true);
        }

        private void HandleTrackTime(double time)
        {
            UpdateTexts();
            UpdatePosition((int)time);

            // If we receive 'time' message and current state is stopped
            // it means that we have no track info, ask for it.
            if (_stopped)
                _actions.ResendLastState();
        }

        private void HandlePauseState(bool state)
        {
            _paused = state;
            UpdateButtonIcons();
            UpdateTexts();

            HandlePlaybackState(!state);
        }

        private void HandleStop()
        {
            _paused = false;
            _stopped = true;
            _aaStubTimer.Enabled = true;

            UpdateButtonIcons();
            UpdatePosition(0);
            UpdateStoppedTexts();
            HandlePlaybackState(false);
        }

        private const double _volumeDbCoeff = 34.0; // this value matches best with foobar2000 volume control behaviour

        private float LimitVolume(float volumeDb) => Math.Min(Math.Max(volumeDb, -100.0f), 0.0f);

        //volume_in_percent = pow(10, (volume_in_db / coeff)) * 100
        private int VolumeDbToPercent(float volumeDb) => (int)(Math.Pow(10.0, (volumeDb / _volumeDbCoeff)) * 100.0);

        // volume_in_db = coeff * log10 (volume_in_percent / 100)
        private float PercentToVolumeDb(int percent) => LimitVolume((float)(_volumeDbCoeff * Math.Log10(percent / 100.0)));

        private void HandleVolume(float volume, float step)
        {
            _volume = volume;
            _volume_step = step;
            _trbVolume.Position = VolumeDbToPercent(volume);
        }

        private void SetVolume(int percent)
        {
            _actions.Volume(PercentToVolumeDb(percent));
        }

        public void MouseWheel(int delta)
        {
            switch (_cfg.MouseWheelMode)
            {
                case MouseWheelMode.Volume:
                    {
                        _actions.Volume(LimitVolume(_volume + (delta / 120) * _volume_step));
                    }
                    break;
                case MouseWheelMode.Position:
                    {
                        var p = Math.Min(Math.Max(_trbPosition.Position + (delta / 120), 0), _trbPosition.Range);
                        _actions.Seek(p);
                    }
                    break;
            }
        }

        private void HandleStopAfterCurrent(bool state)
        {
            _stop_after_current = state;
            _menu.SetItemCheckedState(_miToggleStopAC, state);
            UpdateButtonIcons();
        }

        private void HandleAlbumArt(byte[] imageBytes, bool stub)
        {
            var bmp = ImageHelpers.GetImageFromByteArray(imageBytes);
            _aaAccessor.SetBitmap(bmp, stub);
            _aaStubTimer.Enabled = false;

            UpdateAlbumArt();
        }

        private void HandleVersion(string version)
        {
            if (version != FB2KConstants.DeskbandControlsVersion)
            {
                var msg = String.Format("Plugin version mismatch! Expected \"{0}\" but found \"{1}\".\r\nPlease update plugin to expected version.", FB2KConstants.DeskbandControlsVersion, version);
                MessageBox.Show(msg, FB2KConstants.DeskbandControlsTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _messageForm.Lock();
                _actions.SetVersion(true);
            }
            else
            {
                _actions.SetVersion(false);
                _actions.Init(_stopped, _cfg);
            }
        }

        private void HandlePlaybackState(bool state)
        {
            HandlePlaybackState_Ex(state);

            if (_cfg.HideIfFoobar2000IsNotRunning == true && !_actions.IsFoobarStarted)
            {
                ShowOrHide(state);
            }
        }

        private void HandlePlaybackState_Ex(bool state)
        {
            if (_cfg.HideIfNotPlaying && !state)
            {
                _hideTimer.Enabled = true;
            }
            else
            {
                _hideTimer.Enabled = false;
                _mcontainer.Show(Foobar2000Module.ModuleId);
            }

            _menu.SetItemEnabledState(_miStop, !_stopped);
            _menu.SetItemEnabledState(_miToggleStopAC, !_stopped);
            _menu.SetItemEnabledState(_miCopyArtistAndTitle, !_stopped);
            _menu.SetItemEnabledState(_miCopyArtist, !_stopped);
            _menu.SetItemEnabledState(_miCopyTitle, !_stopped);
            _menu.SetItemEnabledState(_miOpenContainingFolder, !_stopped);
            _menu.SetItemEnabledState(_miSearchInInternet, !_stopped);
        }

        private void HandleHideTimerTick()
        {
            _hideTimer.Enabled = false;
            _hideTimer.Interval = _hideTimerRegularInterval;
            _mcontainer.Hide(Foobar2000Module.ModuleId);
        }

        public void CopyArtistAndTitle()
        {
            _actions.FormatString("%artist% - %title%", s => Clipboard.SetText(s));
            _lastActiveWindowActivator.Activate();
        }

        public void CopyTitle()
        {
            _actions.FormatString("%title%", s => Clipboard.SetText(s));
            _lastActiveWindowActivator.Activate();
        }

        public void CopyArtist()
        {
            _actions.FormatString("%artist%", s => Clipboard.SetText(s));
            _lastActiveWindowActivator.Activate();
        }

        public void OpenContainingFolder()
        {
            _actions.FormatString("%path%", s =>
            {
                if (s.Length > 3 && (s.StartsWith(@"\\") || s.Substring(1).StartsWith(@":\")))
                {
                    var args = String.Format("/select,\"{0}\"", s);
                    Shell32.ShellExecute(IntPtr.Zero, "open", "explorer.exe", args, null, WinApiTypes.SW_SHOWNORMAL);
                }
            });
        }

        public void SearchInInternet()
        {
            _actions.FormatString(_cfg.InternetSearchFormat, s =>
            {
                // www.google.com/search?q=%q%
                var url = _cfg.InternetSearchUrl.Replace("%q%", Uri.EscapeDataString(s));
                Shell32.ShellExecute(IntPtr.Zero, "open", url, null, null, WinApiTypes.SW_SHOWNORMAL);
            });
        }

        public void MouseDoubleClick()
        {
            _actions.ActivateFoobar(_cfg.PathToFoobar2000);
        }

        public void MouseMiddleClick()
        {
            _actions.PlayPause();
            _lastActiveWindowActivator.Activate();
        }

        public void MouseXButton1Click()
        {
            _actions.Next();
            _lastActiveWindowActivator.Activate();
        }

        public void MouseXButton2Click()
        {
            _actions.Prev();
            _lastActiveWindowActivator.Activate();
        }

        private bool _tooltipShowed = false;
        private dcPicture _tooltipAlbumArt = null;

        private void SetTooltipLabelsImage()
        {
            if (_tooltipAlbumArt == null)
                return;

            var tcfg = _cfg.Tooltip;
            var bkImage = _tooltipAlbumArt.Image;
            foreach (var lbl in _tooltipLabels)
            {
                var x = tcfg.AlbumArt.X;
                var y = tcfg.AlbumArt.Y;
                if (bkImage != null)
                {
                    if (bkImage.Width < tcfg.AlbumArt.Width) x = (tcfg.AlbumArt.Width - bkImage.Width) / 2;
                    if (bkImage.Height < tcfg.AlbumArt.Height) y = (tcfg.AlbumArt.Height - bkImage.Height) / 2;
                }
                lbl.SetBkImage(bkImage, x, y);
            }
        }

        private List<dcLabel> _tooltipLabels = new List<dcLabel>();

        private void CreateTooltipControls(Form form)
        {
            _tooltipShowed = true;

            var tcfg = _cfg.Tooltip;
            foreach (var ts in tcfg.Texts)
            {
                var lbl = CreateLabel(ts);
                _tooltipLabels.Add(lbl);
                if (ts.Visible)
                {
                    form.Controls.Add(lbl);
                }
            }
            if (tcfg.AlbumArt.Visible)
            {
                _tooltipAlbumArt = CreateAlbumArt(tcfg.AlbumArt);
                ResolveAlbumArtImage(_tooltipAlbumArt, tcfg.AlbumArt);
                form.Controls.Add(_tooltipAlbumArt);

                SetTooltipLabelsImage();
            }
        }

        private void DestroyTooltipControls()
        {
            _tooltipShowed = false;

            if (_cfg.Tooltip.AlbumArt.Visible)
            {
                RemoveAndDestroyControl(_tooltipAlbumArt);
            }
            _tooltipAlbumArt = null;

            foreach (var lbl in _tooltipLabels)
            {
                RemoveAndDestroyControl(lbl);
            }
            _tooltipLabels.Clear();
        }

        public void ShowTooltip(Point localPoint, Point globalPoint, Rectangle r)
        {
            var tcfg = _cfg.Tooltip;
            if (tcfg.Enabled && !_tooltipShowed && !_stopped)
            {
                var ti = new TooltipInfo
                {
                    Rect = r,
                    Width = tcfg.Width,
                    Height = tcfg.Height,
                    BackgroundColor = tcfg.BackgroundColor,
                    UseBorderlessWindow = tcfg.UseBorderlessWindow,
                    ShowDelay = tcfg.ShowDelay,
                    KeepOpenOnMouseOver = tcfg.KeepOpenOnMouseOver,
                    CreateAction = CreateTooltipControls,
                    DestroyAction = DestroyTooltipControls
                };

                _tooltipProvider.ShowTooltip(Foobar2000Module.ModuleId, ti);

                UpdateTexts();
            }
        }

        public void HideTooltip()
        {
            _tooltipProvider.RequestHideTooltip();
        }

        private List<string> _playlist;
        private List<Guid> _playlistMenuItems = new List<Guid>();
        private Guid? _playlistGroup;

        private void HandlePlaylist(int currentIndex, List<string> playlist)
        {
            foreach (var pi in _playlistMenuItems)
            {
                _menu.RemoveItem(pi);
            }
            _playlistMenuItems.Clear();

            if (_playlistGroup != null)
            {
                _menu.RemoveItem(_playlistGroup.Value);
                _playlistGroup = null;
            }

            if (_playlist != null)
            {
                _playlist.Clear();
                _playlist = null;
            }

            int playlistStartIndex = Math.Max(currentIndex - _cfg.Playlist.NumberOfItemsBeforeCurrent, 0);
            int playlistEndIndex = Math.Min(playlistStartIndex + _cfg.Playlist.NumberOfItemsBeforeCurrent + _cfg.Playlist.NumberOfItemsAfterCurrent + 1, playlist.Count);
            int playlistTakeCount = playlistEndIndex - playlistStartIndex;
            currentIndex = currentIndex - playlistStartIndex;
            _playlist = playlist.Skip(playlistStartIndex).Take(playlistTakeCount).ToList();

            if (_cfg.Menu.Playlist)
            {
                if (_cfg.Playlist.CascadedMenu)
                {
                    _playlistGroup = _menu.AddItem(_id, null, "Playlist", null);
                }

                for (int i = 0; i < _playlist.Count; i++)
                {
                    int idx = i + playlistStartIndex;
                    var pi = _menu.AddItem(_id, _playlistGroup, _playlist[i], () => { _actions.StartPlaylistIndex(idx); _lastActiveWindowActivator.Activate(); });

                    _playlistMenuItems.Add(pi);
                    if (i == currentIndex)
                    {
                        _menu.SetItemCheckedState(pi, true);
                    }
                }
                if (_playlist.Count > 0)
                {
                    _playlistMenuItems.Add(_menu.AddItem(_id, null, "-", null));
                }
            }
        }

        private void HandleVisualizationTimerTick()
        {
            if (!_cfg.PeakMeter.Visible)
                return;

            if (!_stopped)
            {
                _actions.RequestVisualizationData();
            }
            else
            {
                HandleVisualizationData(1, new float[] { 0f });
            }
        }

        private int _lastPeakL;
        private int _lastPeakR;
        private void HandleVisualizationData(int channelCount, float[] samples)
        {
            if (channelCount == 0)
                return;

            int nsamples = samples.Length / channelCount;

            float[] data = new float[Math.Max(2, channelCount)];

            for (int channel = 0; channel < channelCount; channel++)
            {
                data[channel] = 0;
                for (int s = 0; s < nsamples; s++)
                {
                    float amplitude = samples[s + channel * nsamples];
                    if (amplitude > data[channel])
                        data[channel] = amplitude;
                }
            }

            if (channelCount == 1)
            {
                data[1] = data[0];
            }

            int fadeSpeed = _cfg.PeakMeter.FadeSpeed;

            int leftPeak = (int)(Math.Log10(data[0] * 100) * 50);
            int rightPeak = (int)(Math.Log10(data[1] * 100) * 50);

            _lastPeakL = leftPeak < _lastPeakL - fadeSpeed ? _lastPeakL - fadeSpeed : leftPeak;
            _lastPeakR = rightPeak < _lastPeakR - fadeSpeed ? _lastPeakR - fadeSpeed : rightPeak;

            _levelLeft.Position = _lastPeakL;
            _levelRight.Position = _lastPeakR;
        }
    }
}