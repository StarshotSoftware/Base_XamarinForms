using System;
using Android.Content;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics.Drawables;
using Android.Graphics;
using System.Reflection;
using TwinTechsLib.Droid.TwinTechs.Droid.Helper;
using System.Threading.Tasks;
using TwinTechs.Controls;
using TwinTechs.Droid.Controls;
using MonoDroidToolkit.ImageLoader;
using Base1902.Controls;

[assembly: ExportRenderer(typeof(FastImage), typeof(FastImageRenderer))]
namespace TwinTechs.Droid.Controls
{
    public class FastImageRenderer : ViewRenderer<FastImage, ImageView>
    {
        protected FastImage BaseControl { get { return ((FastImage)this.Element); } }
        ImageLoader _imageLoader;
        private CustomImageView imageView = null;

        Page page;
        NavigationPage navigPage;

        private bool _isDisposed;

        protected override void OnElementChanged(ElementChangedEventArgs<FastImage> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                AttachPageEvents(e.NewElement);
            }

            if (Control == null)
            {
                _imageLoader = ImageLoaderCache.GetImageLoader(this);

                imageView = new CustomImageView(Context);
                imageView.CropToPadding = true;

                imageView.SetElement(Element);
                imageView.SetAdjustViewBounds(true);
                imageView.SetWillNotDraw(false);

                SetNativeControl(imageView);
            }

            SetImageUrl(Element.ImageUrl);
            SetAspect();
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Element == null || Control == null)
                return;

            if (e.PropertyName == FastImage.ImageUrlProperty.PropertyName)
            {
                SetImageUrl(Element.ImageUrl);
                return;
            }

            if (e.PropertyName == FastImage.AspectProperty.PropertyName)
            {
                SetAspect();
            }
            else if (e.PropertyName == FastImage.IsLoadingProperty.PropertyName)
            {

            }
            else if (e.PropertyName == FastImage.CornerRadiusProperty.PropertyName)
            {
                try
                {
                    Control.Invalidate();
                }
                catch (Exception)
                {

                }
            }

        }

        public void SetImageUrl(string imageUrl)
        {
            if (Control == null)
            {
                return;
            }

            int resourceID = -1;
            if (BaseControl.ImagePlaceholder != null && BaseControl.ImagePlaceholder is FileImageSource)
            {
                var placeholder = BaseControl.ImagePlaceholder as FileImageSource;
                if (!string.IsNullOrWhiteSpace(placeholder.File))
                    resourceID = placeholder.GetDrawableResource();
            }

            ((CustomImageView)base.Control).SkipInvalidate();

            if (IsValidUrl(imageUrl) == null)
            {
                Control.SetImageResource(resourceID);
                return;
            }

            if (!IsValidUrl(imageUrl).Value)
            {
                Control.SetImageDrawable(this.Resources.GetDrawable(imageUrl));
                return;
            }

            if (imageUrl != null)
            {
                _imageLoader.DisplayImage(imageUrl, Control, resourceID);//, BaseControl.BorderColor, BaseControl.CornerRadius.ConvertDPToPixels());
            }
            //else
            //{
            //    if (resourceID < 0)
            //        Control.SetImageDrawable(null);
            //}

            //Control.Invalidate();
        }

        bool? IsValidUrl(string urlString)
        {
            if (string.IsNullOrWhiteSpace(urlString))
                return null;

            Uri uriResult;
            bool result = Uri.TryCreate(urlString, UriKind.RelativeOrAbsolute, out uriResult);
            //&& (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (uriResult == null)
                return null;

            if (uriResult.IsAbsoluteUri)
                return true;

            return false;
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
                //this.Dispose(true);
                navigPage.Popped -= OnPagePopped;
            }
        }
        #endregion

        private class CustomImageView : ImageView
        {
            private bool _skipInvalidate;
            private FastImage Element;

            public CustomImageView(Context context)
                : base(context)
            {

            }

            public void SetElement(FastImage Element)
            {
                this.Element = Element;
            }

            protected override void OnDraw(Canvas canvas)
            {
                var cornerRadius = Element.CornerRadius;
                Android.Graphics.Path clipPath = new Android.Graphics.Path();
                var width = Element.WidthRequest.ConvertDPToPixels();
                var height = Element.HeightRequest.ConvertDPToPixels();
                if (cornerRadius == 80123)
                {
                    RectF rect = new RectF(0, 0, width, height);
                    float[] radii = new float[] { 40, 40, 40, 40, 0, 0, 0, 0 };
                    clipPath.AddRoundRect(rect, radii, Android.Graphics.Path.Direction.Cw);
                    canvas.ClipPath(clipPath);
                }
                else if (cornerRadius > 0)
                {
                    var cornerRadiusFloat = Element.CornerRadius.ConvertDPToPixels();

                    if (Element.BorderColor != Xamarin.Forms.Color.Default)
                    {
                        RectF rect = new RectF(0, 0, width, height);
                        var cornerRadius1 = cornerRadiusFloat;
                        float[] radii = new float[] { cornerRadius1, cornerRadius1, cornerRadius1, cornerRadius1, cornerRadius1, cornerRadius1, cornerRadius1, cornerRadius1 };
                        clipPath.AddRoundRect(rect, radii, Android.Graphics.Path.Direction.Cw);

                        //var paint = new Paint(PaintFlags.AntiAlias);
                        //paint.Color = Element.BorderColor.ToAndroid();
                        //paint.StrokeWidth = 1f;
                        //paint.SetStyle(Paint.Style.Stroke);


                        //// Important for certain APIs 
                        ////this.SetLayerType(LayerType.Software, paint);
                        ////paint.SetShadowLayer(2, 0, 0, Element.BorderColor.ToAndroid());


                        //Android.Graphics.Path borderPath = new Android.Graphics.Path();
                        //RectF borderRect = new RectF(0, 0, width, height);
                        //var cornerRadius0 = cornerRadius;
                        //float[] borderRadii = new float[] { cornerRadius0, cornerRadius0, cornerRadius0, cornerRadius0, cornerRadius0, cornerRadius0, cornerRadius0, cornerRadius0 };
                        //borderPath.AddRoundRect(borderRect, borderRadii, Android.Graphics.Path.Direction.Cw);

                        //canvas.DrawPath(borderPath, paint);
                        canvas.ClipPath(clipPath);
                    }
                    else
                    {
                        RectF rect = new RectF(0, 0, width, height);
                        float[] radii = new float[] { cornerRadiusFloat, cornerRadiusFloat, cornerRadiusFloat, cornerRadiusFloat, cornerRadiusFloat, cornerRadiusFloat, cornerRadiusFloat, cornerRadiusFloat };
                        clipPath.AddRoundRect(rect, radii, Android.Graphics.Path.Direction.Cw);

                        canvas.ClipPath(clipPath);
                    }
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
}
