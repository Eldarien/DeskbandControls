using Deskband.Common;
using Deskband.Communication;
using Deskband.Native;
using Deskband.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Deskband.Controls
{
    public class ControlHost : UserControl
    {
        public event EventHandler OnApplySettings;

        public event EventHandler<ValueEventArgs<bool>> OnPlaybackState;

        public event EventHandler<ValueEventArgs<bool>> OnFoobarShowHide;

        private List<IDisposable> _disposeList = new List<IDisposable>();

        private T Disposable<T>(T disposable) where T : IDisposable
        {
            _disposeList.Add(disposable);
            return disposable;
        }

        // Private objects

        private readonly Controller _controller;

        public Controller Controller { get { return _controller; } }

        private MessageForm _messageForm;
        private Container _container;
        private Timer _scrollTimer;

        // Visible controls

        private readonly List<AeroLabel> _labels = new List<AeroLabel>();
        private readonly List<AeroButton> _buttons = new List<AeroButton>();
        private readonly List<MediaTrackbar> _trackbars = new List<MediaTrackbar>();
        private readonly List<AlbumArtPicture> _albumArts = new List<AlbumArtPicture>();

        public ControlHost()
        {
            BackColor = Color.Transparent;
            Dock = DockStyle.Fill;

            // Container
            _container = Disposable(new Container());

            // MessageForm
            _messageForm = Disposable(new MessageForm());

            // Scroll timer
            _scrollTimer = Disposable(new Timer(_container));
            _scrollTimer.Tick += (s, ea) => _labels.ForEach(x => x.ScrollTick());

            _controller = new Controller(_messageForm, _labels, _buttons, _trackbars, _albumArts);
            _controller.OnApplySettings += (s, ea) => ApplySettings();
            _controller.OnPlaybackState += (s, ea) => { if (OnPlaybackState != null) OnPlaybackState(s, ea); };
            _controller.OnFoobarShowHide += (s, ea) => { if (OnFoobarShowHide != null) OnFoobarShowHide(s, ea); };
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            for (int i = _disposeList.Count - 1; i >= 0; i--)
            {
                _disposeList[i].Dispose();
                _disposeList[i] = null;
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WinApi.WM_NCHITTEST)
            {
                m.Result = (IntPtr)WinApi.HTTRANSPARENT;
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        private void ClearControl<T>(T control) where T : Control
        {
            if (control != null)
            {
                if (Controls.Contains(control)) Controls.Remove(control);
                control.Dispose();
                control = null;
            }
        }

        private void ClearControls<T>(List<T> controls) where T : Control
        {
            controls.ForEach(c => ClearControl(c));
            controls.Clear();
        }

        private T AddControl<T>(T control, bool makeLastChild = false) where T : Control
        {
            Controls.Add(control);
            if (makeLastChild)
            {
                Controls.SetChildIndex(control, Controls.Count - 1);
            }
            return control;
        }

        public void ApplySettings()
        {
            SuspendLayout();

            var settings = SettingsManager.Instance.Settings;
            bool outline = settings.General.DrawControlsOutline;

            // Scroll timer

            _scrollTimer.Interval = settings.General.TextScrollSpeed;
            _scrollTimer.Enabled = settings.General.TextScrollSpeed > 0;

            // TextBlocks

            ClearControls(_labels);
            _labels.AddRange(settings.TextBlocks.Select(x => AeroLabel.Create(x, outline)));
            _labels.ForEach(x => AddControl(x));

            // Buttons

            ClearControls(_buttons);
            _buttons.AddRange(settings.Buttons.Select(x => AeroButton.Create(x, outline)));
            _buttons.ForEach(x => AddControl(x));

            // Trackbars

            ClearControls(_trackbars);
            _trackbars.AddRange(settings.Trackbars.Select(x => MediaTrackbar.Create(x, outline)));
            _trackbars.ForEach(x => AddControl(x));

            // AlbumArt
            ClearControls(_albumArts);
            _albumArts.AddRange(new[] { AlbumArtPicture.Create(settings.AlbumArt) });
            _albumArts.ForEach(x => AddControl(x, makeLastChild: true));

            ResumeLayout();

            if (OnApplySettings != null)
                OnApplySettings(this, EventArgs.Empty);

            _controller.UpdateControlHandlers();
        }
    }
}