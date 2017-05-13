using System;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using BaseStarShot.Services;
using Xamarin.Forms;
using System.Diagnostics;
using Foundation;
using ObjCRuntime;
using RectangleF = CoreGraphics.CGRect;
using System.ComponentModel;
using System.Reflection;
using Base1902;

namespace  BaseStarShot.Controls
{
    internal class NoCaretField : UITextField
    {
        public NoCaretField() : base(new RectangleF())
        {
        }

        public override bool CanPerform(Selector action, NSObject withSender)
        {
            return false;
        }

        public override RectangleF GetCaretRectForPosition(UITextPosition position)
        {
            return new RectangleF();
        }
    }

    public class DatePickerControlRenderer : ViewRenderer<DatePicker, UITextField>
    {
        UIDatePicker _picker;
        UIColor _defaultTextColor;

        IElementController ElementController => Element as IElementController;

        protected override void OnElementChanged(ElementChangedEventArgs<DatePicker> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement == null)
            {
                var entry = new NoCaretField { BorderStyle = UITextBorderStyle.RoundedRect };

                entry.Started += OnStarted;
                entry.Ended += OnEnded;

                _picker = new UIDatePicker { Mode = UIDatePickerMode.Date, TimeZone = new NSTimeZone("UTC") };

                _picker.ValueChanged += HandleValueChanged;

                var width = UIScreen.MainScreen.Bounds.Width;
                var toolbar = new UIToolbar(new RectangleF(0, 0, width, 44)) { BarStyle = UIBarStyle.Default, Translucent = true };
                var spacer = new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace);
                var doneButton = new UIBarButtonItem(UIBarButtonSystemItem.Done, (o, a) => entry.ResignFirstResponder());

                toolbar.SetItems(new[] { spacer, doneButton }, false);

                entry.InputView = _picker;
                entry.InputAccessoryView = toolbar;

                _defaultTextColor = entry.TextColor;

                SetNativeControl(entry);
            }

            if (e.NewElement != null)
            {
                UpdateDateFromModel(false);
                UpdateMaximumDate();
                UpdateMinimumDate();
                UpdateTextColor();

                Control.InputAccessoryView.TintColor = UIColor.Black;
                Control.Font = UIFont.FromName(Resolver.Get<IFontService>().GetFontName(Element.FontStyle), (nfloat)Element.FontSize);
                Control.TextColor = Element.TextColor.ToUIColor();
                Control.Text = Element.Date.ToString(Element.Format);

                SetBackgroundImage();
                SetTextAlignment();
                SetBgColor();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == DatePicker.DateProperty.PropertyName || e.PropertyName == DatePicker.FormatProperty.PropertyName)
                UpdateDateFromModel(true);
            else if (e.PropertyName == DatePicker.MinimumDateProperty.PropertyName)
                UpdateMinimumDate();
            else if (e.PropertyName == DatePicker.MaximumDateProperty.PropertyName)
                UpdateMaximumDate();
            else if (e.PropertyName == DatePicker.TextColorProperty.PropertyName || e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
                UpdateTextColor();
            else if (e.PropertyName == DatePicker.DateProperty.PropertyName)
                Control.Text = Element.Date.ToString(Element.Format);
            else if (e.PropertyName == DatePicker.TriggerShowDatePickerProperty.PropertyName)
            {
                if (Control != null)
                {
                    Control.BecomeFirstResponder();
                }
            }
            else if (e.PropertyName == DatePicker.FontSizeProperty.PropertyName || e.PropertyName == DatePicker.FontStyleProperty.PropertyName)
                base.Control.Font = UIFont.FromName(Resolver.Get<IFontService>().GetFontName(Element.FontStyle), (nfloat)Element.FontSize);
        }

        void HandleValueChanged(object sender, EventArgs e)
        {
            ElementController?.SetValueFromRenderer(DatePicker.DateProperty, _picker.Date.ToDateTime().Date);
        }

        void OnEnded(object sender, EventArgs eventArgs)
        {
            ElementController.SetValueFromRenderer(VisualElement.IsFocusedProperty, false);
        }

        void OnStarted(object sender, EventArgs eventArgs)
        {
            ElementController.SetValueFromRenderer(VisualElement.IsFocusedProperty, true);
        }

        void UpdateDateFromModel(bool animate)
        {
            if (_picker.Date.ToDateTime().Date != Element.Date.Date)
                _picker.SetDate(Element.Date.ToNSDate(), animate);

            Control.Text = Element.Date.ToString(Element.Format);
        }

        void UpdateMaximumDate()
        {
            _picker.MaximumDate = Element.MaximumDate.ToNSDate();
        }

        void UpdateMinimumDate()
        {
            _picker.MinimumDate = Element.MinimumDate.ToNSDate();
        }

        void UpdateTextColor()
        {
            var textColor = Element.TextColor;

            if (textColor == Xamarin.Forms.Color.Default || !Element.IsEnabled)
                Control.TextColor = _defaultTextColor;
            else
                Control.TextColor = textColor.ToUIColor();
        }
        
        protected virtual void SetBackgroundImage()
        {
            if (Element.BackgroundImage != null && !string.IsNullOrEmpty(Element.BackgroundImage.File))
            {
                //	var image = UIImage.FromBundle(element.BackgroundImage.File);
                //if (image == null)
                //	throw new ArgumentException("Cannot find image '" + element.BackgroundImage.File + "'.");
                Control.BackgroundColor = Color.Transparent.ToUIColor();
                //Control.BorderStyle = UITextBorderStyle.Line;
                Layer.CornerRadius = 7;
                Layer.BorderWidth = 1;
                Layer.BorderColor = UIColor.White.CGColor;
                //Control.SetBackgroundImage(image, UIControlState.Normal);
            }
            else
            {

                Control.BackgroundColor = Color.Transparent.ToUIColor();
            }
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

    //   public class DatePickerControlRenderer: Xamarin.Forms.Platform.iOS.DatePickerRenderer
    //{
    //       protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.DatePicker> e)
    //	{
    //		base.OnElementChanged(e);

    //		var baseElement = e.NewElement as BaseStarShot.Controls.DatePicker;

    //		Control.InputAccessoryView.TintColor = UIColor.Black;


    //		if (baseElement != null)
    //		{
    //			baseElement.PropertyChanged += (s, ev) =>
    //			{
    //                   if (Control == null || Element == null)
    //                       return;

    //				var element = (BaseStarShot.Controls.DatePicker)s;
    //				switch (ev.PropertyName)
    //				{
    //					case "FontStyle": base.Control.Font = UIFont.FromName(Resolver.Get<IFontService>().GetFontName(baseElement.FontStyle), (nfloat)baseElement.FontSize); break;
    //					case "TextColor": base.Control.TextColor = element.TextColor.ToUIColor(); break;
    //					case "BackgroundImage": SetBackgroundImage(baseElement); break;
    //				}
    //			};
    //			base.Control.Font = UIFont.FromName(Resolver.Get<IFontService>().GetFontName(baseElement.FontStyle), (nfloat)baseElement.FontSize);
    //			base.Control.TextColor = baseElement.TextColor.ToUIColor();
    //			SetBackgroundImage(baseElement);
    //			SetTextAlignment (baseElement);
    //			SetBgColor (baseElement);
    //			baseElement.PropertyChanged += baseElement_PropertyChanged;
    //		}
    //	}

    //       public override bool CanPerform(Selector action, NSObject withSender)
    //       {
    //           if (action == new ObjCRuntime.Selector("paste:"))
    //               return false;
    //           else if (action == new ObjCRuntime.Selector("select:"))
    //               return false;
    //           else if (action == new ObjCRuntime.Selector("selectAll:"))
    //               return false;
    //           else
    //               return base.CanPerform(action, withSender);
    //       }

    //       void baseElement_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    //	{
    //		var baseElement = Element as BaseStarShot.Controls.DatePicker;;
    //		switch (e.PropertyName)
    //		{
    //			case "Date":
    //				Control.Text = baseElement.Date.ToString ("d");
    //			break;
    //			case "TriggerShowDatePicker":
    //			if (Control != null)
    //				Control.BecomeFirstResponder ();
    //				//Control.Raise("Click", EventArgs.Empty);
    //				break;
    //			case "FontSize":
    //				base.Control.Font = UIFont.FromName(Resolver.Get<IFontService>().GetFontName(baseElement.FontStyle), (nfloat)baseElement.FontSize);
    //				break;
    //		}
    //	}

    //	protected virtual void SetBackgroundImage(BaseStarShot.Controls.DatePicker element)
    //	{
    //		if (element.BackgroundImage != null && !string.IsNullOrEmpty(element.BackgroundImage.File))
    //		{
    //		//	var image = UIImage.FromBundle(element.BackgroundImage.File);
    //			//if (image == null)
    //			//	throw new ArgumentException("Cannot find image '" + element.BackgroundImage.File + "'.");
    //			Control.BackgroundColor = Color.Transparent.ToUIColor();
    //			//Control.BorderStyle = UITextBorderStyle.Line;
    //			Layer.CornerRadius = 7;
    //			Layer.BorderWidth = 1;
    //			Layer.BorderColor = UIColor.White.CGColor;
    //			//Control.SetBackgroundImage(image, UIControlState.Normal);
    //		}
    //		else
    //		{

    //			Control.BackgroundColor = Color.Transparent.ToUIColor();
    //		}
    //	}

    //	protected virtual void SetTextAlignment(BaseStarShot.Controls.DatePicker element)
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

    //	protected virtual void SetBgColor (BaseStarShot.Controls.DatePicker element)
    //	{
    //			Control.BackgroundColor = element.BackgroundColor.ToUIColor();

    //		if (element.BackgroundColor == Xamarin.Forms.Color.Transparent)
    //			Control.BorderStyle = UITextBorderStyle.None;

    //	}
    //}
}

