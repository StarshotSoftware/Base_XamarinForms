using System;
using Xamarin.Forms;
using System.Threading;
using System.Windows.Input;

namespace BaseStarShot.Controls
{
    public class Image : Xamarin.Forms.Image
    {
		/// <summary>
		/// The checked changed event.
		/// </summary>
		public event EventHandler<TouchEventArgs> TouchViewEvent = delegate { };

        public event EventHandler ImageLoaded;

        public static readonly BindableProperty CommandProperty =
            BindableProperty.Create<Image, ICommand>(p => p.Command, null);

        // 20160513 ffg - Remove IsClickbale use Command != null instead to check if image should intercept tap gestures.
        //public static readonly BindableProperty IsClickableProperty =
        //    BindableProperty.Create<Image, bool>(p => p.IsClickable, false);

        public static readonly BindableProperty MarginProperty =
            BindableProperty.Create<Image, Thickness>(p => p.Margin, new PointThickness(0));

        public static readonly BindableProperty CommandParameterProperty =
            BindableProperty.Create<Image, object>(p => p.CommandParameter, null);

        public static readonly BindableProperty TagProperty =
            BindableProperty.Create<Image, object>(p => p.Tag, null);

		public static readonly BindableProperty CornerRadiusProperty =
            BindableProperty.Create<Image, double>(p => p.CornerRadius, 0);

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create<Image, Color>(p => p.TextColor, Color.Default);

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create<Image, PointFontSize>(p => p.FontSize, new PointFontSize(5));

        public static readonly BindableProperty ImagePlaceholderProperty =
            BindableProperty.Create<Image, ImageSource>(p => p.ImagePlaceholder, null);

        public static readonly BindableProperty LoadingProgressProperty =
            BindableProperty.Create<Image, double>(p => p.LoadingProgress, 0);

        public static readonly BindableProperty LoaderTypeProperty =
            BindableProperty.Create<Image, ImageLoaderType>(p => p.LoaderType, ImageLoaderType.None, defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty ShowActivityIndicatorWhileLoadingroperty =
            BindableProperty.Create<Image, bool>(p => p.ShowActivityIndicatorWhileLoading, false);

        public static readonly BindableProperty IsImageLoadingProperty =
            BindableProperty.Create<Image, bool>(p => p.IsImageLoading, false);

		public static readonly BindableProperty EnableTouchViewListenerProperty =
			BindableProperty.Create<Image, bool> (p => p.EnableTouchViewListener, false);

		public double CornerRadius
		{
            get { return (double)base.GetValue(CornerRadiusProperty); }
			set { base.SetValue(CornerRadiusProperty, value); }
		}

        public ImageSource ImagePlaceholder
        {
            get { return (ImageSource)base.GetValue(ImagePlaceholderProperty); }
            set { base.SetValue(ImagePlaceholderProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)base.GetValue(CommandProperty); }
            set { base.SetValue(CommandProperty, value); }
        }

        public Thickness Margin
        {
            get { return (Thickness)base.GetValue(MarginProperty); }
            set { base.SetValue(MarginProperty, value); }
        }

        //public bool IsClickable
        //{
        //    get { return (bool)base.GetValue(IsClickableProperty); }
        //    set { base.SetValue(IsClickableProperty, value); }
        //}

        public object CommandParameter
        {
            get { return (object)base.GetValue(CommandParameterProperty); }
            set { base.SetValue(CommandParameterProperty, value); }
        }

        public object Tag
        {
            get { return (object)base.GetValue(TagProperty); }
            set { base.SetValue(TagProperty, value); }
        }

        public Color TextColor
        {
            get { return (Color)base.GetValue(TextColorProperty); }
            set { base.SetValue(TextColorProperty, value); }
        }

        public PointFontSize FontSize
        {
            get { return (PointFontSize)base.GetValue(FontSizeProperty); }
            set { base.SetValue(FontSizeProperty, value); }
        }

        public double LoadingProgress
        {
            get { return (double)base.GetValue(LoadingProgressProperty); }
            set { base.SetValue(LoadingProgressProperty, value); }
        }

        public ImageLoaderType LoaderType
        {
            get { return (ImageLoaderType)base.GetValue(LoaderTypeProperty); }
            set { base.SetValue(LoaderTypeProperty, value); }
        }

        public static readonly BindableProperty UpdateImageTriggerProperty =
            BindableProperty.Create<Image, int>(p => p.UpdateImageTrigger, 0);

        public int UpdateImageTrigger
        {
            get { return (int)base.GetValue(UpdateImageTriggerProperty); }
            set { base.SetValue(UpdateImageTriggerProperty, value); }
        }

        public bool ShowActivityIndicatorWhileLoading
        {
            get { return (bool)base.GetValue(ShowActivityIndicatorWhileLoadingroperty); }
            set { base.SetValue(ShowActivityIndicatorWhileLoadingroperty, value); }
        }

		public bool EnableTouchViewListener {
			get { return (bool)base.GetValue (EnableTouchViewListenerProperty); }
			set { base.SetValue (EnableTouchViewListenerProperty, value); }
		}

        public bool IsImageLoading
        {
            get { return (bool)base.GetValue(IsImageLoadingProperty); }
            set { base.SetValue(IsImageLoadingProperty, value); }
        }

		public void OnTouchEvent (TouchEventArgs args)
		{
			if (TouchViewEvent != null)
				TouchViewEvent.Invoke (this, args);

		}

        //		private CancellationTokenSource _imageLoadingCts;
        //		public CancellationToken ImageLoadingCancellationToken 
        //		{ 
        //			get 
        //			{ 
        //				if (_imageLoadingCts != null)
        //					return _imageLoadingCts.Token;
        //
        //				return CancellationToken.None;
        //			}
        //		}
        //
        //		public void StartImageLoading()
        //		{
        //			_imageLoadingCts = new CancellationTokenSource();
        //		}
        //
        //		public void CancelImageLoading()
        //		{
        //			if (_imageLoadingCts != null)
        //			{
        //				_imageLoadingCts.Cancel();
        //				_imageLoadingCts = null;
        //			}
        //		}
    }

	public class TouchEventArgs : EventArgs
	{
		public double x;
		public double y;
		public TouchState state;

		public TouchEventArgs (double x, double y, TouchState touchState) {
			this.x = x;
			this.y = y;
			this.state = touchState;
		}

	}

	public enum TouchState { 
		BEGAN, MOVED, ENDED
	}
}

