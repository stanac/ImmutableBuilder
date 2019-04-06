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
        private T _obj;
        private bool ThrowExceptionOnBuildIfNotAllPropsAreSet { get; }
        private readonly DistinctListOfStrings _setPropertiesNames = new DistinctListOfStrings();
        private readonly IReadOnlyList<string> _propNames;

        /// <summary>
        /// Collection of names of properties that are set
        /// </summary>
        public IEnumerable<string> SetPropertiesNames => _setPropertiesNames;

        /// <summary>
        /// Collection of names of properties that are not set
        /// </summary>
        public IEnumerable<string> NotSetPropertiesNames => _propNames.Except(_setPropertiesNames);

        public bool AreAllPropertiesSet => _setPropertiesNames.Count == _propNames.Count;

        /// <summary>
        /// Creates new instance of <see cref="Builder{T}"/>
        /// </summary>
        public Builder(): this (false) { }

        /// <summary>
        /// Creates new instance of <see cref="Builder{T}"/>
        /// </summary>
        /// <param name="throwExceptionOnBuildIfNotAllPropsAreSet">
        /// If set to true, it will require for all properties to be set on build
        /// by throwing exception if not all properties are set
        /// </param>
        public Builder(bool throwExceptionOnBuildIfNotAllPropsAreSet)
        {
            ThrowExceptionOnBuildIfNotAllPropsAreSet = throwExceptionOnBuildIfNotAllPropsAreSet;
            _propNames = PropertyNamesGetter.GetPropertyNames<T>();
            Clear();
        }

        /// <summary>
        /// Creates new instance of <see cref="Builder{T}"/> and sets builder properties from <see cref="model"/>
        /// </summary>
        /// <param name="model">Model to take properties from (to clone it), cannot be null</param>
        public Builder(T model) : this()
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
            _setPropertiesNames.AddRange(_propNames);
            _obj = CloneShallow(model);
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
            var prop = (exp.Body as MemberExpression).Member as PropertyInfo;
            PropertySetterDelegate<T, P>.GetSetter(prop).Delegate(_obj, value);
            _setPropertiesNames.Add(prop.Name);
            return this;
        }

        /// <summary>
        /// Clears builder, same as crating new <see cref="ImmutableBuilder"/>
        /// </summary>
        /// <returns>Same instance of <see cref="Builder{T}"/></returns>
        public Builder<T> Clear()
        {
            _setPropertiesNames.Clear();
            Func<T> ctor = ConstructorDelegate.GetConstructor<T>();
            _obj = ctor();
            return this;
        }

        /// <summary>
        /// Creates object and sets properties
        /// </summary>
        /// <returns>Instance of {T}</returns>
        public T Build()
        {
            if (ThrowExceptionOnBuildIfNotAllPropsAreSet && !AreAllPropertiesSet)
            {
                throw new InvalidOperationException("Not all properties are set. Set properties are: " +
                    (_setPropertiesNames.Count == 0 ? "{none}" : string.Join(", ", _setPropertiesNames)) +
                    "."
                    );
            }

            return _obj;
        }

        /// <summary>
        /// Clones provided object, creating a shallow copy
        /// </summary>
        /// <param name="model">Model to clone</param>
        /// <returns>Cloned model</returns>
        public static T CloneShallow(T model)
        {
            return CloneDelegate<T>.GetShallowCopyDelegate()(model);
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

    }
}
