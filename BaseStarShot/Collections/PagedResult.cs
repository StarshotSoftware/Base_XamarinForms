using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Collections
{
    public interface IPagedResult<T>
    {
        int VirtualCount { get; }

        IList<T> Result { get; }

        bool Failed { get; }
    }

    public class PagedResult<T> : IPagedResult<T>
    {
        public int VirtualCount { get; set; }

        public IList<T> Result { get; set; }

        public bool Failed { get; set; }
    }
}
