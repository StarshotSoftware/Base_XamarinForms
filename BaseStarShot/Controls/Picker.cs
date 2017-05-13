using BaseStarShot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class Picker : Xamarin.Forms.Picker
    {
        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create<Picker, Color>(b => b.TextColor, Color.Default);

        public static readonly BindableProperty DefaultTextColorProperty =
            BindableProperty.Create<Picker, Color>(b => b.DefaultTextColor, Color.Default);

        public static readonly BindableProperty BorderColorProperty =
            BindableProperty.Create<Picker, Color>(b => b.BorderColor, Color.Default);

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public Color DefaultTextColor
        {
            get { return (Color)GetValue(DefaultTextColorProperty); }
            set { SetValue(DefaultTextColorProperty, value); }
        }

        public Color BorderColor
        {
            get { return (Color)GetValue(BorderColorProperty); }
            set { SetValue(BorderColorProperty, value); }
        }
    }
}
