using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Lang;
using System.Diagnostics;

namespace BaseStarShot
{
    public static class ExceptionExtensions
    {
        static StackTraceElement[] GetTraceFromMonoException(System.Exception e)
        {
            StackTrace s = new StackTrace(e);

            //Get the first stack frame
            var frames = s.GetFrames().Select(frame =>
            {
                //Get the file name
                string fileName = frame.GetFileName();

                //Get the method name
                string methodName = frame.GetMethod().Name;

                //Get the class name
                string className = frame.GetMethod().ReflectedType.FullName;

                //Get the line number from the stack frame
                int line = frame.GetFileLineNumber();

                //Get the column number
                int col = frame.GetFileColumnNumber();

                return new StackTraceElement(className, methodName, fileName, line);
            }).ToArray();
            return frames;
        }

        public static Java.Lang.Exception GetJavaException(this System.Exception e)
        {
            var javaException = new Java.Lang.Exception(e.Message);

            javaException.SetStackTrace(GetTraceFromMonoException(e));

            if (e.InnerException != null)
            {
                javaException.InitCause(e.InnerException.GetJavaException());
            }

            return javaException;
        }

    }
}