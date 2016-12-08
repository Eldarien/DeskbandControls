using Deskband.Core.Interfaces;
using Deskband.Core.WinApi;
using System;
using System.Windows.Forms;

namespace Deskband.Integration
{
    public static class ActiveWindowWatcher
    {
        private static Timer _timer;
        private static IntPtr _previousHandle = IntPtr.Zero;
        private static IntPtr _previousToLastHandle = IntPtr.Zero;

        public static void StartWatching()
        {
            _timer = new Timer();
            _timer.Interval = 100;
            _timer.Tick += (s, e) => TimerTick(s, e);
            _timer.Start();
        }

        public static void StopWatching()
        {
            _timer.Dispose();
            _timer = null;
        }

        public static IntPtr LastHandle => _previousToLastHandle;

        public static void ActivateLastActiveWindow()
        {
            if (_previousToLastHandle != IntPtr.Zero)
            {
                User32.SetForegroundWindow(_previousToLastHandle);
            }
        }

        private static void TimerTick(object sender, EventArgs e)
        {
            SetLastActiveHandle();
        }

        private static void SetLastActiveHandle()
        {
            IntPtr currentHandle = User32.GetForegroundWindow();
            if (currentHandle != _previousHandle)
            {
                _previousToLastHandle = _previousHandle;
                _previousHandle = currentHandle;
            }
        }
    }

    public class ActiveWindowWatcherWrapper : ILastActiveWindowActivator
    {
        public void Activate() => ActiveWindowWatcher.ActivateLastActiveWindow();
    }
}
