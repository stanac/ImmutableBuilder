using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ImmutableBuilder
{
    public class Builder<T>
        where T : class
    {
        private IReadOnlyList<PropertyInfo> _props;
        private IDictionary<PropertyInfo, object> _propValues;
        private T _obj;

        /// <summary>
        /// Creates new instance of <see cref="Builder{T}"/>
        /// </summary>
        public Builder()
        {
            _props = PropertyCache.GetProperties<T>();
            Clear();
        }

        /// <summary>
        /// Creates new instance of <see cref="Builder{T}"/> and sets builder properties from <see cref="model"/>
        /// </summary>
        /// <param name="model">Model to take properties from (to clone it), cannot be null</param>
        public Builder(T model) : this()
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            SetPropertiesFromModel(model);
        }

        /// <summary>
        /// Creates new instance of <see cref="Builder{T}"/> and sets builder properties from <see cref="model"/>
        /// </summary>
        /// <param name="model">Model to take properties from (to clone it), cannot be null</param>
        /// <returns>Instance of <see cref="Builder{T}"/></returns>
        public static Builder<T> FromObject(T model) => new Builder<T>(model);

        /// <summary>
        /// Sets property to be set when building object (calling <see cref="Build"/>)
        /// </summary>
        /// <typeparam name="P">Property type</typeparam>
        /// <param name="exp">Expression to define property for which value needs to be set during <see cref="Build"/>  </param>
        /// <param name="value">Value of the property to set</param>
        /// <returns>Same instance of <see cref="Builder{T}"/></returns>
        public Builder<T> Set<P>(Expression<Func<T, P>> exp, P value)
        {
            var propName = ((MemberExpression)exp.Body).Member.Name;
            return Set(propName, value);
        }

        /// <summary>
        /// Clears builder, same as crating new <see cref="ImmutableBuilder"/>
        /// </summary>
        /// <returns>Same instance of <see cref="Builder{T}"/></returns>
        public Builder<T> Clear()
        {
            _propValues = new Dictionary<PropertyInfo, object>();
            var ctor = PropertyCache.GetConstructor<T>();
            _obj = ctor.Invoke(new object[0]) as T;
            return this;
        }

        /// <summary>
        /// Creates object and sets properties
        /// </summary>
        /// <returns>Instance of {T}</returns>
        public T Build()
        {
            foreach (var prop in _propValues)
            {
                prop.Key.SetValue(_obj, prop.Value, null);
            }
            return _obj;
        }

        /// <summary>
        /// Clones provied object
        /// </summary>
        /// <param name="model">Model to clone</param>
        /// <returns>Cloned model</returns>
        public static T Clone(T model)
        {
            Builder<T> builder = FromObject(model);
            return builder.Build();
        }

        /// <summary>
        /// Creates new instance of {T} from existing object
        /// </summary>
        /// <typeparam name="P">Property type</typeparam>
        /// <param name="model">Model to clone</param>
        /// <param name="exp">Property expression to set</param>
        /// <param name="value">Value of the property to set</param>
        /// <returns>New instance of type {T} with changed property</returns>
        public static T Change<P>(T model, Expression<Func<T, P>> exp, P value)
        {
            return FromObject(model).Set(exp, value).Build();
        }

        private void SetPropertiesFromModel(T model)
        {
            foreach (var prop in _props)
            {
                _propValues[prop] = prop.GetValue(model);
            }
        }

        private Builder<T> Set(string propName, object value)
        {
            PropertyInfo prop = _props.SingleOrDefault(x => x.Name == propName);
            if (prop == null) throw new ArgumentOutOfRangeException(propName);
            _propValues[prop] = value;

            return this;
        }
    }
}
