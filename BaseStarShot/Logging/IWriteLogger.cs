using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot
{
    public enum LogType
    {
        Info,
        Debug,
        Error
    }

    public interface IWriteLogger
    {
        /// <summary>
        /// Writes log to output.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        void Write(string tag, string message, LogType type);
    }
}
