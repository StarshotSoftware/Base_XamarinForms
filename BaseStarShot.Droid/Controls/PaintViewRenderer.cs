using System;
using Xamarin.Forms;
using BaseStarShot.Controls;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Android.Runtime;
using Android.Content;
using Android.Util;
using Android.Graphics.Drawables;
using System.IO;
using System.Threading.Tasks;

[assembly: ExportRenderer (typeof(BaseStarShot.Controls.PaintView), typeof(PaintViewRenderer))]
namespace BaseStarShot.Controls
{
	public class PaintViewRenderer : ViewRenderer
	{
		public Android.Graphics.Color strokeColor;
		public int strokeWidth;
		public ViewWithTouch v;
		public PaintView baseElement;

		public PaintViewRenderer ()
		{
		}

		protected override void OnElementChanged (ElementChangedEventArgs<View> e)
		{
			base.OnElementChanged (e);
			 baseElement = e.NewElement as PaintView;
			if (baseElement != null) {
				v = new ViewWithTouch(base.Context, (int)Element.Width, (int)Element.Height, baseElement.StrokeColor.ToAndroid(), UIHelper.ConvertDPToPixels(baseElement.StrokeWidth), baseElement);
                baseElement.GetImageTaskFunc = GetBitmapBytes;
                SetNativeControl(v);
			}
			if (Element != null) {
				e.NewElement.PropertyChanged += baseElement_PropertyChanged;
			}
		}

        async Task<byte[]> GetBitmapBytes(ImageFormat format)
        {
            return await v.GetBitmapBytesCanvas(format);
        }


		void baseElement_PropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			switch (e.PropertyName) {
				case "TriggerClearPaint":
					if (baseElement != null) {
						v = new ViewWithTouch (base.Context, (int)Element.Width, (int)Element.Height, 
							baseElement.StrokeColor.ToAndroid (), UIHelper.ConvertDPToPixels (baseElement.StrokeWidth), baseElement);
						baseElement.GetImageTaskFunc = GetBitmapBytes;
						SetNativeControl (v);
					}
					break;
			}
		}
	}


	public class ViewWithTouch : Android.Views.View
	{

		public Canvas mCanvas;
		public Android.Graphics.Path mPath;
		public Paint mPaint;
		public Android.Graphics.Color color;
		private float mX, mY;
		private static float TOUCH_TOLERANCE = 4;
		public BaseStarShot.Controls.PaintView paintView;

		protected ViewWithTouch (IntPtr javaReference, JniHandleOwnership transfer) : base (javaReference, transfer)
		{
		
		}

		public ViewWithTouch (Context context, int width, int height, Android.Graphics.Color color, int strokeWidth, BaseStarShot.Controls.PaintView paintView) : base (context)
		{
			this.paintView = paintView;
			this.paintView.HasPath = false;
			this.color = color;
			mPath = new Android.Graphics.Path  ();
			mPaint = new Paint ();
			mPaint.AntiAlias = true;
			mPaint.Color = color;
			mPaint.SetStyle (Paint.Style.Stroke);
			mPaint.StrokeJoin = Paint.Join.Round;
			mPaint.StrokeCap = Paint.Cap.Round;
			mPaint.StrokeWidth = strokeWidth;
		}

		public ViewWithTouch (Context context, IAttributeSet attrs) : base (context, attrs)
		{
			
		}

		public ViewWithTouch (Context context, IAttributeSet attrs, int defStyleAttr) : base (context, attrs, defStyleAttr)
		{
			
		}

		public ViewWithTouch (Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes) : base (context, attrs, defStyleAttr, defStyleRes)
		{
			
		}

		protected override void OnDraw (Android.Graphics.Canvas canvas)
		{
			base.OnDraw (canvas);
			canvas.DrawPath	(mPath, mPaint);
		}

		private void TouchStart (float x, float y)
		{
			mPath.Reset ();
			mPath.MoveTo (x, y);
			mX = x;
			mY = y;
		}

		// TOUCH MOVE
		private void TouchMove (float x, float y)
		{
			float dx = Math.Abs (x - mX);
			float dy = Math.Abs (y - mY);
			if (dx >= TOUCH_TOLERANCE || dy >= TOUCH_TOLERANCE) {
				mPath.QuadTo (mX, mY, (x + mX) / 2, (y + mY) / 2);
				mPath.LineTo (mX, mY);
				mCanvas.DrawPath (mPath, mPaint);
				mPath.Reset ();
				mPath.MoveTo (mX, mY);
				mX = x;
				mY = y;
			}
		}

		// TOUCH UP
		private void TouchUp ()
		{
			mPath.Reset ();
		}

		public async Task<byte[]> GetBitmapBytesCanvas (ImageFormat format){
			Bitmap returnedBitmap = Bitmap.CreateBitmap(this.Width,this.Height,Bitmap.Config.Argb8888);
			Canvas canvas = new Canvas(returnedBitmap);
			Drawable bgDrawable = this.Background;
			if (bgDrawable!=null) 
				bgDrawable.Draw(canvas);
			else 
				canvas.DrawColor(Android.Graphics.Color.White);
			this.Draw(canvas);
			MemoryStream stream = new MemoryStream();
            if (await returnedBitmap.CompressAsync(format == ImageFormat.Jpeg ? Bitmap.CompressFormat.Jpeg : Bitmap.CompressFormat.Png, 100, stream))
                return stream.ToArray();
            return null;
		}

		public override bool OnTouchEvent (Android.Views.MotionEvent e)
		{
			float eventX = e.GetX ();
			float eventY = e.GetY ();

			switch (e.Action) {
				case Android.Views.MotionEventActions.Down:
					mPath.MoveTo(eventX, eventY);
					this.paintView.HasPath = true;
					break;
				case Android.Views.MotionEventActions.Move:
				case Android.Views.MotionEventActions.Up:
					mPath.LineTo (eventX, eventY);
					this.paintView.HasPath = true;
					break;
			}

			Invalidate ();
			return true;


		}
		public void clear() {
			this.paintView.HasPath = false;
			mPath.Reset();
			OnDraw (mCanvas);
			mCanvas.DrawPath	(mPath, mPaint);
			Invalidate();
		}

	}
}

