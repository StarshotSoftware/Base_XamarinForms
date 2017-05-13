using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Media;
using XamCamDevice = Xamarin.Media.CameraDevice;
using XamVideoQuality = Xamarin.Media.VideoQuality;
using XamMediaPicker = Xamarin.Media.MediaPicker;
using System.Net;
using Android.App;
using Android.Content;
using Android.Provider;
using Java.IO;
using Android.Support.V4.Content;
using Android;

namespace BaseStarShot.Services.Media
{
    public class MediaPicker : IMediaPicker
    {
        TaskCompletionSource<object> tcs;

        public static int MediaPickerRequestCode = 333;

        private readonly XamMediaPicker picker;
        private bool isTakeVideo = false;

        private readonly string[] permissionStrings = new string[] 
        {
            Manifest.Permission.RecordAudio, Manifest.Permission.Camera,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.ReadExternalStorage
        };

        public MediaPicker()
        {
            this.picker = new XamMediaPicker(Forms.Context);
        }

        public bool IsPhotoGalleryAvailable
        {
            get { return this.picker.PhotosSupported; }
        }

        public string GetCameraId { get { return string.Empty; } }


        public bool IsCameraAvailable
        {
            get { return this.picker.IsCameraAvailable; }
        }


        public bool IsVideoGalleryAvailable
        {
            get { return this.picker.VideosSupported; }
        }

        public async Task<IMediaFile> PickPhoto()
        {
            if (!this.IsPhotoGalleryAvailable)
                throw new ArgumentException("Photo gallery is not available");

            isTakeVideo = false;

            try
            {
                tcs = new TaskCompletionSource<object>();
                var intent = picker.GetPickPhotoUI();
                var context = Forms.Context as Activity;
                context.StartActivityForResult(intent, MediaPickerRequestCode);

                var file = await tcs.Task;
                if (file != null)
                {
                    var mediaFile = (Xamarin.Media.MediaFile)file;
                    return new BaseStarShot.Services.Media.MediaFile(new Java.IO.File(mediaFile.Path));
                }
                return null;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        public async Task<IMediaFile> TakePhoto(CameraOptions options)
        {
            if (!this.IsCameraAvailable)
                throw new ArgumentException("Camera is not available");

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                var context = Forms.Context as Activity;

                if (ContextCompat.CheckSelfPermission(context, Manifest.Permission.Camera) == Android.Content.PM.Permission.Denied)
                {
                    Android.Support.V4.App.ActivityCompat.RequestPermissions(context as Activity, new String[] { Manifest.Permission.Camera }, 0);
                }

                if (ContextCompat.CheckSelfPermission(context, Manifest.Permission.Camera) == Android.Content.PM.Permission.Denied)
                    return null;
            }

            options = options ?? new CameraOptions();

            isTakeVideo = false;

            try
            {
                tcs = new TaskCompletionSource<object>();

                var intent = picker.GetTakePhotoUI(new StoreCameraMediaOptions
                {
                    Directory = options.GetDirectory(),
                    Name = options.GetFileName(),
                    DefaultCamera = (XamCamDevice)Enum.Parse(typeof(XamCamDevice), options.Camera.ToString())
                });
                var context = Forms.Context as Activity;
                context.StartActivityForResult(intent, MediaPickerRequestCode);

                var file = await tcs.Task;
                if (file != null)
                {
                    var mediaFile = (Xamarin.Media.MediaFile)file;
                    return new BaseStarShot.Services.Media.MediaFile(new Java.IO.File(mediaFile.Path));
                }
                return null;
            }
            catch (OperationCanceledException)
            {
                return null;
            }
            catch (Exception e)
            {
                Logger.DebugWrite("Test", "Error" + e);
                return null;
            }
        }

        public async Task<IMediaFile> PickVideo()
        {
            if (!this.IsVideoGalleryAvailable)
                throw new ArgumentException("Video gallery is not available");

            isTakeVideo = false;

            try
            {
                tcs = new TaskCompletionSource<object>();

                var intent = picker.GetPickVideoUI();
                var context = Forms.Context as Activity;
                context.StartActivityForResult(intent, MediaPickerRequestCode);

                var file = await tcs.Task;
                if (file != null)
                {
                    var mediaFile = (Xamarin.Media.MediaFile)file;
                    return new BaseStarShot.Services.Media.MediaFile(new Java.IO.File(mediaFile.Path));
                }
                return null;
            }
            catch (OperationCanceledException)
            {
                return null;
            }


        }

        //protected override void OnCreate (Bundle bundle)
        //{
        //    var picker = new MediaPicker (this);
        //    if (!picker.IsCameraAvailable)
        //        Console.WriteLine ("No camera!");
        //    else {
        //        var intent = picker.GetTakePhotoUI (new StoreCameraMediaOptions {
        //            Name = "test.jpg",
        //            Directory = "MediaPickerSample"
        //        });
        //        StartActivityForResult (intent, 1);
        //    }
        //}

        //protected override async void OnActivityResult (int requestCode, Result resultCode, Intent data)
        //{
        //    // User canceled
        //    if (resultCode == Result.Canceled)
        //        return;

        //    MediaFile file = await data.GetMediaFileExtraAsync (this);
        //    Console.WriteLine (file.Path);
        //}

        private bool HasTakeVideoPermissions()
        {
            foreach (var permission in permissionStrings)
            {
                if (ContextCompat.CheckSelfPermission(Forms.Context, permission) == Android.Content.PM.Permission.Denied)
                    return false;
            }

            return true;
        }

        public async Task<IMediaFile> TakeVideo(VideoOptions options)
        {
            if (!this.IsCameraAvailable)
                throw new ArgumentException("Camera is not available");

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.M)
            {
                var context = Forms.Context as Activity;

                bool hasVideoPermissions = HasTakeVideoPermissions();
                if (!hasVideoPermissions)
                {
                    Android.Support.V4.App.ActivityCompat.RequestPermissions(context, permissionStrings, 200);
                }

                if (!HasTakeVideoPermissions())
                {
                    return null;
                }
            }

            isTakeVideo = true; 
            options = options ?? new VideoOptions();
            try
            {
                tcs = new TaskCompletionSource<object>();
                
                Intent intent = new Intent(MediaStore.ActionVideoCapture);
                intent.PutExtra(MediaStore.ExtraDurationLimit, options.DesiredLength.TotalSeconds);
                intent.PutExtra(MediaStore.ExtraVideoQuality, (int)VideoQuality.Medium);

                var context = Forms.Context as Activity;
                var appLabel = context.Application.ApplicationInfo.LoadLabel(context.PackageManager);
                var appName = string.IsNullOrWhiteSpace(appLabel) ? "CameraApp" : appLabel;
                var pictureDirectory = new File(Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDcim), appName);

                if (!pictureDirectory.Exists())
                {
                    pictureDirectory.Mkdirs();
                }

                var file = new Java.IO.File(pictureDirectory, String.Format("video_{0}.mp4", DateTime.Now.ToMilliseconds().ToString()));
                intent.PutExtra(MediaStore.ExtraOutput, Android.Net.Uri.FromFile(file));

                //var intent = picker.GetTakeVideoUI(new StoreVideoOptions
                //{
                //    Directory = "MobileXamarinDirectory",
                //    Name = DateTime.Now.Millisecond + "",
                //    DesiredLength = options.DesiredLength,
                //    //DefaultCamera = (XamCamDevice)Enum.Parse(typeof(XamCamDevice), options.Camera.ToString()),
                //    //Quality = (XamVideoQuality)Enum.Parse(typeof(XamVideoQuality), options.Quality.ToString())
                //});

                context.StartActivityForResult(intent, MediaPickerRequestCode);

                var mediaUriTask = await tcs.Task;
                var mediaUri = (Android.Net.Uri)mediaUriTask;

                if (mediaUri != null)
                {
                    var data = new BaseStarShot.Services.Media.MediaFile(file);
                    return data;
                }
                else
                {
                    return null;
                }
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        public Task<IMediaFile> DownloadUri(Uri uri)
        {
            throw new NotImplementedException();
        }


        public int GetRequestCode
        {
            get { return MediaPickerRequestCode; }
        }

        public bool IsTakeVideo
        {
            get { return isTakeVideo; }
        }

        TaskCompletionSource<object> IMediaPicker.tcs
        {
            get { return tcs; }
        }
    }
}