using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Practices.Unity;
using Base1902.Dependencies;

namespace BaseStarShot.Unity
{
    /// <summary>
    /// The default implementation of a unity dependency resolver.
    /// </summary>
    public class UnityDependencyResolver : IDependencyResolver
    {
        private IUnityContainer container;
        private readonly ResolverOverride[] _overrides;
        private readonly Func<ResolverOverride[]> _getOverrides;

        private bool HasOverride { get { return _overrides != null || _getOverrides != null; } }

        #region Constructors
        public UnityDependencyResolver(IUnityContainer container)
        {
            this.container = container;
        }

        public UnityDependencyResolver(IUnityContainer container, params ResolverOverride[] overrides) : this(container)
        {
            this._overrides = overrides;
        }

        public UnityDependencyResolver(IUnityContainer container, Func<ResolverOverride[]> getOverrides)
            : this(container)
        {
            this._getOverrides = getOverrides;
        }
        #endregion

        private ResolverOverride[] CombineOverrides(ResolverOverride[] overrides)
        {
            List<ResolverOverride> overs = new List<ResolverOverride>();

            if (this._overrides != null)
                overs.AddRange(this._overrides);

            if (this._getOverrides != null)
                overs.AddRange(this._getOverrides.Invoke());


            if (overrides != null)
                overs.AddRange(overrides);
            return overs.ToArray();
        }

        #region IDependencyResolver implementation
        object IDependencyResolver.GetContainer()
        {
            return this.GetContainer();
        }

        void IDependencyResolver.Register<TType, TImplementationType>()
        {
            container.RegisterType(typeof(TType), typeof(TImplementationType), new ContainerControlledLifetimeManager());
        }

        void IDependencyResolver.Register(Type type, Type implementationType)
        {
            container.RegisterType(type, implementationType, new ContainerControlledLifetimeManager());
        }

        void IDependencyResolver.Register(Type type, object instance)
        {
            container.RegisterInstance(type, instance);
        }
        #endregion

        public IUnityContainer GetContainer()
        {
            return container;
        }

        public object Get(Type type)
        {
            return Get(type, null);
        }

        public T Get<T>()
        {
            return (T)Get(typeof(T));
        }

        public T Get<T>(ResolverOverride[] overrides)
        {
            return (T)Get(typeof(T), overrides);
        }

        public object Get(Type type, ResolverOverride[] overrides)
        {
            if (!container.IsRegistered(type))
            {
                var typeInfo = type.GetTypeInfo();
                if (typeInfo.IsAbstract || typeInfo.IsInterface)
                {
                    return null;
                }
            }

            if (HasOverride || overrides != null)
            {
                overrides = CombineOverrides(overrides);
                if (overrides.Length > 0)
                    return container.Resolve(type, overrides);
            }
            return container.Resolve(type);
        }

        public TService GetService<TService>()
        {
            return Get<TService>();
        }

        public object GetService(Type serviceType)
        {
            return Get(serviceType);
        }

        public IEnumerable<TService> GetServices<TService>()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}