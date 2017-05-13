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
using BaseStarShot.Controls;
using Android;
using Android.Graphics.Drawables;
using System.Threading.Tasks;
using Android.Graphics;
using BaseStarShot.Services;
using BaseStarShot.Util;
using Base1902;

[assembly: ExportRenderer(typeof(BaseStarShot.Controls.DatePicker), typeof(BaseStarShot.Controls.DatePickerRenderer))]
namespace BaseStarShot.Controls
{
    public class DatePickerRenderer : Xamarin.Forms.Platform.Android.DatePickerRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.DatePicker> e)
        {
            base.OnElementChanged(e);

            var baseElement = e.NewElement as DatePicker;
            base.Control.SetTextColor(baseElement.TextColor.ToAndroid());
            Control.LongClickable = false;

            if (baseElement.BackgroundImage != null && baseElement.BackgroundImage.File != null)
                base.Control.SetBackgroundResource(UIHelper.GetDrawableResource(baseElement.BackgroundImage));

            var fontName = Resolver.Get<IFontService>().GetFontName(baseElement.FontStyle);
            if (!string.IsNullOrEmpty(fontName))
                SetTypeface(fontName);
            if (baseElement != null)
            {
                SetTextAlignment(baseElement);
                SetBgColor(baseElement);
                SetTextSize(baseElement);
				Control.Text = baseElement.Date.ToString(baseElement.Format);
				Control.TextLocale = Java.Util.Locale.Default;

                baseElement.PropertyChanged += baseElement_PropertyChanged;
            }


        }

        void SetTypeface(string fontName)
        {
            if (!string.IsNullOrEmpty(fontName))
                Control.Typeface = FontCache.GetTypeFace(fontName);
        }


        void baseElement_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Control == null)
                return;

            var baseElement = sender as BaseStarShot.Controls.DatePicker;
            switch (e.PropertyName)
            {
				case "Date":
					Control.Text = baseElement.Date.ToString (baseElement.Format);
				break;
                case "TriggerShowDatePicker":
                    Control.CallOnClick();
                    //Control.Raise("Click", EventArgs.Empty);
                    break;

                case "TextColor":
                    //Control.Raise("Click", EventArgs.Empty);

                    if(Control != null)
                    Control.SetTextColor(baseElement.TextColor.ToAndroid());
                    break;
            }
        }

        void SetTextAlignment(BaseStarShot.Controls.DatePicker element)
        {
            if (Control == null)
                return;

            GravityFlags flags = GravityFlags.NoGravity;

            switch (element.XAlign)
            {
                case BaseStarShot.TextAlignment.Center:
                    flags = GravityFlags.CenterHorizontal;
                    break;
                case BaseStarShot.TextAlignment.Left:
                    flags = GravityFlags.Left;
                    break;
                case BaseStarShot.TextAlignment.Right:
                    flags = GravityFlags.Right;
                    break;
            }

            switch (element.YAlign)
            {
                case BaseStarShot.TextAlignment.Center:
                    flags |= GravityFlags.CenterVertical;
                    break;
                case BaseStarShot.TextAlignment.Top:
                    flags |= GravityFlags.Top;
                    break;
                case BaseStarShot.TextAlignment.Bottom:
                    flags |= GravityFlags.Bottom;
                    break;
            }


			Control.SetPadding(0, 0, 0, 0);
            Control.Gravity = flags;
        }

        protected virtual void SetTextSize(BaseStarShot.Controls.DatePicker element)
        {
            Control.SetTextSize(Android.Util.ComplexUnitType.Dip, (float)element.FontSize);
        }


        protected virtual void SetBgColor(BaseStarShot.Controls.DatePicker element)
        {
            if (element.BackgroundColor != null)
                Control.SetBackgroundColor(element.BackgroundColor.ToAndroid());

        }
        
    }
}