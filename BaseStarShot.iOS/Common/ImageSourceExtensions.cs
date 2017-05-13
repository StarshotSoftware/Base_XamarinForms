using System;
using UIKit;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Threading;
using CoreGraphics;

namespace BaseStarShot
{
    public static class ImageSourceExtensions
    {
        public static Task<UIImage> GetUIImage(this ImageSource imageSource)
        {
            return GetUIImage(imageSource, default(CancellationToken));
        }

        public static async Task<UIImage> GetUIImage(this ImageSource imageSource, CancellationToken cancellationToken)
        {
            try
            {
                IImageSourceHandler handler = null;

                if (imageSource is FileImageSource)
                {
                    handler = new FileImageSourceHandler();
                }
                else if (imageSource is StreamImageSource)
                {
                    handler = new StreamImagesourceHandler();
                }
                else if (imageSource is UriImageSource)
                {
                    handler = new ImageLoaderSourceHandler();
                }
                else
                {
                    throw new NotImplementedException("Image source type is not supported.");
                }

                using (var image = await handler.LoadImageAsync(imageSource, cancellationToken, (float)UIScreen.MainScreen.Scale))
                //using (var image = await handler.LoadImageAsync(imageSource))
                {
                    if (image == null)
                        return null;

                    UIGraphics.BeginImageContext(image.Size);
                    image.Draw(new CGRect(0, 0, image.Size.Width, image.Size.Height));
                    return UIGraphics.GetImageFromCurrentImageContext();
                }
                //return handler.LoadImageAsync (imageSource, cancellationToken, (float)UIScreen.MainScreen.Scale);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}

