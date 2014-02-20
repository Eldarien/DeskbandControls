using Deskband.Native;
using DeskbandBridge;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Deskband.Communication
{
    public class FoobarActions
    {
        private readonly IntPtr _msgFormHandle;

        private bool _locked;
        private bool _hasVersion;

        public FoobarActions(IntPtr msgFormHandle)
        {
            _msgFormHandle = msgFormHandle;
        }

        public void SetVersion(bool isWrong)
        {
            _locked = isWrong;
            _hasVersion = true;
        }

        private IntPtr GetFoobarPluginMessageWindow()
        {
            return WinApi.FindWindow(FB2KConstants.FoobarPluginMsgWindowClass, FB2KConstants.FoobarPluginMsgWindowTitle);
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

                WinApi.COPYDATASTRUCT cds;
                cds.dwData = (IntPtr)fb2kCommand;
                cds.cbData = data == null ? 0 : data.Length;
                cds.lpData = data == null ? IntPtr.Zero : pinnedData.AddrOfPinnedObject();
                WinApi.SendMessage(fw, WinApi.WM_COPYDATA, _msgFormHandle, ref cds);

                pinnedData.Free();
            }
        }

        public void PlayPause()
        {
            SendCommand(FB2KCommands.PlayPause);
        }

        public void Stop()
        {
            SendCommand(FB2KCommands.Stop);
        }

        public void Previuos()
        {
            SendCommand(FB2KCommands.Previous);
        }

        public void Next()
        {
            SendCommand(FB2KCommands.Next);
        }

        public void ToggleStopAfterCurrent()
        {
            SendCommand(FB2KCommands.ToggleSAC);
        }

        public void Random()
        {
            SendCommand(FB2KCommands.Random);
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

        public void ResendLastState()
        {
            SendCommand(FB2KCommands.ResendLastState);
        }

        public void ResendLastNonTrackState()
        {
            SendCommand(FB2KCommands.ResendLastNonTrackState);
        }

        public void ActivateFoobar()
        {
            var fw = WinApi.FindWindow(FB2KConstants.FoobarPluginMsgWindowClass, FB2KConstants.FoobarPluginMsgWindowTitle);
            if (fw == IntPtr.Zero)
            {
                string installPath = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\foobar2000", "InstallDir", null);
                if (installPath != null)
                {
                    string foobarExe = Path.Combine(installPath, "foobar2000.exe");
                    if (File.Exists(foobarExe))
                    {
                        WinApi.ShellExecute(IntPtr.Zero, "open", foobarExe, null, null, WinApi.SW_SHOWNORMAL);
                    }
                }
            }

            SendCommand(FB2KCommands.Activate);
        }

        public void GetVersion()
        {
            SendCommand(FB2KCommands.GetVersion);
        }

        public bool IsFoobarStarted
        {
            get { return GetFoobarPluginMessageWindow() != IntPtr.Zero; }
        }
    }
}