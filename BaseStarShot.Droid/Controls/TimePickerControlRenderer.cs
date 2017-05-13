using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BaseStarShot.Controls;
using BaseStarShot.Services;
using BaseStarShot.Util;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Base1902;

[assembly: ExportRenderer(typeof(BaseStarShot.Controls.TimePicker), typeof(TimePickerControlRenderer))]
namespace BaseStarShot.Controls
{
    public class TimePickerControlRenderer : TimePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.TimePicker> e)
        {
            base.OnElementChanged(e);
            var baseElement = e.NewElement as BaseStarShot.Controls.TimePicker;

            if (baseElement != null)
            {
                baseElement.PropertyChanged += (s, ev) =>
                {
                    var element = (BaseStarShot.Controls.TimePicker)s;
                    switch (ev.PropertyName)
                    {
                        case "TextColor": base.Control.SetTextColor(baseElement.TextColor.ToAndroid()); break;
                        case "FontSize": base.Control.TextSize = (float)baseElement.FontSize; break;
                        case "BackgroundImage": SetBackgroundImage(baseElement); break;
                    }
                };
                var fontName = Resolver.Get<IFontService>().GetFontName(baseElement.FontStyle);
                if (!string.IsNullOrEmpty(fontName))
                    base.Control.Typeface = FontCache.GetTypeFace(fontName);

                base.Control.SetTextColor(baseElement.TextColor.ToAndroid());
                base.Control.TextSize = (float)baseElement.FontSize;
                SetBackgroundImage(baseElement);
                SetTextAlignment(baseElement);
                SetBgColor(baseElement);

                Control.LongClickable = false;
                Control.SetPadding(0, 0, 0, 0);
            }
        }

        protected virtual void SetBackgroundImage(BaseStarShot.Controls.TimePicker element)
        {
            Control.SetBackgroundColor(element.BackgroundColor.ToAndroid());
        }

        protected virtual void SetTextAlignment(BaseStarShot.Controls.TimePicker element)
        {
            switch (element.XAlign)
            {
                case BaseStarShot.TextAlignment.Justified:
                case BaseStarShot.TextAlignment.Left:
                    Control.Gravity = GravityFlags.AxisPullBefore | GravityFlags.AxisSpecified | GravityFlags.CenterVertical | GravityFlags.RelativeLayoutDirection; break;
                case BaseStarShot.TextAlignment.Center:
                    Control.Gravity = GravityFlags.Center; break;
                case BaseStarShot.TextAlignment.Right:
                    Control.Gravity = GravityFlags.CenterVertical | GravityFlags.Right; break;
            }
        }

        protected virtual void SetBgColor(BaseStarShot.Controls.TimePicker element)
        {
            Control.SetBackgroundColor(element.BackgroundColor.ToAndroid());
        }
    }
}

