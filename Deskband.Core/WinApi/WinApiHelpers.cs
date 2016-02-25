using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Deskband.Core.WinApi.WinApiTypes;

namespace Deskband.Core.WinApi
{
    public static class WinApiHelpers
    {
        public static bool IsTextRtl(string s)
        {
            int len = s.Length;
            var info = new CharacterTypeFlags[len];
            if (Kernel32.GetStringTypeW(CharacterTypes.CT_CTYPE2, s, len, info))
            {
                for (int i = 0; i < len; i++)
                {
                    if (info[i] == CharacterTypeFlags.C2_RIGHTTOLEFT)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
