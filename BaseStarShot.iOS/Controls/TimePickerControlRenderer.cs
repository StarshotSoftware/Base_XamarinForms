using System;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using Xamarin.Forms;
using BaseStarShot.Services;
using BaseStarShot.Controls;
using Foundation;
using ObjCRuntime;
using RectangleF = CoreGraphics.CGRect;
using System.ComponentModel;
using Base1902;

namespace BaseStarShot.Controls
{
    public class TimePickerControlRenderer : ViewRenderer<TimePicker, UITextField>
    {
        UIDatePicker _picker;
        UIColor _defaultTextColor;

        IElementController ElementController => Element as IElementController;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Control.Started -= OnStarted;
                Control.Ended -= OnEnded;

                _picker.ValueChanged -= OnValueChanged;
            }

            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<TimePicker> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    var entry = new NoCaretField { BorderStyle = UITextBorderStyle.RoundedRect };

                    entry.Started += OnStarted;
                    entry.Ended += OnEnded;

                    _picker = new UIDatePicker { Mode = UIDatePickerMode.Time, TimeZone = new NSTimeZone("UTC") };

                    var width = UIScreen.MainScreen.Bounds.Width;
                    var toolbar = new UIToolbar(new RectangleF(0, 0, width, 44)) { BarStyle = UIBarStyle.Default, Translucent = true };
                    var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
                    var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (o, a) => entry.ResignFirstResponder());

                    toolbar.SetItems(new[] { spacer, doneButton }, false);

                    entry.InputView = _picker;
                    entry.InputAccessoryView = toolbar;

                    _defaultTextColor = entry.TextColor;

                    _picker.ValueChanged += OnValueChanged;

                    SetNativeControl(entry);
                }

                Control.InputAccessoryView.TintColor = UIColor.Black;

                UpdateTime();
                UpdateTextColor();

                base.Control.Font = UIFont.FromName(Resolver.Get<IFontService>().GetFontName(Element.FontStyle), (nfloat)Element.FontSize);
                base.Control.TextColor = Element.TextColor.ToUIColor();
                SetBackgroundImage();
                SetTextAlignment();
                SetBgColor();
            }

            base.OnElementChanged(e);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == TimePicker.TimeProperty.PropertyName || e.PropertyName == TimePicker.FormatProperty.PropertyName)
                UpdateTime();

            if (e.PropertyName == TimePicker.TextColorProperty.PropertyName || e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
                UpdateTextColor();
            else if (e.PropertyName == TimePicker.FontStyleProperty.PropertyName)
                Control.Font = UIFont.FromName(Resolver.Get<IFontService>().GetFontName(Element.FontStyle), (nfloat)Element.FontSize);
            else if (e.PropertyName == TimePicker.TextColorProperty.PropertyName)
                base.Control.TextColor = Element.TextColor.ToUIColor();
        }

        void OnEnded(object sender, EventArgs eventArgs)
        {
            ElementController.SetValueFromRenderer(VisualElement.IsFocusedProperty, false);
        }

        void OnStarted(object sender, EventArgs eventArgs)
        {
            ElementController.SetValueFromRenderer(VisualElement.IsFocusedProperty, true);
        }

        void OnValueChanged(object sender, EventArgs e)
        {
            ElementController.SetValueFromRenderer(TimePicker.TimeProperty, _picker.Date.ToDateTime() - new DateTime(1, 1, 1));
        }

        void UpdateTextColor()
        {
            var textColor = Element.TextColor;

            if (textColor == Xamarin.Forms.Color.Default || !Element.IsEnabled)
                Control.TextColor = _defaultTextColor;
            else
                Control.TextColor = textColor.ToUIColor();
        }

        void UpdateTime()
        {
            _picker.Date = new DateTime(1, 1, 1).Add(Element.Time).ToNSDate();
            Control.Text = DateTime.Today.Add(Element.Time).ToString(Element.Format);
        }

        protected virtual void SetBackgroundImage()
        {
            Control.BackgroundColor = Color.Transparent.ToUIColor();
        }

        protected virtual void SetTextAlignment()
        {
            if (Control == null) return;

            switch (Element.XAlign)
            {
                case BaseStarShot.TextAlignment.Center:
                    Control.TextAlignment = UITextAlignment.Center;
                    break;
                case BaseStarShot.TextAlignment.Left:
                    Control.TextAlignment = UITextAlignment.Left;
                    break;
                case BaseStarShot.TextAlignment.Right:
                    Control.TextAlignment = UITextAlignment.Right;
                    break;
            }
        }

        protected virtual void SetBgColor()
        {
            Control.BackgroundColor = Element.BackgroundColor.ToUIColor();

            if (Element.BackgroundColor == Xamarin.Forms.Color.Transparent)
                Control.BorderStyle = UITextBorderStyle.None;

        }
    }

    //public class TimePickerControlRenderer : TimePickerRenderer
    //{
    //	protected override void OnElementChanged (ElementChangedEventArgs<Xamarin.Forms.TimePicker> e)
    //	{
    //		base.OnElementChanged (e);
    //		var baseElement = e.NewElement as BaseStarShot.Controls.TimePicker;

    //		Control.InputAccessoryView.TintColor = UIColor.Black;
    //		if (baseElement != null)
    //		{
    //			baseElement.PropertyChanged += (s, ev) =>
    //				{
    //					var element = (BaseStarShot.Controls.TimePicker)s;
    //					switch (ev.PropertyName)
    //					{
    //						case "FontStyle": base.Control.Font = UIFont.FromName(Resolver.Get<IFontService>().GetFontName(baseElement.FontStyle), (nfloat)baseElement.FontSize); break;
    //						case "TextColor": base.Control.TextColor = element.TextColor.ToUIColor(); break;
    //						case "BackgroundImage": SetBackgroundImage(baseElement); break;
    //					}
    //				};
    //			base.Control.Font = UIFont.FromName(Resolver.Get<IFontService>().GetFontName(baseElement.FontStyle), (nfloat)baseElement.FontSize);
    //			base.Control.TextColor = baseElement.TextColor.ToUIColor();
    //			SetBackgroundImage(baseElement);
    //			SetTextAlignment (baseElement);
    //			SetBgColor (baseElement);
    //		}
    //	}

    //       protected virtual void SetBackgroundImage(BaseStarShot.Controls.TimePicker element)
    //	{
    //		Control.BackgroundColor = Color.Transparent.ToUIColor();
    //	}

    //	protected virtual void SetTextAlignment(BaseStarShot.Controls.TimePicker element)
    //	{
    //		if (Control == null) return;

    //		switch (element.XAlign)
    //		{
    //			case BaseStarShot.TextAlignment.Center:
    //				Control.TextAlignment = UITextAlignment.Center;
    //				break;
    //			case BaseStarShot.TextAlignment.Left:
    //				Control.TextAlignment = UITextAlignment.Left;
    //				break;
    //			case BaseStarShot.TextAlignment.Right:
    //				Control.TextAlignment = UITextAlignment.Right;
    //				break;
    //		}
    //	}

    //	protected virtual void SetBgColor (BaseStarShot.Controls.TimePicker element)
    //	{
    //		Control.BackgroundColor = element.BackgroundColor.ToUIColor();

    //		if (element.BackgroundColor == Xamarin.Forms.Color.Transparent)
    //			Control.BorderStyle = UITextBorderStyle.None;

    //	}
    //}
}

