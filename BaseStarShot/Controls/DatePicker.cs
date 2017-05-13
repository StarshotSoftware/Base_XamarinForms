using BaseStarShot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class DatePicker : Xamarin.Forms.DatePicker
    {

        public DatePicker()
        {

        }

        public DateTime? DateTag
        {
            get { return (DateTime?)GetValue(DateTagProperty); }
            set { SetValue(DateTagProperty, value); }
        }

        public int TriggerShowDatePicker
        {
            get { return (int)GetValue(TriggerShowDatePickerProperty); }
            set { SetValue(TriggerShowDatePickerProperty, value); }
        }

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

        public static BindableProperty FontStyleProperty =
            BindableProperty.Create<DatePicker, FontStyle>(l => l.FontStyle, FontStyle.Light);

        public static BindableProperty FontSizeProperty =
            BindableProperty.Create<DatePicker, double>(l => l.FontSize, Device.GetNamedSize(NamedSize.Default, typeof(Xamarin.Forms.DatePicker)));

        public static BindableProperty XAlignProperty =
            BindableProperty.Create<DatePicker, TextAlignment>(l => l.XAlign, TextAlignment.Right);

        public static BindableProperty YAlignProperty =
            BindableProperty.Create<DatePicker, TextAlignment>(l => l.YAlign, TextAlignment.Center);

        public static BindableProperty TriggerShowDatePickerProperty =
            BindableProperty.Create<DatePicker, int>(l => l.TriggerShowDatePicker, 0);

        public static BindableProperty DateTagProperty =
            BindableProperty.Create<DatePicker, DateTime?>(p => p.DateTag, null);

        public static BindableProperty BackgroundImageProperty =
            BindableProperty.Create<DatePicker, FileImageSource>(p => p.BackgroundImage, null);

        public FileImageSource BackgroundImage
        {
            get { return (FileImageSource)GetValue(BackgroundImageProperty); }
            set { SetValue(BackgroundImageProperty, value); }
        }

        //public static BindableProperty BgColorProperty =
        //    BindableProperty.Create<DatePicker, Color>(p => p.BackgroundColor, Color.Transparent);

        //public Color BackgroundColor
        //{
        //    get { return (Color)GetValue(BgColorProperty); }
        //    set { SetValue(BgColorProperty, value); }
        //}
    }
}
