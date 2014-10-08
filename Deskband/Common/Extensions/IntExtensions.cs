using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deskband.Common.Extensions
{
    public static class IntExtensions
    {
        public static int LowWord(this int number)
        {
            return number & 0x0000FFFF;
        }

        public static int HighWord(this int number)
        {
            return (int)((number & 0xFFFF0000) >> 16);
        }
    }
}