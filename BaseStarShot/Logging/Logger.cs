using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaseStarShot;
using Xamarin.Forms;
using Base1902;

namespace BaseStarShot
{
    public sealed class Logger
    {
        public static readonly IWriteLogger DefaultWriteLogger = Resolver.Get<IWriteLogger>();
        private static IWriteLogger s_CurrentLogger = DefaultWriteLogger;

        /// <summary>
        /// Sets the current IWriteLogger used for logging methods, default logger uses Android.Util.Log for Android and System.Diagnostics.Debug for Windows.
        /// </summary>
        /// <param name="logger"></param>
        public static void SetWriteLogger(IWriteLogger logger)
        {
            s_CurrentLogger = logger;
        }

        /// <summary>
        /// Writes message to log.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <param name="type"></param>
        public static void Write(string tag, string message, LogType type = LogType.Info)
        {
            s_CurrentLogger.Write(tag, message, type);
        }

        /// <summary>
        /// Writes error to log.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="ex"></param>
        public static void WriteError(string tag, Exception ex)
        {
            s_CurrentLogger.Write(tag, ex.ToString(), LogType.Error);
        }

        /// <summary>
        /// Writes message to log, removed in release version.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <param name="custom"></param>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void DebugWrite(string tag, string message, LogType type = LogType.Debug)
        {
            s_CurrentLogger.Write(tag, message, type);
        }

        /// <summary>
        /// Writes error to log, removed in release version.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="ex"></param>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void DebugWriteErrorLog(string tag, Exception ex)
        {
            s_CurrentLogger.Write(tag, ex.ToString(), LogType.Error);
        }
    }
}
