using Deskband.Core.WinApi;
using DeskbandBridge;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using dcmFoobar2000.Configuration;

namespace dcmFoobar2000.Code
{
    public class Foobar2000Actions
    {
        private readonly IntPtr _msgFormHandle;

        private bool _locked;
        private bool _hasVersion;

        public Foobar2000Actions(MessageForm messageForm)
        {
            _msgFormHandle = messageForm.Handle;
        }

        public void SetVersion(bool isLocked)
        {
            _locked = isLocked;
            _hasVersion = true;
        }

        private IntPtr GetFoobarPluginMessageWindow()
        {
            return User32.FindWindow(FB2KConstants.FoobarPluginMsgWindowClass, FB2KConstants.FoobarPluginMsgWindowTitle);
        }

        private void SendCommand(int fb2kCommand, byte[] data = null)
        {
            if (!_hasVersion && fb2kCommand != FB2KCommands.GetVersion)
            {
                SendCommand(FB2KCommands.GetVersion);
                return;
            }

            if (_locked)
                return;

            var fw = GetFoobarPluginMessageWindow();
            if (fw != IntPtr.Zero)
            {
                GCHandle pinnedData = GCHandle.Alloc(data, GCHandleType.Pinned);

                WinApiTypes.COPYDATASTRUCT cds;
                cds.dwData = (IntPtr)fb2kCommand;
                cds.cbData = data == null ? 0 : data.Length;
                cds.lpData = data == null ? IntPtr.Zero : pinnedData.AddrOfPinnedObject();
                User32.SendMessage(fw, WinApiTypes.WM_COPYDATA, _msgFormHandle, ref cds);

                pinnedData.Free();
            }
        }

        public void Init(bool stopped, ConfigurationModel cfg)
        {
            byte[] formatBytes = Encoding.UTF8.GetBytes(cfg.Playlist.Format);
            SendCommand(FB2KCommands.SetPlaylistFormat, formatBytes);

            if (stopped)
            {
                SendCommand(FB2KCommands.ResendLastNonTrackState);
            }
            else
            {
                SendCommand(FB2KCommands.ResendLastState);
            }
        }

        public void ResendLastState()
        {
            SendCommand(FB2KCommands.ResendLastState);
        }

        public void Stop()
        {
            SendCommand(FB2KCommands.Stop);
        }

        public void PlayPause()
        {
            SendCommand(FB2KCommands.PlayPause);
        }

        public void Prev()
        {
            SendCommand(FB2KCommands.Previous);
        }

        public void Next()
        {
            SendCommand(FB2KCommands.Next);
        }

        public void Random()
        {
            SendCommand(FB2KCommands.Random);
        }

        public void ToggleStopAfterCurrent()
        {
            SendCommand(FB2KCommands.ToggleSAC);
        }

        public void FormatString(int index, string format)
        {
            if (String.IsNullOrEmpty(format))
                return;

            byte[] formatBytes = Encoding.UTF8.GetBytes(format);
            byte[] indexBytes = BitConverter.GetBytes(index);

            byte[] data = new byte[indexBytes.Length + formatBytes.Length];
            Array.Copy(indexBytes, data, indexBytes.Length);
            Array.Copy(formatBytes, 0, data, indexBytes.Length, formatBytes.Length);

            SendCommand(FB2KCommands.FormatString, data);
        }

        public void FilePath(int index)
        {
            SendCommand(FB2KCommands.FilePath, BitConverter.GetBytes(index));
        }

        public void Seek(int position)
        {
            byte[] data = BitConverter.GetBytes(position);
            SendCommand(FB2KCommands.Seek, data);
        }

        public void Volume(float volume)
        {
            byte[] data = BitConverter.GetBytes(volume);
            SendCommand(FB2KCommands.Volume, data);
        }

        public void ActivateFoobar(string pathToFoobar2000)
        {
            var fw = User32.FindWindow(FB2KConstants.FoobarPluginMsgWindowClass, FB2KConstants.FoobarPluginMsgWindowTitle);
            if (fw == IntPtr.Zero)
            {
                var searchPaths = new[]
                {
                    Environment.ExpandEnvironmentVariables(pathToFoobar2000)
                };

                foreach (var path in searchPaths)
                {
                    string foobarExe = Path.Combine(path, "foobar2000.exe");
                    if (File.Exists(foobarExe))
                    {
                        Shell32.ShellExecute(IntPtr.Zero, "open", foobarExe, null, null, WinApiTypes.SW_SHOWNORMAL);
                        break;
                    }
                }
            }

            SendCommand(FB2KCommands.Activate);

            int processId;
            User32.GetWindowThreadProcessId(fw, out processId);
            try
            {
                Microsoft.VisualBasic.Interaction.AppActivate(processId);
            }
            catch { }
        }

        public void GetVersion()
        {
            SendCommand(FB2KCommands.GetVersion);
        }

        public bool IsFoobarStarted
        {
            get { return GetFoobarPluginMessageWindow() != IntPtr.Zero; }
        }

        public void StartPlaylistIndex(int i)
        {
            byte[] data = BitConverter.GetBytes(i);
            SendCommand(FB2KCommands.StartPlaylistIndex, data);
        }

        
    }
}
