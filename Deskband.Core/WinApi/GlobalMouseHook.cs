using Deskband.Core.EventArguments;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Deskband.Core.WinApi.WinApiTypes;

namespace Deskband.Core.WinApi
{
    public class GlobalMouseHook : IDisposable
    {
        public event EventHandler<ValueEventArgs<HookMouseStruct>> MouseWheel;

        private IntPtr _hookID = IntPtr.Zero;
        private HookProcedure _globalHookProc;

        public GlobalMouseHook()
        {
            _globalHookProc = (code, param, lParam) => MouseChanged(code, param, lParam);
            _hookID = User32.SetWindowsHookEx(WH_MOUSE_LL, _globalHookProc, Process.GetCurrentProcess().MainModule.BaseAddress, 0);
        }

        public void Dispose()
        {
            User32.UnhookWindowsHookEx(_hookID);
        }

        private IntPtr MouseChanged(int nCode, IntPtr wParam, IntPtr lParam)
        {
            var passThrough = nCode != 0;
            if (passThrough)
            {
                return User32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
            }

            if ((int)wParam == WM_MOUSEWHEEL)
            {
                var mouseStruct = (HookMouseStruct)Marshal.PtrToStructure(lParam, typeof(HookMouseStruct));
                MouseWheel?.Invoke(null, new ValueEventArgs<HookMouseStruct>(mouseStruct));
            }
            return User32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }
    }
}
