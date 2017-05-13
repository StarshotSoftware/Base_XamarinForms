using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot
{
    public static class EnumerableExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> list)
        {
            ObservableCollection<T> observable = new ObservableCollection<T>();

            if (list != null)
            {
                foreach (var item in list)
                    observable.Add(item);
            }

            return observable;
        }

        //public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> originalList)
        //{
        //    return new ObservableCollection<T>(originalList);
        //}
    }
}
