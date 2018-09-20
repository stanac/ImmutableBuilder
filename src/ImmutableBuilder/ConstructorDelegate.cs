using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ImmutableBuilder
{
    internal static class ConstructorDelegate
    {
        private static readonly IDictionary<string, object> _ctorCache = new Dictionary<string, object>();
        
        public static Func<T> GetConstructor<T>()
        {
            object constructorDelegate;
            if (!_ctorCache.TryGetValue(typeof(T).AssemblyQualifiedName, out constructorDelegate))
            {
                var ctor = GetConstructorInfo<T>();

                DynamicMethod method = new DynamicMethod($"ctor:{typeof(T).FullName}", typeof(T), Type.EmptyTypes, typeof(ConstructorDelegate).Module);
                ILGenerator gen = method.GetILGenerator();
                gen.Emit(OpCodes.Newobj, ctor);
                gen.Emit(OpCodes.Ret);

                constructorDelegate = method.CreateDelegate(typeof(Func<T>));

                _ctorCache[typeof(T).AssemblyQualifiedName] = constructorDelegate;
            }
            return constructorDelegate as Func<T>;
        }

        private static ConstructorInfo GetConstructorInfo<T>()
        {
            var type = typeof(T);

            var constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
            {
                constructor = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(x => x.GetParameters().Length == 0);

                if (constructor == null)
                {
                    throw new Exception($"Parameterless constructor for type: {type} not found");
                }
            }

            return constructor;
        }

    }
}
