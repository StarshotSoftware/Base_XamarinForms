using BaseStarShot.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using CoreGraphics;
using System.Drawing;

using System.Threading.Tasks;
using System.IO;

using BaseStarShot.Enums;
using Base1902.IO;
using Base1902;

namespace BaseStarShot.Services
{
	public class ImageService : IImageService
    {
        public async Task<Base1902.IO.File> ResizeImageAsync(Base1902.IO.File file, float width, float height)
        {
            if (file == null)
                throw new ArgumentNullException("file");
            if (!file.GetMimeType().Contains("image"))
                throw new InvalidOperationException("Only images can be resized");

            var imageData = await file.GetDataAsync();

            // Load the bitmap 
            UIImage image = ImageFromByteArray(imageData);
            UIImage originalImage = RotateUpright(image);
            image.Dispose();
            UIImage resizedImage;
            float aspectRatio = width / height;
            float originalRatio = (float)originalImage.CGImage.Width / (float)originalImage.CGImage.Height;
            if (originalRatio >= aspectRatio)
            {
                var percentWidth = aspectRatio / originalRatio;
                nint newWidth = (nint)(originalImage.CGImage.Width * percentWidth);
                using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
                    newWidth, originalImage.CGImage.Height, 8,
                    4 * newWidth, CGColorSpace.CreateDeviceRGB(), CGBitmapFlags.PremultipliedFirst))
                {
                    context.ClipToRect(new RectangleF(0, 0, newWidth, originalImage.CGImage.Height));

                    RectangleF imageRect = new RectangleF(-(nint)((originalImage.CGImage.Width * (1f - percentWidth)) / 2),
                        0, originalImage.CGImage.Width, originalImage.CGImage.Height);

                    context.DrawImage(imageRect, originalImage.CGImage);

                    resizedImage = UIImage.FromImage(context.ToImage());
                }
            }
            else
            {
                var percentHeight = originalRatio / aspectRatio;
                nint newHeight = (nint)(originalImage.CGImage.Height * percentHeight);
                using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
                    originalImage.CGImage.Width, newHeight, 8,
                    4 * originalImage.CGImage.Width, CGColorSpace.CreateDeviceRGB(), CGBitmapFlags.PremultipliedFirst))
                {
                    context.ClipToRect(new RectangleF(0, 0, originalImage.CGImage.Width, newHeight));

                    RectangleF imageRect = new RectangleF(0,
                        -(nint)((originalImage.CGImage.Height * (1f - percentHeight)) / 2), originalImage.CGImage.Width, originalImage.CGImage.Height);

                    context.DrawImage(imageRect, originalImage.CGImage);

                    resizedImage = UIImage.FromImage(context.ToImage());
                }
            }

            if (resizedImage.CGImage.Height != (nint)height || resizedImage.CGImage.Width != (nint)width)
            {
                var oldImage = resizedImage;

				using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
                    (nint)width, (nint)height, 8, 4 * (nint)width,
                    CGColorSpace.CreateDeviceRGB(), CGBitmapFlags.PremultipliedFirst))
                {
                    context.DrawImage(new CGRect(0, 0, width, height), resizedImage.CGImage);

                    resizedImage = UIImage.FromImage(context.ToImage());
                }

                oldImage.Dispose();
            }

            string extension;
            bool usePng = false;
            if (file.GetMimeType() == Base1902.IO.File.PngMimeType)
            {
                extension = ".png";
                usePng = true;
            }
            else
                extension = ".jpg";

            var resizedImageFile = Base1902.IO.File.FromData(file.NameWithouthExtension + string.Format("_{0}x{1}", width, height),
                extension,
                file.Folder,
				(usePng ? resizedImage.AsPNG() : resizedImage.AsJPEG(0.8f)).ToArray());
            resizedImage.Dispose();
            //await Resolver.Get<IFileService>().SaveAsync(resizedImageFile);
            return resizedImageFile;
        }

        public Task<Base1902.IO.File> ResizeImageAsync(ScaleType scaleType, Base1902.IO.File file, float maxWidth, float maxHeight)
        {
            throw new NotImplementedException();
        }

        public Task<System.IO.Stream> ResizeImageFromStreamAsync(ScaleType scaleType, System.IO.Stream imageStream, float maxWidth, float maxHeight)
        {
            throw new NotImplementedException();
        }

		public async Task<Base1902.IO.File> PremiumResizeImage(Base1902.IO.File file, float maxWidth, float maxHeight)
		{
			if (file == null)
				throw new ArgumentNullException("file");
			if (!file.GetMimeType().Contains("image"))
				throw new InvalidOperationException("Only images can be resized");

			var imageData = await file.GetDataAsync();

			UIImage image = ImageFromByteArray(imageData);
			UIImage originalImage = RotateUpright(image);
			image.Dispose();

			if (originalImage.CGImage.Height > maxHeight || originalImage.CGImage.Width > maxWidth)
			{

				if (originalImage.CGImage.Height > originalImage.CGImage.Width)
				{
					float aspect = (float)originalImage.CGImage.Width
						/ (float)originalImage.CGImage.Height;
					int newWidth = 0;
					newWidth = (int)(maxHeight * aspect);
					return await ResizeImageAsync(file, newWidth, maxHeight);


				}
				else
				{
					float aspect = (float)originalImage.CGImage.Height
						/ (float)originalImage.CGImage.Width;
					int newHeight = 0;
					newHeight = (int)(maxWidth * aspect);
					return await ResizeImageAsync(file, maxWidth, newHeight);
				}
			}
			else
			{
				return await ResizeImageAsync(file, originalImage.CGImage.Width, originalImage.CGImage.Height);
			}

		}

		public async Task<Base1902.IO.File> RotateBitmap(Base1902.IO.File file, int degrees)
		{
			var imageData = await file.GetDataAsync();
			UIImage source = ImageFromByteArray(imageData);

			UIGraphics.BeginImageContext(new CGSize(source.Size.Height, source.Size.Width));
			var context = UIGraphics.GetCurrentContext();

			context.RotateCTM((nfloat)(-degrees / 180d * Math.PI));
			context.TranslateCTM(-source.Size.Width, 0);

			source.Draw(new CGRect(0, 0, source.Size.Width, source.Size.Height));

			var image = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();

			var imageFile = Base1902.IO.File.FromData(file.NameWithouthExtension, ".jpg", file.Folder, image.AsJPEG(0.8f).ToArray());
			image.Dispose();

			return imageFile;
		}

		public async Task<Base1902.IO.File> RotateUpright(Base1902.IO.File file)
		{
			var imageData = await file.GetDataAsync();

			// Load the bitmap 
			UIImage image = ImageFromByteArray(imageData);
			UIImage correctedImage = RotateUpright(image);
			image.Dispose();

			string extension;
			bool usePng = false;
			if (file.GetMimeType() == Base1902.IO.File.PngMimeType)
			{
				extension = ".png";
				usePng = true;
			}
			else
				extension = ".jpg";

			var resizedImageFile = Base1902.IO.File.FromData(file.NameWithouthExtension, extension, file.Folder,
				(usePng ? correctedImage.AsPNG() : correctedImage.AsJPEG(0.8f)).ToArray());
			correctedImage.Dispose();
			await Resolver.Get<IFileService>().SaveAsync(resizedImageFile);
			return resizedImageFile;
		}

        public static UIImage RotateUpright(UIImage image)
        {
            Func<double, nfloat> rad = deg => (nfloat)(deg / 180d * Math.PI);

            nint width = image.CGImage.Width,
                height = image.CGImage.Height;

            CGImage imageRef = image.CGImage;
            CGBitmapFlags bitmapInfo = imageRef.BitmapInfo;
            CGColorSpace colorSpaceInfo = imageRef.ColorSpace;

            if (bitmapInfo == CGBitmapFlags.None)
                bitmapInfo = CGBitmapFlags.NoneSkipLast;

            CGBitmapContext bitmap;

            if (image.Orientation == UIImageOrientation.Up || image.Orientation == UIImageOrientation.Down)
                bitmap = new CGBitmapContext(null, width, height, imageRef.BitsPerComponent, imageRef.BytesPerRow, colorSpaceInfo, bitmapInfo);
            else
                bitmap = new CGBitmapContext(null, height, width, imageRef.BitsPerComponent, imageRef.BytesPerRow, colorSpaceInfo, bitmapInfo);

            switch (image.Orientation)
            {
                case UIImageOrientation.Left:
                    bitmap.RotateCTM(rad(90));
                    bitmap.TranslateCTM(0, -height);
                    break;
                case UIImageOrientation.Right:
                    bitmap.RotateCTM(rad(-90));
                    bitmap.TranslateCTM(-width, 0);
                    break;
                case UIImageOrientation.Down:
                    bitmap.TranslateCTM(width, height);
                    bitmap.RotateCTM(rad(-180));
                    break;
            }

            bitmap.DrawImage(new CGRect(0, 0, width, height), imageRef);
            CGImage imageRef2 = bitmap.ToImage();
            UIImage result = UIImage.FromImage(imageRef2);

            bitmap.Dispose();
            imageRef.Dispose();
            imageRef2.Dispose();

            return result;

            //nint width, height;
            //CGAffineTransform rectTransform;
            //switch (image.Orientation)
            //{
            //    case UIImageOrientation.Left:
            //        rectTransform = CGAffineTransform.MakeRotation(rad(90));
            //        height = image.CGImage.Width;
            //        width = image.CGImage.Height;
            //        break;
            //    case UIImageOrientation.Right:
            //        rectTransform = CGAffineTransform.MakeRotation(rad(-90));
            //        height = image.CGImage.Width;
            //        width = image.CGImage.Height;
            //        break;
            //    case UIImageOrientation.Down:
            //        rectTransform = CGAffineTransform.MakeRotation(rad(-180));
            //        width = image.CGImage.Width;
            //        height = image.CGImage.Height;
            //        break;
            //    default:
            //        rectTransform = CGAffineTransform.MakeIdentity();
            //        width = image.CGImage.Width;
            //        height = image.CGImage.Height;
            //        break;
            //}
            //rectTransform = CGAffineTransform.Scale(rectTransform, image.CurrentScale, image.CurrentScale);

            //CGImage imageRef = image.CGImage.WithImageInRect(rectTransform.TransformRect(new CGRect(0, 0, width, height)));
            //UIImage result = UIImage.FromImage(imageRef, image.CurrentScale, UIImageOrientation.Up);
            //imageRef.Dispose();

            //return result;
        }

        public static UIImage ImageFromByteArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            UIKit.UIImage image;
            try
            {
                image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
            }
            catch (Exception ex)
            {
                Logger.Write("ImageService", "Image load failed: " + ex.Message, LogType.Error);
                return null;
            }
            return image;
        }
    }
}