using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Services
{
    public interface IMediaFile : IDisposable
    {
        string Path { get; }

        long Size { get; }
        /// <summary>
        /// Get Stream on Async
        /// </summary>
        Task<Stream> GetStreamAsync();

        Task<Base1902.IO.File> GetVideoThumbnail();
    }
}
