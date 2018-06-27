using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ImmutableBuilder
{
    internal static class PropertyCache
    {
        private static readonly IDictionary<Type, IReadOnlyList<PropertyInfo>> _cache = new ConcurrentDictionary<Type, IReadOnlyList<PropertyInfo>>();
        private static readonly IDictionary<Type, ConstructorInfo> _ctorCache = new ConcurrentDictionary<Type, ConstructorInfo>();

        public static IReadOnlyList<PropertyInfo> GetProperties<T>()
        {
            var type = typeof(T);
            IReadOnlyList<PropertyInfo> props = null;
            if (_cache.TryGetValue(type, out props))
            {
                return props;
            }

            props = type.GetProperties().Where(x => x.CanRead && x.CanWrite).ToList();
            _cache[type] = props;
            return props;
        }

        public static ConstructorInfo GetConstructor<T>()
        {
            var type = typeof(T);

            ConstructorInfo constructor = null;
            if (_ctorCache.TryGetValue(type, out constructor))
            {
                return constructor;
            }

            constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
            {
                constructor = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(x => x.GetParameters().Length == 0);

                if (constructor == null)
                {
                    throw new Exception($"Parameterless constructor for type: {type} not found");
                }
            }

            _ctorCache[type] = constructor;

            return constructor;
        }
    }
}
