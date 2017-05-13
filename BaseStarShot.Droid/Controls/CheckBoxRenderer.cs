using Xamarin.Forms;

[assembly: ExportRenderer(typeof(BaseStarShot.Controls.CheckBox), typeof(BaseStarShot.Controls.CheckBoxRenderer))]
namespace BaseStarShot.Controls
{
    using System;
    using System.ComponentModel;

    using Android.Graphics;

    using Xamarin.Forms.Platform.Android;
    using Android.Graphics.Drawables;
    using Android.OS;
    using Android.Text.Method;
    using Android.Widget;
    using Android.Text;
    using BaseStarShot.Util;
    using Services;
    using Base1902;

    /// <summary>
    /// Class CheckBoxRenderer.
    /// </summary>
    public class CheckBoxRenderer : ViewRenderer<CheckBox, LinearLayout>
    {
        Android.Widget.CheckBox checkBox = null;
        /// <summary>
        /// Called when [element changed].
        /// </summary>
        /// <param name="e">The e.</param>
        protected override void OnElementChanged(ElementChangedEventArgs<CheckBox> e)
        {
            base.OnElementChanged(e);
            var linearLayout = new LinearLayout(base.Context);
            linearLayout.Orientation = Orientation.Horizontal;

            if (this.Control == null)
            {
                checkBox = new Android.Widget.CheckBox(this.Context);
                checkBox.CheckedChange += CheckBoxCheckedChange;

                linearLayout.AddView(checkBox);
                this.SetNativeControl(linearLayout);
            }

            checkBox.Checked = e.NewElement.Checked;
            checkBox.SetTextColor(e.NewElement.TextColor.ToAndroid());
            checkBox.CompoundDrawablePadding = 15;

            int spacing = checkBox.PaddingLeft;
            if (Element.Spacing.HasValue)
                spacing = UIHelper.ConvertDPToPixels(Element.Spacing.Value);

            checkBox.SetPadding(spacing, checkBox.PaddingTop, checkBox.PaddingRight, checkBox.PaddingBottom);

            if (e.NewElement.EnableLink)
            {
                checkBox.Text = "";

                var textView = new TextView(base.Context);
                textView.SetTextColor(e.NewElement.TextColor.ToAndroid());
                var color = e.NewElement.TextColor.ToHex();

                textView.TextFormatted = Html.FromHtml("<span style='color: " + color + "'>" + e.NewElement.Text + "</span> <a href='" + e.NewElement.LinkString + "' style='color: " + color + "'>" + e.NewElement.InlineText + "</a>");
                textView.MovementMethod = LinkMovementMethod.Instance;
                textView.Gravity = Android.Views.GravityFlags.CenterVertical;

                Control.AddView(textView);
            }
            else
            {
                checkBox.Text = e.NewElement.Text;
            }

            int id = -1;
            if (Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.Lollipop)
                id = this.Context.Resources.GetIdentifier("btn_check_material_anim", "drawable", "android");
            else
                id = this.Context.Resources.GetIdentifier("btn_check_holo_light", "drawable", "android");

            if (Element.CheckedImage != null && Element.UnCheckedImage != null)
            {
                var checkedDrawable = this.Resources.GetDrawable(UIHelper.GetDrawableResource(Element.CheckedImage));
                var uncheckedDrawable = this.Resources.GetDrawable(UIHelper.GetDrawableResource(Element.UnCheckedImage));

                if (Element.ImageHeight > 0 && Element.ImageWidth > 0)
                {
                    checkedDrawable.SetBounds(0, 0, UIHelper.ConvertDPToPixels(Element.ImageWidth), UIHelper.ConvertDPToPixels(Element.ImageHeight));
                    uncheckedDrawable.SetBounds(0, 0, UIHelper.ConvertDPToPixels(Element.ImageWidth), UIHelper.ConvertDPToPixels(Element.ImageHeight));
                }

                var states = new StateListDrawable();
                states.AddState(new int[] { -Android.Resource.Attribute.StateChecked, Android.Resource.Attribute.StateEnabled }, uncheckedDrawable);
                states.AddState(new int[] { Android.Resource.Attribute.StateChecked, Android.Resource.Attribute.StateEnabled }, checkedDrawable);
                states.AddState(new int[] { -Android.Resource.Attribute.StateChecked, -Android.Resource.Attribute.StateEnabled }, uncheckedDrawable);
                states.AddState(new int[] { Android.Resource.Attribute.StateChecked, -Android.Resource.Attribute.StateEnabled }, checkedDrawable);

                //drawable.AddState(new[] { Android.Resource.Attribute.Checked },
                //                          this.Resources.GetDrawable(UIHelper.GetDrawableResource(Element.CheckedImage)));

                //drawable.AddState(new[] { ~Android.Resource.Attribute.Checked },
                //                          this.Resources.GetDrawable(UIHelper.GetDrawableResource(Element.UnCheckedImage)));
                checkBox.SetButtonDrawable(states);
            }
            else
            {
                Drawable drawable = Resources.GetDrawable(id);
                drawable.SetColorFilter(Color.White, PorterDuff.Mode.SrcAtop);
                checkBox.SetButtonDrawable(drawable);
            }


            if (e.NewElement.FontSize > 0)
            {
                checkBox.TextSize = (float)e.NewElement.FontSize;
            }

            if (!string.IsNullOrEmpty(e.NewElement.FontName))
            {
                SetTypeface(e.NewElement.FontName);
            }
        }

        void SetTypeface(string fontName)
        {
            if (!string.IsNullOrEmpty(fontName))
            {
                checkBox.Typeface = FontCache.GetTypeFace(fontName);
            }
            else
            {
                var font = Resolver.Get<IFontService>().GetFontName(FontStyle.Regular);
                if (!string.IsNullOrEmpty(font))
                    checkBox.Typeface = FontCache.GetTypeFace(font);
            }
            //label.TextSize = (float)element.FontSize;
        }

        /// <summary>
        /// Handles the <see cref="E:ElementPropertyChanged" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            switch (e.PropertyName)
            {
                case "Checked":
                    if (!Element.EnableLink)
                        checkBox.Text = Element.Text;
                    checkBox.Checked = Element.Checked;
                    break;
                case "TextColor":
                    checkBox.SetTextColor(Element.TextColor.ToAndroid());
                    break;
                case "FontName":
                    if (!string.IsNullOrEmpty(Element.FontName))
                    {
                        checkBox.Typeface = TrySetFont(Element.FontName);
                    }
                    break;
                case "FontSize":
                    if (Element.FontSize > 0)
                    {
                        checkBox.TextSize = (float)Element.FontSize;
                    }
                    break;
                case "CheckedText":
                case "UncheckedText":
                    if (!Element.EnableLink)
                        checkBox.Text = Element.Text;
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine("Property change for {0} has not been implemented.", e.PropertyName);
                    break;
            }
        }

        /// <summary>
        /// CheckBoxes the checked change.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="Android.Widget.CompoundButton.CheckedChangeEventArgs"/> instance containing the event data.</param>
        void CheckBoxCheckedChange(object sender, Android.Widget.CompoundButton.CheckedChangeEventArgs e)
        {
            this.Element.Checked = e.IsChecked;
        }

        /// <summary>
        /// Tries the set font.
        /// </summary>
        /// <param name="fontName">Name of the font.</param>
        /// <returns>Typeface.</returns>
        private Typeface TrySetFont(string fontName)
        {
            Typeface tf = Typeface.Default;
            try
            {
                return FontCache.GetTypeFace(fontName);
            }
            catch (Exception ex)
            {
                Console.Write("not found in assets {0}", ex);
                try
                {
                    tf = Typeface.CreateFromFile(fontName);
                    return tf;
                }
                catch (Exception ex1)
                {
                    Console.Write(ex1);
                    return Typeface.Default;
                }
            }
        }
    }

}