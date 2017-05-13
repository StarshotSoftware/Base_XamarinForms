using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseStarShot
{
    /// <summary>
    /// Provides methods to resolve types.
    /// </summary>
    //public class Resolver
    //{
    //    private static IDependencyResolver _Current;
    //    public static IDependencyResolver Current { get { return _Current; } }

    //    /// <summary>
    //    /// Gets the current IoC container.
    //    /// </summary>
    //    /// <returns></returns>
    //    public static object GetContainer()
    //    {
    //        return Current != null ? Current.GetContainer() : null;
    //    }

    //    /// <summary>
    //    /// Gets the current IoC container.
    //    /// </summary>
    //    /// <typeparam name="TContainer"></typeparam>
    //    /// <returns></returns>
    //    public static TContainer GetContainer<TContainer>()
    //    {
    //        return (TContainer)GetContainer();
    //    }

    //    /// <summary>
    //    /// Sets the current resolver.
    //    /// </summary>
    //    /// <param name="resolver"></param>
    //    public static void SetResolver(IDependencyResolver resolver)
    //    {
    //        _Current = resolver;
    //    }

    //    /// <summary>
    //    /// Resolves the type to an instance.
    //    /// </summary>
    //    /// <param name="type"></param>
    //    /// <returns></returns>
    //    public static object Get(Type type)
    //    {
    //        return Current != null ? Current.Get(type) : null;
    //    }

    //    /// <summary>
    //    /// Resolves the type to an instance.
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <returns></returns>
    //    public static T Get<T>()
    //    {
    //        return (T)Get(typeof(T));
    //    }

    //    /// <summary>
    //    /// Registers an implementation of a type.
    //    /// </summary>
    //    /// <param name="type"></param>
    //    /// <param name="implementationType"></param>
    //    public static void Register(Type type, Type implementationType)
    //    {
    //        if (Current != null)
    //            Current.Register(type, implementationType);
    //    }

    //    /// <summary>
    //    /// Registers an implementation of a type.
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    /// <typeparam name="TImp"></typeparam>
    //    public static void Register<T, TImp>()
    //        where TImp : T
    //    {
    //        Register(typeof(T), typeof(TImp));
    //    }

    //    /// <summary>
    //    /// Registers an instance of a type.
    //    /// </summary>
    //    /// <param name="type"></param>
    //    /// <param name="instance"></param>
    //    public static void Register(Type type, object instance)
    //    {
    //        if (Current != null)
    //            Current.Register(type, instance);
    //    }

    //    /// <summary>
    //    /// Registers an instance of a type.
    //    /// </summary>
    //    /// <param name="instance"></param>
    //    public static void Register(object instance)
    //    {
    //        if (Current != null)
    //            Current.Register(instance.GetType(), instance);
    //    }
    //}
}
