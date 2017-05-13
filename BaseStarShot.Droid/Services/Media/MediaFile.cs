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
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using Base1902;

namespace BaseStarShot.Services.Media
{
    public class MediaFile : IMediaFile
    {
        private readonly Java.IO.File file; //Xamarin.Media.MediaFile file;

        //public MediaFile(Xamarin.Media.MediaFile file)
        //{
        //    this.file = file;
        //}
        public MediaFile(Java.IO.File file)
        {
            this.file = file;
        }

        public string Path
        {
            get { return this.file.Path; }
        }

        public Stream GetStream()
        {
            return File.OpenRead(file.Path);
            //return this.file.GetStream();
        }

        public void Dispose()
        {
            this.file.Dispose();
        }

        public Task<Stream> GetStreamAsync()
        {
            return Task.FromResult(GetStream());
        }



        public Task<Stream> GetUriStream()
        {
            throw new NotImplementedException();
        }

        public byte[] GetUriStreamBytes()
        {
            throw new NotImplementedException();
        }


        public long Size
        {
            get
            {
                if (file != null && !string.IsNullOrEmpty(file.Path))
                {
                    return new FileInfo(file.Path).Length;
                }

                return 0;
            }
        }

        public async Task<Base1902.IO.File> GetVideoThumbnail()
        {
            if (file == null || (file != null && string.IsNullOrEmpty(file.Path)))
                return null;

            var bitmap = await Android.Media.ThumbnailUtils.CreateVideoThumbnailAsync(this.file.Path, Android.Provider.ThumbnailKind.MicroKind);
            if (bitmap == null)
            {
                bitmap = Bitmap.CreateBitmap(420, 420, Bitmap.Config.Argb8888);
                bitmap.EraseColor(Android.Graphics.Color.Black);
            }

            byte[] bitmapData;
            using (var stream = new System.IO.MemoryStream())
            {
                await bitmap.CompressAsync(Android.Graphics.Bitmap.CompressFormat.Png, 0, stream);
                bitmapData = stream.ToArray();
            }

            var nameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(file.Path);
            var extension = System.IO.Path.GetExtension(file.Path);
            var folder = System.IO.Path.GetDirectoryName(file.Path);

            var myFile = Base1902.IO.File.FromData(nameWithoutExtension, ".png", folder, bitmapData);
            await Resolver.Get<IFileService>().SaveAsync(myFile);
            return myFile;
        }
    }
}