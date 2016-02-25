using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Deskband.Core.WinApi.WinApiTypes;

namespace Deskband.Core.WinApi
{
    public static class Kernel32
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetStringTypeW(CharacterTypes dwInfoType, string lpSrcStr, int cchSrc, [Out] CharacterTypeFlags[] lpCharType);

        [DllImport("kernel32.dll")]
        public static extern bool Beep(uint dwFreq, uint dwDuration);
    }
}
