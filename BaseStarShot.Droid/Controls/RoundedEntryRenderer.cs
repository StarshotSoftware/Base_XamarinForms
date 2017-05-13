using Android.Content;
using Android.Graphics.Drawables;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using BaseStarShot.Controls;
using BaseStarShot.Util;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(RoundedEntry), typeof(RoundedEntryRenderer))]

namespace BaseStarShot.Controls
{
    public class RoundedEntryRenderer : ViewRenderer<RoundedEntry, EditText>, TextView.IOnEditorActionListener
    {
        public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            if (Element.NextElement == null && (actionId == ImeAction.Done || actionId == ImeAction.Next))
            {
                UIHelper.CloseSoftKeyboard(Control);

                if (Element.Command != null && Element.Command.CanExecute(Element.CommandParameter))
                {
                    Element.Command.Execute(Element.CommandParameter);
                }
                return false;
            }

            if (actionId == ImeAction.Done || actionId == ImeAction.Next)
            {
                Element.NextElement.Focus();
                UIHelper.OpenSoftKeyboard(Control);
                return true;
            }

            return false;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<RoundedEntry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;

            var editText = new EditText(this.Context);

            // will remove cursor and not scrollable for tablet
            //editText.MovementMethod = null;
           
            editText.Ellipsize = TextUtils.TruncateAt.End;
            editText.SetCursorVisible(Element.ShowCursor);

            if (!Element.AllowCutCopyPaste)
            {
                editText.LongClickable = false;
            }

            if (Element.MaxCharacter.HasValue)
            {
                var filter = new InputFilterLengthFilter(Element.MaxCharacter.Value);
                editText.SetFilters(new IInputFilter[] { filter });
            }

            SetNativeControl(editText);

            CreateShapeDrawable();
            this.Background = new ColorDrawable(Color.Transparent.ToAndroid());

            //if (Element.IsPassword)
            //    Control.InputType = InputTypes.ClassText | InputTypes.TextVariationPassword;

            if (Element.IsReadOnly)
                Control.KeyListener = null;

            if (Element.IsSingleLine)
            {
                editText.SetMaxLines(1);
                editText.SetSingleLine(true);

                if (Element.NextElement != null)
                    Control.ImeOptions = ImeAction.Next;

                if (Element.NextElement == null && Element.Command != null)
                    Control.ImeOptions = ImeAction.Done;

                if (Element.NextElement != null || (Element.NextElement == null && Element.Command != null))
                    Control.SetOnEditorActionListener(this);
                else
                    Control.ImeOptions = ImeAction.Done;
            }
            

            //int selectionStart = -1;
            //string previousText = "";
            
            editText.AfterTextChanged += (s, ev) =>
            {
                //if (editText.Text.Length > previousText.Length)
                //    selectionStart = editText.CursorPosition + 1;
                //else
                //    selectionStart = editText.CursorPosition - 1;
                ////if (!editText.Text.ToString().Replace(Environment.NewLine, "").Equals(editText.Text.ToString()))
                ////editText.Text = editText.Text.ToString().Replace(Environment.NewLine, "");

                //if (selectionStart > editText.Text.Length || selectionStart < 0)
                //    selectionStart = editText.Text.Length;

                //editText.SetSelection(selectionStart);

                ((IElementController)Element).SetValueFromRenderer(Xamarin.Forms.Entry.TextProperty, editText.Text);
                //previousText = editText.Text;
            };

            Control.SetPadding(UIHelper.ConvertDPToPixels(Element.TextPadding.Left),
                UIHelper.ConvertDPToPixels(Element.TextPadding.Top),
                UIHelper.ConvertDPToPixels(Element.TextPadding.Right),
                UIHelper.ConvertDPToPixels(Element.TextPadding.Bottom));

            SetText();
            SetTextColor();
            SetTextAlignment();
            SetHint();
            SetTextInputTypes();
            ShowKeyboard();
            //CheckEnabled();
        }

        void ShowKeyboard()
        {

            if (Element.ShowKeyboard)
            {
                UIHelper.OpenSoftKeyboard(Control);
            }else
            {
                UIHelper.CloseSoftKeyboard(Control);
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            switch (e.PropertyName)
            {
                case "IsFocused":
                    if (Element.IsFocused)
                    {
                        SetTextAlignment();
                        UIHelper.OpenSoftKeyboard(Control);
                    }
                    break;

                case "FontSize":
                case "Font":
                    SetTypeface();
                    break;

                case "TextColor":
                    SetTextColor();
                    break;

                case "Text":
                    SetText();
                    break;

                case "TextInputType":
                    SetTextInputTypes();
                    break;

                case "Placeholder":
                    SetHint();
                    break;

                case "IsEnabled":
                    //CheckEnabled();
                    break;

                case "ShowKeyboard":
                    ShowKeyboard();
                    break;
            }
        }

        //private void CheckEnabled()
        //{
        //    if (Element.IsEnabled)
        //    {
        //        if (!Element.AllowCutCopyPaste)
        //        {
        //            Control.RequestFocus();
        //            UIHelper.OpenSoftKeyboard(Control);
        //        }

        //    }
        //    else
        //    {
        //        if (!Element.AllowCutCopyPaste)
        //        {
        //            UIHelper.CloseSoftKeyboard(Control);
        //        }
        //  }
        //}
        private void CreateShapeDrawable()
        {
            GradientDrawable shape = new GradientDrawable();
            shape.SetShape(ShapeType.Rectangle);
            shape.SetCornerRadius(UIHelper.ConvertDPToPixels(Element.CornerRadius));

            if (Element.BackgroundColor != Xamarin.Forms.Color.Default)
                shape.SetColor(Element.BackgroundColor.ToAndroid());

            if (Element.ImageRight != null && Element.ImageRight.File != null)
            {
                Drawable drawable = Context.Resources.GetDrawable(UIHelper.GetDrawableResource(Element.ImageRight));
                drawable.SetBounds(0, 0, UIHelper.ConvertDPToPixels(Element.ImageRightWidth),
                    UIHelper.ConvertDPToPixels(Element.ImageRightHeight));
                Control.SetCompoundDrawablesRelative(null, null, drawable, null);
            }
            //if (Element.CornerRadius > 0)
            //    this.SetPadding(5, 2, 5, 2);

            Control.Background = shape;
        }

        private void SetHint()
        {
            Control.Hint = Element.Placeholder;

            if (Element.PlaceholderColor != Color.Default)
                Control.SetHintTextColor(Element.PlaceholderColor.ToAndroid());
        }

        private void SetText()
        {
            var text = Element.Text;
            if ((text+"").Trim() == (Control.Text + "").Trim())
                return;

            if (!string.IsNullOrEmpty(text))
            {
                Control.Text = text;
            }
            else
            {
                Control.Text = String.Empty;

            }

            //((IElementController)Element).SetValueFromRenderer(RoundedEntry.AndroidTextProperty, Control.Text);
        }

        private void SetTextAlignment()
        {
            switch (Element.HorizontalTextAlignment)
            {
                case Xamarin.Forms.TextAlignment.Center: Control.Gravity = GravityFlags.Center; break;
                case Xamarin.Forms.TextAlignment.End: Control.Gravity = GravityFlags.End | GravityFlags.CenterVertical; break;
                case Xamarin.Forms.TextAlignment.Start: Control.Gravity = GravityFlags.Start | GravityFlags.CenterVertical; break;
            }
        }

        private void SetTextColor()
        {
            if (Element.TextColor != Color.Default)
                Control.SetTextColor(Element.TextColor.ToAndroid());
        }

        private void SetTextInputTypes()
        {
            switch (Element.TextInputType)
            {
                case TextInputType.Normal: Control.InputType = InputTypes.TextVariationNormal; break;
                case TextInputType.EmailAddress: Control.InputType = InputTypes.ClassText | InputTypes.TextVariationEmailAddress; break;
                case TextInputType.Password:
                    Control.InputType = InputTypes.TextVariationPassword;
                    //Control.TransformationMethod = new HiddenPasswordTransformationMethod();
                    break;
                case TextInputType.Phone: Control.InputType = InputTypes.ClassPhone; break;
                case TextInputType.Number: Control.InputType = InputTypes.ClassNumber; break;
                case TextInputType.PersonName: Control.InputType = InputTypes.TextVariationPersonName | InputTypes.TextFlagCapSentences; break;
                case TextInputType.Decimal: Control.InputType = InputTypes.NumberFlagDecimal| InputTypes.ClassNumber; break;
            }
            if (Element.IsPassword)
            {
                Control.InputType |= InputTypes.TextVariationPassword;
                Control.TransformationMethod = new HiddenPasswordTransformationMethod();
            }
            else if (Control.TransformationMethod is HiddenPasswordTransformationMethod)
            {
                Control.TransformationMethod = new Android.Text.Method.SingleLineTransformationMethod();
            }
            SetTypeface();
        }

        private void SetTypeface()
        {
            if (!string.IsNullOrEmpty(Element.FontFamily))
                Control.Typeface = FontCache.GetTypeFace(Element.FontFamily);

            Control.TextSize = (int)Math.Round(Element.FontSize);
        }

        private class HiddenPasswordTransformationMethod : Android.Text.Method.PasswordTransformationMethod
        {
            public override Java.Lang.ICharSequence GetTransformationFormatted(Java.Lang.ICharSequence source, Android.Views.View view)
            {
                return new PasswordCharSequence(source);
            }
        }

        private class PasswordCharSequence : Java.Lang.Object, Java.Lang.ICharSequence
        {
            private char DOT = '\u2022';

            private Java.Lang.ICharSequence _source;
            public PasswordCharSequence(Java.Lang.ICharSequence source)
            {
                _source = source;
            }

            public char CharAt(int index)
            {
                return DOT;
            }

            public int Length()
            {
                return _source.Length();
            }

            public Java.Lang.ICharSequence SubSequenceFormatted(int start, int end)
            {
                return _source.SubSequenceFormatted(start, end); // Return default
            }

            public IEnumerator<char> GetEnumerator()
            {
                return _source.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _source.GetEnumerator();
            }
        }
    }
}