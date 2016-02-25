using Deskband.Common;
using Deskband.Core.WinApi;
using DeskbandBridge;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Deskband.Communication
{
    public partial class MessageForm : Form
    {
        public event EventHandler<EventArgs> OnThemeChanged;

        public event EventHandler<EventArgs> OnFoobarShow;

        public event EventHandler<EventArgs> OnFoobarHide;

        public event EventHandler<ValueEventArgs<double>> OnTrackTime;

        public event EventHandler<TrackTextEventArgs> OnTrackText;

        public event EventHandler<ValueEventArgs<bool>> OnPauseState;

        public event EventHandler<EventArgs> OnStop;

        public event EventHandler<ValueEventArgs<double>> OnTrackLength;

        public event EventHandler<ValueEventArgs<float>> OnTrackVolume;

        public event EventHandler<ValueEventArgs<bool>> OnStopAfterCurrentState;

        public event EventHandler<ValueEventArgs<Tuple<byte[], bool>>> OnAlbumArt;

        public event EventHandler<TrackTextEventArgs> OnFilePath;

        public event EventHandler<ValueEventArgs<string>> OnVersion;

        public TextBox DebugTextBox;

        public MessageForm()
        {
            InitializeComponent();

            this.Tag = this.Handle; // Access this property to force window handle creation
            this.Text = FB2KConstants.DeskbandMsgWindowTitle;

            //this.Width = 400;
            //this.Height = 300;
            //DebugTextBox = new TextBox();
            //DebugTextBox.Multiline = true;
            //DebugTextBox.Dock = DockStyle.Fill;
            //Controls.Add(DebugTextBox);
            //Show();
        }

        private bool _locked;

        public void Lock()
        {
            _locked = true;
        }

        private void FireEvent<T>(EventHandler<T> eventHandler, T eventArgs) where T : EventArgs
        {
            if (eventHandler != null)
                eventHandler(this, eventArgs);
        }

        protected override void WndProc(ref Message m)
        {
            if (!_locked)
            {
                switch (m.Msg)
                {
                    case WinApiTypes.WM_THEMECHANGED:
                        FireEvent(OnThemeChanged, EventArgs.Empty);
                        m.Result = IntPtr.Zero;
                        break;

                    case WinApiTypes.WM_COPYDATA:
                        var csd = (WinApiTypes.COPYDATASTRUCT)Marshal.PtrToStructure(m.LParam, typeof(WinApiTypes.COPYDATASTRUCT));
                        int cmd = (int)csd.dwData;
                        switch (cmd)
                        {
                            case DeskbandCommands.Show:
                                FireEvent(OnFoobarShow, EventArgs.Empty);
                                break;

                            case DeskbandCommands.Hide:
                                FireEvent(OnFoobarHide, EventArgs.Empty);
                                break;

                            case DeskbandCommands.TrackTime:
                                {
                                    double time = (double)Marshal.PtrToStructure(csd.lpData, typeof(double));
                                    FireEvent(OnTrackTime, new ValueEventArgs<double>(time));
                                }
                                break;

                            case DeskbandCommands.Text:
                                {
                                    int index = (int)Marshal.PtrToStructure(csd.lpData, typeof(int));
                                    int text8size = (int)Marshal.PtrToStructure(csd.lpData + sizeof(int), typeof(int));
                                    byte[] text8bytes = new byte[text8size];
                                    Marshal.Copy(csd.lpData + sizeof(int) * 2, text8bytes, 0, text8size);
                                    string text = Encoding.UTF8.GetString(text8bytes);
                                    FireEvent(OnTrackText, new TrackTextEventArgs(index, text));
                                }
                                break;

                            case DeskbandCommands.PauseState:
                                {
                                    bool state = (bool)Marshal.PtrToStructure(csd.lpData, typeof(bool));
                                    FireEvent(OnPauseState, new ValueEventArgs<bool>(state));
                                }
                                break;

                            case DeskbandCommands.Stop:
                                FireEvent(OnStop, EventArgs.Empty);
                                break;

                            case DeskbandCommands.TrackLength:
                                {
                                    double length = (double)Marshal.PtrToStructure(csd.lpData, typeof(double));
                                    FireEvent(OnTrackLength, new ValueEventArgs<double>(length));
                                }
                                break;

                            case DeskbandCommands.VolumeLevel:
                                {
                                    float volume = (float)Marshal.PtrToStructure(csd.lpData, typeof(float));
                                    FireEvent(OnTrackVolume, new ValueEventArgs<float>(volume));
                                }
                                break;

                            case DeskbandCommands.StopAfterCurrentState:
                                {
                                    bool state = (bool)Marshal.PtrToStructure(csd.lpData, typeof(bool));
                                    FireEvent(OnStopAfterCurrentState, new ValueEventArgs<bool>(state));
                                }
                                break;

                            case DeskbandCommands.AlbumArt:
                                {
                                    int imageLen = (int)Marshal.PtrToStructure(csd.lpData, typeof(int));
                                    int totalLen = sizeof(int) + imageLen + sizeof(bool);
                                    byte[] buf = new byte[totalLen];
                                    Marshal.Copy(csd.lpData, buf, 0, totalLen);

                                    byte[] art = new byte[imageLen];
                                    Array.Copy(buf, sizeof(int), art, 0, imageLen);

                                    bool stub = buf[totalLen - 1] != 0;

                                    //This marshal stuff breaks on some random image with CoreEngineException in CLR...
                                    //Marshal.Copy(csd.lpData + sizeof(int), art, 0, imageLen);
                                    //bool stub = (bool)Marshal.PtrToStructure(csd.lpData + sizeof(int) + imageLen, typeof(bool));

                                    FireEvent(OnAlbumArt, new ValueEventArgs<Tuple<byte[], bool>>(new Tuple<byte[], bool>(art, stub)));
                                }
                                break;

                            case DeskbandCommands.FilePath:
                                {
                                    int index = (int)Marshal.PtrToStructure(csd.lpData, typeof(int));
                                    int text8size = (int)Marshal.PtrToStructure(csd.lpData + sizeof(int), typeof(int));
                                    byte[] text8bytes = new byte[text8size];
                                    Marshal.Copy(csd.lpData + sizeof(int) * 2, text8bytes, 0, text8size);
                                    string text = Encoding.UTF8.GetString(text8bytes);
                                    FireEvent(OnFilePath, new TrackTextEventArgs(index, text));
                                }
                                break;

                            case DeskbandCommands.Version:
                                {
                                    int text8size = (int)Marshal.PtrToStructure(csd.lpData, typeof(int));
                                    byte[] text8bytes = new byte[text8size];
                                    Marshal.Copy(csd.lpData + sizeof(int), text8bytes, 0, text8size);
                                    string text = Encoding.UTF8.GetString(text8bytes);
                                    FireEvent(OnVersion, new ValueEventArgs<string>(text));
                                }
                                break;
                        }
                        m.Result = IntPtr.Zero;
                        break;
                }
            }
            base.WndProc(ref m);
        }
    }
}