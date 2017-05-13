using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Services
{
    public abstract class AbstractOptions
    {

        public string FilePath { get; set; }


        public string GetFileName()
        {
            return String.IsNullOrWhiteSpace(this.FilePath)
                ? null
                : Path.GetFileName(this.FilePath);
        }


        public string GetDirectory()
        {
            return String.IsNullOrWhiteSpace(this.FilePath)
                ? null
                : Path.GetDirectoryName(this.FilePath);
        }
    }
}
