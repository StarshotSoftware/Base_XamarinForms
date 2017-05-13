using AVFoundation;
using CoreGraphics;
using CoreMedia;
using Foundation;
using MediaPlayer;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UIKit;

namespace BaseStarShot.Services.Media
{
    public class MediaFile : IMediaFile
    {
        private readonly Xamarin.Media.MediaFile file;

        public MediaFile(Xamarin.Media.MediaFile file)
        {
            this.file = file;
        }

        public string Path
        {
            get { return this.file.Path; }
        }

        public Stream GetStream()
        {
            return this.file.GetStream();
        }

        public void Dispose()
        {
            this.file.Dispose();
        }

        public Task<Stream> GetStreamAsync()
        {
            return Task.FromResult(GetStream());
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
            UIImage thumbnail = null;

            try
            {

                var url = NSUrl.FromFilename(this.file.Path);
                var asset = AVAsset.FromUrl(url);
                //var size = "";

                var t = TimeSpan.FromSeconds(Convert.ToInt32(asset.Duration.Seconds));
                var Length = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);

                //if (url.IsFileUrl) {
                //    var fs = new FileInfo (url.Path);
                //    var filesize = fs.Length / 1024;
                //    if (filesize < 1024)
                //        size = filesize + "Kb";
                //    else {
                //        var sizeInMB = Convert.ToDouble (filesize / 1024);
                //        size = sizeInMB.ToString ("##.##") + "Mb";
                //    }
                //}

                CMTime time = new CMTime(0, 1);
                CMTime actualTime;
                NSError outError;

                using (var imageGenerator = AVAssetImageGenerator.FromAsset(asset))
                {
                    imageGenerator.AppliesPreferredTrackTransform = true;

                    using (var imageRef = imageGenerator.CopyCGImageAtTime(time, out actualTime, out outError))
                    {
                        thumbnail = UIImage.FromImage(imageRef);

                    }
                }
            }
            catch (Exception assetException)
            {
                thumbnail = null;
            }

            if (thumbnail != null)
            {
                MemoryStream ms = new MemoryStream();
				thumbnail.AsJPEG(0.8f).AsStream().CopyTo(ms);
				var path = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.file.Path), System.IO.Path.GetFileNameWithoutExtension(this.file.Path) + ".jpg");
                return Base1902.IO.File.FromData(path, ms.ToArray());
            }
            return null;
        }

    }
}

