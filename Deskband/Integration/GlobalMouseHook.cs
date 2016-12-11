using Deskband.Core.EventArguments;
using Deskband.Core.WinApi;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Deskband.Core.WinApi.WinApiTypes;

namespace Deskband.Integration
{
    public static class GlobalMouseHook
    {
        public static event EventHandler<ValueEventArgs<Point>> MousePoint;
        public static event EventHandler<ValueEventArgs<HookMouseStruct>> MouseWheel;

        private static IntPtr _hookID = IntPtr.Zero;
        private static HookProcedure _globalHookProc;

        private static Point _prevPoint = new Point { X = -1, Y = -1 };

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

            var mouseStruct = (HookMouseStruct)Marshal.PtrToStructure(lParam, typeof(HookMouseStruct));

            if ((int)wParam == WM_MOUSEWHEEL)
            {
                MouseWheel?.Invoke(null, new ValueEventArgs<HookMouseStruct>(mouseStruct));
            }

            if (mouseStruct.MouseData == 0)
            {
                var newPoint = mouseStruct.Point.AsPoint();
                if (newPoint != _prevPoint)
                {
                    _prevPoint = newPoint;
                    MousePoint?.Invoke(null, new ValueEventArgs<Point>(newPoint));
                }
            }

            return User32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }
    }
}
