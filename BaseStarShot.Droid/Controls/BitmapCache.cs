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
using Java.Lang;
using Android.Graphics;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Java.Lang.Ref;

namespace BaseStarShot.Controls
{
    public class ImageCache
    {
        private BitmapLruCache cache;
        readonly System.Collections.Concurrent.ConcurrentBag<SoftReference> reusableBitmaps;

        private static ImageCache _instance;
        public static ImageCache Instance { get { return _instance ?? (_instance = new ImageCache()); } }

        public ImageCache()
        {
            reusableBitmaps = new System.Collections.Concurrent.ConcurrentBag<SoftReference>();
            cache = new BitmapLruCache(reusableBitmaps);
        }

        public void Add(string key, Bitmap bitmap)
        {
            if (Get(key) == null)
                cache.Put(key, bitmap);
        }

        public Bitmap Get(string key)
        {
            return cache.Get(key).JavaCast<Bitmap>();
        }

        public static int CalculateInSampleSize(
            BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            int height = options.OutHeight;
            int width = options.OutWidth;
            int inSampleSize = 1;

            if (height > reqHeight || width > reqWidth)
            {

                int halfHeight = height / 2;
                int halfWidth = width / 2;

                // Calculate the largest inSampleSize value that is a power of 2 and keeps both
                // height and width larger than the requested height and width.
                while ((halfHeight / inSampleSize) > reqHeight
                        && (halfWidth / inSampleSize) > reqWidth)
                {
                    inSampleSize *= 2;
                }
            }

            return inSampleSize;
        }

        public static Bitmap DecodeSampledBitmapFromResource(Resources res, int resId, int reqWidth, int reqHeight)
        {
            // First decode with inJustDecodeBounds=true to check dimensions
            BitmapFactory.Options options = new BitmapFactory.Options();
            if (reqWidth > -1 && reqHeight > -1)
            {
                options.InJustDecodeBounds = true;
                BitmapFactory.DecodeResource(res, resId, options);

                // Calculate inSampleSize
                options.InSampleSize = CalculateInSampleSize(options, reqWidth, reqHeight);

                // Decode bitmap with inSampleSize set
                options.InJustDecodeBounds = false;
            }
            return BitmapFactory.DecodeResource(res, resId, options);
        }

        public static Bitmap DecodeSampledBitmapFromFile(string filename, int reqWidth, int reqHeight, ImageCache cache)
        {
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            BitmapFactory.DecodeFile(filename, options);

            AddInBitmapOptions(options, cache);

            options.InJustDecodeBounds = false;
            return BitmapFactory.DecodeFile(filename, options);
        }

        private static void AddInBitmapOptions(BitmapFactory.Options options, ImageCache cache)
        {
            // inBitmap only works with mutable bitmaps, so force the decoder to
            // return mutable bitmaps.
            options.InMutable = true;

            if (cache != null)
            {
                // Try to find a bitmap to use for inBitmap.
                Bitmap inBitmap = cache.GetBitmapFromReusableSet(options);

                if (inBitmap != null)
                {
                    // If a suitable bitmap has been found, set it as the value of
                    // inBitmap.
                    options.InBitmap = inBitmap;
                }
            }
        }

        protected Bitmap GetBitmapFromReusableSet(BitmapFactory.Options options)
        {
            Bitmap bitmap = null;

            if (reusableBitmaps != null && !reusableBitmaps.IsEmpty)
            {
                foreach (var rBitmap in reusableBitmaps.ToList())
                {
                    var obj = rBitmap.Get();
                    bool remove = true;
                    if (obj != null)
                    {
                        var item = obj.JavaCast<Bitmap>();

                        if (item.IsMutable)
                        {
                            // Check to see it the item can be used for inBitmap.
                            if (CanUseForInBitmap(item, options))
                            {
                                bitmap = item;

                                // Remove from reusable set so it can't be used again.
                                SoftReference result;
                                reusableBitmaps.TryTake(out result);
                                break;
                            }
                            else
                                remove = false;
                        }
                    }
                    if (remove)
                    {
                        SoftReference result;
                        reusableBitmaps.TryTake(out result);
                    }
                }
            }
            return bitmap;
        }

        static bool CanUseForInBitmap(Bitmap candidate, BitmapFactory.Options targetOptions)
        {

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                // From Android 4.4 (KitKat) onward we can re-use if the byte size of
                // the new bitmap is smaller than the reusable bitmap candidate
                // allocation byte count.
                int width = targetOptions.OutWidth / targetOptions.InSampleSize;
                int height = targetOptions.OutHeight / targetOptions.InSampleSize;
                int byteCount = width * height * getBytesPerPixel(candidate.GetConfig());
                return byteCount <= candidate.AllocationByteCount;
            }

            // On earlier versions, the dimensions must match exactly and the inSampleSize must be 1
            return candidate.Width == targetOptions.OutWidth
                    && candidate.Height == targetOptions.OutHeight
                    && targetOptions.InSampleSize == 1;
        }

        /**
         * A helper function to return the byte usage per pixel of a bitmap based on its configuration.
         */
        static int getBytesPerPixel(Bitmap.Config config)
        {
            if (config == Bitmap.Config.Argb8888)
            {
                return 4;
            }
            else if (config == Bitmap.Config.Rgb565)
            {
                return 2;
            }
            else if (config == Bitmap.Config.Argb4444)
            {
                return 2;
            }
            else if (config == Bitmap.Config.Alpha8)
            {
                return 1;
            }
            return 1;
        }
    }

    public class BitmapLruCache : LruCache
    {
        static readonly int maxMemory = (int)(Runtime.GetRuntime().MaxMemory() / 1024);
        readonly System.Collections.Concurrent.ConcurrentBag<SoftReference> reusableBitmaps;

        public BitmapLruCache(System.Collections.Concurrent.ConcurrentBag<SoftReference> reusableBitmaps)
            : base(maxMemory / 8)
        {
            this.reusableBitmaps = reusableBitmaps;
        }

        protected BitmapLruCache(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {

        }

        protected override int SizeOf(Java.Lang.Object key, Java.Lang.Object value)
        {
            return value.JavaCast<Bitmap>().ByteCount / 1024;
        }

        protected override void EntryRemoved(bool evicted, Java.Lang.Object key, Java.Lang.Object oldValue, Java.Lang.Object newValue)
        {
            base.EntryRemoved(evicted, key, oldValue, newValue);

            reusableBitmaps.Add(new SoftReference(oldValue));
        }
    }

    public class BitmapWorkerTask : AsyncTask
    {
        private readonly WeakReference<ImageView> imageViewReference;
        private int data = 0, widthRequest, heightRequest;
        private ImageCache cache;

        public event EventHandler<Bitmap> Finished;

        public BitmapWorkerTask(ImageView imageView, int widthRequest, int heightRequest, ImageCache cache = null)
        {
            imageViewReference = new WeakReference<ImageView>(imageView);
            this.cache = cache;
            this.widthRequest = widthRequest;
            this.heightRequest = heightRequest;
        }

        protected BitmapWorkerTask(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {

        }

        protected override Java.Lang.Object DoInBackground(params Java.Lang.Object[] native_parms)
        {
            data = (int)native_parms[0];
            ImageView imageView;
            if (imageViewReference.TryGetTarget(out imageView))
            {
                var bitmap = ImageCache.DecodeSampledBitmapFromResource(imageView.Resources, data, widthRequest, heightRequest);
                if (cache != null)
                    cache.Add(data.ToString(), bitmap);
                return bitmap;
            }
            return null;
        }

        protected override void OnPostExecute(Java.Lang.Object result)
        {
            if (imageViewReference != null && result != null)
            {
                var bitmap = result.JavaCast<Bitmap>();
                ImageView imageView;
                if (imageViewReference.TryGetTarget(out imageView))
                {
                    try
                    {
                        var bitmapWorkerTask = GetBitmapWorkerTask(imageView);
                        if (this == bitmapWorkerTask && imageView != null)
                        {
                            if (Finished != null)
                                Finished(this, bitmap);
                            else
                                imageView.SetImageBitmap(bitmap);
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }

        private static BitmapWorkerTask GetBitmapWorkerTask(ImageView imageView)
        {
            if (imageView != null)
            {
                Drawable drawable = imageView.Drawable;
                if (drawable is AsyncDrawable)
                {
                    AsyncDrawable asyncDrawable = (AsyncDrawable)drawable;
                    return asyncDrawable.GetBitmapWorkerTask();
                }
            }
            return null;
        }

        public static bool CancelPotentialWork(int data, ImageView imageView)
        {
            BitmapWorkerTask bitmapWorkerTask = GetBitmapWorkerTask(imageView);

            if (bitmapWorkerTask != null)
            {
                int bitmapData = bitmapWorkerTask.data;
                // If bitmapData is not yet set or it differs from the new data
                if (bitmapData == 0 || bitmapData != data)
                {
                    // Cancel previous task
                    bitmapWorkerTask.Cancel(true);
                }
                else
                {
                    // The same work is already in progress
                    return false;
                }
            }
            // No task associated with the ImageView, or an existing task was cancelled
            return true;
        }
    }

    public class AsyncDrawable : BitmapDrawable
    {
        private readonly WeakReference<BitmapWorkerTask> bitmapWorkerTaskReference;

        public AsyncDrawable(Resources res, Bitmap bitmap,
                BitmapWorkerTask bitmapWorkerTask)
            : base(res, bitmap)
        {
            bitmapWorkerTaskReference =
                new WeakReference<BitmapWorkerTask>(bitmapWorkerTask);
        }

        protected AsyncDrawable(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {

        }

        public BitmapWorkerTask GetBitmapWorkerTask()
        {
            BitmapWorkerTask task;
            if (bitmapWorkerTaskReference.TryGetTarget(out task))
                return task;
            return null;
        }
    }
}