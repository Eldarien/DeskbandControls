using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Deskband.Extensions
{
    public static class IEnumerableOfTypeExtensions
    {
        public static IEnumerable<Type> TypesImplementingInterface<InterfaceType>(this IEnumerable<Type> types)
        {
            return types.Where(t => !t.IsAbstract && !t.IsInterface && (
                typeof(InterfaceType).IsAssignableFrom(t)
                ||
                t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(InterfaceType))
                )
            );
        }
    }
}
