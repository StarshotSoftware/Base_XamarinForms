using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Base1902;

namespace BaseStarShot.Unity
{
    public static class UnityExtensions
    {
        /// <summary>
        /// Registers a factory method to obtain an instance of T.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        public static IUnityContainer RegisterFactory<T>(this IUnityContainer container)
        {
            container.RegisterInstance<Func<T>>(() => Resolver.Get<T>());
            return container;
        }
    }
}
