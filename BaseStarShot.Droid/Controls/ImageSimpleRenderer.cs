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

//[assembly: ExportRenderer(typeof(BaseStarShot.Controls.Image), typeof(BaseStarShot.Controls.ImageSimpleRenderer))]
namespace BaseStarShot.Controls
{
    public class ImageSimpleRenderer : Xamarin.Forms.Platform.Android.ViewRenderer<BaseStarShot.Controls.Image, CustomImageView>
    {
        private ImageDownloader imageDownloader = null;
        private CustomImageView imageView = null;

        Page page;
        NavigationPage navigPage;
        private bool isImageInitialSet;

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<BaseStarShot.Controls.Image> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                AttachPageEvents(e.NewElement);
            }

            //if (e.OldElement != null || Element == null)
            //    return;

            if (Control == null)
            {
                imageDownloader = new AndroidImageDownloader();
                imageView = new CustomImageView(Context);
                imageView.SetElement(Element);
                imageView.SetAdjustViewBounds(true);

                SetNativeControl(imageView);

                int resourceID = -1;
                if (Element.ImagePlaceholder != null && Element.ImagePlaceholder is FileImageSource)
                {
                    var placeholder = Element.ImagePlaceholder as FileImageSource;
                    resourceID = UIHelper.GetDrawableResource(placeholder);
                }

                if (resourceID < 0)
                    resourceID = Android.Resource.Drawable.GalleryThumb;

            }

            if (e.NewElement != null)
            {
                SetAspect();
                SetOpaque();
                SetImagePlaceholder();
                SetImageSource();
                SetIsClickable();

                Element.PropertyChanged += Base_PropertyChanged;
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
                case Aspect.AspectFill: imageView.SetScaleType(ImageView.ScaleType.CenterCrop); break;
                case Aspect.AspectFit: imageView.SetScaleType(ImageView.ScaleType.FitCenter); break;
                case Aspect.Fill: imageView.SetScaleType(ImageView.ScaleType.FitXy); break;
                default: imageView.SetScaleType(ImageView.ScaleType.CenterInside); break;
            }
        }

        async void SetImageSource()
        {
            if (Element.Source != null)
            {
                if (Element.Source is UriImageSource)
                {
                    isImageInitialSet = true;


                    var uri = Element.Source.GetValue(UriImageSource.UriProperty) as Uri;

                    var bitmap = await imageDownloader.GetImageAsync(uri);
                    SetImageCorners(bitmap);
                }
                else if (Element.Source is FileImageSource)
                {
                    if ((Element.Width > 0 && Element.Height > 0) || (Element.WidthRequest > 0 && Element.HeightRequest > 0))
                    {
                        isImageInitialSet = true;

                        var source = Element.Source as FileImageSource;
                        if (source != null && source.File != null)
                        {
                            var resourceID = UIHelper.GetDrawableResource(source);
                            if (resourceID > 0)
                            {
                                Bitmap imageBitmap = ImageCache.Instance.Get(resourceID.ToString());
                                if (imageBitmap == null)
                                {
                                    var width = UIHelper.ConvertDPToPixels(Element.Width <= 0 ? Element.WidthRequest : Element.Width);
                                    var height = UIHelper.ConvertDPToPixels(Element.Height <= 0 ? Element.HeightRequest : Element.Height);
                                    BitmapWorkerTask task = new BitmapWorkerTask(imageView, width, height, ImageCache.Instance);
                                    imageView.SetImageDrawable(new AsyncDrawable(imageView.Resources, null, task));
                                    task.Execute(resourceID);
                                    task.Finished += (s, e) =>
                                        SetImageCorners(e);
                                }
                                else
                                    SetImageCorners(imageBitmap);
                            }
                            else
                            {
                                var imageBitmap = await BitmapFactory.DecodeFileAsync(source.File);
                                SetImageCorners(imageBitmap);
                            }
                        }
                    }
                }
                else if (Element.Source is StreamImageSource)
                {
                    isImageInitialSet = true;
                    var source = Element.Source as StreamImageSource;
                    var cts = new System.Threading.CancellationTokenSource();
                    var str = await source.Stream(cts.Token);
                    using (var reader = new System.IO.BinaryReader(str))
                    {
                        var data = reader.ReadBytes((int)str.Length);
                        var bitmap = await BitmapFactory.DecodeByteArrayAsync(data, 0, data.Length);
                        SetImageCorners(bitmap);
                    }
                }
            }
            else
            {
                if (Element.ImagePlaceholder == null)
                    imageView.SetImageBitmap(null);
            }
        }

        void SetImagePlaceholder()
        {
            if (Element.ImagePlaceholder != null)
            {
                var placeholder = Element.ImagePlaceholder as FileImageSource;
                if (placeholder != null && placeholder.File != null)
                {
                    var resourceID = UIHelper.GetDrawableResource(placeholder);
                    //Bitmap imageBitmap = BitmapFactory.DecodeResource(Context.Resources, resourceID);

                    //SetImageCorners(imageBitmap);

                    if (resourceID > 0)
                    {
                        Bitmap imageBitmap = ImageCache.Instance.Get(resourceID.ToString());
                        if (imageBitmap == null)
                        {
                            BitmapWorkerTask task = new BitmapWorkerTask(imageView, -1, -1, ImageCache.Instance);
                            imageView.SetImageDrawable(new AsyncDrawable(imageView.Resources, null, task));
                            task.Execute(resourceID);
                            task.Finished += (s, e) =>
                                SetImageCorners(e);
                        }
                        else
                            SetImageCorners(imageBitmap);
                    }
                }
            }
        }

        void SetImageCorners(Bitmap imageBitmap = null)
        {
            if (imageBitmap == null && imageView.Drawable == null)
                return;

            if (imageBitmap != null && imageView.Drawable != null)
            {
                imageView.Drawable.Dispose();
                //(((BitmapDrawable)imageView.Drawable).Bitmap).Dispose();
            }

            if (imageBitmap == null)
                imageBitmap = ((BitmapDrawable)imageView.Drawable).Bitmap;

            imageView.SetImageBitmap(imageBitmap);
            if (Element != null && Element.CornerRadius > 0)
            {
                imageView.Invalidate();
            }
        }

        void SetIsClickable()
        {
            if (Control.HasOnClickListeners)
                Control.SetOnClickListener(null);

            if (Element.Command != null)
            {
                Control.Clickable = true;

                Control.Click -= Control_Click;
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
            if (Element.Command != null && Element.Command.CanExecute(Element.CommandParameter))
                Element.Command.Execute(Element.CommandParameter);
        }


        void Base_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Element == null || Control == null)
                return;

            switch (e.PropertyName)
            {
                case "Width":
                case "Height":
                    if (!isImageInitialSet)
                        SetImageSource(); break;
                //case "Renderer": SetImageSource(); break;
                case "Aspect": SetAspect(); break;
                case "IsLoading": break;
                case "IsOpaque": SetOpaque(); break;
                case "Source": SetImageSource(); break;
                case "CornerRadius": SetImageCorners(); break;
                case "ImagePlaceholder": SetImagePlaceholder(); break;
                case "Command":
                case "CommandParameter":
                    SetIsClickable(); break;
                case "Margin": break;
                case "Text": break;
                case "NativeSource": break;
                case "TextColor": break;
                case "FontSize": break;
                case "LoadingProgress": break;
                case "LoaderType": break;
                case "UpdateImageTrigger": break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (imageView != null)
                {
                    if (imageView.Drawable != null && ((BitmapDrawable)imageView.Drawable).Bitmap != null)
                    {
                        (((BitmapDrawable)imageView.Drawable).Bitmap).Dispose();
                    }
                }
            }

            base.Dispose(disposing);
        }

        private void AttachPageEvents(Element element)
        {
            var viewCell = UIHelper.GetContainingViewCell(element);
            if (viewCell != null)
            {
                viewCell.PropertyChanged += (s, ev) =>
                {
                    var propertyName = ev.PropertyName;

                    var test = viewCell.BindingContext;
                    var test1 = Element.BindingContext;
                    var test2 = Control;

                    if (ev.PropertyName == "Renderer")
                    {
                        if (test == null || test1 == null)
                            return;

                        SetImageSource();
                    }
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

                navigPage = UIHelper.GetContainingNavigationPage(page);
                if (navigPage != null)
                {
                    navigPage.Popped += OnPagePopped;
                }
            }
        }

        private void OnPagePopped(object s, NavigationEventArgs e)
        {
            if (e.Page == page)
            {
                this.Dispose(true);
                navigPage.Popped -= OnPagePopped;
            }
        }
    }

    public class CustomImageView : ImageView
    {
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
                Path clipPath = new Path();
                RectF rect = new RectF(0, 0, UIHelper.ConvertDPToPixels(Element.WidthRequest), UIHelper.ConvertDPToPixels(Element.HeightRequest));
                float[] radii = new float[] { cornerRadius, cornerRadius, cornerRadius, cornerRadius, cornerRadius, cornerRadius, cornerRadius, cornerRadius };
                clipPath.AddRoundRect(rect, radii, Path.Direction.Cw);
                canvas.ClipPath(clipPath);
            }
            base.OnDraw(canvas);
        }

        //protected override void OnVisibilityChanged(Android.Views.View changedView, ViewStates visibility)
        //{
        //    base.OnVisibilityChanged(changedView, visibility);

        //    if (visibility != ViewStates.Visible)
        //        this.SetImageBitmap(null);
        //}

    }
}