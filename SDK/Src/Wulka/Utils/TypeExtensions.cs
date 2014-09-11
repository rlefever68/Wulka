using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Wulka.Utils
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> KnownTypes(this Type type)
        {
            return Attribute
                .GetCustomAttributes(type)
                .OfType<KnownTypeAttribute>()
                .Select(attr => (attr).Type);
        }
    }
}
