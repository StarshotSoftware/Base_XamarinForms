using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BaseStarShot.Controls
{
    public static partial class UIHelperExtensions
    {
        public static void Raise<TEventArgs>(this object source, string eventName, TEventArgs eventArgs) where TEventArgs : EventArgs
        {
            //var eventInfo = source.GetType().GetEvent(eventName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            //var eventDelegate = (MulticastDelegate)source.GetType().GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(source);

            FieldInfo fieldInfo = null;
            Type type = source.GetType();
            do
            {
#if !NETFX_CORE
                fieldInfo = type.GetField(eventName, BindingFlags.Instance | BindingFlags.NonPublic);
                type = type.BaseType;
#else
                fieldInfo = type.GetRuntimeFields().FirstOrDefault(f => f.Name == eventName);
                type = type.GetTypeInfo().BaseType;
#endif
            }
            while (fieldInfo == null && type != typeof(object));
            if (fieldInfo == null) return;

            var eventDelegate = (MulticastDelegate)fieldInfo.GetValue(source);
            if (eventDelegate != null)
            {
                foreach (var handler in eventDelegate.GetInvocationList())
                {
                    if (handler.Target != null)
#if !NETFX_CORE
						handler.Method.Invoke(handler.Target, new object[] { source, eventArgs });
#else
                        handler.GetMethodInfo().Invoke(handler.Target, new object[] { source, eventArgs });
#endif
                }
            }
        }

        public static string ToHex(this Xamarin.Forms.Color color)
        {
            var hex = "#";
            if (color.A < 1d)
                hex += ToHex(color.A);
            hex += ToHex(color.R);
            hex += ToHex(color.G);
            hex += ToHex(color.B);
            return hex;
        }

        private static string ToHex(double value)
        {
            return ((int)Math.Round(value * 255d)).ToString("X2");
        }
    }
}
