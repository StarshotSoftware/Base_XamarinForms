using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot
{
    public class Mapper
    {
        static Dictionary<string, Mapping> mappings = new Dictionary<string, Mapping>();

        /// <summary>
        /// Registers a mapping function between two types.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="mapping"></param>
        public static void RegisterMap<TSource, TDestination>(Func<TSource, TDestination> mapping)
        {
            var map = new Mapping
            {
                Source = typeof(TSource),
                Destination = typeof(TDestination),
                NoDestination = true,
				MappingFunc = (s, d) => mapping((TSource)s)
            };
            mappings[map.ToString()] = map;
        }

		/// <summary>
		/// Registers a mapping function between two types to existing object.
		/// </summary>
		/// <param name="mapping">Mapping.</param>
		/// <typeparam name="TSource">The 1st type parameter.</typeparam>
		/// <typeparam name="TDestination">The 2nd type parameter.</typeparam>
		public static void RegisterMap<TSource, TDestination>(Func<TSource, TDestination, TDestination> mapping)
		{
			var map = new Mapping
			{
				Source = typeof(TSource),
				Destination = typeof(TDestination),
				MappingFunc = (s, d) => mapping((TSource)s, (TDestination)d)
			};
			mappings[map.ToString()] = map;
		}

        /// <summary>
        /// Creates the destination type by using a registered mapping function.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static TDestination Map<TSource, TDestination>(TSource obj) where TDestination : class
        {
            var map = new Mapping
            {
                Source = typeof(TSource),
                Destination = typeof(TDestination)
            };
            var key = map.ToString();
            if (mappings.ContainsKey(key))
            {
                var map1 = mappings[key];
                return (TDestination)map1.MappingFunc(obj, map1.NoDestination ? null : Activator.CreateInstance(map.Destination));
            }
            throw new InvalidOperationException(string.Format("The types {0} and {1} are not yet mapped.", map.Source.FullName, map.Destination.FullName));
        }

		/// <summary>
		/// Creates the destination type by using a registered mapping function.
		/// </summary>
		/// <param name="source">Source.</param>
		/// <param name="destination">Destination.</param>
		/// <typeparam name="TSource">The 1st type parameter.</typeparam>
		/// <typeparam name="TDestination">The 2nd type parameter.</typeparam>
		public static TDestination Map<TSource, TDestination>(TSource source, TDestination destination) where TDestination : class
		{
			var map = new Mapping
			{
				Source = typeof(TSource),
				Destination = typeof(TDestination)
			};
			var key = map.ToString();
            if (mappings.ContainsKey(key))
            {
                var map1 = mappings[key];
                if (map1.NoDestination)
                    throw new InvalidOperationException(string.Format("Mapping of types {0} and {1} does not support destination object", map.Source.FullName, map.Destination.FullName));
                return (TDestination)map1.MappingFunc(source, destination ?? Activator.CreateInstance(map.Destination));
            }
			throw new InvalidOperationException(string.Format("The types {0} and {1} are not yet mapped.", map.Source.FullName, map.Destination.FullName));
		}
        
        class Mapping
        {
            public Type Source { get; set; }
            public Type Destination { get; set; }
            public Func<object, object, object> MappingFunc {get;set;}
            public bool NoDestination { get; set; }

            public override string ToString()
            {
                return Source.FullName + "->" + Destination.FullName;
            }
        }
    }
}
