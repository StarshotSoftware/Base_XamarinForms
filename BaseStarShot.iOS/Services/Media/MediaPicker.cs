using System;
using System.Threading.Tasks;
using Xamarin.Media;
using XamCamDevice = Xamarin.Media.CameraDevice;
using XamVideoQuality = Xamarin.Media.VideoQuality;
using XamMediaPicker = Xamarin.Media.MediaPicker;
using Foundation;
using BaseStarShot.IO;
using UIKit;
using MediaPlayer;
using System.IO;

namespace BaseStarShot.Services.Media
{
    public class MediaPicker : IMediaPicker
    {
        private readonly XamMediaPicker picker;
        private bool isTakeVideo = false;

        public MediaPicker()
        {
            this.picker = new XamMediaPicker();
        }

        public bool IsPhotoGalleryAvailable
        {
            get { return this.picker.PhotosSupported; }
        }

        public bool IsCameraAvailable
        {
            get { return this.picker.IsCameraAvailable; }
        }

        public bool IsVideoGalleryAvailable
        {
            get { return this.picker.VideosSupported; }
        }

        public string GetCameraId { get { return string.Empty; } }

        public int GetRequestCode { get { return 0; } }

        public bool IsTakeVideo { get { return isTakeVideo; } }

        public TaskCompletionSource<object> tcs { get { return null; } }

        public async Task<IMediaFile> PickPhoto()
        {
			try {

			if (!this.IsPhotoGalleryAvailable || !picker.PhotosSupported)
				return null; //throw new ArgumentException("Photo gallery is not available");

            isTakeVideo = false;

           
                var file = await this.picker.PickPhotoAsync();
                return new MediaFile(file);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        public async Task<IMediaFile> TakePhoto(CameraOptions options)
        {

			try {
				if (!this.IsCameraAvailable || !picker.PhotosSupported)
                return null; //throw new ArgumentException("Camera is not available");

            isTakeVideo = false;

            options = options ?? new CameraOptions();
           
                var file = await this.picker.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    Directory = options.GetDirectory(),
                    Name = options.GetFileName(),
                    DefaultCamera = (XamCamDevice)Enum.Parse(typeof(XamCamDevice), options.Camera.ToString())
                });
                return new MediaFile(file);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        public async Task<IMediaFile> PickVideo()
        {
			if (!this.IsVideoGalleryAvailable || !picker.VideosSupported)
                return null; //throw new ArgumentException("Video gallery is not available");

            isTakeVideo = false;

            try
            {
                var file = await this.picker.PickVideoAsync();
                return new MediaFile(file);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        public async Task<IMediaFile> TakeVideo(VideoOptions options)
        {
            if (!this.IsCameraAvailable || !picker.VideosSupported)
                return null; //throw new ArgumentException("Camera is not available");

            isTakeVideo = true;

            options = options ?? new VideoOptions();
            try
            {
                var file = await this.picker.TakeVideoAsync(new StoreVideoOptions
                {
                    Directory = options.GetDirectory(),
                    Name = options.GetFileName(),
                    DesiredLength = options.DesiredLength,
                    DefaultCamera = (XamCamDevice)Enum.Parse(typeof(XamCamDevice), options.Camera.ToString()),
                    Quality = (XamVideoQuality)Enum.Parse(typeof(XamVideoQuality), options.Quality.ToString())
                });
                return new MediaFile(file);
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

    }
}

