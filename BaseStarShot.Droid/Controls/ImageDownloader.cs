using Android.Content;
using Android.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace BaseStarShot.Controls
{
    public class ImageInProgress
    {
        public string URI { get; set; }

        public Task<Bitmap> LoadingTask { get; set; }
    }

    public abstract class ImageDownloader
    {
        //readonly IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication();
        readonly ThrottledHttp http;
        readonly TimeSpan cacheDuration;

        public event EventHandler<Bitmap> Finished = delegate { };
        public event EventHandler Started = delegate { };

        private static ReaderWriterLockSlim downloadLocker;
        private static List<ImageInProgress> imageDownloadsInProgress;
        private readonly string FOLDER = "ImageCache";

        public ImageDownloader(int maxConcurrentDownloads = 2)
            : this(TimeSpan.FromDays(1))
        {
            http = new ThrottledHttp(maxConcurrentDownloads);
        }

        public ImageDownloader(TimeSpan cacheDuration)
        {
            this.cacheDuration = cacheDuration;

            if (imageDownloadsInProgress == null)
                imageDownloadsInProgress = new List<ImageInProgress>();
            if (downloadLocker == null)
                downloadLocker = new ReaderWriterLockSlim();

            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!store.DirectoryExists(FOLDER))
                {
                    store.CreateDirectory(FOLDER);
                }
            }
        }

        public void DeleteCache()
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.DirectoryExists(FOLDER))
                {
                    store.DeleteDirectory(FOLDER);
                }
            }
        }

        private void RemoveDownloadProgress(Uri uri)
        {
            if (imageDownloadsInProgress != null && imageDownloadsInProgress.Count > 0)
            {
                downloadLocker.EnterWriteLock();
                try
                {
                    var index = imageDownloadsInProgress.FindIndex(p => p.URI == Uri.EscapeUriString(uri.AbsoluteUri));
                    if (index >= 0 && imageDownloadsInProgress.Count > index)
                        imageDownloadsInProgress.RemoveAt(index);
                }
                finally
                {
                    if (downloadLocker.IsWriteLockHeld)
                        downloadLocker.ExitWriteLock();
                }
            }
        }

        public bool HasLocallyCachedCopy(Uri uri)
        {
            //using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            //{
            //    var path = System.IO.Path.Combine(FOLDER, Uri.EscapeUriString(uri.AbsoluteUri));

            //    if (store.FileExists(path))
            //    {
            //        return true;
            //    }
            //    else
            //        return false;
            //}

            var now = DateTime.UtcNow;
            var filename = Uri.EscapeDataString(uri.AbsoluteUri);
            var lastWriteTime = GetLastWriteTimeUtc(filename);

            return lastWriteTime.HasValue;
            //return lastWriteTime.HasValue && ((now - lastWriteTime.Value) < cacheDuration);
        }

        public Task<Bitmap> GetImageAsync(Uri uri)
        {
            downloadLocker.EnterWriteLock();
            try
            {
                var task = imageDownloadsInProgress.Find(p => p.URI == Uri.EscapeUriString(uri.AbsoluteUri));
                if (task != null)
                {
                    return task.LoadingTask;
                }
                else
                {
                    var loadImageTask = Task.Factory.StartNew(() =>
                    {
                        return GetImage(uri);
                    });

                    imageDownloadsInProgress.Add(new ImageInProgress { URI = uri.AbsoluteUri, LoadingTask = loadImageTask });
                    return loadImageTask;
                }
            }
            finally
            {
                if (downloadLocker.IsWriteLockHeld)
                    downloadLocker.ExitWriteLock();
            }
        }

        public Bitmap GetImage(Uri uri)
        {
            var filename = Uri.EscapeDataString(uri.AbsoluteUri);

            Started(this, null);
            if (HasLocallyCachedCopy(uri))
            {
                Bitmap myBitmap = null;

                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    try
                    {
                        //var task = imageDownloadsInProgress.Find(p => p.URI == Uri.EscapeUriString(uri.AbsoluteUri));
                        //if (task != null)
                        //{

                        //}

                        IsolatedStorageFileStream isoFileStream = store.OpenFile(System.IO.Path.Combine(FOLDER, filename), FileMode.Open);
                        var bitmap = LoadImage(isoFileStream);
                        Finished(this, bitmap);

                        myBitmap = bitmap;

                        isoFileStream.Close();
                    }
                    catch (Exception)
                    {
                        RemoveDownloadProgress(uri);
                        return null;
                    }

                    RemoveDownloadProgress(uri);
                    return myBitmap;

                }
            }
            else
            {
                using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    Stream stream = http.Get(uri);
                    if (stream != null)
                    {
                        using (IsolatedStorageFileStream isoFileStream = store.OpenFile(System.IO.Path.Combine(FOLDER, filename), FileMode.Create))
                        {
                            stream.CopyTo(isoFileStream);
                        }
                        stream.Close();
                    }
                    else
                    {
                        RemoveDownloadProgress(uri);
                        return null;
                    }

                    using (IsolatedStorageFileStream isoFileStream = store.OpenFile(System.IO.Path.Combine(FOLDER, filename), FileMode.Open))
                    {
                        var bitmap = LoadImage(isoFileStream);
                        Finished(this, bitmap);

                        RemoveDownloadProgress(uri);
                        return bitmap;
                    }
                }

            }
        }

        protected virtual DateTime? GetLastWriteTimeUtc(string fileName)
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                var path = System.IO.Path.Combine(FOLDER, fileName);

                if (store.FileExists(path))
                {
                    var hasDate = store.GetLastWriteTime(path).UtcDateTime;
                    return hasDate;
                }
                else
                    return null;
            }
        }

        protected virtual Stream OpenStorage(string fileName, FileMode mode)
        {
            using (IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForApplication())
                return store.OpenFile(System.IO.Path.Combine(FOLDER, fileName), mode);
        }

        protected abstract Bitmap LoadImage(Stream stream);
    }

    public class ThrottledHttp
    {
        Semaphore throttle;

        public ThrottledHttp(int maxConcurrent)
        {
            throttle = new Semaphore(maxConcurrent, maxConcurrent);
        }

        /// <summary>
        /// Get the specified resource. Blocks the thread until
        /// the throttling logic allows it to execute.
        /// </summary>
        /// <param name='uri'>
        /// The URI of the resource to get.
        /// </param>
        public Stream Get(Uri uri)
        {
            throttle.WaitOne();

            var req = WebRequest.Create(uri);

            var getTask = Task.Factory.FromAsync<WebResponse>(
                              req.BeginGetResponse, req.EndGetResponse, null);

            return getTask.ContinueWith(task =>
            {
                throttle.Release();
                if (task.IsFaulted)
                {
                    return null;
                }
                else
                {
                    var res = task.Result;
                    return res.GetResponseStream();
                }
            }).Result;
        }
    }

    public class AndroidImageDownloader : ImageDownloader
    {
        public AndroidImageDownloader()
            : base(2)
        {
        }

        protected override Bitmap LoadImage(System.IO.Stream stream)
        {
            return BitmapFactory.DecodeStream(stream);
        }
    }
}