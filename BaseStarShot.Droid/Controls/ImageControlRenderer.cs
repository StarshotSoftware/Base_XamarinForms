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
using BaseStarShot.Controls;
using System.Reflection;
using Java.Lang.Ref;
using Org.Apache.Http.Impl.Client;
using Org.Apache.Http.Client.Methods;
using Org.Apache.Http;
using System.Threading.Tasks;
using System.IO;
using Android.Renderscripts;

[assembly: ExportRenderer(typeof(BaseStarShot.Controls.Image), typeof(BaseStarShot.Controls.ImageControlRenderer))]
namespace BaseStarShot.Controls
{
    public class ImageControlRenderer : ViewRenderer<BaseStarShot.Controls.Image, ImageView>
    {
        protected BaseStarShot.Controls.Image BaseControl { get { return ((BaseStarShot.Controls.Image)this.Element); } }

        private ImageDownloader imageDownloader = null;
        private CustomImageView imageView = null;
        private Android.Widget.ProgressBar loaderIndicator = null;

        Page page;
        NavigationPage navigPage;

        static ImageCache imageCache = new ImageCache();
        private bool _isDisposed;

        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                AttachPageEvents(e.NewElement);
            }

            if (Control == null)
            {
                imageDownloader = new AndroidImageDownloader();

                imageView = new CustomImageView(Context);
                imageView.SetElement(Element);
                imageView.SetAdjustViewBounds(true);
                imageView.SetWillNotDraw(false);

				if (BaseControl.EnableTouchViewListener)
					imageView.SetOnTouchListener (new MyTouchListener (Element));
				
                SetNativeControl(imageView);

                //if (Element.ShowActivityIndicatorWhileLoading)
                //{
                //    var parent = Control.Parent as ViewGroup;
                //    if (parent != null)
                //    {

                //    }
                //}

            }

            SetOpaque();

            UpdateBitmap(e.OldElement);

            SetAspect();
            SetIsClickable();

            imageDownloader.Started += imageDownloader_Started;
            imageDownloader.Finished += imageDownloader_Finished;
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Element == null || Control == null)
                return;

            if (e.PropertyName == Image.SourceProperty.PropertyName)
            {
                UpdateBitmap(null);
				Control?.Invalidate ();
                return;
            }
            if (e.PropertyName == Image.AspectProperty.PropertyName)
            {
                SetAspect();
            }
            else if (e.PropertyName == Image.IsLoadingProperty.PropertyName)
            {

            }
            else if (e.PropertyName == Image.CornerRadiusProperty.PropertyName)
            {
                Control.Invalidate();
            }
            else if (e.PropertyName == Image.CommandParameterProperty.PropertyName)
            {
                SetIsClickable();
            }
            else if (e.PropertyName == Image.IsImageLoadingProperty.PropertyName)
            {
                if (BaseControl.ShowActivityIndicatorWhileLoading)
                {
                    if (BaseControl.IsImageLoading)
                        loaderIndicator.Visibility = ViewStates.Visible;
                    else
                        loaderIndicator.Visibility = ViewStates.Gone;
                }
            }

        }

        private async void UpdateBitmap(Image previous = null)
        {
            if (Element == null || Control == null)
                return;

            Bitmap bitmap = null;
            ImageSource source = Element.Source;
            if (previous == null || !object.Equals(previous.Source, Element.Source))
            {
                SetIsLoading(true);
                ((CustomImageView)base.Control).SkipInvalidate();

                // I'm not sure where this comes from.
                Control.SetImageResource(17170445);

                if (BaseControl.ImagePlaceholder != null)
                {
                    var placeholder = BaseControl.ImagePlaceholder as FileImageSource;

                    var resourceID = UIHelper.GetDrawableResource(placeholder);
                    if (resourceID > 0)
                    {
                        Bitmap imageBitmap = imageCache.Get(resourceID.ToString());
                        //imageBitmap = BitmapFactory.DecodeResource(Context.Resources, resourceID);

                        if (imageBitmap == null)
                        {
                            BitmapWorkerTask task = new BitmapWorkerTask(Control, -1, -1, imageCache);
                            Control.SetImageDrawable(new AsyncDrawable(Control.Resources, null, task));
                            task.Execute(resourceID);
                            task.Finished += (s, e) =>
                            {
                                Control.SetImageBitmap(e);
                                e.Dispose();
                            };
                        }
                        else
                        {
                            Control.SetImageBitmap(imageBitmap);
                            imageBitmap.Dispose();
                        }

                    }
                }


                if (source != null)
                {
                    try
                    {
                        bitmap = await GetImageFromImageSource(source, Context);
                    }
                    catch (TaskCanceledException)
                    {
                    }
                    catch (IOException)
                    {
                    }
                    catch (NotImplementedException)
                    {
                    }
                }

                if (Element != null && object.Equals(Element.Source, source))
                {
                    if (!_isDisposed)
                    {
                        if (bitmap == null && BaseControl.ImagePlaceholder != null)
                        {

                        }
                        else
                        {
                            Control.SetImageBitmap(bitmap);
                        }

                        if (bitmap != null)
                        {
                            bitmap.Dispose();
                        }
                        SetIsLoading(false);
                        ((IVisualElementController)base.Element).NativeSizeChanged();
                    }
                }
            }
        }

        private async Task<Bitmap> GetImageFromImageSource(ImageSource imageSource, Context context)
        {
            IImageSourceHandler handler;
            bool isUriImage = false;

            if (imageSource is FileImageSource)
            {
                handler = new FileImageSourceHandler();
            }
            else if (imageSource is StreamImageSource)
            {
                handler = new StreamImagesourceHandler(); // sic
            }
            else if (imageSource is UriImageSource)
            {
                isUriImage = true;
                handler = new ImageLoaderSourceHandler(); // sic
            }
            else
            {
                throw new NotImplementedException();
            }

            Bitmap originalBitmap = null;

            if (isUriImage)
            {

                var uri = Element.Source.GetValue(UriImageSource.UriProperty) as Uri;

                if (imageDownloader.HasLocallyCachedCopy(uri))
                {
                    var bitmap = imageDownloader.GetImage(uri);
                    if (BaseControl.ImagePlaceholder != null && bitmap == null)
                    {

                    }
                    else
                    {
                        originalBitmap = bitmap;
                    }
                }
                else
                {
                    await imageDownloader.GetImageAsync(uri).ContinueWith(t =>
                    {
                        if (t != null)
                        {
                            if (!t.IsFaulted)
                            {
                                //imageDownloader.RemoveDownloadProgress(uri);
                                if (BaseControl == null)
                                    return;

                                if (BaseControl.ImagePlaceholder != null && t.Result == null)
                                    return;

                                originalBitmap = t.Result;
                            }
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            else
            {
                originalBitmap = await handler.LoadImageAsync(imageSource, context);
            }



            return originalBitmap;
        }

        void imageDownloader_Finished(object sender, Bitmap e)
        {
            if (Element == null)
                return;

            if (BaseControl.ShowActivityIndicatorWhileLoading && loaderIndicator != null)
                	loaderIndicator.Visibility = Android.Views.ViewStates.Gone;
        }

        void imageDownloader_Started(object sender, EventArgs e)
        {
            if (Element == null)
                return;

			if (BaseControl.ShowActivityIndicatorWhileLoading && loaderIndicator != null)
                loaderIndicator.Visibility = Android.Views.ViewStates.Visible;
        }

        void SetImageLoader()
        {
            if (BaseControl.ShowActivityIndicatorWhileLoading)
            {
                if (BaseControl.LoaderType == BaseStarShot.ImageLoaderType.ProgressRing)
                {
                    loaderIndicator.Indeterminate = true;


                    //<!==================== uncomment this if custom image is used for indicator
                    //loaderIndicator.IndeterminateDrawable = Resources.GetDrawable(Resource.Drawable.loading);

                    //var rotateAnimation = new RotateAnimation(0f, 360f, Dimension.RelativeToSelf, 0.5f, Dimension.RelativeToSelf, 0.5f);
                    //rotateAnimation.Interpolator = new LinearInterpolator();
                    //rotateAnimation.RepeatCount = RotateAnimation.Infinite;
                    //rotateAnimation.Duration = 1000;
                    //loaderIndicator.Animation = rotateAnimation;
                    //loaderIndicator.StartAnimation(rotateAnimation);
                    //====================>
                }
                else
                {
                    loaderIndicator.Indeterminate = false;
                }
            }
        }

        void SetOpaque()
        {
            //var value = Element.IsOpaque ? 1 : 0;
            //Control.ImageAlpha = value;
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

        void SetIsClickable()
        {
            if (Control.HasOnClickListeners)
                Control.SetOnClickListener(null);

            if (BaseControl.Command != null)
            {
                Control.Clickable = true;

                Control.Click += Control_Click;
            }
            else
            {
                Control.Clickable = false;

                Control.Click -= Control_Click;
            }
        }

        void Control_Click(object sender, EventArgs e)
        {
            if (BaseControl.Command != null && BaseControl.Command.CanExecute(BaseControl.CommandParameter))
                BaseControl.Command.Execute(BaseControl.CommandParameter);
        }

        protected override void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;
            BitmapDrawable bitmapDrawable;
            if (disposing && Control != null && (bitmapDrawable = (Control.Drawable as BitmapDrawable)) != null)
            {
                Bitmap bitmap = bitmapDrawable.Bitmap;
                if (bitmap != null)
                {
                    //bitmap.Recycle();
                    bitmap.Dispose();
                }
            }

            imageDownloader.Started -= imageDownloader_Started;
            imageDownloader.Finished -= imageDownloader_Finished;
            base.Dispose(disposing);
        }


        #region attachpageevents
        private void AttachPageEvents(Xamarin.Forms.Element element)
        {
            var viewCell = UIHelper.GetContainingViewCell(element);
            if (viewCell != null)
            {
                viewCell.PropertyChanged += (s, ev) =>
                {
                    var propertyName = ev.PropertyName;

                };

                var listView = UIHelper.GetContainingListView(element);
                if (listView != null)
                {
                    listView.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "Renderer")
                        {
                            if (listView.Parent == null)
                            {
                                this.Dispose(true);
                            }
                        }
                    };
                }

                page = UIHelper.GetContainingPage(element);

                if (page != null)
                {
                    page.Disappearing += (s, ev) =>
                    {
                        //if (imageLoader != null)
                        {
                            //imageLoader.CancelDisplayTask(imageView);
                        }
                    };
                }

                if (page == null)
                {
                    var root = UIHelper.GetRootElement(element);
                    root.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "Parent")
                        {
                            AttachPageEvents(root);
                        }
                    };
                    return;
                }
                // As of Xamarin.Forms 1.4+, image will be reused when moving from tabs.
                // Uncomment this if using Xamarin.Forms < 1.4.
                //if (page.Parent is TabbedPage)
                //{
                //    page.Disappearing += PageContainedInTabbedPageDisapearing;
                //    return;
                //}

                navigPage = UIHelper.GetContainingNavigationPage(page);
                if (navigPage != null)
                {
                    navigPage.Popped += OnPagePopped;

                    // As of Xamarin.Forms 1.4+, image will be reused when moving from tabs.
                    // Uncomment this if using Xamarin.Forms < 1.4.
                    //if (navigPage.Parent is TabbedPage)
                    //{
                    //    navigPage.Disappearing += PageContainedInTabbedPageDisapearing;
                    //}
                }
            }
            // As of Xamarin.Forms 1.4+, image will be reused when moving from tabs.
            // Uncomment this if using Xamarin.Forms < 1.4.
            //else if ((page = GetContainingTabbedPage(element)) != null)
            //{
            //    page.Disappearing += PageContainedInTabbedPageDisapearing;
            //}
        }

        void PageContainedInTabbedPageDisapearing(object sender, EventArgs e)
        {
            this.Dispose(true);
            page.Disappearing -= PageContainedInTabbedPageDisapearing;
        }

        private void OnPagePopped(object s, NavigationEventArgs e)
        {
            if (e.Page == page)
            {
                this.Dispose(true);
                navigPage.Popped -= OnPagePopped;
            }
        }
        #endregion

        private class CustomImageView : ImageView
        {
            private bool _skipInvalidate;
            private BaseStarShot.Controls.Image Element;

            public CustomImageView(Context context)
                : base(context)
            {
            }

            public void SetElement(BaseStarShot.Controls.Image Element)
            {
                this.Element = Element;
            }

            protected override void OnDraw(Canvas canvas)
            {
                var cornerRadius = UIHelper.ConvertDPToPixels(Element.CornerRadius);
                if (cornerRadius > 0)
                {
                    Android.Graphics.Path clipPath = new Android.Graphics.Path();
                    RectF rect = new RectF(0, 0, UIHelper.ConvertDPToPixels(Element.WidthRequest), UIHelper.ConvertDPToPixels(Element.HeightRequest));
                    float[] radii = new float[] { cornerRadius, cornerRadius, cornerRadius, cornerRadius, cornerRadius, cornerRadius, cornerRadius, cornerRadius };
                    clipPath.AddRoundRect(rect, radii, Android.Graphics.Path.Direction.Cw);
                    canvas.ClipPath(clipPath);
                }
                base.OnDraw(canvas);
            }

            public override void Invalidate()
            {
                if (this._skipInvalidate)
                {
                    this._skipInvalidate = false;
                    return;
                }
                base.Invalidate();
            }

            public void SkipInvalidate()
            {
                this._skipInvalidate = true;
            }
        }

        private static FieldInfo _isLoadingPropertyKeyFieldInfo;

        private static FieldInfo IsLoadingPropertyKeyFieldInfo
        {
            get
            {
                if (_isLoadingPropertyKeyFieldInfo == null)
                {
                    _isLoadingPropertyKeyFieldInfo = typeof(Xamarin.Forms.Image).GetField("IsLoadingPropertyKey", BindingFlags.Static | BindingFlags.NonPublic);
                }
                return _isLoadingPropertyKeyFieldInfo;
            }
        }

        private void SetIsLoading(bool value)
        {
            var fieldInfo = IsLoadingPropertyKeyFieldInfo;
            ((IElementController)base.Element).SetValueFromRenderer((BindablePropertyKey)fieldInfo.GetValue(null), value);
        }
    }
	public class MyTouchListener
	: Java.Lang.Object
	, Android.Views.View.IOnTouchListener
	{
		BaseStarShot.Controls.Image element;

		public MyTouchListener (BaseStarShot.Controls.Image element)
		{
			this.element = element;
		}

		public bool OnTouch (Android.Views.View v, MotionEvent e)
		{
			switch (e.Action) {
			case MotionEventActions.Up:
				this.element.OnTouchEvent (new BaseStarShot.Controls.TouchEventArgs ((double)e.GetX (), (double)e.GetY (), TouchState.ENDED));
				break;
			case MotionEventActions.Down:
				this.element.OnTouchEvent (new BaseStarShot.Controls.TouchEventArgs ((double)e.GetX (), (double)e.GetY (), TouchState.BEGAN));
				break;
			case MotionEventActions.Move:
				this.element.OnTouchEvent (new BaseStarShot.Controls.TouchEventArgs ((double)e.GetX (), (double)e.GetY (), TouchState.MOVED));
				break;
			}
			return true;
		}
	}
}
