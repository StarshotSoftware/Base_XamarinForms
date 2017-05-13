using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot
{
    /// <summary>
    /// Represents a factory method with a weak reference to an instance.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WeakFactoryMethod<T>
        where T : class
    {
        WeakReference<T> reference;

        /// <summary>
        /// Gets a reference to the instance.
        /// </summary>
        /// <returns></returns>
        public T Get()
        {
            T instance;
            if (reference.TryGetTarget(out instance))
                return instance;
            return null;
        }

        /// <summary>
        /// Sets the references instance.
        /// </summary>
        /// <param name="instance"></param>
        public void Set(T instance)
        {
            this.reference = new WeakReference<T>(instance);
        }
    }
}
