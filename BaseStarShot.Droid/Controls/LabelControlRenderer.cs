using System;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Xamarin.Forms;
using BaseStarShot.Controls;
using Android.Text;
using Android.Text.Method;
using Android.Text.Style;
using BaseStarShot.Util;

[assembly: ExportRenderer(typeof(BaseStarShot.Controls.Label), typeof(LabelControlRenderer))]
namespace BaseStarShot.Controls
{
    public class LabelControlRenderer : LabelRenderer
    {
        private float minSize;
        private int currentWidth = 0;

        protected BaseStarShot.Controls.Label Base { get { return (BaseStarShot.Controls.Label)Element; } }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Label> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
                return;

            var labelControl = (BaseStarShot.Controls.Label)Element;
            labelControl.PropertyChanged += (s, ev) =>
                {
                    //if (labelControl != null)
                    //    UpdateInputType(labelControl);

                    switch (ev.PropertyName)
                    {
                        case "Font": SetTypeface(); break;
                        case "MaxLines": SetMaxLines(); break;
                        case "Text": SetTypeface(); break;
                        case "TextStyle": SetTextStyle(); break;
                        case "Command": SetCommand(); break;
                        case "MaxWidth": SetMaxWidth(); break;
                    }
                };
            SetTypeface();
            SetMaxLines();
            SetCommand();
            SetMaxWidth();

            if (Base.RefitTextEnabled)
                minSize = convertFromDp(Resources.GetDimension(UIHelper.GetDimensionResource("text_size")));

            UpdateInputType();
        
            currentWidth = Control.Width;
            
            Control.SetPadding(UIHelper.ConvertDPToPixels(labelControl.Padding.Left),
                UIHelper.ConvertDPToPixels(labelControl.Padding.Top),
                UIHelper.ConvertDPToPixels(labelControl.Padding.Right),
                UIHelper.ConvertDPToPixels(labelControl.Padding.Bottom));

            //if (Base.MaxCharacters.HasValue)
            //{
            //    Control.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(Base.MaxCharacters.Value) });
            //}

            SetTextStyle();
        }

        void SetMaxWidth()
        {
            if (Base.MaxWidth.HasValue)
                Control.SetMaxWidth(UIHelper.ConvertDPToPixels(Base.MaxWidth.Value));
            else
                Control.SetMaxWidth(int.MaxValue);
        }

        void SetCommand()
        {
            if (Control.HasOnClickListeners)
                Control.SetOnClickListener(null);

            if (Base.Command != null)
            {
                Control.Clickable = true;
                Control.Click += Control_Click;
            }
            else
            {
                Control.Clickable = false;
                Control.Click -= Control_Click;
            }
        }

        void Control_Click(object sender, EventArgs e)
        {
            if (Base.Command != null && Base.Command.CanExecute(Base.CommandParameter))
                Base.Command.Execute(Base.CommandParameter);
        }

        public void SetTextStyle()
        {
            if (Base.TextStyle != TextStyling.None)
            {
                SpannableString ss = new SpannableString(Control.Text);
                ss.SetSpan(new UnderlineSpan(), 0, ss.Length(), SpanTypes.ExclusiveExclusive);
                
                
                Control.TextFormatted = ss;
            }
            //TextStyle
        }

        public void UpdateInputType()
        {
            if (Base == null || Control == null)
                return;

            if (Base == null || Control == null) return;

            if (Base.InputType == TextInputType.Phone)
            {
                SpannableString ss = new SpannableString(Control.Text);
                ss.SetSpan(new URLSpan("tel:" + Control.Text), 0, Control.Text.Length, SpanTypes.ExclusiveExclusive);

                Control.TextFormatted = ss;
                Control.MovementMethod = LinkMovementMethod.Instance;
            }
            else if (Base.InputType == TextInputType.EmailAddress)
            {
                SpannableString ss = new SpannableString(Control.Text);
                ss.SetSpan(new URLSpan("mailto:" + Control.Text), 0, Control.Text.Length, SpanTypes.ExclusiveExclusive);

                Control.TextFormatted = ss;
                Control.MovementMethod = LinkMovementMethod.Instance;
            }
            else if (Base.InputType == TextInputType.Website)
            {
                SpannableString ss = new SpannableString(Control.Text);
                ss.SetSpan(new ForegroundColorSpan(Base.TextColor.ToAndroid()), 0, Control.Text.Length, SpanTypes.ExclusiveExclusive);
                ss.SetSpan(new UnderlineSpan(), 0, Control.Text.Length, SpanTypes.ExclusiveExclusive);
                //ss.SetSpan(new URLSpan(Control.Text), 0, Control.Text.Length, SpanTypes.ExclusiveExclusive);

                int clickCounter = 0;

                Control.TextFormatted = ss;
                Control.Click += (s, e) =>
                {
                    if (clickCounter > 0)
                        return;

                    Xamarin.Forms.Device.OpenUri(new System.Uri(Base.Text));

                    clickCounter += 1;
                };

                //Control.MovementMethod = LinkMovementMethod.Instance;
            }
        }
        //public override void ChildDrawableStateChanged(Android.Views.View child)
        //{
        //    base.ChildDrawableStateChanged(child);

        //    Control.Text = Control.Text;
        //    SetTextAlignment(Base);
        //    Control.SetWidth(currentWidth);
        //}

        void SetTypeface()
        {
            if (Base == null || Control == null)
                return;

            if (!string.IsNullOrEmpty(Base.FontFamily))
            {
                var label = (TextView)Control;
                if (label != null)
                    label.Typeface = FontCache.GetTypeFace(Base.FontFamily);
            }
        }

        void SetMaxLines()
        {
            if (Base == null || Control == null)
                return;

            if (Base.MaxLines > 1)
            {
                Control.SetSingleLine(false);
                Control.SetMaxLines(Base.MaxLines);
            }
            else if (Base.MaxLines == 1)
            {
                Control.SetSingleLine(true);
                Control.SetMaxLines(1);

                if (Element.LineBreakMode == LineBreakMode.NoWrap)
                    this.Control.Ellipsize = TextUtils.TruncateAt.End;
            }
        }

        string tempText = "";
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            //Control.Text = base.Element.Text;
            //SetTextAlignment(Base);
            //Control.SetWidth(width);
            //currentWidth = width;
        }

        void SetTextAlignment()
        {
            if (Base == null || Control == null)
                return;

            GravityFlags flags = GravityFlags.NoGravity;

            switch (Base.XAlign)
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

            switch (Base.YAlign)
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

        void RefitText(int width)
        {
            if (tempText.Equals(this.Control.Text))
            {
                return;
            }
            else
            {
                tempText = this.Control.Text;
            }

            if (width <= 0)
                return;

            int targetWidth = width - this.Control.PaddingLeft - this.Control.PaddingRight;
            float hi = (float)Base.FontSize;
            float lo = 2;
            float threshold = 0.5f; // How close we have to be

            var mTestPaint = new Paint();
            mTestPaint.Set(this.Control.Paint);

            //float tempLo = lo;
            //this.Control.SetSingleLine(true);

            while ((hi - lo) > threshold)
            {
                float size = (hi + lo) / 2;
                mTestPaint.TextSize = size;
                if (mTestPaint.MeasureText(Base.Text) >= targetWidth)
                    hi = size; // too big
                else
                {
                    lo = size; // too small
                }
            }

            //TODO: Fit in two lines
            //if (isTooLarge())
            //{
            //    //this.Control.SetTextSize(ComplexUnitType.Px, lo); //this will fit the text in the button even if it is unreadable
            //    this.Control.TextSize = (int)minSize;
            //    SetEllipSize();
            //}
            //else
            //{
            //    this.Control.TextSize = (int)Base.FontSize;
            //}

        }

        private bool isTooLarge()
        {
            float textWidth = base.Control.Paint.MeasureText(Base.Text);
            return (textWidth >= base.Control.MeasuredWidth);
        }

        void SetEllipSize()
        {
            this.Control.SetSingleLine(false);
            this.Control.SetMaxLines(2);
            this.Control.Ellipsize = TextUtils.TruncateAt.End;
        }

        public float convertFromDp(float input)
        {
            float scale = this.Resources.DisplayMetrics.Density;
            return ((input - 0.5f) / scale);
        }

    }
}