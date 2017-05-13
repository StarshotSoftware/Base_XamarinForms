using System;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
	public class TimePicker : Xamarin.Forms.TimePicker
	{
		public TextAlignment XAlign
		{
			get { return (TextAlignment)GetValue(XAlignProperty); }
			set { SetValue(XAlignProperty, value); }
		}

		public TextAlignment YAlign
		{
			get { return (TextAlignment)GetValue(YAlignProperty); }
			set { SetValue(YAlignProperty, value); }
		}

		public double FontSize
		{
			get { return (double)GetValue(FontSizeProperty); }
			set { SetValue(FontSizeProperty, value); }
		}

		public FontStyle FontStyle
		{
			get { return (FontStyle)GetValue(FontStyleProperty); }
			set { SetValue(FontStyleProperty, value); }
		}

		public Color TextColor
		{
			get { return (Color)GetValue(TextColorProperty); }
			set { SetValue(TextColorProperty, value); }
		}

		public static BindableProperty FontStyleProperty =
			BindableProperty.Create<DatePicker, FontStyle>(l => l.FontStyle, FontStyle.Light);

		public static BindableProperty TextColorProperty =
			BindableProperty.Create<DatePicker, Color>(p => p.TextColor, Xamarin.Forms.Color.White);

		public static BindableProperty FontSizeProperty =
			BindableProperty.Create<DatePicker, double>(l => l.FontSize, Device.GetNamedSize(NamedSize.Default, typeof(Xamarin.Forms.DatePicker)));

		public static BindableProperty XAlignProperty =
			BindableProperty.Create<DatePicker, TextAlignment>(l => l.XAlign, TextAlignment.Right);

		public static BindableProperty YAlignProperty =
			BindableProperty.Create<DatePicker, TextAlignment>(l => l.YAlign, TextAlignment.Center);
	}
}

