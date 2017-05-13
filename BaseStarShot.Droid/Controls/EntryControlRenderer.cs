using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Xamarin.Forms;
using BaseStarShot.Controls;
using System.Threading.Tasks;
using Android.Views.InputMethods;
using Android.Graphics.Drawables;
using Android.Text.Method;
using BaseStarShot.Util;

[assembly: ExportRenderer(typeof(BaseStarShot.Controls.Entry), typeof(EntryControlRenderer))]
namespace BaseStarShot.Controls
{
    public class EntryControlRenderer : EntryRenderer, Android.Widget.TextView.IOnEditorActionListener //ViewRenderer<BaseStarShot.Controls.Entry, EditTextCustom>, 
    {
        bool hasCompoundDrawable;

        private Activity _activity;

        const string PlatformRendererFullName = "Xamarin.Forms.Platform.Android.PlatformRenderer";
        static Android.Graphics.Drawables.Drawable originalBackground;

        protected BaseStarShot.Controls.Entry BaseElement { get { return (BaseStarShot.Controls.Entry)Element; } }

        IKeyListener DefaultKeyListener = null;
        InputMethodManager inputMethodManager;

        public EntryControlRenderer()
        {
            _activity = this.Context as Activity;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Entry> e)
        {
            bool hasSet = false;
            if (e.OldElement == null && this.Element != null)
            {
                //SetNativeControl(new EditTextCustom(Context));
                //SetNativeControl(new EditTextCustom());
                hasSet = true;
            }

            if (inputMethodManager == null)
                inputMethodManager = base.Context.GetSystemService(Context.InputMethodService) as InputMethodManager;

            base.OnElementChanged(e);

            if (!hasSet)
                return;

            var entryControl = (BaseStarShot.Controls.Entry)Element;

            ((IKeyPressHandler)entryControl).SetPerformKeyPress(key =>
                {
                    var code = UIHelper.Convert(key);
                    if (code.HasValue)
                        PerformKeystroke(code.Value);
                });

            DefaultKeyListener = Control.KeyListener;

            entryControl.PropertyChanged += (s, ev) =>
            {
                var element = (BaseStarShot.Controls.Entry)s;

                switch (ev.PropertyName)
                {
                    case "TextPadding":
                    case "ImageLeft":
                    case "ImageRight":
                    case "ImageTop":
                    case "ImageBottom":
                        SetImages(element);
                        break;

                    case "ClearFocusTrigger": HideSoftKeyBoardOnTextChanged(element); break;
                    case "IsFocused": element.ClearFocus = !element.IsFocused; break;
                    case "BackgroundImage": SetBackground(element); break;
                    case "FontFamily": SetTypeface(element); break;
                    case "FontSize": SetTextSize(element); break;
#if XAMARIN_FORMS_1_5
                    case "TextAlignment": SetTextAlignment(element); break;
#endif
                    case "SuppressKeyboard": SetShowSoftInputOnFocus(element); break;
                    case "Text":
                        if (element.ClearFocus)
                            HideSoftKeyBoardOnTextChanged(element);
                        break;
                    case "ClearFocus":
                        //if (!EntryControl.IsNumeric) break;
                        if (element.ClearFocus)
                        {
                            //inputMethodManager.HideSoftInputFromWindow(base.Control.WindowToken, HideSoftInputFlags.None);
                            //Control.Focusable = false;
                            //Control.ClearFocus();
                            HideSoftKeyBoardOnTextChanged(element);
                        }
                        break;
                    case "NextFocus":
                        inputMethodManager.ShowSoftInput(Control, ShowFlags.Implicit);
                        //inputMethodManager.ToggleSoftInput(ShowFlags.Forced, InputMethodManager.ShowImplicit);
                        break;
                    case "IsEnabled":
                        if (base.Control == null) return;

                        if (!element.IsEnabled)
                            base.Control.KeyListener = null;
                        else
                            base.Control.KeyListener = DefaultKeyListener;
                        break;
					case "IsReadOnly":
						SetIsReadOnly(element);
						break;

                    //case "Padding": SetPadding(element); break;
                }
            };

            //if (EntryControl.IsNumeric)
            //    Control.Touch += Control_Touch;

            SetBackground(entryControl);
            SetImages(entryControl);
            SetTypeface(entryControl);
            SetTextSize(entryControl);
#if XAMARIN_FORMS_1_5
            SetTextAlignment(entryControl);
#endif
            SetShowSoftInputOnFocus(entryControl);
            SetHintColor(entryControl);
			SetIsReadOnly(entryControl);
            //SetPadding(entryControl);

            if (BaseElement.NextElement != null)
                Control.ImeOptions = ImeAction.Next;
            if (BaseElement.NextElement == null && BaseElement.Command != null)
                Control.ImeOptions = ImeAction.Done;


            Control.ViewAttachedToWindow += Control_ViewAttachedToWindow;
            Control.SetPadding(UIHelper.ConvertDPToPixels(BaseElement.Padding.Left),
                UIHelper.ConvertDPToPixels(BaseElement.Padding.Top),
                UIHelper.ConvertDPToPixels(BaseElement.Padding.Right),
                UIHelper.ConvertDPToPixels(BaseElement.Padding.Bottom));

            if (BaseElement.NextElement != null || BaseElement.Command != null)
                Control.SetOnEditorActionListener(this);

            SetTextInputTypes(entryControl);

            if (!entryControl.IsEnabled)
                base.Control.KeyListener = null;

            base.Control.Focusable = BaseElement.IsEnabled;

            //Control.Touch += Control_Touch;
            //Control.FocusChange += Control_FocusChange;
            if (BaseElement.IsSingleLine)
            {
                Control.SetMaxLines(1);
                Control.SetSingleLine(true);
                Control.Ellipsize = TextUtils.TruncateAt.End;

            }

            if (entryControl.MaxCharacter.HasValue)
            {
                var filter = new InputFilterLengthFilter(entryControl.MaxCharacter.Value);
                Control.SetFilters(new IInputFilter[] { filter });
            }
            Control.Background.SetColorFilter(Android.Graphics.Color.Transparent, PorterDuff.Mode.SrcIn);
        }

        public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            if (BaseElement.NextElement == null && (actionId == ImeAction.Done || actionId == ImeAction.Next))
            {
                UIHelper.CloseSoftKeyboard(Control);

                if (BaseElement.Command != null && BaseElement.Command.CanExecute(null))
                {
                    System.Windows.Input.ICommand cmd = BaseElement.Command;
                    cmd.Execute(null);
                }
                return false;
            }

            if (actionId == ImeAction.Done || actionId == ImeAction.Next)
            {
                BaseElement.NextElement.Focus();
                UIHelper.OpenSoftKeyboard(Control);
                return true;
            }

           

            return false;
        }

        void SetHintColor(BaseStarShot.Controls.Entry element)
        {
            //base.Control.SetHintTextColor(element.HintColor.ToAndroid ());
        }

		void SetIsReadOnly(BaseStarShot.Controls.Entry element)
		{
			if (element.IsReadOnly)
				base.Control.KeyListener = null;
			else
				base.Control.KeyListener = DefaultKeyListener;
		}

        void SetTextInputTypes(BaseStarShot.Controls.Entry element)
        {
            switch (element.TextInputType)
            {
                case TextInputType.Normal: Control.InputType = InputTypes.TextVariationNormal; break;
                case TextInputType.EmailAddress: Control.InputType = InputTypes.ClassText | InputTypes.TextVariationEmailAddress; break;
                case TextInputType.Password: Control.InputType = InputTypes.TextVariationPassword; break;
                case TextInputType.Phone: Control.InputType = InputTypes.ClassPhone; break;
                case TextInputType.Number: Control.InputType = InputTypes.ClassNumber; break;
                case TextInputType.PersonName: Control.InputType = InputTypes.TextVariationPersonName | InputTypes.TextFlagCapSentences; break;
            }
        }

        //void Control_Touch(object sender, Android.Views.View.TouchEventArgs e)
        //{
        //    Control.Focusable = true;
        //    Control.RequestFocus();
        //    Control.RequestFocusFromTouch();

        //    Control.SetSelection(Control.Text.Length);

        //    InputMethodManager inputMethodManager = this.Context.GetSystemService(Context.InputMethodService) as InputMethodManager;
        //    inputMethodManager.ShowSoftInput(Control, ShowFlags.Forced);
        //    inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
        //}

        //override appl
        void HideSoftKeyBoardOnTextChanged(BaseStarShot.Controls.Entry element)
        {
            if (element.IsFocused && BaseElement.SuppressKeyboard)
            {
                UIHelper.CloseSoftKeyboard(Control);
            }
        }

        void SetShowSoftInputOnFocus(BaseStarShot.Controls.Entry element)
        {
            Control.ShowSoftInputOnFocus = !BaseElement.SuppressKeyboard;
        }

        protected override void OnFocusChanged(bool gainFocus, FocusSearchDirection direction, Android.Graphics.Rect previouslyFocusedRect)
        {
            base.OnFocusChanged(gainFocus, direction, previouslyFocusedRect);

            if (gainFocus && BaseElement.SuppressKeyboard)
            {
                UIHelper.CloseSoftKeyboard(Control);
            }
        }

        //void Control_FocusChange(object sender, Android.Views.View.FocusChangeEventArgs e)
        //{
        //    if (e.HasFocus && EntryControl.SuppressKeyboard)
        //    {
        //        UIHelper.CloseSoftKeyboard(Control);
        //    }
        //}

        //void Control_Touch(object sender, Android.Views.View.TouchEventArgs e)
        //{
        //    Control.OnTouchEvent(e.Event);

        //    if (EntryControl.SuppressKeyboard)
        //    {
        //        UIHelper.CloseSoftKeyboard(Control);
        //    }
        //}

        async void Control_ViewAttachedToWindow(object sender, Android.Views.View.ViewAttachedToWindowEventArgs e)
        {
            if (BaseElement.SuppressKeyboard)
            {
                var activity = ((Activity)Context);

                var viewGroup = (ViewGroup)activity.Window.DecorView;
                UnfocusPlatformRenderer(viewGroup);

                await Task.Delay(100);
                UIHelper.CloseSoftKeyboard(Control);
            }
        }

        void PerformKeystroke(Keycode keyCode)
        {
            KeyEvent evt = new KeyEvent(KeyEventActions.Down, keyCode);
            Control.DispatchKeyEvent(evt);
        }

        protected void UnfocusPlatformRenderer(ViewGroup viewGroup)
        {
            for (int i = 0; i < viewGroup.ChildCount; i++)
            {
                var vg = viewGroup.GetChildAt(i) as ViewGroup;
                if (vg != null)
                {
                    if (vg.GetType().FullName == PlatformRendererFullName)
                    {
                        vg.Focusable = false;
                        vg.FocusableInTouchMode = false;
                        return;
                    }
                    else
                        UnfocusPlatformRenderer(vg);
                }
            }
        }

#if XAMARIN_FORMS_1_5
        void SetTextAlignment(BaseStarShot.Controls.Entry element)
        {
            switch (element.TextAlignment)
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
#endif

        void SetBackground(BaseStarShot.Controls.Entry element)
        {
            if (Control == null) return;

            bool hasSetOriginal = false;
            if (originalBackground == null)
            {
                originalBackground = Control.Background;
                hasSetOriginal = true;
            }

            if (element.BackgroundImage != null)
            {
                var resourceId = UIHelper.GetDrawableResource(element.BackgroundImage);
                if (resourceId > 0)
                {
                    Control.SetBackgroundResource(resourceId);
                }
                else
                {
                    if (!hasSetOriginal)
                        Control.Background = originalBackground;
                }
            }
            else
            {
                if (!hasSetOriginal)
                    Control.Background = originalBackground;
            }
        }

        void SetImages(BaseStarShot.Controls.Entry element)
        {
            int leftResourceId = 0, topResourceId = 0, rightResourceId = 0, bottomResourceId = 0;

            if (element.ImageLeft != null && element.ImageLeft.File != null)
                leftResourceId = UIHelper.GetDrawableResource(element.ImageLeft);
            if (element.ImageTop != null && element.ImageTop.File != null)
                topResourceId = UIHelper.GetDrawableResource(element.ImageTop);
            if (element.ImageRight != null && element.ImageRight.File != null)
                rightResourceId = UIHelper.GetDrawableResource(element.ImageRight);
            if (element.ImageBottom != null && element.ImageBottom.File != null)
                bottomResourceId = UIHelper.GetDrawableResource(element.ImageBottom);

            bool hasResource = leftResourceId > 0 || rightResourceId > 0 || topResourceId > 0 || bottomResourceId > 0;
            if (hasCompoundDrawable || hasResource)
            {
                hasCompoundDrawable = true;

                //Android.Graphics.Drawables.Drawable leftDrawable = leftResourceId > 0 ? Resources.GetDrawable(leftResourceId) : null;
                //if (leftDrawable != null)
                //    leftDrawable.SetBounds(0, 0, leftDrawable.IntrinsicWidth, leftDrawable.IntrinsicHeight);
                //Android.Graphics.Drawables.Drawable topDrawable = topResourceId > 0 ? Resources.GetDrawable(topResourceId) : null;
                //if (topDrawable != null)
                //    topDrawable.SetBounds(0, 0, topDrawable.IntrinsicWidth, topDrawable.IntrinsicHeight);
                //Android.Graphics.Drawables.Drawable rightDrawable = rightResourceId > 0 ? Resources.GetDrawable(rightResourceId) : null;
                //if (rightDrawable != null)
                //    rightDrawable.SetBounds(0, 0, rightDrawable.IntrinsicWidth, rightDrawable.IntrinsicHeight);
                //Android.Graphics.Drawables.Drawable bottomDrawable = bottomResourceId > 0 ? Resources.GetDrawable(bottomResourceId) : null;
                //if (bottomDrawable != null)
                //    bottomDrawable.SetBounds(0, 0, bottomDrawable.IntrinsicWidth, bottomDrawable.IntrinsicHeight);

                //Control.SetCompoundDrawablesRelative(leftDrawable, topDrawable, rightDrawable, bottomDrawable);
                //Control.SetCompoundDrawables(leftDrawable, topDrawable, rightDrawable, bottomDrawable);


                var leftDrawable = (leftResourceId > 0) ? Resources.GetDrawable(leftResourceId) : null;
                if (leftDrawable != null)
                {
                    if (element.ImageLeftWidth > 0)
                    {
                        leftDrawable = ResizeImage(leftDrawable, UIHelper.ConvertDPToPixels(element.ImageLeftWidth), UIHelper.ConvertDPToPixels(element.ImageLeftWidth));

                    }
                    else
                    {
                        Resources.GetDrawable(leftResourceId);
                    }
                }

                var topDrawable = (topResourceId > 0) ? Resources.GetDrawable(topResourceId) : null;
                var rightDrawable = (rightResourceId > 0) ? Resources.GetDrawable(rightResourceId) : null;
                var bottomDrawable = (bottomResourceId > 0) ? Resources.GetDrawable(bottomResourceId) : null;

                //Control.SetCompoundDrawablesRelativeWithIntrinsicBounds(leftDrawable, topResourceId, rightResourceId, bottomResourceId);
                Control.SetCompoundDrawablesRelativeWithIntrinsicBounds(leftDrawable, topDrawable, rightDrawable, bottomDrawable);
                Control.CompoundDrawablePadding = 20;

                if (!hasResource)
                    hasCompoundDrawable = false;
            }

            if (element.ImageCenter != null)
            {
                Control.SetCompoundDrawablesRelativeWithIntrinsicBounds(base.Resources.GetDrawable(UIHelper.GetDrawableResource(element.ImageCenter)), null, null, null);
                Control.Gravity = GravityFlags.Center;
            }
        }

        private Drawable ResizeImage(Drawable image, int width, int height)
        {
            Bitmap b = ((BitmapDrawable)image).Bitmap;
            Bitmap bitmapResized = Bitmap.CreateScaledBitmap(b, width, height, false);
            return new BitmapDrawable(bitmapResized);
        }

        void SetTypeface(BaseStarShot.Controls.Entry element)
        {
            if (!string.IsNullOrEmpty(element.FontFamily))
                 Control.Typeface = FontCache.GetTypeFace(element.FontFamily);
        }

        void SetTextSize(BaseStarShot.Controls.Entry element)
        {
            Control.TextSize = (float)element.FontSize;
        }

        //void SetPadding(BaseStarShot.Controls.Entry element)
        //{
        //    Control.SetPadding(UIHelper.ConvertDPToPixels(element.Padding.Left),
        //        UIHelper.ConvertDPToPixels(element.Padding.Top),
        //        UIHelper.ConvertDPToPixels(element.Padding.Right),
        //        UIHelper.ConvertDPToPixels(element.Padding.Bottom));
        //}

    }
}