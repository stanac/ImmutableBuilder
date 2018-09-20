using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ImmutableBuilder
{
    public class Builder<T>
        where T : class
    {
        private T _obj;

        /// <summary>
        /// Creates new instance of <see cref="Builder{T}"/>
        /// </summary>
        public Builder()
        {
            Clear();
        }

        /// <summary>
        /// Creates new instance of <see cref="Builder{T}"/> and sets builder properties from <see cref="model"/>
        /// </summary>
        /// <param name="model">Model to take properties from (to clone it), cannot be null</param>
        public Builder(T model) : this()
        {
            if (model == null) throw new ArgumentNullException(nameof(model));
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
            return this;
        }

        /// <summary>
        /// Clears builder, same as crating new <see cref="ImmutableBuilder"/>
        /// </summary>
        /// <returns>Same instance of <see cref="Builder{T}"/></returns>
        public Builder<T> Clear()
        {
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
