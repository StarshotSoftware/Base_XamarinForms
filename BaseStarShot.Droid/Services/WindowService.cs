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
using Android.Util;
using Android.Graphics;

namespace BaseStarShot.Services
{
    public class WindowService : IWindowService
    {
        readonly Context context;

        public WindowService(Context context)
        {
            this.context = context;
        }
        public double GetBottomAppBarHeight()
        {
            return 0d;
        }

        public double GetNavigationBarHeight()
        {
            TypedValue tv = new TypedValue();
            context.Theme.ResolveAttribute(Android.Resource.Attribute.ActionBarSize, tv, true);
            int px = context.Resources.GetDimensionPixelSize(tv.ResourceId);
            float height = context.Resources.GetDimension(tv.ResourceId);
            return (double)height / ((double)Globals.ResolutionScale / 100d);
        }

        public double GetSoftwareButtonsBarHeight()
        {
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBeanMr1)
            {
                IWindowManager windowManager = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

                double scale = ((double)Globals.ResolutionScale / 100d);
                DisplayMetrics metrics = new DisplayMetrics();
                windowManager.DefaultDisplay.GetMetrics(metrics);
                double usableHeight = metrics.HeightPixels / scale;

                windowManager.DefaultDisplay.GetRealMetrics(metrics);
                double realHeight = metrics.HeightPixels / scale;
                if (realHeight > usableHeight)
                    return realHeight - usableHeight;
                else
                    return 0d;
            }
            return 0d;
        }

        public double GetSoftwareButtonsBarWidth()
        {
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.JellyBeanMr1)
            {
                IWindowManager windowManager = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();

                double scale = ((double)Globals.ResolutionScale / 100d);
                DisplayMetrics metrics = new DisplayMetrics();
                windowManager.DefaultDisplay.GetMetrics(metrics);
                double usableWidth = metrics.WidthPixels / scale;

                windowManager.DefaultDisplay.GetRealMetrics(metrics);
                double realWidth = metrics.WidthPixels / scale;
                if (realWidth > usableWidth)
                    return realWidth - usableWidth;
                else
                    return 0d;
            }
            return 0d;
        }

        public double GetStatusBarHeight()
        {
            if (context is Activity)
            {
                Android.Graphics.Rect rectangle = new Android.Graphics.Rect();
                ((Activity)context).Window.DecorView.GetWindowVisibleDisplayFrame(rectangle);
                int statusBarHeight = rectangle.Top;

                return (double)statusBarHeight / ((double)Globals.ResolutionScale / 100d);
                //int contentViewTop = context.Window.FindViewById(Window.IdAndroidContent).Top;
                //int titleBarHeight = contentViewTop - statusBarHeight;
                //return titleBarHeight;
            }
            return 0;
        }

        public double GetStatusBarWidth()
        {
            if (context is Activity)
            {
                Android.Graphics.Rect rectangle = new Android.Graphics.Rect();
                ((Activity)context).Window.DecorView.GetWindowVisibleDisplayFrame(rectangle);
                int statusBarWidth = rectangle.Width();

                return (double)statusBarWidth / ((double)Globals.ResolutionScale / 100d);
            }
            return 0;
        }

        public event EventHandler<Rect> OnVisibleScreenSizeChangedEvent;
        public void OnVisibleSizeChanged(Rect rect)
        {
            if (OnVisibleScreenSizeChangedEvent != null)
            {
                OnVisibleScreenSizeChangedEvent(this, rect);
            }
        }
    }
}