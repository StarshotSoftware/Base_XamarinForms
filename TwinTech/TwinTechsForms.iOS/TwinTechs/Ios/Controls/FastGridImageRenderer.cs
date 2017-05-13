using CoreGraphics;
using System;
using System.ComponentModel;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using FFImageLoading.Work;
using Foundation;
using FFImageLoading.Forms;
using FFImageLoading.Forms.Touch;
using FFImageLoading.Extensions;
using System.Threading.Tasks;
using FFImageLoading.Helpers;
using FFImageLoading.Forms.Args;
using TwinTechs.Controls;
using TwinTechs.Ios.Controls;
using TwinTechsLib.Ios.TwinTechs.Ios.Controls;
using FFImageLoading;
using System.Collections.Generic;
using FFImageLoading.Transformations;

[assembly: ExportRenderer(typeof(FastGridImage), typeof(FastGridImageRenderer))]
namespace TwinTechs.Ios.Controls
{
    /// <summary>
    /// CachedImage Implementation
    /// </summary>
    [Preserve(AllMembers = true)]
    public class FastGridImageRenderer : ViewRenderer<FastGridImage, UIImageView>
    {
        private bool _isDisposed;
        private IScheduledWork _currentTask;

        /// <summary>
        ///   Used for registration with dependency service
        /// </summary>
        public static new void Init()
        {
            // needed because of this STUPID linker issue: https://bugzilla.xamarin.com/show_bug.cgi?id=31076
#pragma warning disable 0219
            var dummy = new FastGridImageRenderer();
#pragma warning restore 0219
        }

        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing && Control != null)
            {
                UIImage image = Control.Image;
                if (image != null)
                {
                    image.Dispose();
                    image = null;
                }
            }

            _isDisposed = true;
            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<FastGridImage> e)
        {
            if (Control == null)
            {
                SetNativeControl(new UIImageView(CGRect.Empty)
                {
                    ContentMode = UIViewContentMode.ScaleAspectFit,
                    ClipsToBounds = true
                });
            }

            if (e.NewElement != null)
            {
                SetAspect();
                SetImage(e.OldElement);
                SetOpacity();

                e.NewElement.InternalReloadImage = new Action(ReloadImage);
                e.NewElement.InternalCancel = new Action(Cancel);
                e.NewElement.InternalGetImageAsJPG = new Func<GetImageAsJpgArgs, Task<byte[]>>(GetImageAsJpgAsync);
                e.NewElement.InternalGetImageAsPNG = new Func<GetImageAsPngArgs, Task<byte[]>>(GetImageAsPngAsync);
            }
            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == FastGridImage.SourceProperty.PropertyName)
            {
                SetImage();
            }
            if (e.PropertyName == FastGridImage.IsOpaqueProperty.PropertyName)
            {
                SetOpacity();
            }
            if (e.PropertyName == FastGridImage.AspectProperty.PropertyName)
            {
                SetAspect();
            }
        }

        private void SetAspect()
        {
            Control.ContentMode = Element.Aspect.ToUIViewContentMode();
        }

        private void SetOpacity()
        {
            Control.Opaque = Element.IsOpaque;
        }

        private void SetImage(FastGridImage oldElement = null)
        {
            var source = Element.Source;

            if (oldElement != null)
            {
                var oldSource = oldElement.Source;
                if (Equals(oldSource, source))
                    return;

                if (oldSource is FileImageSource && source is FileImageSource && ((FileImageSource)oldSource).File == ((FileImageSource)source).File)
                    return;

                Control.Image = null;
            }

            ((IElementController)Element).SetValueFromRenderer(FastGridImage.IsLoadingPropertyKey, true);

            Cancel();
            TaskParameter imageLoader = null;

            var ffSource = ImageSourceBinding.GetImageSourceBinding(source);

            if (ffSource == null)
            {
                if (Control != null)
                {
                    if (Element.ImagePlaceholder != null && Element.ImagePlaceholder is FileImageSource)
                    {
                        var resource = ((FileImageSource)Element.ImagePlaceholder).File;

                        if (!string.IsNullOrWhiteSpace(resource))
                        {
                            ImageService.Instance.LoadCompiledResource(resource).Transform(new List<FFImageLoading.Work.ITransformation>() {
                                        new RoundedTransformation(Element.CornerRadius.PointsToPixels())}).WithCache(FFImageLoading.Cache.CacheType.Disk).Into(Control);
                        }
                    }
                    else
                    {
                        Control.Image = null;
                    }
                }

                ImageLoadingFinished(Element);
            }
            else if (ffSource.ImageSource == FFImageLoading.Work.ImageSource.Url)
            {
                imageLoader = ImageService.Instance.LoadUrl(ffSource.Path, Element.CacheDuration);
            }
            else if (ffSource.ImageSource == FFImageLoading.Work.ImageSource.CompiledResource)
            {
                imageLoader = ImageService.Instance.LoadCompiledResource(ffSource.Path);
            }
            else if (ffSource.ImageSource == FFImageLoading.Work.ImageSource.ApplicationBundle)
            {
                imageLoader = ImageService.Instance.LoadFileFromApplicationBundle(ffSource.Path);
            }
            else if (ffSource.ImageSource == FFImageLoading.Work.ImageSource.Filepath)
            {
                imageLoader = ImageService.Instance.LoadFile(ffSource.Path);
            }
            else if (ffSource.ImageSource == FFImageLoading.Work.ImageSource.Stream)
            {
                imageLoader = ImageService.Instance.LoadStream(ffSource.Stream);
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
                        imageLoader.LoadingPlaceholder(placeholderSource.Path, placeholderSource.ImageSource);
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
                        imageLoader.DownSample(height: Element.Height.PointsToPixels());
                    }
                    else
                    {
                        imageLoader.DownSample(width: Element.Width.PointsToPixels());
                    }
                }
                else if (Element.DownsampleToViewSize && (Element.WidthRequest > 0 || Element.HeightRequest > 0))
                {
                    if (Element.HeightRequest > Element.WidthRequest)
                    {
                        imageLoader.DownSample(height: Element.HeightRequest.PointsToPixels());
                    }
                    else
                    {
                        imageLoader.DownSample(width: Element.WidthRequest.PointsToPixels());
                    }
                }
                else if ((int)Element.DownsampleHeight != 0 || (int)Element.DownsampleWidth != 0)
                {
                    if (Element.DownsampleHeight > Element.DownsampleWidth)
                    {
                        imageLoader.DownSample(height: Element.DownsampleUseDipUnits
                            ? Element.DownsampleHeight.PointsToPixels() : (int)Element.DownsampleHeight);
                    }
                    else
                    {
                        imageLoader.DownSample(width: Element.DownsampleUseDipUnits
                            ? Element.DownsampleWidth.PointsToPixels() : (int)Element.DownsampleWidth);
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

                if (Element.CornerRadius > 0)
                {
                    imageLoader.Transform(new List<FFImageLoading.Work.ITransformation>() {
						                new RoundedTransformation(Element.CornerRadius.PointsToPixels())});
                }

                // Transformations
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

                _currentTask = imageLoader.Into(Control);
            }
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
            SetImage(null);
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
            return GetImageAsByteAsync(false, args.Quality, args.DesiredWidth, args.DesiredHeight);
        }

        private Task<byte[]> GetImageAsPngAsync(GetImageAsPngArgs args)
        {
            return GetImageAsByteAsync(true, 90, args.DesiredWidth, args.DesiredHeight);
        }

        private async Task<byte[]> GetImageAsByteAsync(bool usePNG, int quality, int desiredWidth, int desiredHeight)
        {
            UIImage image = null;

            await MainThreadDispatcher.Instance.PostAsync(() =>
            {
                if (Control != null)
                    image = Control.Image;
            });

            if (image == null)
                return null;

            if (desiredWidth != 0 || desiredHeight != 0)
            {
                image = image.ResizeUIImage((double)desiredWidth, (double)desiredHeight, InterpolationMode.Default);
            }

            NSData imageData = usePNG ? image.AsPNG() : image.AsJPEG((nfloat)quality / 100f);

            if (imageData == null || imageData.Length == 0)
                return null;

            var encoded = imageData.ToArray();
            imageData.Dispose();

            if (desiredWidth != 0 || desiredHeight != 0)
            {
                image.Dispose();
            }

            return encoded;
        }
    }
}

