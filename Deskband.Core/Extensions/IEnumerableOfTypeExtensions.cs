using System;
using System.Collections.Generic;
using System.Linq;

namespace Deskband.Core.Extensions
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
