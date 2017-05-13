using Android.Text;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using BaseStarShot.Controls;
using Android.App;
using System.Threading.Tasks;
using BaseStarShot.Util;
using Android.Text.Method;

[assembly: ExportRenderer(typeof(BaseStarShot.Controls.Editor), typeof(EditorControlRenderer))]
namespace BaseStarShot.Controls
{
    public class EditorControlRenderer : EditorRenderer //ViewRenderer<BaseStarShot.Controls.Entry, EditTextCustom>, 
    {
        const string PlatformRendererFullName = "Xamarin.Forms.Platform.Android.PlatformRenderer";
        static Android.Graphics.Drawables.Drawable originalBackground;

		IKeyListener DefaultKeyListener = null;

        protected Editor BaseElement { get { return (Editor)Element; } }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Editor> e)
        {
            bool hasSet = false;
            if (e.OldElement == null && this.Element != null)
            {
                //SetNativeControl(new EditTextCustom(Context));
                //SetNativeControl(new EditTextCustom());
                hasSet = true;
            }

            base.OnElementChanged(e);

            if (!hasSet)
                return;

            if (Control != null)
            {
				DefaultKeyListener = Control.KeyListener;

                // do whatever you want to the textField here!
                Control.SetBackgroundColor(global::Android.Graphics.Color.Transparent);

                if (BaseElement.BackgroundImage != null && BaseElement.BackgroundImage.File != null)
                {
                    var resourceId = UIHelper.GetDrawableResource(BaseElement.BackgroundImage);
                    Control.SetBackgroundResource(resourceId);
                }
                
                Control.Hint = BaseElement.Placeholder;

                if (BaseElement.MaxCharacter.HasValue)
                    Control.SetFilters(new IInputFilter[] { new InputFilterLengthFilter(BaseElement.MaxCharacter.Value) });

            }
            var editorControl = (Editor)Element;

            ((IKeyPressHandler)editorControl).SetPerformKeyPress(key =>
            {
                var code = UIHelper.Convert(key);
                if (code.HasValue)
                    PerformKeystroke(code.Value);
            });

            editorControl.PropertyChanged += (s, ev) =>
            {
                if (Element == null || Control == null)
                    return;
                
                var element = (Editor)s;

                switch (ev.PropertyName)
                {
                    case "BackgroundImage": SetBackground(element); break;
                    case "FontFamily": SetTypeface(element); break;
                    case "FontSize": SetTextSize(element); break;
                    case "TextAlignment": SetTextAlignment(element); break;
                    case "SuppressKeyboard": SetShowSoftInputOnFocus(element); break;
                    case "Text": HideSoftKeyBoardOnTextChanged(element); break;
					case "IsReadOnly": SetIsReadOnly(element); break;
                }
            };

            //Control.TextChanged += (s, ev) => Element.Raise("TextChanged", ev);
            //Control.Completed += (s, ev) => Element.Raise("Completed", ev);
            Control.SetPadding(5, 5, 5, 5);
            SetBackground(editorControl);
            SetTypeface(editorControl);
            SetTextSize(editorControl);
            SetTextAlignment(editorControl);
            SetShowSoftInputOnFocus(editorControl);
			SetIsReadOnly(editorControl);

            Control.ViewAttachedToWindow += Control_ViewAttachedToWindow;
            //Control.Touch += Control_Touch;
            //Control.FocusChange += Control_FocusChange;
        }
        
        void HideSoftKeyBoardOnTextChanged(BaseStarShot.Controls.Editor element)
        {
            if (element.IsFocused && BaseElement.SuppressKeyboard)
            {
                UIHelper.CloseSoftKeyboard(Control);
            }

            //if (string.IsNullOrWhiteSpace(Element.Text))
            //    Control.Text = "";
            //else
            //    Control.Text = element.Text;
            
        }

        void SetShowSoftInputOnFocus(BaseStarShot.Controls.Editor element)
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

        void Control_FocusChange(object sender, Android.Views.View.FocusChangeEventArgs e)
        {
            if (e.HasFocus && BaseElement.SuppressKeyboard)
            {
                UIHelper.CloseSoftKeyboard(Control);
            }
        }

        async void Control_ViewAttachedToWindow(object sender, ViewAttachedToWindowEventArgs e)
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

        void SetTextAlignment(BaseStarShot.Controls.Editor element)
        {
            switch (element.TextAlignment)
            {
                case BaseStarShot.TextAlignment.Justified:
                case BaseStarShot.TextAlignment.Left:
                    Control.Gravity = GravityFlags.AxisPullBefore | GravityFlags.AxisSpecified | GravityFlags.Top | GravityFlags.RelativeLayoutDirection; break;
                case BaseStarShot.TextAlignment.Center:
                    Control.Gravity = GravityFlags.CenterHorizontal | GravityFlags.Top; break;
                case BaseStarShot.TextAlignment.Right:
                    Control.Gravity = GravityFlags.Top | GravityFlags.Right; break;
            }
        }

        void SetBackground(BaseStarShot.Controls.Editor element)
        {
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
                        Control.SetBackgroundDrawable(originalBackground);
                }
            }
            else
            {
                if (!hasSetOriginal)
                    Control.SetBackgroundDrawable(originalBackground);
            }
        }

        void SetTypeface(BaseStarShot.Controls.Editor element)
        {
            if (!string.IsNullOrEmpty(element.FontFamily))
                Control.Typeface = FontCache.GetTypeFace(element.FontFamily);
        }

        void SetTextSize(BaseStarShot.Controls.Editor element)
        {
            Control.TextSize = (float)element.FontSize;
        }
        
		void SetIsReadOnly(BaseStarShot.Controls.Editor element)
		{
			if (element.IsReadOnly)
				base.Control.KeyListener = null;
			else
				base.Control.KeyListener = DefaultKeyListener;
		}
    }
}