using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Plugin.Settings.Abstractions;
using Plugin.Settings;

namespace BaseStarShot
{
    public static class SettingsHelper
    {

        //Default Values for Save Data
        //private  readonly string DefaultString = string.Empty;
        //private  readonly int DefaultInt = 0;
        //private  readonly float DefaultFloat = 0f;
        //private  readonly double DefaultDouble = 0d;
        //private  readonly bool DefaultBoolean = false;
        //private  readonly DateTime DefaultDateTime = DateTime.MinValue;

        //Save any type of value to local storage.
        public static void SaveSettings<T>(string key, T value)
        {
            AppSettings.AddOrUpdateValue<T>(key, value);
        }

        /* 	Get Current Settings Per Platform.
                Android: SharedPreferences
                iOS: NSUserDefault
                Windows: Setting
         */
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        public static T GetSettings<T>(string key, T defaultValue = default(T))
        {
            return AppSettings.GetValueOrDefault<T>(key, defaultValue);
            //var entityType = typeof(T);
            //if (entityType == typeof(string))
            //{
            //    return AppSettings.GetValueOrDefault<T>(key, (T)(object)DefaultString);
            //}
            //else if (entityType == typeof(int))
            //{
            //    return AppSettings.GetValueOrDefault<T>(key, (T)(object)DefaultInt);
            //}
            //else if (entityType == typeof(float))
            //{
            //    return AppSettings.GetValueOrDefault<T>(key, (T)(object)DefaultFloat);
            //}
            //else if (entityType == typeof(bool))
            //{
            //    return AppSettings.GetValueOrDefault<T>(key, (T)(object)DefaultBoolean);
            //}
            //else if (entityType == typeof(DateTime))
            //{
            //    return AppSettings.GetValueOrDefault<T>(key, (T)(object)DefaultDateTime);
            //}
            //else
            //{
            //    Debug.WriteLine("Type Is Not Supported");
            //    return default(T);
            //}
        }

        public static string GetSettingsString(string key, string DefaultString = "")
        {
            return AppSettings.GetValueOrDefault<String>(key, DefaultString);
        }

        public static void RemoveKey(string key)
        {
            AppSettings.Remove(key);
        }

        public static DateTime? GetSettingsDatetime(string key, DateTime? DefaultDateTime = null)
        {
            return AppSettings.GetValueOrDefault<DateTime?>(key, DefaultDateTime);
        }

        //public static ObservableCollection<Object> GetSettingsObservableList(string key)
        //{
        //    return AppSettings.GetValueOrDefault<ObservableCollection<Object>>(key, new ObservableCollection<Object>());
        //}

        public static int GetSettingsInt(string key, int DefaultInt = 0)
        {
            return AppSettings.GetValueOrDefault<int>(key, DefaultInt);
        }




        public static float GetSettingsFloat(string key, float DefaultFloat = 0f)
        {
            return AppSettings.GetValueOrDefault<float>(key, DefaultFloat);
        }


        public static double GetSettingsDouble(string key, double DefaultDouble= 0d)
        {
            return AppSettings.GetValueOrDefault<double>(key, DefaultDouble);
        }

        public static long GetSettingsLong(string key, long DefaultLong =0)
        {
            return AppSettings.GetValueOrDefault<long>(key, DefaultLong);
        }

        public static bool GetSettingsBoolean(string key, bool DefaultBoolean = false)
        {
            return AppSettings.GetValueOrDefault<bool>(key, DefaultBoolean);
        }

    }


}