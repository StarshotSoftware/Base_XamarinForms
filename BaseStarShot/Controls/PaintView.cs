using System;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace  BaseStarShot.Controls
{
	public class PaintView : View
	{
		public double StrokeWidth {
			get { return (double)GetValue (StrokeWidthProperty); } 
			set { SetValue (StrokeWidthProperty, value); }
		}

		public static readonly BindableProperty StrokeWidthProperty = BindableProperty.Create<PaintView, double> (p => p.StrokeWidth, 1);

		public Color StrokeColor {
			get { return (Color)GetValue (StrokeColorProperty); } 
			set { SetValue (StrokeColorProperty, value); }
		}

		public static readonly BindableProperty StrokeColorProperty = BindableProperty.Create<PaintView, Color> (p => p.StrokeColor, Color.Black);

		public int TriggerClearPaint {
			get { return (int)GetValue (TriggerClearPaintProperty); } 
			set { SetValue (TriggerClearPaintProperty, value); }
		}

		public static readonly BindableProperty TriggerClearPaintProperty = BindableProperty.Create<PaintView, int> (p => p.TriggerClearPaint, 0);

		internal Func<ImageFormat, Task<byte[]>> GetImageTaskFunc { get; set; }

		public async Task<byte[]> GetImageAsync (ImageFormat format)
		{
			return await GetImageTaskFunc (format);
		}

		public bool HasPath {get; set;}

		public PaintView ()
		{

		}
	}
}

