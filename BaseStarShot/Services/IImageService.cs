using BaseStarShot.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BaseStarShot.Enums;
using System.IO;
using Base1902.IO;

namespace BaseStarShot.Services
{
    public interface IImageService
    {
        /// <summary>
        /// Resizes an image while maintaining aspect ratio by cropping.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        Task<File> ResizeImageAsync(File file, float width, float height);

        Task<File> RotateBitmap(File file, int degrees);
        /// <summary>
        /// Get Image
        /// </summary>
        /// <param name="file"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        Task<File> PremiumResizeImage(File file, float maxWidth, float maxHeight);

        /// <summary> 
        /// Resizes an image while maintaining its aspect ratio
        /// ScaleType
        /// Aspect Fit & Aspect Fill 
        /// </summary>
        /// <param name="scaleType"></param>
        /// <param name="file"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        Task<File> ResizeImageAsync(ScaleType scaleType, File file, float maxWidth, float maxHeight);

        /// <summary>
        /// Resizes an image while maintaining its aspect ratio by Stream
        /// ScaleType
        /// Aspect Fit & Aspect Fill 
        /// </summary>
        /// <param name="scaleType"></param>
        /// <param name="file"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <returns></returns>
        Task<Stream> ResizeImageFromStreamAsync(ScaleType scaleType, Stream imageStream, float maxWidth, float maxHeight);

		Task<File> RotateUpright(File file);
    }
}
