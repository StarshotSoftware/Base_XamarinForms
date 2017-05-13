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
using Xamarin.Forms.Platform.Android;
using Android.Util;
using Android.Graphics.Drawables;
using Android.Graphics;
using System.Reflection;
using Java.Lang.Ref;
using Org.Apache.Http.Impl.Client;
using Org.Apache.Http.Client.Methods;
using Org.Apache.Http;
using System.Threading.Tasks;
using System.IO;
using Android.Renderscripts;
using TwinTechs.Controls;
using TwinTechs.Droid.Controls;
using FFImageLoading.Views;
using FFImageLoading;
using FFImageLoading.Transformations;
using FFImageLoading.Work;
using FFImageLoading.Forms.Args;
using FFImageLoading.Forms;
using TwinTechsLib.Droid.TwinTechs.Droid.Controls;
//using MonoDroidToolkit.ImageLoader;

[assembly: ExportRenderer(typeof(FastGridImage), typeof(FastGridImageRenderer))]
namespace TwinTechs.Droid.Controls
{
    [Preserve(AllMembers = true)]
    public class FastGridImageRenderer : ViewRenderer<FastGridImage, ImageViewAsync>
    {

        private bool _isDisposed;

        public static void Init()
        {
        }

        private IScheduledWork _currentTask;

        public FastGridImageRenderer()
        {
            AutoPackage = false;
        }

        public FastGridImageRenderer(IntPtr javaReference, JniHandleOwnership transfer)
            : this()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<FastGridImage> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
            {
                var imageView = new CustomImageView(Context);

                imageView.SetAdjustViewBounds(true);
                imageView.SetWillNotDraw(false);
                SetNativeControl(imageView);

            }

            if (e.NewElement != null)
            {
                e.NewElement.InternalReloadImage = new Action(ReloadImage);
                e.NewElement.InternalCancel = new Action(Cancel);
                e.NewElement.InternalGetImageAsJPG = new Func<GetImageAsJpgArgs, Task<byte[]>>(GetImageAsJpgAsync);
                e.NewElement.InternalGetImageAsPNG = new Func<GetImageAsPngArgs, Task<byte[]>>(GetImageAsPngAsync);
            }

            UpdateBitmap(e.OldElement);
            SetAspect();
        }

        private void ImageLoadingFinished(FastGridImage element)
        {
            if (element != null && !_isDisposed)
            {
                ((IElementController)element).SetValueFromRenderer(FastGridImage.IsLoadingPropertyKey, false);
                ((IVisualElementController)element).NativeSizeChanged();
                element.InvalidateViewMeasure();
            }
        }

        private void ReloadImage()
        {
            UpdateBitmap(null);
        }

        private void Cancel()
        {
            if (_currentTask != null && !_currentTask.IsCancelled)
            {
                _currentTask.Cancel();
            }
        }

        private Task<byte[]> GetImageAsJpgAsync(GetImageAsJpgArgs args)
        {
            return GetImageAsByteAsync(Bitmap.CompressFormat.Jpeg, args.Quality, args.DesiredWidth, args.DesiredHeight);
        }

        private Task<byte[]> GetImageAsPngAsync(GetImageAsPngArgs args)
        {
            return GetImageAsByteAsync(Bitmap.CompressFormat.Png, 90, args.DesiredWidth, args.DesiredHeight);
        }

        private async Task<byte[]> GetImageAsByteAsync(Bitmap.CompressFormat format, int quality, int desiredWidth, int desiredHeight)
        {
            if (Control == null)
                return null;

            var drawable = Control.Drawable as BitmapDrawable;

            if (drawable == null || drawable.Bitmap == null)
                return null;

            Bitmap bitmap = drawable.Bitmap;

            if (desiredWidth != 0 || desiredHeight != 0)
            {
                double widthRatio = (double)desiredWidth / (double)bitmap.Width;
                double heightRatio = (double)desiredHeight / (double)bitmap.Height;

                double scaleRatio = Math.Min(widthRatio, heightRatio);

                if (desiredWidth == 0)
                    scaleRatio = heightRatio;

                if (desiredHeight == 0)
                    scaleRatio = widthRatio;

                int aspectWidth = (int)((double)bitmap.Width * scaleRatio);
                int aspectHeight = (int)((double)bitmap.Height * scaleRatio);

                bitmap = Bitmap.CreateScaledBitmap(bitmap, aspectWidth, aspectHeight, true);
            }

            using (var stream = new MemoryStream())
            {
                await bitmap.CompressAsync(format, quality, stream).ConfigureAwait(false);
                var compressed = stream.ToArray();

                if (desiredWidth != 0 || desiredHeight != 0)
                {
                    bitmap.Recycle();
                    bitmap.Dispose();
                }

                return compressed;
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Element == null || Control == null)
                return;

            //if (e.PropertyName == FastImage.ImagePlaceholderProperty.PropertyName)
            //{
            //    //var source = Element.ImagePlaceholder;
            //    //var placeholder = "";
            //    //if (source != null && source is FileImageSource)
            //    //    placeholder = ((FileImageSource)source).File;

            //    //if (!string.IsNullOrWhiteSpace(placeholder))
            //    //{
            //    //    ImageService.Instance.LoadCompiledResource(placeholder).WithCache(FFImageLoading.Cache.CacheType.Disk).Into(Control);
            //    //    Control.SetTag(Control.Id, Guid.NewGuid().ToString());
            //    //}
            //}

            if (e.PropertyName == FastGridImage.SourceProperty.PropertyName)
            {
                UpdateBitmap(null);
                return;
            }

            if (e.PropertyName == FastGridImage.AspectProperty.PropertyName)
            {
                SetAspect();
            }
            else if (e.PropertyName == FastGridImage.IsLoadingProperty.PropertyName)
            {

            }
        }

        private void UpdateBitmap(FastGridImage previous = null)
        {
            if (previous != null && Equals(previous.Source, Element.Source))
                return;

            var source = Element.Source;
            var imageView = Control as CustomImageView;

            ((IElementController)Element).SetValueFromRenderer(FastGridImage.IsLoadingPropertyKey, true);

            Control.SetImageResource(global::Android.Resource.Color.Transparent);

            if (Element != null && object.Equals(Element.Source, source) && !_isDisposed)
            {
                Cancel();
                TaskParameter imageLoader = null;

                var ffSource = ImageSourceBinding.GetImageSourceBinding(source);

                if (ffSource == null)
                {
                    if (imageView != null)
                    {
                        if (Element.ImagePlaceholder != null && Element.ImagePlaceholder is FileImageSource)
                        {
                            var resource = ((FileImageSource)Element.ImagePlaceholder).File;

                            if (!string.IsNullOrWhiteSpace(resource))
                            {
                                ImageService.Instance.LoadCompiledResource(resource).Transform(new List<FFImageLoading.Work.ITransformation>() {
                                        new RoundedTransformation(Element.CornerRadius.DpToPixels())}).WithCache(FFImageLoading.Cache.CacheType.Disk).Into(imageView);

                            }
                        }
                        else
                        {
                            imageView.SetImageDrawable(null);
                        }

                        //imageView.SetImageDrawable(null);
                        ImageLoadingFinished(Element);
                    }
                }
                else if (ffSource.ImageSource == FFImageLoading.Work.ImageSource.Url)
                {
                    imageLoader = ImageService.Instance.LoadUrl(ffSource.Path, Element.CacheDuration)
                        .WithCache(FFImageLoading.Cache.CacheType.Disk);
                    imageLoader.Preload = true;
                }
                else if (ffSource.ImageSource == FFImageLoading.Work.ImageSource.CompiledResource)
                {
                    imageLoader = ImageService.Instance.LoadCompiledResource(ffSource.Path).WithCache(FFImageLoading.Cache.CacheType.Disk);
                }
                else if (ffSource.ImageSource == FFImageLoading.Work.ImageSource.ApplicationBundle)
                {
                    imageLoader = ImageService.Instance.LoadFileFromApplicationBundle(ffSource.Path).WithCache(FFImageLoading.Cache.CacheType.Disk);
                }
                else if (ffSource.ImageSource == FFImageLoading.Work.ImageSource.Filepath)
                {
                    imageLoader = ImageService.Instance.LoadFile(ffSource.Path).WithCache(FFImageLoading.Cache.CacheType.Disk);
                }
                else if (ffSource.ImageSource == FFImageLoading.Work.ImageSource.Stream)
                {
                    imageLoader = ImageService.Instance.LoadStream(ffSource.Stream).WithCache(FFImageLoading.Cache.CacheType.Disk);
                }

                if (imageLoader != null)
                {
                    ImageService.Instance.Config.TransformPlaceholders = true;
                    imageLoader.TransformPlaceholders(true);
                    // CustomKeyFactory
                    if (Element.CacheKeyFactory != null)
                    {
                        var bindingContext = Element.BindingContext;
                        imageLoader.CacheKey(Element.CacheKeyFactory.GetKey(source, bindingContext));
                    }

                    // LoadingPlaceholder
                    if (Element.ImagePlaceholder != null)
                    {
                        var placeholderSource = ImageSourceBinding.GetImageSourceBinding(Element.ImagePlaceholder);
                        if (placeholderSource != null)
                        {
                            imageLoader.LoadingPlaceholder(placeholderSource.Path, placeholderSource.ImageSource);

                            if (Element.ErrorPlaceholder == null)
                                imageLoader.ErrorPlaceholder(placeholderSource.Path, placeholderSource.ImageSource);
                        }
                    }

                    // ErrorPlaceholder
                    if (Element.ErrorPlaceholder != null)
                    {
                        var placeholderSource = ImageSourceBinding.GetImageSourceBinding(Element.ErrorPlaceholder);
                        if (placeholderSource != null)
                            imageLoader.ErrorPlaceholder(placeholderSource.Path, placeholderSource.ImageSource);
                    }

                    // Downsample
                    if (Element.DownsampleToViewSize && (Element.Width > 0 || Element.Height > 0))
                    {
                        if (Element.Height > Element.Width)
                        {
                            imageLoader.DownSample(height: Element.Height.DpToPixels());
                        }
                        else
                        {
                            imageLoader.DownSample(width: Element.Width.DpToPixels());
                        }
                    }
                    else if (Element.DownsampleToViewSize && (Element.WidthRequest > 0 || Element.HeightRequest > 0))
                    {
                        if (Element.HeightRequest > Element.WidthRequest)
                        {
                            imageLoader.DownSample(height: Element.HeightRequest.DpToPixels());
                        }
                        else
                        {
                            imageLoader.DownSample(width: Element.WidthRequest.DpToPixels());
                        }
                    }
                    else if ((int)Element.DownsampleHeight != 0 || (int)Element.DownsampleWidth != 0)
                    {
                        if (Element.DownsampleHeight > Element.DownsampleWidth)
                        {
                            imageLoader.DownSample(height: Element.DownsampleUseDipUnits
                                ? Element.DownsampleHeight.DpToPixels() : (int)Element.DownsampleHeight);
                        }
                        else
                        {
                            imageLoader.DownSample(width: Element.DownsampleUseDipUnits
                                ? Element.DownsampleWidth.DpToPixels() : (int)Element.DownsampleWidth);
                        }
                    }

                    // RetryCount
                    if (Element.RetryCount > 0)
                    {
                        imageLoader.Retry(Element.RetryCount, Element.RetryDelay);
                    }

                    // TransparencyChannel
                    if (Element.TransparencyEnabled.HasValue)
                        imageLoader.TransparencyChannel(Element.TransparencyEnabled.Value);

                    // FadeAnimation
                    if (Element.FadeAnimationEnabled.HasValue)
                        imageLoader.FadeAnimation(Element.FadeAnimationEnabled.Value);

                    // TransformPlaceholders
                    if (Element.TransformPlaceholders.HasValue)
                        imageLoader.TransformPlaceholders(Element.TransformPlaceholders.Value);

                    // Transformations
                    if (Element.CornerRadius > 0)
                    {
                        imageLoader.Transform(new List<FFImageLoading.Work.ITransformation>() {
						new RoundedTransformation(Element.CornerRadius.DpToPixels())});
                    }

                    //if (Element.Transformations != null && Element.Transformations.Count > 0)
                    //{
                    //    imageLoader.Transform(Element.Transformations);
                    //}

                    imageLoader.WithPriority(Element.LoadingPriority);
                    if (Element.CacheType.HasValue)
                    {
                        imageLoader.WithCache(Element.CacheType.Value);
                    }

                    if (Element.LoadingDelay.HasValue)
                    {
                        imageLoader.Delay(Element.LoadingDelay.Value);
                    }

                    var element = Element;

                    imageLoader.Finish((work) =>
                    {
                        element.OnFinish(new CachedImageEvents.FinishEventArgs(work));
                        ImageLoadingFinished(element);
                    });

                    imageLoader.Success((imageInformation, loadingResult) =>
                        element.OnSuccess(new CachedImageEvents.SuccessEventArgs(imageInformation, loadingResult)));

                    imageLoader.Error((exception) =>
                        element.OnError(new CachedImageEvents.ErrorEventArgs(exception)));

                    _currentTask = imageLoader.Into(imageView);
                }
            }
        }

        void SetAspect()
        {
            switch (Element.Aspect)
            {
                case Aspect.AspectFill: Control.SetScaleType(ImageView.ScaleType.CenterCrop); break;
                case Aspect.AspectFit: Control.SetScaleType(ImageView.ScaleType.FitCenter); break;
                case Aspect.Fill: Control.SetScaleType(ImageView.ScaleType.FitXy); break;
                default: Control.SetScaleType(ImageView.ScaleType.CenterInside); break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                _isDisposed = true;
                base.Dispose(disposing);
            }
        }

    }

    [Preserve(AllMembers = true)]
    public class CustomImageView : ImageViewAsync
    {
        bool _skipInvalidate;

        public CustomImageView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
            SetWillNotDraw(false);
        }

        public CustomImageView(Context context)
            : base(context)
        {
            SetWillNotDraw(false);
        }

        public CustomImageView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            SetWillNotDraw(false);
        }

        public override void Invalidate()
        {
            if (_skipInvalidate)
            {
                _skipInvalidate = false;
                return;
            }

            base.Invalidate();
        }

        public void SkipInvalidate()
        {
            _skipInvalidate = true;
        }
    }
}
