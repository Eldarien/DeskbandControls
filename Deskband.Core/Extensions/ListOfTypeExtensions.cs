using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deskband.Core.Extensions
{
    public static class ListOfTypeExtensions
    {
        public static bool TryGetElementAt<T>(this List<T> list, int index, out T element)
        {
            if (index < 0 || index > list.Count - 1)
            {
                element = default(T);
                return false;
            }
            element = list[index];
            return true;
        }
    }
}
