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

            UpdateControlsState();
        }

        public void Dispose()
        {
            _config.UpdateConfiguration(_cfg);

            _messageForm.Lock();
            DestroyControls();
            _disposable.Dispose();
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
            //var group = Foobar2000Module.ModuleName;
            //_menu.ClearGroup(group);
            _menu.ClearByModule(_id);

            _miStop = _menu.AddItem(_id, null, "Stop", _actions.Stop);
            _miPlayPause = _menu.AddItem(_id, null, "Play / Pause", _actions.PlayPause);
            _miPrev = _menu.AddItem(_id, null, "Previous", _actions.Prev);
            _miNext = _menu.AddItem(_id, null, "Next", _actions.Next);
            _miRandom = _menu.AddItem(_id, null, "Random", _actions.Random);
            _miToggleStopAC = _menu.AddItem(_id, null, "Toggle Stop After Current", _actions.ToggleStopAfterCurrent);

            _menu.AddItem(_id, null, "-", null);

            _miCopyArtistAndTitle = _menu.AddItem(_id, null, "Copy Artist and Title", CopyArtistAndTitle);
            _miCopyTitle = _menu.AddItem(_id, null, "Copy Title", CopyTitle);
            _miCopyArtist = _menu.AddItem(_id, null, "Copy Artist", CopyArtist);
            _miOpenContainingFolder = _menu.AddItem(_id, null, "Open Containing Folder", OpenContainingFolder);
            _miSearchInInternet = _menu.AddItem(_id, null, "Search in Internet", SearchInInternet);

            _menu.AddItem(_id, null, "-", null);
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

            _btnStopAC = CreateButton(_cfg.Buttons.BtnStopAC, Resources.Icon_StopAfterCurrentOn, Resources.Icon_StopAfterCurrentOff,
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
            Image image = icon.ToBitmap();
            if (color == Color.White) return image;

            Image colorizedImage = ImageHelpers.Colorize(image, color);
            image.Dispose();
            return colorizedImage;
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
            aa.EnableStubImage = !settings.DoNotShowStubImage;
            Image stubImage = ImageHelpers.Empty;
            if (settings.StubImagePath != null)
            {
                stubImage = ImageHelpers.GetImageFromFile(Environment.ExpandEnvironmentVariables(settings.StubImagePath));
            }
            if (stubImage == ImageHelpers.Empty)
                stubImage = Resources.Image_NoCoverArt;
            aa.SetStubImage(stubImage);
            aa.SetImage(null);
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
            trb.UseBackgroundColor = settings.UseBackgroundColor;
            trb.HideBorders = settings.HideBorders;
            trb.Range = 100;
            trb.Position = 0;
            trb.ChangeOnMouseUp = changeOnMouseUp;
            trb.OnPositionChanged += (s, e) => action(e.Value);
            trb.MouseUp += (s, e) => mouseUpAction();
            return trb;
        }

        private dcLabel CreateLabel(TextSettings settings)
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
            lbl.AlignTextToRight = settings.AlightToRight;
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

        private void UpdateAlbumArt(Image image, bool stub)
        {
            _picAlbumArt.SetImage(stub && _cfg.AlbumArt.DoNotShowStubImage ? null : image);

            //SetTooltipImage(image);
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
                var format = _paused
                    ? (String.IsNullOrWhiteSpace(cfg.PausedFormat) ? cfg.Format : cfg.PausedFormat)
                    : cfg.Format;

                _actions.FormatString(i, format);
            }

            //_actions.FormatString(FormatStringIndex.TooltipText1, "%artist%");
            //_actions.FormatString(FormatStringIndex.TooltipText2, "%title%");
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
            _messageForm.OnTrackVolume += (s, e) => HandleVolume(e.Value);
            _messageForm.OnStopAfterCurrentState += (s, e) => HandleStopAfterCurrent(e.Value);
            _messageForm.OnAlbumArt += (s, e) => HandleAlbumArt(e.Value.Item1, e.Value.Item2);
            _messageForm.OnFilePath += (s, e) => HandleFilePath(e.Text, e.Index);
            _messageForm.OnVersion += (s, e) => HandleVersion(e.Value);
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
            if (index >= 0)
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

        private void HandleVolume(float volume)
        {
            //volume_in_percent = pow(10, (volume_in_db / 30)) * 100
            int percent = (int)(Math.Pow(10.0, (volume / 30.0)) * 100.0);
            _trbVolume.Position = percent;
        }

        private void SetVolume(int percent)
        {
            // volume_in_db = 30 * log10 (volume_in_percent / 100)
            float vdb = (float)(30.0 * Math.Log10((float)percent / 100.0));
            if (vdb < -100.0f) vdb = -100.0f;
            _actions.Volume(vdb);
        }

        private void HandleStopAfterCurrent(bool state)
        {
            _stop_after_current = state;
            _menu.SetItemCheckedState(_miToggleStopAC, state);
            UpdateButtonIcons();
        }

        private void HandleAlbumArt(byte[] imageBytes, bool stub)
        {
            var img = ImageHelpers.GetImageFromByteArray(imageBytes);
            UpdateAlbumArt(img, stub);
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
                //case FormatStringIndex.TooltipText1:
                //case FormatStringIndex.TooltipText2:
                //    {
                //        SetTooltipText(index, text);
                //    }
                //    break;
            }
        }

        private void UpdateControlsState()
        {
            if (_stopped)
            {
                _actions.ResendLastNonTrackState();
                ClearTexts();
                HandlePlaybackState(false);
            }
            else
            {
                _actions.ResendLastState();
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

        public void MouseWheel(int delta)
        {
            var volume = _trbVolume.Position;
            volume += delta * 3 / 120;
            if (volume < 0) volume = 0;
            if (volume > 100) volume = 100;
            SetVolume(volume);
        }

        /*
        private bool _tooltipShowed = false;

        private dcPicture _tooltipAlbumArt = null;
        private Image _tooltipAlbumArtImage;
        private void SetTooltipImage(Image image)
        {
            _tooltipAlbumArtImage = image;
            if (_tooltipAlbumArt != null)
            {
                _tooltipAlbumArt.SetImage(image);
            }
        }

        private dcLabel _tooltipLabel1 = null;
        private string _tooltipLabel1Text;
        private void SetTooltipText(int index, string text)
        {
            if (index == FormatStringIndex.TooltipText1)
            {
                _tooltipLabel1Text = text;
                if (_tooltipLabel1 != null) _tooltipLabel1.Text = text;
            }
            else if (index == FormatStringIndex.TooltipText2)
            {
                _tooltipLabel2Text = text;
                if (_tooltipLabel2 != null) _tooltipLabel2.Text = text;
            }
        }

        private dcLabel _tooltipLabel2 = null;
        private string _tooltipLabel2Text;

        public void ShowTooltip(Point localPoint, Point globalPoint, Rectangle r)
        {
            if (!_tooltipShowed && !_stopped)
            {
                int x = r.Left + r.Width / 2;
                int y = r.Top + r.Height / 2;
                _tooltipProvider.ShowTooltip(Foobar2000Module.ModuleId, x, y, f =>
                {
                    int offset = 5;
                    int s = f.ClientSize.Height - offset * 2;
                    var aa = new dcPicture();
                    aa.Location = new Point(offset, offset); //_sp.MakePoint(settings.X, settings.Y);
                    aa.Size = new Size(s, s); //_sp.MakeSize(settings.Width, settings.Height);
                    aa.PreserveAspectRatio = _picAlbumArt.PreserveAspectRatio;
                    aa.SetImage(_tooltipAlbumArtImage);
                    f.Controls.Add(aa);
                    _tooltipAlbumArt = aa;

                    var t1 = new dcLabel(_sp.DPI, new FontConfiguration("Segoe UI", 16, FontStyles.Regular));
                    t1.ForeColor = Color.FromKnownColor(KnownColor.InfoText);
                    t1.Left = offset * 3 + s;
                    t1.Width = f.ClientSize.Width - t1.Left - offset;
                    t1.Top = 14;
                    t1.Height = 30;
                    t1.EnableScrolling = true;
                    if (_tooltipLabel1Text != null) t1.Text = _tooltipLabel1Text;
                    f.Controls.Add(t1);
                    _tooltipLabel1 = t1;

                    var t2 = new dcLabel(_sp.DPI, new FontConfiguration("Segoe UI", 16, FontStyles.Regular));
                    t2.ForeColor = Color.FromKnownColor(KnownColor.InfoText);
                    t2.Left = offset * 3 + s;
                    t2.Width = f.ClientSize.Width - t2.Left - offset;
                    t2.Top = 14 + 30 + offset;
                    t2.Height = 30;
                    t2.EnableScrolling = true;
                    if (_tooltipLabel2Text != null) t2.Text = _tooltipLabel2Text;
                    f.Controls.Add(t2);
                    _tooltipLabel2 = t2;
                });
                _tooltipShowed = true;
            }
        }

        public void HideTooltip()
        {
            _tooltipProvider.HideTooltip();
            _tooltipShowed = false;

            _tooltipAlbumArt = null;
            _tooltipLabel1 = null;
            _tooltipLabel2 = null;
        }
        */
    }
}