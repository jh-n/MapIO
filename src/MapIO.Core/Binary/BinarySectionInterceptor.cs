using Castle.DynamicProxy;
using MapIO.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MapIO.Core.Binary
{

    public class BinarySectionInterceptor : IInterceptor
    {
        readonly Type _type;
        readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

        public BinarySectionInterceptor(Type targetType)
        {
            if ((_type = targetType) == null || !_type.IsAssignableTo(typeof(BinarySection))) throw new ArgumentException(nameof(targetType));
        }

        public void Intercept(IInvocation invocation)
        {
            var methodName = invocation.Method.Name;
            var isGetter = methodName.StartsWith("get_");
            var isSetter = methodName.StartsWith("set_");
            if (isGetter || isSetter)
            {
                var propertyName = methodName.Substring(4);
                var property = _type.GetProperty(propertyName);
                FieldAttribute attribute;
                if (property != null && (attribute = property.GetCustomAttribute<FieldAttribute>()) != null)
                {
                    var section = invocation.InvocationTarget as BinarySection;
                    if (isGetter)
                    {
                        if (attribute.Type == TypeCode.Object)
                        {
                            if (!_cache.TryGetValue(propertyName, out var value))
                                _cache[propertyName] = value = section.ReadAsType(attribute.Offset, attribute.Length, property.PropertyType);
                            invocation.ReturnValue = value;
                        }
                        else
                        {
                            //property.SetValue(section, section.Read(attribute));
                            //invocation.ReturnValue = property.GetValue(section);
                            var midValue = section.Read(attribute);
                            if (midValue.GetType().IsAssignableTo(property.PropertyType)) invocation.ReturnValue = midValue;
                            else invocation.ReturnValue = Convert.ChangeType(midValue, property.PropertyType);
                        }
                    }
                    else section.Write(attribute, invocation.Arguments[0]);
                    return;
                }
            }
            invocation.Proceed();
        }


        public static BinarySection CreateProxy(Type targetType, params object[] args)
        {
            if (targetType == null) throw new ArgumentNullException(nameof(targetType));
            if (!targetType.IsAssignableTo(typeof(BinarySection))) throw new ArgumentException("Target type must be assignable to BinarySection", nameof(targetType));

            var proxyGenerator = new ProxyGenerator();
            var interceptor = new BinarySectionInterceptor(targetType);
            var proxied = proxyGenerator.CreateClassProxy(targetType, args, interceptor);
            return proxied as BinarySection;
        }

        public static object CreateProxy<T>(params object[] args) where T : BinarySection => CreateProxy(typeof(T), args) as T;
    }
}