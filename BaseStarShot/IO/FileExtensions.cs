using Base1902.IO;
using BaseStarShot.Services;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.IO
{
    public static class PclFileExtensions
    {
        public static File ToFile(this IFile file)
        {
            var f = File.FromData(file.Path, null);
            return File.FromStreamTask(() => file.OpenAsync(FileAccess.ReadAndWrite), file.Path);
        }

        public static File ToFile(this IMediaFile mediaFile)
        {
            if (string.IsNullOrEmpty(mediaFile.Path)) return null;
            return File.FromStreamTask(() => mediaFile.GetStreamAsync(), mediaFile.Path);
        }

    }
}
