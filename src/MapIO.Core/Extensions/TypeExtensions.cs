using System;

namespace MapIO.Core.Extensions
{
    public static class TypeExtensions
    {
#if NETSTANDARD
        public static bool IsAssignableTo(this Type type, Type targetType)
        {
            return targetType != null && targetType.IsAssignableFrom(type);
        }
#endif
    }
}
