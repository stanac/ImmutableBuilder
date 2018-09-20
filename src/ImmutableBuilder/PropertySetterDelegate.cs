using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ImmutableBuilder
{
    internal abstract class PropertySetterDelegate
    {
        public string PropertyName => Property?.Name;

        public PropertyInfo Property { get; internal set; }

        protected static readonly IDictionary<string, PropertySetterDelegate> SettersCache
            = new Dictionary<string, PropertySetterDelegate>();
    }

    internal class PropertySetterDelegate<TModel, TProperty> : PropertySetterDelegate
    {
        private PropertySetterDelegate() { }

        public Action<TModel, TProperty> Delegate { get; internal set; }

        internal void SetDelegate(object delegat)
        {
            Delegate = delegat as Action<TModel, TProperty>;
        }

        public static PropertySetterDelegate<TModel, TProperty> GetSetter(PropertyInfo prop)
        {
            string setterKey = GetKeyFromProperty(prop);

            PropertySetterDelegate setter;
            if (!SettersCache.TryGetValue(setterKey, out setter))
            {
                CacheSettersForModel();
                setter = SettersCache[setterKey];
            }

            return setter as PropertySetterDelegate<TModel, TProperty>;
        }

        private static void CacheSettersForModel()
        {
            List<PropertyInfo> properties = typeof(TModel)
                .GetProperties()
                .Where(x => x.CanRead && x.CanWrite)
                .ToList();

            foreach (PropertyInfo prop in properties)
            {
                string key = GetKeyFromProperty(prop);

                Type setterType = typeof(PropertySetterDelegate<,>).MakeGenericType(prop.DeclaringType, prop.PropertyType);
                PropertySetterDelegate setter = Activator.CreateInstance(setterType, true) as PropertySetterDelegate;
                setter.Property = prop;

                var deleg = CreateDelegate(prop);
                setterType.GetMethod("SetDelegate", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                    .Invoke(setter, new[] { deleg });

                SettersCache[key] = setter;
            }
        }

        private static object CreateDelegate(PropertyInfo prop)
        {
            MethodInfo setMethod = prop.GetSetMethod();
            if (setMethod == null)
            {
                setMethod = prop.GetSetMethod(true);
            }

            DynamicMethod method = new DynamicMethod("SetPropId", typeof(void), new[] { prop.DeclaringType, prop.PropertyType }, typeof(PropertySetterDelegate).Module);
            var ilGen = method.GetILGenerator();
            ilGen.Emit(OpCodes.Ldarg_0);
            ilGen.Emit(OpCodes.Ldarg_1);
            ilGen.Emit(OpCodes.Callvirt, setMethod);
            ilGen.Emit(OpCodes.Ret);

            var delegateType = typeof(Action<,>).MakeGenericType(prop.DeclaringType, prop.PropertyType);

            return method.CreateDelegate(delegateType);
        }

        private static string GetKeyFromProperty(PropertyInfo prop)
        {
            return prop.DeclaringType.AssemblyQualifiedName + ":::" + prop.Name;
        }
    }
}
