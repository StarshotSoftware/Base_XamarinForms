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
using Android.Util;
using Xamarin.Forms;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.Android;

namespace BaseStarShot.Controls
{
    public class EditTextCustom : EditText
    {
        const string PlatformRendererFullName = "Xamarin.Forms.Platform.Android.PlatformRenderer";

        private bool isTextEditor = true;
        private string oldText;

        private bool _suppressKeyboard;
        public bool SuppressKeyboard
        {
            get { return _suppressKeyboard; }
            set
            {
                if (_suppressKeyboard != value)
                {
                    _suppressKeyboard = value;
                }
            }
        }

        public new event EventHandler<TextChangedEventArgs> TextChanged;
        public event EventHandler Completed;

        #region Constructors
        public EditTextCustom(Context context)
            : base(context)
        {

        }

        public EditTextCustom(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {

        }

        protected EditTextCustom(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {

        }

        public EditTextCustom(Context context, IAttributeSet attrs, int defStyleAttr)
            : base(context, attrs, defStyleAttr)
        {

        }

        public EditTextCustom(Context context, IAttributeSet attrs, int defStyleAttr, int defStyleRes)
            : base(context, attrs, defStyleAttr)
        {

        }
        #endregion

        public void InsertText(string text)
        {
            if (SelectionStart == SelectionEnd)
                EditableText.Insert(SelectionStart, new Java.Lang.String(text));
            else
                EditableText.Replace(SelectionStart, SelectionEnd, new Java.Lang.String(text));
        }

        public void PerformKeystroke(Keycode keyCode)
        {
            KeyEvent evt = new KeyEvent(KeyEventActions.Down, keyCode);
            DispatchKeyEvent(evt);
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

        protected async override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();

            if (SuppressKeyboard)
            {
                var activity = ((Activity)Context);

                var viewGroup = (ViewGroup)activity.Window.DecorView;
                UnfocusPlatformRenderer(viewGroup);

                await Task.Delay(100);
                UIHelper.CloseSoftKeyboard(this);
            }
        }

        protected override void OnFocusChanged(bool gainFocus, FocusSearchDirection direction, Android.Graphics.Rect previouslyFocusedRect)
        {
            base.OnFocusChanged(gainFocus, direction, previouslyFocusedRect);
        }

        protected override void OnTextChanged(Java.Lang.ICharSequence text, int start, int lengthBefore, int lengthAfter)
        {
            base.OnTextChanged(text, start, lengthBefore, lengthAfter);

            var evt = TextChanged;
            if (evt != null && !string.Equals(oldText, this.Text))
                evt(this, new TextChangedEventArgs(oldText, this.Text));
            oldText = this.Text;
        }

        public override bool OnKeyPreIme(Keycode keyCode, KeyEvent e)
        {
            //if (e.KeyCode == Keycode.Back && e.Action == KeyEventActions.Up)
            //{
            //    this.ClearFocus();
            //    UIHelper.CloseSoftKeyboard(this);
            //    return false;
            //}

            return base.OnKeyPreIme(keyCode, e);
        }

        public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
        {
            //if (keyCode == Keycode.Enter)
            //{
            //    var evt = Completed;
            //    if (evt != null)
            //        evt(this, EventArgs.Empty);
            //    this.ClearFocus();
            //    UIHelper.CloseSoftKeyboard(this);
            //    return false;
            //}

            return base.OnKeyDown(keyCode, e);
        }

        public override bool OnCheckIsTextEditor()
        {
            //if (SuppressKeyboard)
            //    return isTextEditor;
            return base.OnCheckIsTextEditor();
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            //if (SuppressKeyboard)
            //    isTextEditor = false;
            var returnValue = base.OnTouchEvent(e);
            if (SuppressKeyboard)
            {
                //isTextEditor = true;
                UIHelper.CloseSoftKeyboard(this);
            }
            return returnValue;
        }
    }
}