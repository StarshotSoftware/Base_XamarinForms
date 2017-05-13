using System;
using UIKit;
using Xamarin.Forms;
using BaseStarShot.Controls;
using Xamarin.Forms.Platform.iOS;
using Foundation;
using CoreGraphics;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

[assembly: ExportRenderer (typeof(BaseStarShot.Controls.PaintView), typeof(PaintViewRenderer))]
namespace BaseStarShot.Controls
{
	public class PaintViewRenderer : ViewRenderer<View, UIView>
	{
		public CGRect rect;
		public FrameWithTouch frame;
		public UIColor strokeColor;
		public int strokeWidth;

		public PaintViewRenderer ()
		{
			rect = new CGRect ();
			strokeColor = UIColor.Black;
			strokeWidth = 1;
		}

		protected override void OnElementChanged (ElementChangedEventArgs<View> e)
		{
			base.OnElementChanged (e);

			var baseElement = e.NewElement as BaseStarShot.Controls.PaintView;
			if (baseElement != null) {
				if (baseElement != null) {
					e.NewElement.PropertyChanged += baseElement_PropertyChanged;
				}
				frame = new FrameWithTouch (rect, (float)baseElement.StrokeWidth, baseElement.StrokeColor.ToUIColor (), baseElement);
				baseElement.GetImageTaskFunc = GetBitmapBytes;
				rect.Height = (float)Element.Height;
				rect.Width = (float)Element.Width;
				if (Control == null) {
					SetNativeControl (frame);
				}
			}
		}

		async Task<byte[]> GetBitmapBytes (ImageFormat format)
		{
			return await frame.GetImageBytesFromView (format);
		}


		void baseElement_PropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			var element = (BaseStarShot.Controls.PaintView)sender;
			switch (e.PropertyName) {
				case "TriggerClearPaint":
					frame.clear ();
					break;
			}
		}

	}

	[Register ("DrawView")]
	public class FrameWithTouch : UIView
	{
		List<Line> lines;
		Line currentLine;
		UIColor color;
		float lineWidth;
		BaseStarShot.Controls.PaintView paintView;

		public FrameWithTouch (CGRect frame, float lineWidth, UIColor color,BaseStarShot.Controls.PaintView paintView)
		{
			this.paintView = paintView;
			this.paintView.HasPath = false;
			this.Frame = frame;
			this.BackgroundColor = UIColor.Clear;
			this.lines = new List<Line> ();
			this.color = color;
			this.lineWidth = lineWidth;
			this.currentLine = new Line (lineWidth, color, 1.0f);
		}

		public override void TouchesMoved (NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);
			UITouch uTouch = touches.AnyObject as UITouch;
			CGPoint currentPoint = uTouch.LocationInView (this);
			this.currentLine.linePath.AddLineToPoint (currentPoint.X, currentPoint.Y);
			this.SetNeedsDisplay ();
		}

		public override void TouchesBegan (NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);
			UITouch uTouch = touches.AnyObject as UITouch;
			CGPoint cPoint = uTouch.LocationInView (this);
			this.currentLine.linePath.MoveToPoint (cPoint.X, cPoint.Y);
			//this.SetNeedsDisplay ();
		}

		public override void TouchesEnded (NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);
			UITouch uTouch = touches.AnyObject as UITouch;
			CGPoint cPoint = uTouch.LocationInView (this);
			this.currentLine.linePath.AddLineToPoint (cPoint.X, cPoint.Y);
			this.SetNeedsDisplay ();
			lines.Add (this.currentLine);
			Line nextLine = new Line (this.currentLine.lineWidth, this.currentLine.lineColor, this.currentLine.opacity);
			this.currentLine = nextLine;
		}

		public void clear ()
		{
			this.paintView.HasPath = false;
			this.currentLine = new Line (lineWidth, color, 1.0f);
			this.lines.Clear ();
			this.SetNeedsDisplay ();
		}

		public async Task<byte[]> GetImageBytesFromView (ImageFormat format)
		{
			UIGraphics.BeginImageContext (this.Frame.Size);
			this.Layer.RenderInContext (UIGraphics.GetCurrentContext ());
			UIImage image = UIGraphics.GetImageFromCurrentImageContext ();
			UIGraphics.EndImageContext ();
			var s = format == ImageFormat.Jpeg ? image.AsJPEG ().AsStream () : image.AsPNG ().AsStream ();
			return ReadFully (s);
		}

		public byte[] ReadFully (Stream input)
		{
			byte[] buffer = new byte[16 * 1024];
			using (MemoryStream ms = new MemoryStream ()) {
				int read;
				while ((read = input.Read (buffer, 0, buffer.Length)) > 0) {
					ms.Write (buffer, 0, read);
				}
				return ms.ToArray ();
			}
		}

		public override void Draw (CGRect rect)
		{
			CGContext context = UIGraphics.GetCurrentContext ();
			UIGraphics.BeginImageContext (this.Frame.Size);

			if (this.lines.Count > 0) {
				paintView.HasPath = true;
				foreach (Line tempLine in lines) {
					context.SetAlpha (tempLine.opacity);
					context.SetStrokeColor (tempLine.lineColor.CGColor);
					context.SetLineWidth (tempLine.lineWidth);
					context.SetLineCap (CGLineCap.Round);
					context.SetLineJoin (CGLineJoin.Round);
					context.AddPath (tempLine.linePath);
					context.StrokePath ();
				}
			}

			context.SetAlpha (this.currentLine.opacity);
			context.SetStrokeColor (this.currentLine.lineColor.CGColor);
			context.SetLineWidth (this.currentLine.lineWidth);
			context.SetLineCap (CGLineCap.Round);
			context.SetLineJoin (CGLineJoin.Round);
			context.BeginPath ();
			context.AddPath (this.currentLine.linePath);
			context.StrokePath ();
			base.Draw (rect);
			UIGraphics.EndImageContext ();
		}
	}

	public class Line : NSObject
	{

		public float lineWidth;
		public UIColor lineColor;
		public float opacity;
		public CGPath linePath;

		public Line (float lineWidth, UIColor lineColor, float opacity)
		{
			this.lineWidth = lineWidth;
			this.lineColor = lineColor;
			this.opacity = opacity;
			this.linePath = new CGPath ();
		}


	}


}

