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
using System.Threading.Tasks;
using Android.Text;
using Android.Text.Style;
using Android.Graphics.Drawables;
using Android.Util;
using Java.Lang.Reflect;
using BaseStarShot.Util;

namespace BaseStarShot.Controls
{
    public class ButtonControlRenderer : ButtonRenderer
    {
        private int currentWidth = 0;

        protected BaseStarShot.Controls.Button BaseElement { get { return (BaseStarShot.Controls.Button)Element; } }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || e.NewElement == null) return;

            var buttonControl = (BaseStarShot.Controls.Button)Element;

            buttonControl.PropertyChanged += (s, ev) =>
                {
                    if (Control == null)
                        return;

                    var element = (BaseStarShot.Controls.Button)s;
                    switch (ev.PropertyName)
                    {
                        case "Text":
                            Control.Text = element.Text;
                            Control.SetWidth(currentWidth);
                            break;
                        case "Renderer":


                            if (element.Height > 0 && element.Width > 0)
                            {
                                SetImages(element);
                            }
                            break;
                        case "Font":
                            SetTypeface(element);
                            break;
                        case "BackgroundColor":

                            CreateShapeDrawable(buttonControl);
                            break;
                        case "BackgroundImage":
                            SetBackground(element);
                            SetPadding(element);
                            break;
                        case "ImageLeft":
                        case "ImageRight":
                        case "ImageTop":
                        case "ImageBottom":
                            SetImages(element);
                            SetPadding(element);
                            break;
                        case "Padding":
                            SetPadding(element);
                            SetTextAlignment(buttonControl);
                            break;
                        case "TitlePadding":
                            SetTitlePadding(element);
                            break;
                        case "XAlign":
                        case "YAlign":
                            SetTextAlignment(buttonControl);
                            break;
                        //case "BorderColor":
                        //    UpdateBorderColor();
                        //    break;
                        case "Width":
                            SetImages(buttonControl);
                            break;
                        case "IsEnabled": SetDisabledTextColor(); 
                            break;
                    }
                };

            SetTypeface(buttonControl);
            SetBackground(buttonControl);
            SetPadding(buttonControl);
            SetTitlePadding(buttonControl);
            SetTextAlignment(buttonControl);

            if (buttonControl.RefitTextEnabled)
            {
                var mTestPaint = new Paint();
                mTestPaint.Set(this.Control.Paint);

                base.Control.Text = base.Element.Text;
                //minSize = convertFromDp(Resources.GetDimension(UIHelper.GetDimensionResource("text_size"))); //This line causes crash in balut project
                base.Control.SetSingleLine(false);
                base.Control.SetMaxLines(2);
                base.Control.Ellipsize = TextUtils.TruncateAt.End;
                base.Control.SetPadding(0, 0, 0, 0);
                base.Control.CompoundDrawablePadding = 0;
                base.Control.LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.WrapContent);

                //if (base.Control.LineCount > 1 && (mTestPaint.MeasureText(base.Control.Text) * 2) > base.Control.MeasuredWidth)
                //    base.Control.SetTextSize(ComplexUnitType.Px, minSize);
            }
            CreateShapeDrawable(buttonControl);
            this.Control.LongClick += Control_LongClick;
            SetDisabledTextColor();

            this.SetWillNotDraw(false);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            currentWidth = MeasureSpec.GetSize(widthMeasureSpec);
            if (currentWidth != Control.Width)
            {
                Control.SetWidth(currentWidth);
            }

        }

        public override void ChildDrawableStateChanged(Android.Views.View child)
        {
            base.ChildDrawableStateChanged(child);
            Control.Text = Control.Text;
            Control.SetWidth(currentWidth);
        }

        void UpdateBorderColor()
        {
            if (Element.BorderColor == Xamarin.Forms.Color.Default)
                return;

            var background = Control.Background;

            Android.Graphics.Color color = Element.BorderColor.ToAndroid();

            //int[] states = new int[] 
            //{
            //    Android.Resource.Attribute.StateNotNeeded,
            //    Android.Resource.Attribute.StatePressed,
            //    Android.Resource.Attribute.StateEnabled,
            //    Android.Resource.Attribute.StateFocused,
            //    -Android.Resource.Attribute.StateEnabled,
            //}; 
            int[] states = background.GetState();

            for (var state = 0; state < states.Count(); state++)
            {

                //IntPtr cls = JNIEnv.FindClass(background.Class.Name);
                //IntPtr mth = JNIEnv.GetMethodID(cls, "getStateDrawableIndex", "(I)V");
                //const int x = 16777216;
                //JNIEnv.CallIntMethod(background.Handle, mth, new JValue(x));


                Method getStateDrawable = background.Class.GetMethod("getStateDrawable", new Java.Lang.Class[] { Java.Lang.Integer.Type });
                if (getStateDrawable != null)
                {
                    var backgroundDrawable = getStateDrawable.Invoke(background, state);
                    if (backgroundDrawable is LayerDrawable) //used for roundbutton control
                    {
                        ((GradientDrawable)((LayerDrawable)backgroundDrawable).GetDrawable(0)).SetStroke(2, color);
                        Control.InvalidateDrawable(((GradientDrawable)((LayerDrawable)backgroundDrawable).GetDrawable(0)));

                        //var layerDrawable = (LayerDrawable)backgroundDrawable;
                        //if (layerDrawable.GetDrawable(0) is GradientDrawable)
                        //{
                        //    ((GradientDrawable)layerDrawable.GetDrawable(0)).SetStroke(2, color);
                        //}
                    }
                    else if (backgroundDrawable is GradientDrawable)
                    {
                        ((GradientDrawable)backgroundDrawable).SetStroke(2, color);
                    }
                }
            }
            Control.Hovered = true;
            Control.Hovered = false;
        }
        
        void SetDisabledTextColor()
        {
            if (BaseElement.DisabledTextColor == Xamarin.Forms.Color.Default)
                return;

            if (Element.IsEnabled)
                Control.SetTextColor(Element.TextColor.ToAndroid());
            else
                Control.SetTextColor(BaseElement.DisabledTextColor.ToAndroid());
        }

        public float convertFromDp(float input)
        {
            float scale = this.Resources.DisplayMetrics.Density;
            return ((input - 0.5f) / scale);
        }

        void Control_LongClick(object sender, Android.Views.View.LongClickEventArgs e)
        {
            BaseElement.OnLongPress();
            //UpdateBorderColor();
        }

        void SetPadding(BaseStarShot.Controls.Button element)
        {
            Control.SetPadding(UIHelper.ConvertDPToPixels(element.Padding.Left),
                UIHelper.ConvertDPToPixels(element.Padding.Top),
                UIHelper.ConvertDPToPixels(element.Padding.Right),
                UIHelper.ConvertDPToPixels(element.Padding.Bottom));
        }

        void SetTitlePadding(BaseStarShot.Controls.Button element)
        {
            Control.CompoundDrawablePadding = UIHelper.ConvertDPToPixels(element.TextPadding.Left);
        }

        void SetTypeface(BaseStarShot.Controls.Button element)
        {
            if (!string.IsNullOrEmpty(element.FontFamily))
                Control.Typeface = FontCache.GetTypeFace(element.FontFamily);
        }

        void SetBackground(BaseStarShot.Controls.Button element)
        {
            if (element.BackgroundImage != null)
            {
                // seems this is not working, LoadImageAsync returns null
                //var bitmap = await imageLoader.LoadImageAsync(element.BackgroundImage, Forms.Context);
                //button.SetBackgroundDrawable(new Android.Graphics.Drawables.BitmapDrawable(bitmap));

                var resourceId = UIHelper.GetDrawableResource(element.BackgroundImage);
                if (resourceId > 0)
                {
                    Control.SetBackgroundResource(resourceId);
                }
                else
                {
                    Control.SetBackgroundResource(Android.Resource.Drawable.ButtonDefault);
                }
            }
            else
            {
                if (element.BackgroundColor == Xamarin.Forms.Color.Transparent
                    || element.BackgroundColor == Xamarin.Forms.Color.Default)
                    Control.SetBackgroundResource(Android.Resource.Drawable.ButtonDefault);
            }
        }

        void SetTextAlignment(BaseStarShot.Controls.Button element)
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
        
        void SetImages(BaseStarShot.Controls.Button element)
        {
            ButtonText = Element.Text;

            if (!HaseResource(element.ImageLeft) && !HaseResource(element.ImageTop) && !HaseResource(element.ImageRight) && !HaseResource(element.ImageBottom))
            {
                Control.SetCompoundDrawablesWithIntrinsicBounds(null, null, null, null);
                Control.Invalidate();
                return;
            }

            Drawable leftResource = null, topResource = null, rightResource = null, bottomResource = null;

            if (element.ImageLeft != null && element.ImageLeft.File != null)
                leftResource = Context.Resources.GetDrawable(UIHelper.GetDrawableResource(element.ImageLeft));
            if (element.ImageTop != null && element.ImageTop.File != null)
                topResource = Context.Resources.GetDrawable(UIHelper.GetDrawableResource(element.ImageTop));
            if (element.ImageRight != null && element.ImageRight.File != null)
                rightResource = Context.Resources.GetDrawable(UIHelper.GetDrawableResource(element.ImageRight));
            if (element.ImageBottom != null && element.ImageBottom.File != null)
                bottomResource = Context.Resources.GetDrawable(UIHelper.GetDrawableResource(element.ImageBottom));

            bool hasResource = leftResource != null || rightResource != null || topResource != null || bottomResource != null;
            if (hasResource && !element.CenterImage)
            {
                Control.SetCompoundDrawablesWithIntrinsicBounds(leftResource, topResource, rightResource, bottomResource);
            }

            if (hasResource && element.CenterImage)
            {
                Xamarin.Forms.Size size = new Xamarin.Forms.Size();

                if (leftResource != null)
                {
                    if (element.ImageLeftHeight > 0 && element.ImageLeftWidth > 0)
                    {
                        size.Width = element.ImageLeftWidth;
                        size.Height = element.ImageLeftHeight;
                    }
                    else
                    {
                        var maxWidth = UIHelper.ConvertDPToPixels(element.Padding.Left) - UIHelper.ConvertDPToPixels(element.Padding.Right) - Control.MeasuredWidth;
                        var maxHeight = UIHelper.ConvertDPToPixels(element.Padding.Top) - UIHelper.ConvertDPToPixels(element.Padding.Bottom) - Control.MeasuredHeight;

                        var temp = GetSize(leftResource.IntrinsicWidth, leftResource.IntrinsicHeight, Math.Abs(maxWidth), Math.Abs(maxHeight));
                        size.Width = temp.Width;
                        size.Height = temp.Height;
                    }

                    var scaledDrawable = new ScaleDrawable(leftResource, 0, (int)size.Width, (int)size.Height).Drawable;
                    scaledDrawable.SetBounds(0, 0, (int)size.Width, (int)size.Height);

                    Control.SetCompoundDrawables(scaledDrawable, null, null, null);
                }

                if (rightResource != null)
                {
                    if (element.ImageRightHeight > 0 && element.ImageRightWidth > 0)
                    {
                        size.Width = element.ImageRightWidth;
                        size.Height = element.ImageRightHeight;
                    }
                    else
                    {
                        var maxWidth = UIHelper.ConvertDPToPixels(element.Padding.Left) - UIHelper.ConvertDPToPixels(element.Padding.Right) - Control.MeasuredWidth;
                        var maxHeight = UIHelper.ConvertDPToPixels(element.Padding.Top) - UIHelper.ConvertDPToPixels(element.Padding.Bottom) - Control.MeasuredHeight;

                        var temp = GetSize(rightResource.IntrinsicWidth, rightResource.IntrinsicHeight, Math.Abs(maxWidth), Math.Abs(maxHeight));
                        size.Width = temp.Width;
                        size.Height = temp.Height;
                    }

                    var scaledDrawable = new ScaleDrawable(rightResource, 0, (int)size.Width, (int)size.Height).Drawable;
                    scaledDrawable.SetBounds(0, 0, (int)size.Width, (int)size.Height);

                    Control.SetCompoundDrawables(null, null, scaledDrawable, null);
                }

                if (!element.CenterImage)
                    SetImagePadding((int)size.Width);

                Control.Invalidate();
            }
        }

        void SetImagePadding(int drawableWidth)
        {
            var button = this.Control as Android.Widget.Button;
            Android.Graphics.Rect bounds = new Android.Graphics.Rect();
            Paint p = button.Paint;

            float textWidth = 0;
            if (!string.IsNullOrWhiteSpace(Element.Text))
            {
                p.GetTextBounds(Element.Text, 0, Element.Text.Length, bounds);
                textWidth = p.MeasureText(Element.Text);
            }

            var paddingRight = UIHelper.ConvertDPToPixels(BaseElement.Padding.Right);

            var left = Control.MeasuredWidth - ((Control.MeasuredWidth + textWidth + drawableWidth) / 2 + paddingRight);
            if (textWidth > 0)
                left /= 2;

            if (BaseElement.Padding.Top > 0 || BaseElement.Padding.Bottom > 0)
                Control.SetPadding((int)left, (int)BaseElement.Padding.Top, (int)left, (int)BaseElement.Padding.Bottom);
            else
                Control.SetPadding((int)left, Control.PaddingTop, (int)left, Control.PaddingBottom);
        }

        private string ButtonText = "";

        private Xamarin.Forms.Size GetSize(int imageWidth, int imageHeight, int maxWidth, int maxHeight)
        {
            Xamarin.Forms.Size size;
            if (maxHeight > 0 && maxWidth > 0 && imageWidth > maxWidth && imageHeight > maxHeight)
            {
                float ratioBitmap = (float)imageWidth / (float)imageHeight;
                float ratioMax = (float)maxWidth / (float)maxHeight;

                int finalWidth = maxWidth;
                int finalHeight = maxHeight;
                if (ratioMax > 1)
                {
                    finalWidth = (int)((float)maxHeight * ratioBitmap);
                }
                else
                {
                    finalHeight = (int)((float)maxWidth / ratioBitmap);
                }

                size = new Xamarin.Forms.Size(finalWidth, finalHeight);

            }
            else
            {
                size = new Xamarin.Forms.Size(imageWidth, imageHeight);
            }

            return size;
        }

        private bool HaseResource(FileImageSource source)
        {
            return source != null && source.File != null;
        }

        private readonly Android.Graphics.Rect textBounds = new Android.Graphics.Rect();
        private readonly Android.Graphics.Rect drawableBounds = new Android.Graphics.Rect();
        
        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            if (Control.GetCompoundDrawables() != null && BaseElement.CenterImage)
            {
                var text = Control.Text;
                int iconTextSpacing = 0;

                if (!string.IsNullOrWhiteSpace(text))
                {
                    TextPaint textPaint = Control.Paint;
                    textPaint.GetTextBounds(text, 0, text.Length, textBounds);
                    iconTextSpacing = 20;
                }
                else
                {
                    textBounds.SetEmpty();
                }

                int width = Control.Width - (Control.PaddingLeft + Control.PaddingRight);

                Drawable[] drawables = Control.GetCompoundDrawables();

                if (drawables[0] != null)
                {
                    drawables[0].CopyBounds(drawableBounds);

                    int leftOffset =
                            (width - (textBounds.Width() + drawableBounds.Width())) / 2 - Control.CompoundDrawablePadding - iconTextSpacing;

                    if (drawableBounds.Left == 0 && drawableBounds.Left != leftOffset)
                    {
                        drawableBounds.Offset(leftOffset, 0);
                        drawables[0].SetBounds(drawableBounds.Left, drawableBounds.Top, drawableBounds.Right, drawableBounds.Bottom);
                    }
                }

                if (drawables[2] != null)
                {
                    drawables[2].CopyBounds(drawableBounds);

                    int rightOffset =
                            ((textBounds.Width() + drawableBounds.Width()) - width) / 2 + Control.CompoundDrawablePadding + iconTextSpacing;

                    if (drawableBounds.Left == 0 && drawableBounds.Right != rightOffset)
                    {
                        drawableBounds.Offset(rightOffset, 0);
                        drawables[2].SetBounds(drawableBounds.Left, drawableBounds.Top, drawableBounds.Right, drawableBounds.Bottom);
                    }
                }
            }


            //if (Control.GetCompoundDrawables() != null)
            //{
            //    if (BaseElement.CenterImage)
            //    {
            //        var imageRight = Control.GetCompoundDrawables()[2];
            //        if (imageRight != null)
            //        {
            //            var drawableBounds = new Android.Graphics.Rect();
            //            imageRight.CopyBounds(drawableBounds);
            //            SetImagePadding(drawableBounds.Width());
            //        }

            //        var imageLeft = Control.GetCompoundDrawables()[0];
            //        if (imageLeft != null)
            //        {
            //            var drawableBounds = new Android.Graphics.Rect();
            //            imageLeft.CopyBounds(drawableBounds);
            //            SetImagePadding(drawableBounds.Width());
            //        }
            //    }
            //}
        }

        void CreateShapeDrawable(BaseStarShot.Controls.Button element)
        {
            GradientDrawable shape = new GradientDrawable();
            shape.SetShape(ShapeType.Rectangle);

            //if (element.CornerRadius != 0)
            {
                shape.SetCornerRadii(new float[] 
                { 
                    UIHelper.ConvertDPToPixels(element.CornerRadius.Top), 
                    UIHelper.ConvertDPToPixels(element.CornerRadius.Top), 
                    UIHelper.ConvertDPToPixels(element.CornerRadius.Right), 
                    UIHelper.ConvertDPToPixels(element.CornerRadius.Right), 
                    UIHelper.ConvertDPToPixels(element.CornerRadius.Bottom), 
                    UIHelper.ConvertDPToPixels(element.CornerRadius.Bottom), 
                    UIHelper.ConvertDPToPixels(element.CornerRadius.Left), 
                    UIHelper.ConvertDPToPixels(element.CornerRadius.Left)
                 });
            }
            if (Element.BackgroundColor != Xamarin.Forms.Color.Default)
                shape.SetColor(Element.BackgroundColor.ToAndroid());

            if (element.BorderColor != Xamarin.Forms.Color.Default)
                shape.SetStroke(2, element.BorderColor.ToAndroid());


            Control.Background = shape;


        }
    }

}