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
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Xamarin.Forms;
using BaseStarShot.Controls;
using Android.Text;
using Android.Text.Style;
using Android.Text.Method;
using BaseStarShot.Util;

[assembly: ExportRenderer(typeof(BaseStarShot.Controls.FormattedLabelChild), typeof(FormattedLabelRenderer))]
namespace BaseStarShot.Controls
{
    public class FormattedLabelRenderer : LabelRenderer
    {
        protected BaseStarShot.Controls.FormattedLabelChild Base { get { return (BaseStarShot.Controls.FormattedLabelChild)Element; } }

        public FormattedLabelChild GetFormattedElement()
        {
            BaseStarShot.Controls.FormattedLabelChild test = Base;
            return Base;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Label> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
                return;

            var labelControl = (BaseStarShot.Controls.FormattedLabelChild)Element;


            labelControl.PropertyChanged += (s, ev) =>
            {
                switch (ev.PropertyName)
                {
                    case "FormattedText": SetTextStyle(); break;
                    case "Font": SetTypeface((BaseStarShot.Controls.FormattedLabelChild)s); break;
                    case "Text": SetTypeface((BaseStarShot.Controls.FormattedLabelChild)s); break;
                }
            };
            SetTypeface(labelControl);
            SetTextStyle();
        }

        void SetTextStyle()
        {
            if (base.Control == null) return;

            SpannableString styledString = new SpannableString(base.Control.Text);

            int counter = 0;
            foreach (var span in Base.FormattedText.Spans)
            {
                var spanText = Base.FormattedText.Spans[counter].Text;
                var startIndex = base.Control.Text.IndexOf(spanText);
                var endIndex = startIndex + spanText.Length;

                if (Base.ClickableIndex == counter)
                {
                    var clickableSpan = new MyClickableSpan();
                    TextPaint paint = new TextPaint();

                    paint.LinkColor = Base.FormattedText.Spans[counter].ForegroundColor.ToAndroid();
                    paint.BgColor = Base.FormattedText.Spans[counter].ForegroundColor.ToAndroid();
                    paint.Color = Base.FormattedText.Spans[counter].ForegroundColor.ToAndroid();

                    clickableSpan.UpdateDrawState(paint);

                    clickableSpan.Click += v =>
                    {
                        if (Base.Command != null && Base.Command.CanExecute(null))
                            Base.Command.Execute(null);
                    };

                    styledString.SetSpan(clickableSpan, startIndex, endIndex, SpanTypes.ExclusiveExclusive);
                    styledString.SetSpan(new UnderlineSpan(), startIndex, endIndex, 0);
                }

                styledString.SetSpan(new ForegroundColorSpan(Base.FormattedText.Spans[counter].ForegroundColor.ToAndroid()), startIndex, endIndex, 0);
                counter += 1;
            }


            //paint.LinkColor = Base.FormattedText.Spans[2].ForegroundColor.ToAndroid();
            //paint.BgColor = Base.FormattedText.Spans[2].ForegroundColor.ToAndroid();
            //paint.Color = Base.FormattedText.Spans[2].ForegroundColor.ToAndroid();

            //styledString.SetSpan(clickableSpan, span3StartIndex, base.Control.Text.Length, SpanTypes.ExclusiveExclusive);
            //styledString.SetSpan(new UnderlineSpan(), span3StartIndex, base.Control.Text.Length, 0);
            //styledString.SetSpan(new ForegroundColorSpan(Base.FormattedText.Spans[2].ForegroundColor.ToAndroid()), span3StartIndex, base.Control.Text.Length, 0);
            //styledString.SetSpan(new ForegroundColorSpan(Base.FormattedText.Spans[0].ForegroundColor.ToAndroid()), 0, span3StartIndex, 0);

            this.Control.TextAlignment = Android.Views.TextAlignment.Center;
            this.Control.TextFormatted = styledString;
            this.Control.MovementMethod = new LinkMovementMethod();

        }

        void SetTypeface(BaseStarShot.Controls.FormattedLabelChild element)
        {
            if (!string.IsNullOrEmpty(element.FontFamily))
            {
               var label = (TextView)Control;
               label.Typeface = FontCache.GetTypeFace(element.FontFamily);
            }
        }

        void SetTextAlignment(BaseStarShot.Controls.Label element)
        {
            if (Control == null) return;

            GravityFlags flags = GravityFlags.NoGravity;

            switch (element.XAlign)
            {
                case Xamarin.Forms.TextAlignment.Center:
                    flags = GravityFlags.CenterHorizontal;
                    break;
                case Xamarin.Forms.TextAlignment.Start:
                    flags = GravityFlags.Left;
                    break;
                case Xamarin.Forms.TextAlignment.End:
                    flags = GravityFlags.Right;
                    break;
            }

            switch (element.YAlign)
            {
                case Xamarin.Forms.TextAlignment.Center:
                    flags |= GravityFlags.CenterVertical;
                    break;
                case Xamarin.Forms.TextAlignment.Start:
                    flags |= GravityFlags.Top;
                    break;
                case Xamarin.Forms.TextAlignment.End:
                    flags |= GravityFlags.Bottom;
                    break;
            }

            Control.Gravity = flags;
        }


        private class MyClickableSpan : ClickableSpan
        {
            public Action<Android.Views.View> Click;

            public override void OnClick(Android.Views.View widget)
            {
                if (Click != null)
                    Click(widget);
            }

            //public override void UpdateDrawState(TextPaint ds)
            //{
            //    ds.Color = Android.Graphics.Color.ParseColor("#bab19d");//Base.FormattedText.Spans[2].ForegroundColor;
            //    base.UpdateDrawState(ds);
            //}
        }
    }
}