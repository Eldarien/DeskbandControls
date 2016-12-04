using Deskband.Core.EventArguments;
using Deskband.Core.WinApi;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Deskband.Core.WinApi.WinApiTypes;

namespace Deskband.Integration
{
    public static class GlobalMouseHook
    {
        public static event EventHandler<ValueEventArgs<HookMouseStruct>> MouseWheel;

        private static IntPtr _hookID = IntPtr.Zero;
        private static HookProcedure _globalHookProc;

        public static void SetGlobalMouseHook()
        {
            _globalHookProc = (code, param, lParam) => MouseChanged(code, param, lParam);
            _hookID = User32.SetWindowsHookEx(WH_MOUSE_LL, _globalHookProc, Process.GetCurrentProcess().MainModule.BaseAddress, 0);
        }

        public static void RemoveGlobalMouseHook()
        {
            User32.UnhookWindowsHookEx(_hookID);
            GC.KeepAlive(_globalHookProc);
        }

        private static IntPtr MouseChanged(int nCode, IntPtr wParam, IntPtr lParam)
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
