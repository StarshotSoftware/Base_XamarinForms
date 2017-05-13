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
using Android.Text;
using Android.Text.Style;
using Android.Text.Method;
using System.Xml.Linq;

using Color = Android.Graphics.Color;
using BaseStarShot.Services;
using System.Text.RegularExpressions;
using BaseStarShot.Util;
using Base1902;

[assembly: ExportRenderer(typeof(BaseStarShot.Controls.FormattedTextLabel), typeof(FormattedTextLabelRenderer))]
namespace BaseStarShot.Controls
{
    public class FormattedTextLabelRenderer : LabelRenderer
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
        private const string FONTSIZE = "FONTSIZE";
        private const string FONTCOLOR = "FONTCOLOR";

        Page page;
        NavigationPage navigPage;

        public FormattedTextLabel baseElement { get { return (FormattedTextLabel)Element; } }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Label> e)
        {
            base.OnElementChanged(e);
            //if (e.NewElement != null)
            //{
            //    AttachPageEvents(e.NewElement);
            //}

            if (Control == null)
            {
                Control.TextSize = (float)baseElement.DefaultFontSize;
                Control.SetPadding(10, 10, 10, 10);
            }

            if (baseElement != null)
            {
                SetTypeface();
                SetMaxLines(baseElement);
                UpdateText();
                baseElement.PropertyChanged += baseElement_PropertyChanged;
            }
        }

        void SetMaxLines(BaseStarShot.Controls.FormattedTextLabel element)
        {
            if (element.MaxLines > 1)
            {
                Control.SetSingleLine(false);
                Control.SetMaxLines(element.MaxLines);
            }
            else if (element.MaxLines == 1)
            {
                Control.SetSingleLine(true);
                Control.SetMaxLines(1);
            }

            //if (Element.LineBreakMode == LineBreakMode.NoWrap)
            this.Control.Ellipsize = TextUtils.TruncateAt.End;
        }


        void baseElement_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (Element == null || Control == null)
                return;

            var property = e.PropertyName;
            switch (property)
            {
                case "HTMLString":
                    Control.Text = "";
                    UpdateText(); break;
            }
        }



        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        void SetTypeface()
        {
            if (!string.IsNullOrEmpty(Resolver.Get<IFontService>().GetFontName(FontStyle.Regular)))
                   Control.Typeface = FontCache.GetTypeFace(Resolver.Get<IFontService>().GetFontName(FontStyle.Regular));
        }

        void UpdateText()
        {
            string text = baseElement.HTMLString;

            //var fs = Html.FromHtml(text).ToString();
            string modifiedText = string.Format("<div>{0}</div>", text);

            XElement element = null;
            try
            {
                element = XElement.Parse(modifiedText);
            }
            catch (Exception e)
            {

            }

            if (element != null)
                Control.Append(ParseText(element));
        }

        enum SpanType
        {
            BULLET,
            HYPERLINK,
            QUOTE,
            BOLD,
            UNDERLINE,
            STRIKETHROUGH,
            BGCOLOR,
            FGCOLOR,
            MASKFILTER_EMBOSS,
            SUBSCRIPT,
            ITALIC,
            ABSOLUTE_SIZE_SPAN,
            RELATIVE_SIZE_SPAN,
            TEXTAPPEARANCE_SPAN,
            SUPERSCRIPT,
            LOCALE_SPAN,
            SCALEX_SPAN,
            TYPEFACE_SPAN,
            IMAGE_SPAN,
            MASKFILTER_BLUR,
            ALIGNMENT_STANDARD
        };

        private SpannableString SpanIt(SpanType type, string text, List<KeyValuePair<string, string>> attributes)
        {
            bool setFontSize = false;
            bool setFontColor = false;
            int fontSize = 0;
            string fontColor = "";

            if (attributes != null && attributes.Count > 0)
            {
                foreach (var item in attributes)
                {
                    if (item.Key == FONTSIZE)
                    {
                        setFontSize = true;
                        fontSize = int.Parse(item.Value);
                    }

                    if (item.Key == FONTCOLOR)
                    {
                        setFontColor = true;
                        fontColor = item.Value;
                    }
                }
            }

            var mBaconIpsumSpannableString = new SpannableString(text);

            float density = Context.Resources.DisplayMetrics.Density;
            //WordPosition wordPosition = GetWordPosition(text);
            Java.Lang.Object span = null;
            int allTextStart = 0;
            int allTextEnd = text.Length;
            switch (type)
            {
                case SpanType.BULLET:
                    span = new BulletSpan(15, Color.Black);
                    SetAttributes(mBaconIpsumSpannableString, setFontSize, setFontColor, fontSize, fontColor, allTextStart, allTextEnd);
                    mBaconIpsumSpannableString.SetSpan(span, allTextStart, allTextEnd, SpanTypes.ExclusiveExclusive);


                    //spannedString.Insert(0, "   • ");
                    //spannedString.SetSpan(new TabStopSpanStandard(500), 0, spanEnd, SpanTypes.ExclusiveExclusive);
                    //spannedString.SetSpan(new BulletSpan(100), 1, spanEnd, SpanTypes.ExclusiveExclusive);
                    //spannedString.Append("\n");
                    break;
                case SpanType.QUOTE:
                    span = new QuoteSpan(Color.Red);
                    SetAttributes(mBaconIpsumSpannableString, setFontSize, setFontColor, fontSize, fontColor, allTextStart, allTextEnd);
                    mBaconIpsumSpannableString.SetSpan(span, allTextStart, allTextEnd, SpanTypes.ExclusiveExclusive);
                    break;
                case SpanType.BOLD:
                    span = new StyleSpan(TypefaceStyle.Bold);
                    SetAttributes(mBaconIpsumSpannableString, setFontSize, setFontColor, fontSize, fontColor, allTextStart, allTextEnd);
                    mBaconIpsumSpannableString.SetSpan(span, allTextStart, allTextEnd, SpanTypes.ExclusiveExclusive);
                    break;
                case SpanType.HYPERLINK:
                    span = new MyClickableSpan();

                    int clickCounter = 0;
                    ((MyClickableSpan)span).Click += v =>
                    {
                        if (clickCounter > 0)
                        {
                            clickCounter = 0;
                            return;
                        }
                        Xamarin.Forms.Device.OpenUri(new System.Uri(text));

                        clickCounter += 1;
                    };

                    mBaconIpsumSpannableString.SetSpan(span, 22, 27, SpanTypes.ExclusiveExclusive);
                    SetAttributes(mBaconIpsumSpannableString, setFontSize, setFontColor, fontSize, fontColor, allTextStart, allTextEnd);
                    mBaconIpsumSpannableString.SetSpan(span, allTextStart, allTextEnd, SpanTypes.ExclusiveExclusive);

                    break;
                case SpanType.ALIGNMENT_STANDARD:
                    span = new AlignmentSpanStandard(Android.Text.Layout.Alignment.AlignCenter);
                    SetAttributes(mBaconIpsumSpannableString, setFontSize, setFontColor, fontSize, fontColor, allTextStart, allTextEnd);
                    mBaconIpsumSpannableString.SetSpan(span, allTextStart, allTextEnd, SpanTypes.ExclusiveExclusive);
                    break;
                case SpanType.UNDERLINE:
                    span = new UnderlineSpan();
                    SetAttributes(mBaconIpsumSpannableString, setFontSize, setFontColor, fontSize, fontColor, allTextStart, allTextEnd);
                    mBaconIpsumSpannableString.SetSpan(span, allTextStart, allTextEnd, SpanTypes.ExclusiveExclusive);
                    break;
                case SpanType.STRIKETHROUGH:
                    span = new StrikethroughSpan();
                    SetAttributes(mBaconIpsumSpannableString, setFontSize, setFontColor, fontSize, fontColor, allTextStart, allTextEnd);
                    mBaconIpsumSpannableString.SetSpan(span, allTextStart, allTextEnd, SpanTypes.ExclusiveExclusive);
                    break;
                case SpanType.BGCOLOR:
                    span = new BackgroundColorSpan(Color.Green);
                    SetAttributes(mBaconIpsumSpannableString, setFontSize, setFontColor, fontSize, fontColor, allTextStart, allTextEnd);
                    mBaconIpsumSpannableString.SetSpan(span, allTextStart, allTextEnd, SpanTypes.ExclusiveExclusive);
                    break;
                case SpanType.MASKFILTER_BLUR:
                    span = new MaskFilterSpan(new BlurMaskFilter(density * 2, BlurMaskFilter.Blur.Normal));
                    mBaconIpsumSpannableString.SetSpan(span, allTextStart, allTextEnd, SpanTypes.ExclusiveExclusive);
                    break;
                case SpanType.MASKFILTER_EMBOSS:
                    span = new MaskFilterSpan(new EmbossMaskFilter(new float[] { 1, 1, 1 }, 0.4f, 6, 3.5f));
                    ForegroundColorSpan fg = new ForegroundColorSpan(Color.Blue);
                    StyleSpan style = new StyleSpan(TypefaceStyle.Bold);

                    mBaconIpsumSpannableString.SetSpan(fg, allTextStart, allTextEnd, SpanTypes.ExclusiveExclusive);
                    mBaconIpsumSpannableString.SetSpan(style, allTextStart, allTextEnd, SpanTypes.ExclusiveExclusive);
                    mBaconIpsumSpannableString.SetSpan(span, allTextStart, allTextEnd, SpanTypes.ExclusiveExclusive);
                    break;
                case SpanType.SUBSCRIPT:
                    span = new SubscriptSpan();
                    SetAttributes(mBaconIpsumSpannableString, setFontSize, setFontColor, fontSize, fontColor, allTextStart, allTextEnd);
                    mBaconIpsumSpannableString.SetSpan(span, allTextStart, allTextEnd, SpanTypes.ExclusiveExclusive);
                    break;
                case SpanType.SUPERSCRIPT:
                    span = new SuperscriptSpan();
                    SetAttributes(mBaconIpsumSpannableString, setFontSize, setFontColor, fontSize, fontColor, allTextStart, allTextEnd);
                    mBaconIpsumSpannableString.SetSpan(span, allTextStart, allTextEnd, SpanTypes.ExclusiveExclusive);
                    break;
                case SpanType.ITALIC:
                    span = new StyleSpan(TypefaceStyle.Italic);
                    SetAttributes(mBaconIpsumSpannableString, setFontSize, setFontColor, fontSize, fontColor, allTextStart, allTextEnd);
                    mBaconIpsumSpannableString.SetSpan(span, allTextStart, allTextEnd, SpanTypes.ExclusiveExclusive);
                    break;
                case SpanType.ABSOLUTE_SIZE_SPAN:
                    span = new AbsoluteSizeSpan(24, true);
                    SetAttributes(mBaconIpsumSpannableString, setFontSize, setFontColor, fontSize, fontColor, allTextStart, allTextEnd);
                    mBaconIpsumSpannableString.SetSpan(span, allTextStart, allTextEnd, SpanTypes.ExclusiveExclusive);
                    break;
                case SpanType.RELATIVE_SIZE_SPAN:
                    span = new RelativeSizeSpan(2.0f);
                    SetAttributes(mBaconIpsumSpannableString, setFontSize, setFontColor, fontSize, fontColor, allTextStart, allTextEnd);
                    mBaconIpsumSpannableString.SetSpan(span, allTextStart, allTextEnd, SpanTypes.ExclusiveExclusive);
                    break;
                //case SpanType.TEXTAPPEARANCE_SPAN:
                //    span = new TextAppearanceSpan(this, R.style.SpecialTextAppearance);
                //    mBaconIpsumSpannableString.SetSpan(span, allTextStart, allTextEnd, SpanTypes.ExclusiveExclusive);
                //    break;
                //case SpanType.LOCALE_SPAN:
                //    span = new LocaleSpan(Locale.CHINESE);
                //    mBaconIpsumSpannableString.SetSpan(span, allTextStart, allTextEnd, SpanTypes.ExclusiveExclusive);
                //    break;
                //case SpanType.SCALEX_SPAN:
                //    span = new ScaleXSpan(3.0f);
                //    mBaconIpsumSpannableString.SetSpan(span, allTextStart, allTextEnd, SpanTypes.ExclusiveExclusive);
                //    break;
                //case SpanType.TYPEFACE_SPAN:
                //    span = new TypefaceSpan("serif");
                //    mBaconIpsumSpannableString.SetSpan(span, allTextStart, allTextEnd, SpanTypes.ExclusiveExclusive);
                //    break;
                //case SpanType.IMAGE_SPAN:
                //    span = new ImageSpan(this, R.drawable.pic1_small);
                //    mBaconIpsumSpannableString.SetSpan(span, allTextStart, allTextEnd, SpanTypes.ExclusiveExclusive);
                //    break;
            }
            if (span == null)
            {
                return null;
            }

            return mBaconIpsumSpannableString;
        }

        void SetAttributes(SpannableString spannableString, bool setFontSize, bool setFontColor, int fontSize, string fontColor, int start, int end)
        {
            if (setFontSize)
                spannableString.SetSpan(new AbsoluteSizeSpan(fontSize, true), start, end, SpanTypes.ExclusiveExclusive);

            if (setFontColor)
                spannableString.SetSpan(new ForegroundColorSpan(Xamarin.Forms.Color.FromHex(fontColor).ToAndroid()),
                    start, end, SpanTypes.ExclusiveExclusive);
        }

        SpannableStringBuilder ParseText(XElement currentElement, SpannableStringBuilder spannedString = null)
        {
            if (currentElement == null) return spannedString;

            if (spannedString == null)
                spannedString = new SpannableStringBuilder();

            var elementName = currentElement.Name.ToString().ToUpper();
            string elementValue = "";

            if (!string.IsNullOrWhiteSpace(currentElement.Value))
                elementValue = currentElement.Value.ToString().Replace ("&amp;", "&");

            string firstAttributeName = "";
            string firstAttributeValue = "";

            string nextAttributeName = "";
            string nextAttributeValue = "";
            List<KeyValuePair<string, string>> nodeAttributes = null;

            if (currentElement.FirstAttribute != null)
            {
                nodeAttributes = new List<KeyValuePair<string, string>>();

                firstAttributeName = currentElement.FirstAttribute.Name.ToString().ToUpper();
                firstAttributeValue = currentElement.FirstAttribute.Value.ToUpper();

                nodeAttributes.Add(new KeyValuePair<string, string>(firstAttributeName, firstAttributeValue));

                if (currentElement.FirstAttribute.NextAttribute != null)
                {
                    nextAttributeName = currentElement.FirstAttribute.NextAttribute.Name.ToString().ToUpper();
                    nextAttributeValue = currentElement.FirstAttribute.NextAttribute.Value.ToUpper();

                    nodeAttributes.Add(new KeyValuePair<string, string>(nextAttributeName, nextAttributeValue));
                }
            }

            int spanEnd = elementValue.Length;
            //SpannableStringBuilder spannedString = new SpannableStringBuilder(elementValue);
            switch (elementName)
            {
                case ElementA:
                    spannedString.Append(SpanIt(SpanType.HYPERLINK, elementValue, nodeAttributes));
                    break;
                case ElementB:
                case ElementStrong:
                    spannedString.Append(SpanIt(SpanType.BOLD, elementValue, nodeAttributes));
                    break;
                case ElementI:
                case ElementEm:
                    spannedString.Append(SpanIt(SpanType.ITALIC, elementValue, nodeAttributes));
                    break;
                case ElementU:
                    spannedString.Append(SpanIt(SpanType.UNDERLINE, elementValue, nodeAttributes));
                    break;
                case ElementBr:
                    spannedString.Append("\n");
                    //spannedString.Append("\n");
                    //Control.Append(spannedString);
                    break;
                case ElementDiv:
                    break;
                case ElementP:
                    spannedString.Append("\n");
                    break;
                case ElementLi:
                    var liSpan = new SpannableStringBuilder();
                    liSpan.Append("   • ");
                    liSpan.Append(elementValue);
                    liSpan.SetSpan(new TabStopSpanStandard(500), 0, spanEnd, SpanTypes.ExclusiveExclusive);
                    liSpan.SetSpan(new BulletSpan(100), 1, spanEnd, SpanTypes.ExclusiveExclusive);
                    liSpan.Append("\n");
                    spannedString.Append(liSpan);
                    //Control.Append(spannedString);
                    //Control.Append(SpanIt(SpanType.BULLET, elementValue, nodeAttributes));
                    break;
                case ElementUl:
                    spannedString.Append("\n");
                    break;
                case ElementH:
                    spannedString.Append("\n");
                    spannedString.Append(SpanIt(SpanType.BOLD, elementValue, nodeAttributes));
                    spannedString.Append("\n");

                    break;
            }

            foreach (var node in currentElement.Nodes())
            {
                XText textElement = node as XText;
                if (textElement != null)
                {
					var str = (node + "").Replace("&amp;", "&");

                    var index = spannedString.ToString().Trim().LastIndexOf((str).Trim());
                    var isNearbyText = (index + str.Trim().Length) == spannedString.ToString().Trim().Length;

                    if (index < 0 || (index >= 0 && !isNearbyText))
                    {
                        SpannableStringBuilder spannedString0 = new SpannableStringBuilder(str);
                        if (!string.IsNullOrWhiteSpace(firstAttributeName))
                        {
                            switch (firstAttributeName)
                            {
                                case FONTSIZE: spannedString0.SetSpan(new AbsoluteSizeSpan(int.Parse(firstAttributeValue), true),
                                        0, spanEnd, SpanTypes.ExclusiveExclusive);
                                    break;
                                case FONTCOLOR: spannedString0.SetSpan(new ForegroundColorSpan(Xamarin.Forms.Color.FromHex(firstAttributeValue).ToAndroid()),
                                        0, spanEnd, SpanTypes.ExclusiveExclusive);
                                    break;
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(nextAttributeName))
                        {
                            switch (nextAttributeName)
                            {
                                case FONTSIZE: spannedString0.SetSpan(new AbsoluteSizeSpan(int.Parse(nextAttributeValue), true),
                                        0, spanEnd, SpanTypes.ExclusiveExclusive);
                                    break;
                                case FONTCOLOR: spannedString0.SetSpan(new ForegroundColorSpan(Xamarin.Forms.Color.FromHex(nextAttributeValue).ToAndroid()),
                                        0, spanEnd, SpanTypes.ExclusiveExclusive);
                                    break;
                            }
                        }

                        spannedString.Append(spannedString0);
                    }

                }
                else
                {
                    ParseText(node as XElement, spannedString);
                }
            }
            return spannedString;
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

        private class MyClickableSpan : ClickableSpan
        {
            public Action<Android.Views.View> Click;

            public override void OnClick(Android.Views.View widget)
            {
                if (Click != null)
                    Click(widget);
            }
        }
    }
}