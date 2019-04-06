using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ImmutableBuilder
{
    internal abstract class CloneDelegate
    {
        protected static readonly IDictionary<string, object> Cache = new Dictionary<string, object>();
        protected static readonly IDictionary<string, DynamicMethod> MethodsCache = new Dictionary<string, DynamicMethod>();
    }

    // Shallow copy
    internal class CloneDelegate<T> : CloneDelegate
        where T : class
    {
        public static Func<T, T> GetShallowCopyDelegate()
        {
            string key = typeof(T).AssemblyQualifiedName;
            object del;

            if (!Cache.TryGetValue(key, out del))
            {
                del = CreateShallowCopyDelegateFunction();
                Cache[key] = del;
            }

            return del as Func<T, T>;
        }

        private static Func<T, T> CreateShallowCopyDelegateFunction()
        {
            var type = typeof(T);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.CanWrite && x.CanRead)
                .ToList();

            var constructor = type.GetConstructor(Type.EmptyTypes);
            if (constructor == null)
            {
                constructor = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(x => x.GetParameters().Length == 0);
            }

            string methodName = $"Clone:{type.AssemblyQualifiedName}";
            var method = new DynamicMethod(methodName, typeof(T), new Type[] { typeof(T) }, true);
            var gen = method.GetILGenerator();

            var newObj = gen.DeclareLocal(typeof(T));
            gen.Emit(OpCodes.Newobj, constructor);
            gen.Emit(OpCodes.Stloc, newObj);
            
            foreach (var prop in props)
            {
                var getMethod = prop.GetGetMethod() ?? prop.GetGetMethod(true);
                var setMethod = prop.GetSetMethod() ?? prop.GetSetMethod(true);

                gen.Emit(OpCodes.Ldloc_0);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Callvirt, getMethod);
                gen.Emit(OpCodes.Callvirt, setMethod);
            }

            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ret);

            MethodsCache[methodName] = method;

            return method.CreateDelegate(typeof(Func<T, T>)) as Func<T, T>;
        }
    }
}
