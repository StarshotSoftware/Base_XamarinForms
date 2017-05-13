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
using Xamarin.Forms;
using BaseStarShot.Controls;
using Xamarin.Forms.Platform.Android;
using BaseStarShot.Helpers;
using Android.Views.InputMethods;
using Android.Support.V4.Widget;
using Android.Graphics;
using BaseStarShot.Services;
using Android.Graphics.Drawables;
using Java.Net;
using System.IO;
using Java.IO;
using System.Net;
using System.Threading.Tasks;
using Android.Util;

using Path = Android.Graphics.Path;
using Console = System.Console;
using BaseStarShot.Util;

[assembly: ExportRenderer(typeof(BaseStarShot.Controls.ImageCell), typeof(BaseStarShot.Controls.ImageCellRenderer))]
namespace BaseStarShot.Controls
{
    public class ImageCellRenderer : Xamarin.Forms.Platform.Android.ImageCellRenderer
    {
        LinearLayout cell;
        ImageView imageView;
        TextView label, detailLabel;

        protected BaseStarShot.Controls.ImageCell Base { get { return (BaseStarShot.Controls.ImageCell)base.Cell; } }

        public ImageCellRenderer()
        {

        }

        private void ReshapeImage(ImageView image, Bitmap sourceBitmap)
        {
            if (sourceBitmap != null)
            {
                var sourceRect = this.GetScaledRect(sourceBitmap.Height, sourceBitmap.Width);
                var rect = this.GetTargetRect(sourceBitmap.Height, sourceBitmap.Width);

                using (var output = Bitmap.CreateBitmap(rect.Width(), rect.Height(), Bitmap.Config.Argb8888))
                {
                    var canvas = new Canvas(output);
                    var paint = new Paint();
                    var rectF = new RectF(rect);
                    var roundRx = rect.Width() / 2;
                    var roundRy = rect.Height() / 2;
                    paint.AntiAlias = true;
                    canvas.DrawARGB(0, 0, 0, 0);
                    paint.Color = Android.Graphics.Color.ParseColor("#ff424242");
                    canvas.DrawRoundRect(rectF, roundRx, roundRy, paint);
                    paint.SetXfermode(new PorterDuffXfermode(PorterDuff.Mode.SrcIn));
                    canvas.DrawBitmap(sourceBitmap, sourceRect, rect, paint);

                    image.SetImageBitmap(output);
                };
            }
        }

        private Android.Graphics.Rect GetScaledRect(int sourceHeight, int sourceWidth)
        {
            int height = 0;
            int width = 0;
            int top = 0;
            int left = 0;

            height = sourceHeight;
            width = sourceWidth;
            height = this.MakeSquare(height, ref width);
            left = (int)((sourceWidth - width) / 2);
            top = (int)((sourceHeight - height) / 2);

            var rect = new Android.Graphics.Rect(left, top, width + left, height + top);

            return rect;
        }

        private Android.Graphics.Rect GetTargetRect(int sourceHeight, int sourceWidth)
        {
            int height = 0;
            int width = 0;

            //height = this.Element.HeightRequest > 0
            //           ? (int)System.Math.Round(this.Element.HeightRequest, 0)
            //           : sourceHeight;
            //width = this.Element.WidthRequest > 0
            //           ? (int)System.Math.Round(this.Element.WidthRequest, 0)
            //           : sourceWidth;


            height = sourceHeight; //UIHelper.ConvertDPToPixels(new PointSize(40));
            width = sourceWidth; //UIHelper.ConvertDPToPixels(new PointSize(40));

            // Make Square
            height = MakeSquare(height, ref width);

            return new Android.Graphics.Rect(0, 0, width, height);
        }

        private int MakeSquare(int height, ref int width)
        {
            if (height < width)
            {
                width = height;
            }
            else
            {
                height = width;
            }
            return height;
        }

        protected override void OnCellPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            base.OnCellPropertyChanged(sender, args);

            switch (args.PropertyName)
            {
                case "Padding":
                    SetPadding();
                    break;
                case "ImageSource":
                    SetImage();
                    break;
                case "ImageWidth":
                case "ImageHeight":
                    SetImageSize();
                    break;
                case "TextFontFamily":
                    SetTextFont();
                    break;
                case "TextFontSize":
                    SetTextSize();
                    break;
                case "DetailFontFamily":
                    SetDetailFont();
                    break;
                case "DetailFontSize":
                    SetDetailSize();
                    break;
            }
        }

        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, ViewGroup parent, Context context)
        {
            cell = (LinearLayout)base.GetCellCore(item, convertView, parent, context);

            imageView = (ImageView)cell.GetChildAt(0);
            var linear = (LinearLayout)cell.GetChildAt(1);
            label = (TextView)linear.GetChildAt(0);
            detailLabel = (TextView)linear.GetChildAt(1);
            SetImage();

            if (convertView == null)
            {
                SetPadding();

                SetImageSize();

                linear.SetGravity(Android.Views.GravityFlags.CenterVertical);

                label.SetLines(1);
                label.Ellipsize = Android.Text.TextUtils.TruncateAt.End;
                label.Gravity = (Android.Views.GravityFlags.CenterVertical);
                label.SetSingleLine(true);
                label.SetMaxLines(1);
                SetTextFont();
                SetTextSize();

                detailLabel.Gravity = (Android.Views.GravityFlags.CenterVertical);
                detailLabel.SetSingleLine(false);
                SetDetailFont();
                SetDetailSize();
            }

            return cell;
        }

        void SetPadding()
        {
            cell.SetPadding(UIHelper.ConvertDPToPixels(Base.Padding.Left),
                UIHelper.ConvertDPToPixels(Base.Padding.Top),
                UIHelper.ConvertDPToPixels(Base.Padding.Right),
                UIHelper.ConvertDPToPixels(Base.Padding.Bottom));
        }

        async void SetImage()
        {
            bool hasSetImage = false;
            //if (Base.ImagePlaceholder != null)
            //    imageView.SetImageResource(UIHelper.GetDrawableResource(Base.ImagePlaceholder));
            ImageSource imageSource = Base.ImageSource;

            if (imageSource != null && imageSource is FileImageSource)
            {
                imageView.SetImageResource(UIHelper.GetDrawableResource(imageSource as FileImageSource));
                return;
            }

            if (imageSource != null)
            {
                var thisImageId = Guid.NewGuid();
                imageView.Tag = new ImageGetId { Id = thisImageId };
                Bitmap image;
                try
                {
                    BaseStarShot.Logger.DebugWrite("ImageCellRenderer", "Loading: " + ((UriImageSource)imageSource).Uri);
                    image = await imageSource.GetImage();
                }
                catch (TaskCanceledException)
                {
                    image = null;
                }
                catch (Exception ex)
                {
                    BaseStarShot.Logger.WriteError("ImageCellRenderer", ex);
                    image = null;
                }

                var imageGetId = (ImageGetId)imageView.Tag;
                if (thisImageId != imageGetId.Id)
                    return;

                if (image != null)
                {
                    BaseStarShot.Logger.DebugWrite("ImageCellRenderer", "Loaded: " + ((UriImageSource)imageSource).Uri);
                    imageView.SetImageBitmap(image);
                    hasSetImage = true;
                }
                else
                {
                    BaseStarShot.Logger.DebugWrite("ImageCellRenderer", "Null: " + ((UriImageSource)imageSource).Uri);
                }
            }
            else
            {
                BaseStarShot.Logger.DebugWrite("ImageCellRenderer", "No image source: " + ((UriImageSource)imageSource).Uri);
            }

            //if (!hasSetImage && Base.ImagePlaceholder != null)
            //{
            //    imageView.SetImageResource(UIHelper.GetDrawableResource(Base.ImagePlaceholder));
            //}
        }

        void SetImageSize()
        {
            int width = UIHelper.ConvertDPToPixels(Base.ImageWidth),
                height = UIHelper.ConvertDPToPixels(Base.ImageHeight);
            imageView.LayoutParameters.Width = width;
            imageView.LayoutParameters.Height = height;
        }

        void SetTextFont()
        {
            if (label != null && !string.IsNullOrEmpty(Base.TextFontFamily))
                label.Typeface = FontCache.GetTypeFace(Base.TextFontFamily);
        }

        void SetTextSize()
        {
            if (label != null)
                label.TextSize = (int)Base.TextFontSize;
        }

        void SetDetailFont()
        {
            if (detailLabel != null && !string.IsNullOrEmpty(Base.DetailFontFamily))
                detailLabel.Typeface = FontCache.GetTypeFace(Base.DetailFontFamily);
        }

        void SetDetailSize()
        {
            if (detailLabel != null)
                detailLabel.TextSize = (int)Base.DetailFontSize;
        }
    }



    class ImageGetId : Java.Lang.Object
    {
        public Guid Id { get; set; }
    }
}