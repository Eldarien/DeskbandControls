using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Deskband.Core.WinApi.WinApiTypes;

namespace Deskband.Core.WinApi
{
    public static class UxTheme
    {
        [DllImport("uxtheme.dll", ExactSpelling = true)]
        public static extern Int32 DrawThemeParentBackground(IntPtr hWnd, IntPtr hdc, ref RECT pRect);

        [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr OpenThemeData(IntPtr hWnd, String classList);

        [DllImport("uxtheme.dll", ExactSpelling = true)]
        public static extern Int32 CloseThemeData(IntPtr hTheme);

        [DllImport("uxtheme.dll", ExactSpelling = true)]
        public static extern Int32 GetThemeColor(IntPtr hTheme, int iPartId, int iStateId, int iPropId, out COLORREF pColor);

        [DllImport("uxtheme.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern Int32 DrawThemeTextEx(IntPtr hTheme, IntPtr hdc, int iPartId, int iStateId, String text, int length, UInt32 flags, ref RECT rect, ref DTTOPTS poptions);
    }
}
