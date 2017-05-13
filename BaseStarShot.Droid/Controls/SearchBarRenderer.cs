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
using Android.Text;
using Android.Views.InputMethods;
using BaseStarShot.Helpers;
using Android.Content.PM;
using Java.Lang.Reflect;
using Android.Graphics.Drawables;
using Android.Graphics;
using BaseStarShot.Util;

[assembly: ExportRenderer(typeof(BaseStarShot.Controls.SearchBar), typeof(BaseStarShot.Controls.SearchBarRenderer))]
namespace BaseStarShot.Controls
{
    public class SearchBarRenderer : Xamarin.Forms.Platform.Android.SearchBarRenderer
    {
        private EditText editText;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.SearchBar> e)
        {
            base.OnElementChanged(e);

            var element = (BaseStarShot.Controls.SearchBar)Element;

            // Get native control (background set in shared code, but can use SetBackgroundColor here)
            Android.Widget.SearchView searchView = (base.Control as Android.Widget.SearchView);
            searchView.SetInputType(InputTypes.ClassText | InputTypes.TextVariationNormal);

            int textViewId = searchView.Context.Resources.GetIdentifier("android:id/search_src_text", null, null);
            editText = (searchView.FindViewById(textViewId) as EditText);
            editText.SetTextColor(element.TextColor.ToAndroid());
            //editText.TextAlignment = Android.Views.TextAlignment.
            editText.Gravity = GravityFlags.CenterVertical | GravityFlags.Start;

            if (element.PlaceholderColor != Xamarin.Forms.Color.Default)
                editText.SetHintTextColor(element.PlaceholderColor.ToAndroid());

            //if (element.ShowSearchIconAsPlaceHolder)
            //    editText.SetCompoundDrawablesRelativeWithIntrinsicBounds(Android.Resource.Drawable.IcMenuSearch, 0, 0, 0);

            //editText.TextChanged += editText_TextChanged;

            int searchPlateId = searchView.Context.Resources.GetIdentifier("android:id/search_plate", null, null);
            LinearLayout searchPlateLayout = (searchView.FindViewById(searchPlateId) as LinearLayout);
            //searchPlateLayout.SetBackgroundColor(Android.Graphics.Color.Red);

            if (element.Background != null && element.Background.File != null)
                searchPlateLayout.SetBackgroundResource(UIHelper.GetDrawableResource(element.Background));
            else
            {
                if (element.BackgroundColor != Xamarin.Forms.Color.Default)
                    searchPlateLayout.SetBackgroundColor(element.BackgroundColor.ToAndroid());
            }

            int frameId = searchView.Context.Resources.GetIdentifier("android:id/search_edit_frame", null, null);
            //Android.Views.View frameView = (searchView.FindViewById(frameId) as Android.Views.View);
            LinearLayout frameView = (searchView.FindViewById(frameId) as LinearLayout);

            if (element.Background != null && element.Background.File != null)
                frameView.SetBackgroundResource(UIHelper.GetDrawableResource(element.Background));

            int searchIconId = searchView.Context.Resources.GetIdentifier("android:id/search_mag_icon", null, null);
            ImageView searchIcon = (searchView.FindViewById(searchIconId) as ImageView);
            if (element.SearchIcon != null && element.SearchIcon.File != null)
                searchIcon.SetImageResource(UIHelper.GetDrawableResource(element.SearchIcon));
            else
            {
                searchIcon.SetImageResource(Android.Resource.Drawable.IcMenuSearch);
            }

            if (element.AlignSearchIconRight)
            {
                searchIcon.LayoutParameters = new LinearLayout.LayoutParams(0, 0);

                var iv = new ImageView(this.Context);
                iv.Click += (s, ev) =>
                {
                    if (Element.SearchCommand != null && Element.SearchCommand.CanExecute(Element.SearchCommandParameter))
                        Element.SearchCommand.Execute(Element.SearchCommandParameter);
                };

                if (searchIcon.Drawable != null)
                    iv.SetImageDrawable(searchIcon.Drawable);

                frameView.AddView(iv);

                this.SetBackgroundColor(Android.Graphics.Color.Transparent);
            }


            int search_close_btnID = searchView.Context.Resources.GetIdentifier("android:id/search_close_btn", null, null);
            ImageView search_close_btn = (searchView.FindViewById(search_close_btnID) as ImageView);
            search_close_btn.Click += search_close_btn_Click;

            SetTextInputTypes(element);
            search_close_btn.SetImageResource(Android.Resource.Drawable.IcMenuCloseClearCancel);

            //if (GetThemeName() == Android.Resource.Style.ThemeMaterialLightDarkActionBar)
            //{

            //}

            CreateShapeDrawable(element);
            SetTypeface();
            //if (element.HeightRequest > 0)
            //{
            //    editText.SetHeight(UIHelper.ConvertDPToPixels(element.HeightRequest));
            //}
            searchView.QueryTextFocusChange += SearchView_QueryTextFocusChange;
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var element = (BaseStarShot.Controls.SearchBar)sender;

            switch (e.PropertyName)
            {
                case "FontSize":
                case "Font":
                    SetTypeface();
                    break;
                case "ClearText":
                    if (element.ClearText)
                    {
                        editText.Text = "";
                        element.ClearText = false;
                    }
                    break;
            }
        }

        void SetTypeface()
        {
            if (!string.IsNullOrEmpty(Element.FontFamily))
                editText.Typeface = FontCache.GetTypeFace(Element.FontFamily);

            editText.TextSize = (float)Element.FontSize;
        }

        void CreateShapeDrawable(BaseStarShot.Controls.SearchBar element)
        {
            GradientDrawable shape = new GradientDrawable();
            shape.SetShape(ShapeType.Rectangle);
            shape.SetCornerRadius(UIHelper.ConvertDPToPixels(element.CornerRadius));

            if (Element.BackgroundColor != Xamarin.Forms.Color.Default)
                shape.SetColor(Element.BackgroundColor.ToAndroid());

            Control.Background = shape;
        }

        //void editText_TextChanged(object sender, Android.Text.TextChangedEventArgs e)
        //{
        //    if (!String.IsNullOrWhiteSpace(editText.Text.ToString()))
        //    {
        //        editText.SetCompoundDrawablesWithIntrinsicBounds(0, 0, 0, 0);
        //    }
        //    else
        //    {
        //        //Assign your image again to the view, otherwise it will always be gone even if the text is 0 again.
        //        editText.SetCompoundDrawablesWithIntrinsicBounds(Android.Resource.Drawable.IcMenuSearch, 0, 0, 0);
        //    }
        //}

        //public int GetThemeName()
        //{
        //    var themeId = base.Context.PackageManager.GetActivityInfo((base.Context as Activity).ComponentName, PackageInfoFlags.MetaData).Theme;
        //    return themeId;
        //}

        void search_close_btn_Click(object sender, EventArgs e)
        {
            editText.Text = "";
            base.Element.Text = "";

            InputMethodManager inputMethodManager = this.Context.GetSystemService(Context.InputMethodService) as InputMethodManager;
            inputMethodManager.HideSoftInputFromWindow(this.Control.WindowToken, HideSoftInputFlags.None);
        }

        void SetTextInputTypes(BaseStarShot.Controls.SearchBar element)
        {
            switch (element.TextInputType)
            {
                case TextInputType.Normal: editText.InputType = InputTypes.TextVariationNormal; break;
                case TextInputType.EmailAddress: editText.InputType = InputTypes.TextVariationEmailAddress; break;
                case TextInputType.Password: editText.InputType = InputTypes.TextVariationPassword; break;
                case TextInputType.Phone: editText.InputType = InputTypes.ClassPhone; break;
                case TextInputType.Number: editText.InputType = InputTypes.ClassNumber; break;
                case TextInputType.PersonName: editText.InputType = InputTypes.TextVariationPersonName | InputTypes.TextFlagCapSentences; break;
            }
        }

        void SearchView_QueryTextFocusChange(object sender, FocusChangeEventArgs e)
        {
            var element = (BaseStarShot.Controls.SearchBar)Element;
            element.ChangeFocus(e.HasFocus);
        }
    }
}