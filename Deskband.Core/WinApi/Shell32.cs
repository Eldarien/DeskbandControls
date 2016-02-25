using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Deskband.Core.WinApi
{
    public static class Shell32
    {
        [DllImport("Shell32.dll")]
        public static extern int ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirecotry, int nShowCmd);
    }
}
