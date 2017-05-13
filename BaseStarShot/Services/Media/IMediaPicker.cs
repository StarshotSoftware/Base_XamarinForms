using BaseStarShot.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Services
{
    public interface IMediaPicker
    {
        /// <summary>
        /// For WinRT Gets the hardware id to make custom MediaCapture
        /// </summary>
        /// 
        string GetCameraId { get; }

        /// <summary>
        /// Only for Android
        /// </summary>
        TaskCompletionSource<object> tcs { get; }

        int GetRequestCode { get; }
        
        bool IsTakeVideo { get; }

        bool IsCameraAvailable { get; }
        bool IsPhotoGalleryAvailable { get; }
        bool IsVideoGalleryAvailable { get; }

        Task<IMediaFile> PickPhoto();
        Task<IMediaFile> TakePhoto(CameraOptions options = null);
        Task<IMediaFile> PickVideo();
        Task<IMediaFile> TakeVideo(VideoOptions config = null);
    }
}
