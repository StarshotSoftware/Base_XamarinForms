using System;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using CoreGraphics;
using Foundation;
using System.Xml.Linq;
using System.Text;
using BaseStarShot.Services;
using System.Collections;
using System.Collections.Generic;
using Xamarin.Forms;
using Base1902;

namespace BaseStarShot.Controls
{
	public class FormattedTextLabelRenderer : Xamarin.Forms.Platform.iOS.LabelRenderer
    {
        private const string ElementA = "A";
        private const string ElementB = "B";
        private const string ElementBr = "BR";
        private const string ElementEm = "EM";
        private const string ElementI = "I";
        private const string ElementP = "P";
        private const string ElementStrong = "STRONG";
        private const string ElementU = "U";
        private const string ElementUl = "UL";
        private const string ElementLi = "LI";
        private const string ElementDiv = "DIV";
        private const string ElementH = "H";

        //Attributes
        private const string FontSize = "FontSize";
        private const string FontColor = "FontColor";

        Page page;
        NavigationPage navigPage;

	    FormattedTextLabel baseElement { get { return Element as FormattedTextLabel; } }

		protected override void OnElementChanged (ElementChangedEventArgs<Xamarin.Forms.Label> e)
		{
			base.OnElementChanged (e);
//			if (e.NewElement != null)
//			{
//				AttachPageEvents(e.NewElement);
//			}
			if (e.NewElement == null) return;

			if (baseElement != null) {
				Control.BackgroundColor = UIColor.Clear;
				Control.Frame = new CGRect (0, 0, baseElement.Width, baseElement.HeightRequest);
				Control.Lines = baseElement.MaxLines;
				Control.TextColor = baseElement.DefaultTextColor.ToUIColor ();
				var fontName = Resolver.Get<IFontService> ().GetFontName (FontStyle.Regular);
				if (!string.IsNullOrEmpty (fontName))
					Control.Font = UIFont.FromName (Resolver.Get<IFontService> ().GetFontName (FontStyle.Regular), (nfloat)baseElement.DefaultFontSize);
				else
					Control.Font = UIFont.SystemFontOfSize ((nfloat)baseElement.DefaultFontSize);
				baseElement.PropertyChanged += Element_PropertyChanged;
			}
			UpdateText();
		}

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        void Element_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
				case "HTMLString": UpdateText(); 
					Control.Frame = new CGRect (0, 0, baseElement.Width, baseElement.Height); break;
					Control.Lines = baseElement.MaxLines;
				case "Width":
					Control.Frame = new CGRect (0, 0, baseElement.Width, baseElement.Height); break;
					Control.Lines = baseElement.MaxLines;
					break;
            }
        }

        void UpdateText()
        {
			if (!string.IsNullOrEmpty(baseElement.HTMLString)) {
				var unformattedText = string.Format ("<html>{0}</html>", baseElement.HTMLString.Trim ());
				Control.AttributedText = GetAttributedString (unformattedText);
			} else {
				Control.AttributedText = null;
			}
        }

        NSAttributedString GetAttributedString(string unformattedString)
        {
			XElement element;
			try {
				element = XElement.Parse (unformattedString);
			} catch (Exception ex) {
				element = XElement.Parse ("<html> </html>");
			}
            var attributes = new Dictionary<UIStringAttributes, NSRange>();
            var stringBuilder = new StringBuilder();
            ParseText(element, attributes, stringBuilder);

            var attributedString = new NSMutableAttributedString(stringBuilder.ToString());
            foreach (var item in attributes)
            {
                attributedString.SetAttributes(item.Key, item.Value);
            }
            return attributedString;
        }

        void ParseText(XElement element, IDictionary<UIStringAttributes, NSRange> attributes, StringBuilder stringBuilder)
        {
            var elementName = element.Name.ToString().ToUpper();
            switch (elementName)
            {
                case ElementBr:
                    stringBuilder.AppendLine();
                    break;
                case ElementDiv:
                    break;
                case ElementLi:
                    stringBuilder.Append("   • ");
                    break;
                //				case ElementUl:
                //					spannedString.Clear();
                //					//spannedString.Append("\n"); 
                //					break;
                //				case ElementH:
                //					if (Element.DefaultHFontSize > 0)
                //					{
                //						spannedString.SetSpan(new AbsoluteSizeSpan((int)Element.DefaultHFontSize, true),
                //							0, spanEnd, SpanTypes.ExclusiveExclusive);
                //					}
                //
                //					spannedString.Append("\n");
                //					spannedString.SetSpan(new StyleSpan(TypefaceStyle.Bold), 0, spanEnd, SpanTypes.ExclusiveExclusive);
                //
                //					//spannedString.Append("\n");
                //
                //					break;
            }

            foreach (var node in element.Nodes())
            {
                XText textElement = node as XText;
                if (textElement != null)
                {
                    switch (elementName)
                    {
                        case ElementH:
							var fontSize = (nfloat)baseElement.DefaultHFontSize;
                            var fontSizeAttribute = element.Attribute(XName.Get(FontSize));
                            if (fontSizeAttribute != null)
                                fontSize = (nfloat)Convert.ToInt32(fontSizeAttribute.Value);

							var fontColor = baseElement.DefaultTextColor.ToUIColor();
                            var fontColorAttribute = element.Attribute(XName.Get(FontColor));
                            if (fontColorAttribute != null)
                                fontColor = Color.FromHex(fontColorAttribute.Value).ToUIColor();

                            var attribute = new UIStringAttributes
                            {
                                ForegroundColor = fontColor,
                                Font = UIFont.FromName(Resolver.Get<IFontService>().GetFontName(FontStyle.Bold), fontSize),
                            };
                            attributes.Add(attribute, new NSRange(stringBuilder.Length, textElement.Value.Length));
                            break;
                        case ElementB:
							fontSize = (nfloat)baseElement.DefaultFontSize;
                            fontSizeAttribute = element.Attribute(XName.Get(FontSize));
                            if (fontSizeAttribute != null)
                                fontSize = (nfloat)Convert.ToInt32(fontSizeAttribute.Value);

							fontColor = baseElement.DefaultTextColor.ToUIColor();
                            fontColorAttribute = element.Attribute(XName.Get(FontColor));
                            if (fontColorAttribute != null)
                                fontColor = Color.FromHex(fontColorAttribute.Value).ToUIColor();

                            var fontName = Resolver.Get<IFontService>().GetFontName(FontStyle.Bold);
                            if (!string.IsNullOrEmpty(fontName))
                            {

                                attribute = new UIStringAttributes
                                {
                                    ForegroundColor = fontColor,
                                    Font = UIFont.FromName(Resolver.Get<IFontService>().GetFontName(FontStyle.Bold), fontSize)
                                };
                            }
                            else
                            {
                                attribute = new UIStringAttributes
                                {
                                    ForegroundColor = fontColor,
									Font = UIFont.BoldSystemFontOfSize((nfloat)baseElement.DefaultFontSize)
                                };
                            }
                            attributes.Add(attribute, new NSRange(stringBuilder.Length, textElement.Value.Length));
                            break;

						default:
							 fontSize = (nfloat)baseElement.DefaultFontSize;
							 fontSizeAttribute = element.Attribute(XName.Get(FontSize));
							if (fontSizeAttribute != null)
								fontSize = (nfloat)Convert.ToInt32(fontSizeAttribute.Value);

							 fontColor = baseElement.DefaultTextColor.ToUIColor();
							 fontColorAttribute = element.Attribute(XName.Get(FontColor));
							if (fontColorAttribute != null)
								fontColor = Color.FromHex(fontColorAttribute.Value).ToUIColor();

							 attribute = new UIStringAttributes
								{
									ForegroundColor = fontColor,
									Font = UIFont.FromName(Resolver.Get<IFontService>().GetFontName(FontStyle.Regular), fontSize)
								};
							attributes.Add(attribute, new NSRange(stringBuilder.Length, textElement.Value.Length));
							break;
                    }
                    stringBuilder.Append(textElement.Value);

                    switch (elementName)
                    {
                        case ElementH:
                        case ElementP:
                        case ElementLi:
                            stringBuilder.AppendLine();
                            break;
                    }
                }
                else
                {
                    ParseText(node as XElement, attributes, stringBuilder);
                }
            }
        }

        private void AttachPageEvents(Element element)
        {
            var viewCell = GetContainingViewCell(element);
            if (viewCell != null)
            {
                viewCell.PropertyChanged += (s, ev) =>
                {
                    var propertyName = ev.PropertyName;

                    if (ev.PropertyName == "Renderer")
                    {
                        UpdateText();
                    }
                };

                var listView = GetContainingListView(element);
                if (listView != null)
                {
                    listView.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "Renderer")
                        {
                            if (listView.Parent == null)
                            {
                                this.Dispose(true);
                            }
                        }
                    };
                }

                page = GetContainingPage(element);
                if (page == null)
                {
                    var root = GetRootElement(element);
                    root.PropertyChanged += (s, e) =>
                    {
                        if (e.PropertyName == "Parent")
                        {
                            AttachPageEvents(root);
                        }
                    };
                    return;
                }
                // As of Xamarin.Forms 1.4+, image will be reused when moving from tabs.
                // Uncomment this if using Xamarin.Forms < 1.4.
                //if (page.Parent is TabbedPage)
                //{
                //    page.Disappearing += PageContainedInTabbedPageDisapearing;
                //    return;
                //}

                navigPage = GetContainingNavigationPage(page);
                if (navigPage != null)
                {
                    navigPage.Popped += OnPagePopped;

                    // As of Xamarin.Forms 1.4+, image will be reused when moving from tabs.
                    // Uncomment this if using Xamarin.Forms < 1.4.
                    //if (navigPage.Parent is TabbedPage)
                    //{
                    //    navigPage.Disappearing += PageContainedInTabbedPageDisapearing;
                    //}
                }
            }
            // As of Xamarin.Forms 1.4+, image will be reused when moving from tabs.
            // Uncomment this if using Xamarin.Forms < 1.4.
            //else if ((page = GetContainingTabbedPage(element)) != null)
            //{
            //    page.Disappearing += PageContainedInTabbedPageDisapearing;
            //}
        }

        void PageContainedInTabbedPageDisapearing(object sender, EventArgs e)
        {
            this.Dispose(true);
            page.Disappearing -= PageContainedInTabbedPageDisapearing;
        }

        private void OnPagePopped(object s, NavigationEventArgs e)
        {
            if (e.Page == page)
            {
                this.Dispose(true);
                navigPage.Popped -= OnPagePopped;
            }
        }

        private Xamarin.Forms.ListView GetContainingListView(Xamarin.Forms.Element element)
        {
            Element parentElement = element.ParentView;

            if (parentElement == null)
                return null;

            if (typeof(Xamarin.Forms.ListView).IsAssignableFrom(parentElement.GetType()))
                return (Xamarin.Forms.ListView)parentElement;
            else
                return GetContainingListView(parentElement);
        }

        private Page GetContainingPage(Xamarin.Forms.Element element)
        {
            Element parentElement = element.ParentView;

            if (parentElement == null)
                return null;

            if (typeof(Page).IsAssignableFrom(parentElement.GetType()))
                return (Page)parentElement;
            else
                return GetContainingPage(parentElement);
        }

        private VisualElement GetRootElement(Xamarin.Forms.Element element)
        {
            VisualElement parentElement = element.ParentView;

            while (parentElement.ParentView != null)
                parentElement = parentElement.ParentView;

            return parentElement;
        }

        private ViewCell GetContainingViewCell(Xamarin.Forms.Element element)
        {
            Element parentElement = element.Parent;

            if (parentElement == null)
                return null;

            if (typeof(ViewCell).IsAssignableFrom(parentElement.GetType()))
                return (ViewCell)parentElement;
            else
                return GetContainingViewCell(parentElement);
        }

        private TabbedPage GetContainingTabbedPage(Element element)
        {
            Element parentElement = element.Parent;

            if (parentElement == null)
                return null;

            if (typeof(TabbedPage).IsAssignableFrom(parentElement.GetType()))
                return (TabbedPage)parentElement;
            else
                return GetContainingTabbedPage(parentElement);
        }

        private NavigationPage GetContainingNavigationPage(Element element)
        {
            Element parentElement = element.Parent;

            if (parentElement == null)
                return null;

            if (typeof(NavigationPage).IsAssignableFrom(parentElement.GetType()))
                return (NavigationPage)parentElement;
            else
                return GetContainingNavigationPage(parentElement);
        }
    }
}

