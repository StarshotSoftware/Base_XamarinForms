using BaseStarShot.IO;
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
using System.Threading.Tasks;
using Android.Graphics;
using System.IO;
using Android.Media;
using BaseStarShot.Enums;
using Base1902.IO;

namespace BaseStarShot.Services
{
    public class ImageService : IImageService
    {

        public async Task<Base1902.IO.File> PremiumResizeImage(Base1902.IO.File file, float maxWidth, float maxHeight)
        {
            var options = new BitmapFactory.Options();
            var bitmap = await BitmapFactory.DecodeFileAsync(file.FullPath, options);

            int rotatedWidth, rotatedHeight;
            int orientation = GetOrientation(file);

            if (orientation == 90 || orientation == 270)
            {
                rotatedWidth = options.OutHeight;
                rotatedHeight = options.OutWidth;
            }
            else
            {
                rotatedWidth = options.OutWidth;
                rotatedHeight = options.OutHeight;
            }
            if (rotatedHeight > maxHeight || rotatedWidth > maxWidth)
            {

                if (rotatedHeight > rotatedWidth)
                {
                    float aspect = (float)rotatedWidth
                            / (float)rotatedHeight;
                    int newWidth = 0;
                    newWidth = (int)(maxHeight * aspect);
                    return await ResizeImageAsync(file, newWidth, maxHeight);


                }
                else
                {
                    float aspect = (float)rotatedHeight
                       / (float)rotatedWidth;
                    int newHeight = 0;
                    newHeight = (int)(maxWidth * aspect);
                    return await ResizeImageAsync(file, maxWidth, newHeight);
                }
            }
            else
            {
                return await ResizeImageAsync(file, rotatedWidth, rotatedHeight);
            }
        }

        public async Task<Base1902.IO.File> ResizeImageAsync(Base1902.IO.File file, float width, float height)
        {
            if (file == null)
                throw new ArgumentNullException("file");
            if (!file.GetMimeType().Contains("image"))
                throw new InvalidOperationException("Only images can be resized");

            Bitmap originalImage, resizedImage;

            // Load bitmap info only
            var options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;

            using (var stream = await file.GetStreamAsync())
                await BitmapFactory.DecodeStreamAsync(stream, null, options);

            int rotatedWidth, rotatedHeight;
            int orientation = GetOrientation(file);

            if (orientation == 90 || orientation == 270)
            {
                rotatedWidth = options.OutHeight;
                rotatedHeight = options.OutWidth;
            }
            else
            {
                rotatedWidth = options.OutWidth;
                rotatedHeight = options.OutHeight;
            }

            var runtime = Java.Lang.Runtime.GetRuntime();
            var availableMemory = runtime.MaxMemory() - runtime.TotalMemory();
            var imageSize = options.OutWidth * options.OutHeight * 4 * 2.25;

            if (availableMemory < imageSize)
            {
                int sampleSize = 2;
                var overMemory = availableMemory - imageSize;
                while (availableMemory < (float)imageSize / (float)(sampleSize * 2))
                    sampleSize *= 2;

                BitmapFactory.Options opts = new BitmapFactory.Options();
                opts.InSampleSize = sampleSize;

                //using (var stream = await file.GetStreamAsync())
                //    originalImage = await BitmapFactory.DecodeStreamAsync(stream, null, opts);

                originalImage = await BitmapFactory.DecodeFileAsync(file.FullPath, opts);
            }
            else
            {
                originalImage = await BitmapFactory.DecodeFileAsync(file.FullPath);
                //using (var stream = await file.GetStreamAsync())
                //    originalImage = await BitmapFactory.DecodeStreamAsync(stream);
            }

            if (orientation > 0)
            {
                Matrix matrix = new Matrix();
                matrix.PostRotate(orientation);

                var oldImage = originalImage;
                originalImage = Bitmap.CreateBitmap(originalImage, 0, 0, originalImage.Width, originalImage.Height, matrix, true);
                oldImage.Dispose();
            }

            float aspectRatio = width / height;
            float originalRatio = (float)originalImage.Width / (float)originalImage.Height;

            if (originalRatio >= aspectRatio)
            {
                var percentWidth = aspectRatio / originalRatio;
                resizedImage = Bitmap.CreateBitmap(
                    originalImage,
                    (int)((originalImage.Width * (1f - percentWidth)) / 2),
                    0,
                    (int)(originalImage.Width * percentWidth),
                    originalImage.Height);
            }
            else
            {
                var percentHeight = originalRatio / aspectRatio;
                resizedImage = Bitmap.CreateBitmap(
                    originalImage,
                    0,
                    (int)((originalImage.Height * (1f - percentHeight)) / 2),
                    originalImage.Width,
                    (int)(originalImage.Height * percentHeight));
            }

            if (resizedImage.Height != (int)height || resizedImage.Width != (int)width)
            {
                var oldImage = resizedImage;
                resizedImage = Bitmap.CreateScaledBitmap(resizedImage, (int)width, (int)height, false);
                oldImage.Dispose();
            }
            //originalImage.Dispose();

            string extension;
            bool usePng = false;
            if (file.GetMimeType() == Base1902.IO.File.PngMimeType)
            {
                extension = ".png";
                usePng = true;
            }
            else
                extension = ".jpg";

            if (resizedImage.Handle == IntPtr.Zero)
                return null;

            using (MemoryStream ms = new MemoryStream())
            {
                await resizedImage.CompressAsync(usePng ? Bitmap.CompressFormat.Png : Bitmap.CompressFormat.Jpeg, 80, ms);
                resizedImage.Dispose();

                var resizedImageFile = Base1902.IO.File.FromData(file.NameWithouthExtension + string.Format("_{0}x{1}", width, height),
                    extension,
                    file.Folder,
                    ms.ToArray());
                //await Resolver.Get<IFileService>().SaveAsync(resizedImageFile);
                return resizedImageFile;
            }
        }

        public Task<Base1902.IO.File> ResizeImageAsync(ScaleType scaleType, Base1902.IO.File file, float maxWidth, float maxHeight)
        {
            throw new NotImplementedException();
        }

        public Task<System.IO.Stream> ResizeImageFromStreamAsync(ScaleType scaleType, System.IO.Stream imageStream, float maxWidth, float maxHeight)
        {
            throw new NotImplementedException();
        }


        public async Task<Base1902.IO.File> RotateUpright(Base1902.IO.File file)
        {
            throw new NotImplementedException();
        }

        public static int GetOrientation(Base1902.IO.File file)
        {
            int degree = -1;

            if (file.GetMimeType() != Base1902.IO.File.JpegMimeType)
                return -1;

            ExifInterface exif = null;
            try
            {
                exif = new ExifInterface(file.FullPath);
            }
            catch (Exception ex)
            {

            }

            if (exif != null)
            {
                var orientation = exif.GetAttributeInt(ExifInterface.TagOrientation, -1);

                switch (orientation)
                {
                    case -1:
                        degree = -1; break;
                    case 3:
                    case 4:
                        degree = 180; break;
                    case 5:
                    case 6:
                        degree = 90; break;
                    case 7:
                    case 8:
                        degree = 270; break;
                    default:
                        degree = 0; break;
                }
            }

            return degree;
        }


        public async Task<Base1902.IO.File> RotateBitmap(Base1902.IO.File file, int degree)
        {
            var options = new BitmapFactory.Options();
            var bitmap = BitmapFactory.DecodeFile(file.FullPath, options);
            Matrix matrix = new Matrix();
            matrix.PostRotate(-90);
            var rotatedBitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);

            using (MemoryStream ms = new MemoryStream())
            {
                await rotatedBitmap.CompressAsync(Bitmap.CompressFormat.Png, 80, ms);
                rotatedBitmap.Dispose();

                var resizedImageFile = Base1902.IO.File.FromData(file.NameWithouthExtension,
                    ".png",
                    file.Folder,
                    ms.ToArray());
                //await Resolver.Get<IFileService>().SaveAsync(resizedImageFile);
                return resizedImageFile;
            }
        }

        //public static int GetOrientation(Base1902.IO.File file)
        //{
        //    //var filePath = BaseStarShot.Controls.UIHelper.Context.GetFileStreamPath("file:///mnt" + file.FullPath);
        //    var filePath = new Java.Base1902.IO.File(file.FullPath);

        //    Android.Database.ICursor cursor = BaseStarShot.Controls.UIHelper.Context.ContentResolver.Query(
        //        //Android.Net.Uri.Parse("content://" + file.FullPath),
        //        Android.Net.Uri.FromFile(filePath),
        //        new string[] { Android.Provider.MediaStore.Images.ImageColumns.Orientation }, null, null, null);


        //    if (cursor == null || cursor.Count != 1)
        //        return -1;

        //    cursor.MoveToFirst();
        //    return cursor.GetInt(0);
        //}
    }
}