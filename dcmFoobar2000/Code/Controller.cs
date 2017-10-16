using dcmFoobar2000.Configuration;
using dcmFoobar2000.Properties;
using Deskband.Core.Common;
using Deskband.Core.Controls;
using Deskband.Core.Interfaces;
using Deskband.Core.WinApi;
using DeskbandBridge;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
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
        //private Container _container;
        private Timer _hideTimer;
        private const int _hideTimerInitialInterval = 1000;
        private const int _hideTimerRegularInterval = 300;

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

        private List<dcLabel> _labels = new List<dcLabel>();

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
            //_container = _disposable.Add(new Container());
            _hideTimer = _disposable.Add(new Timer());
            _hideTimer.Interval = _hideTimerInitialInterval;
            _hideTimer.Tick += (s, e) => HandleHideTimerTick();
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

            if (_stopped)
            {
                ClearTexts();
                HandlePlaybackState(false);
            }

            _actions.Init(_stopped, _cfg);
        }

        public void Dispose()
        {
            _config.UpdateConfiguration(_cfg);

            _messageForm.Lock();
            DestroyControls();
            _disposable.Dispose();

            if (_tooltipAlbumArtImage != null) _tooltipAlbumArtImage.Dispose();
        }

        private List<IDisposable> _controls = new List<IDisposable>();

        private void DestroyControls()
        {
            _controls.Reverse();
            foreach (var x in _controls)
            {
                x.Dispose();
            }
            _controls.Clear();
        }

        private T CreateControl<T>() where T : Control, new()
        {
            var control = new T();
            return AddControl(control);
        }

        private T AddControl<T>(T control) where T : Control
        {
            _controls.Add(control);
            return control;
        }

        private void RemoveAndDestroyControl<T>(T control) where T : Control
        {
            _controls.Remove(control);
            control.Dispose();
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
                _miStop = m.Stop ? _menu.AddItem(_id, null, "Stop", _actions.Stop) : Guid.Empty;
                _miPlayPause = m.PlayPause ? _menu.AddItem(_id, null, "Play / Pause", _actions.PlayPause) : Guid.Empty;
                _miPrev = m.Previous ? _menu.AddItem(_id, null, "Previous", _actions.Prev) : Guid.Empty;
                _miNext = m.Next ? _menu.AddItem(_id, null, "Next", _actions.Next) : Guid.Empty;
                _miRandom = m.Random ? _menu.AddItem(_id, null, "Random", _actions.Random) : Guid.Empty;
                _miToggleStopAC = m.StopAfterCurrent ? _menu.AddItem(_id, null, "Stop After Current", _actions.ToggleStopAfterCurrent) : Guid.Empty;

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
            DestroyControls();

            _picAlbumArt = CreateAlbumArt(_cfg.AlbumArt);
            AddControlToModuleContainer(_picAlbumArt);

            _btnStop = CreateButton(_cfg.Buttons.BtnStop, Resources.Icon_Stop, null,
                () => { _actions.Stop(); _lastActiveWindowActivator.Activate(); });
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

            if (settings.Icon1Path != null)
            {
                var image = ImageHelpers.GetImageFromFile(Environment.ExpandEnvironmentVariables(settings.Icon1Path));
                if (image != null) btn.Image = image;
            }
            if (icon1 != null && btn.Image == null)
            {
                btn.Image = MakeColorizedImageFromIcon(icon1, settings.ColorizeColor);
            }

            if (settings.Icon2Path != null)
            {
                var image = ImageHelpers.GetImageFromFile(Environment.ExpandEnvironmentVariables(settings.Icon2Path));
                if (image != null) btn.AdditionalImage = image;
            }
            if (icon2 != null && btn.AdditionalImage == null)
            {
                btn.AdditionalImage = MakeColorizedImageFromIcon(icon2, settings.ColorizeColor);
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
            aa.SetImage(ResolveAlbumArtImage(null, true, settings));
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
            return lbl;
        }

        private void UpdateButtonIcons()
        {
            bool isPlaying = !_stopped && !_paused;

            _btnPlayPause.ShowAdditionalImage = isPlaying;
            _btnPlayPause.Refresh();

            _btnStopAC.ShowAdditionalImage = _stop_after_current;
            _btnStopAC.Refresh();
        }

        private Image ResolveAlbumArtImage(Image image, bool stub, AlbumArtSettings aaSettings)
        {
            if (aaSettings.DoNotShowStubImage && stub)
            {
                image = null;
            }
            if (image == null)
            {
                if (aaSettings.StubImagePath != null)
                {
                    image = ImageHelpers.GetImageFromFile(Environment.ExpandEnvironmentVariables(aaSettings.StubImagePath));
                }
            }
            if (image == null || image == ImageHelpers.Empty)
            {
                image = Resources.Image_NoCoverArt;
            }
            return image;
        }

        private void UpdateAlbumArt(Image image, bool stub)
        {
            _picAlbumArt.SetImage(ResolveAlbumArtImage(image, stub, _cfg.AlbumArt));
            SetTooltipImage(image, stub);
        }

        private void UpdatePosition(int pos, int? range = null)
        {
            if (range != null)
                _trbPosition.Range = range.Value;
            _trbPosition.Position = pos;
        }

        private void UpdateTexts()
        {
            for (int i = 0; i < _labels.Count(); i++)
            {
                var cfg = _cfg.Texts[i];
                FormatString(i, cfg);
            }

            for (int i = _tooltipIndex; i < _tooltipLabels.Count() + _tooltipIndex; i++)
            {
                var cfg = _cfg.Tooltip.Texts[i - _tooltipIndex];
                FormatString(i, cfg);
            }
        }

        private void FormatString(int index, TextSettingsBase ts)
        {
            var format = _paused
                ? (String.IsNullOrWhiteSpace(ts.PausedFormat) ? ts.Format : ts.PausedFormat)
                : ts.Format;

            _actions.FormatString(index, format);
        }

        private void ClearTexts()
        {
            _cfg.Texts.Zip(_labels, (settings, lbl) => new { settings, lbl }).ToList().ForEach(x => { x.lbl.Text = x.settings.StoppedText; });
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
            _messageForm.OnTrackText += (s, e) => HandleTrackText(e.Text, e.Index);
            _messageForm.OnPauseState += (s, e) => HandlePauseState(e.Value);
            _messageForm.OnStop += (s, e) => HandleStop();
            _messageForm.OnTrackVolume += (s, e) => HandleVolume(e.Value.Item1, e.Value.Item2);
            _messageForm.OnStopAfterCurrentState += (s, e) => HandleStopAfterCurrent(e.Value);
            _messageForm.OnAlbumArt += (s, e) => HandleAlbumArt(e.Value.Item1, e.Value.Item2);
            _messageForm.OnFilePath += (s, e) => HandleFilePath(e.Text, e.Index);
            _messageForm.OnVersion += (s, e) => HandleVersion(e.Value);
            _messageForm.OnPlaylist += (s, e) => HandlePlaylist(e.CurrentIndex, e.Playlist);
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
            UpdateAlbumArt(null, true);
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

        private void HandleTrackText(string text, int index)
        {
            if (index >= _tooltipIndex)
            {
                if (index < _tooltipLabels.Count() + _tooltipIndex)
                {
                    var lbl = _tooltipLabels[index - _tooltipIndex];
                    lbl.Text = text;
                }
            }
            else if (index >= 0)
            {
                if (index < _labels.Count())
                {
                    var lbl = _labels[index];
                    lbl.Text = text;
                }
            }
            else
            {
                HandleFormatString(text, index);
            }
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
            UpdateButtonIcons();
            UpdatePosition(0);
            UpdateAlbumArt(null, true);
            ClearTexts();

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
            _actions.Volume(LimitVolume(_volume + (delta / 120) * _volume_step));
        }

        private void HandleStopAfterCurrent(bool state)
        {
            _stop_after_current = state;
            _menu.SetItemCheckedState(_miToggleStopAC, state);
            UpdateButtonIcons();
        }

        private void HandleAlbumArt(byte[] imageBytes, bool stub)
        {
            using (Image img = ImageHelpers.GetImageFromByteArray(imageBytes))
            {
                UpdateAlbumArt(img, stub);
            }
        }

        private void HandleFilePath(string text, int index)
        {
            if (text.StartsWith("file://"))
            {
                var args = String.Format("/select,\"{0}\"", text);
                Shell32.ShellExecute(IntPtr.Zero, "open", "explorer.exe", args, null, WinApiTypes.SW_SHOWNORMAL);
            }
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

        private void HandleFormatString(string text, int index)
        {
            switch (index)
            {
                case FormatStringIndex.InternetSearch:
                    {
                        // www.google.com/search?q=%q%
                        var url = _cfg.InternetSearchUrl.Replace("%q%", Uri.EscapeDataString(text));
                        Shell32.ShellExecute(IntPtr.Zero, "open", url, null, null, WinApiTypes.SW_SHOWNORMAL);
                    }
                    break;

                case FormatStringIndex.CopyArtistAndTitle:
                case FormatStringIndex.CopyTitle:
                case FormatStringIndex.CopyArtist:
                    {
                        Clipboard.SetText(text);
                    }
                    break;
            }
        }

        

        public void CopyArtistAndTitle()
        {
            _actions.FormatString(FormatStringIndex.CopyArtistAndTitle, "%artist% - %title%");
            _lastActiveWindowActivator.Activate();
        }

        public void CopyTitle()
        {
            _actions.FormatString(FormatStringIndex.CopyTitle, "%title%");
            _lastActiveWindowActivator.Activate();
        }

        public void CopyArtist()
        {
            _actions.FormatString(FormatStringIndex.CopyArtist, "%artist%");
            _lastActiveWindowActivator.Activate();
        }

        public void OpenContainingFolder()
        {
            _actions.FilePath(0);
        }

        public void SearchInInternet()
        {
            _actions.FormatString(FormatStringIndex.InternetSearch, _cfg.InternetSearchFormat);
        }

        public void DoubleClick()
        {
            _actions.ActivateFoobar();
        }

        private bool _tooltipShowed = false;
        private dcPicture _tooltipAlbumArt = null;
        private Image _tooltipAlbumArtImage;
        private void SetTooltipImage(Image image, bool stub)
        {
            var tcfg = _cfg.Tooltip;
            image = ResolveAlbumArtImage(image, stub, tcfg.AlbumArt);
            if (image != null)
            {
                image = new Bitmap(image); // Copy image for tooltip
            }

            if (_tooltipAlbumArtImage != null)
            {
                _tooltipAlbumArtImage.Dispose();
                _tooltipAlbumArtImage = null;
            }
            _tooltipAlbumArtImage = image;
            
            if (_tooltipAlbumArt != null)
            {
                _tooltipAlbumArt.SetImage(_tooltipAlbumArtImage);
                SetTooltipLabelsImage();
            }
        }

        private void SetTooltipLabelsImage()
        {
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
        private const int _tooltipIndex = 1000;

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
                _tooltipAlbumArt.SetImage(_tooltipAlbumArtImage);
                form.Controls.Add(_tooltipAlbumArt);

                SetTooltipLabelsImage();
            }
        }

        private void DestroyTooltipControls()
        {
            _tooltipShowed = false;

            RemoveAndDestroyControl(_tooltipAlbumArt);
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

        private void HandlePlaylist(int currentIndex, List<string> playlist)
        {
            foreach (var pi in _playlistMenuItems)
            {
                _menu.RemoveItem(pi);
            }
            _playlistMenuItems.Clear();

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
                for (int i = 0; i < _playlist.Count; i++)
                {
                    int idx = i + playlistStartIndex;
                    var pi = _menu.AddItem(_id, null, _playlist[i], () => _actions.StartPlaylistIndex(idx));

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
    }
}