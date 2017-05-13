using BaseStarShot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class TabbedPage : Xamarin.Forms.TabbedPage
    {
        public IList<string> ActiveIcons { get; protected set; }

        public event EventHandler<int> TappedIndex = (s, e) => { };

        public void RaiseTappedIndex(int indexNumber)
        {
            this.TappedIndex(this, indexNumber);
        }

        public TabbedPage()
        {
            this.ActiveIcons = new List<string>();
        }

        public TabGravity TabScreenLocation
        {
            get { return (TabGravity)base.GetValue(TabScreenLocationProperty); }
            set { SetValue(TabScreenLocationProperty, value); }
        }

        public bool TextVisible
        {
            get { return (bool)base.GetValue(TextVisibleProperty); }
            set { SetValue(TextVisibleProperty, value); }
        }

        public int BadgeLocation
        {
            get { return (int)base.GetValue(BadgeLocationProperty); }
            set { SetValue(BadgeLocationProperty, value); }
        }

        public bool ShowTabs
        {
            get { return (bool)base.GetValue(ShowTabsProperty); }
            set { SetValue(ShowTabsProperty, value); }
        }

        public bool ShowIndicator
        {
            get { return (bool)base.GetValue(ShowIndicatorProperty); }
            set { SetValue(ShowIndicatorProperty, value); }
        }

        public int BadgeCount
        {
            get { return (int)base.GetValue(BadgeCountProperty); }
            set { SetValue(BadgeCountProperty, value); }
        }

        public Color SelectedTextColor
        {
            get { return (Color)base.GetValue(SelectedTextColorProperty); }
            set { SetValue(SelectedTextColorProperty, value); }
        }

        public Color TextColor
        {
            get { return (Color)base.GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        public Color TabBackgroundColor
        {
            get { return (Color)base.GetValue(TabBackgroundColorProperty); }
            set { SetValue(TabBackgroundColorProperty, value); }
        }

        public FileImageSource BackgroundDrawable
        {
            get { return (FileImageSource)base.GetValue(BackgroundDrawableProperty); }
            set { SetValue(BackgroundDrawableProperty, value); }
        }

        public FileImageSource SelectedDrawable
        {
            get { return (FileImageSource)base.GetValue(SelectedDrawableProperty); }
            set { SetValue(SelectedDrawableProperty, value); }
        }

        public string CustomViewBackground
        {
            get { return (string)base.GetValue(CustomViewBackgroundProperty); }
            set { SetValue(CustomViewBackgroundProperty, value); }
        }

        public string CustomViewEmptyBackground
        {
            get { return (string)base.GetValue(CustomViewEmptyBackgroundProperty); }
            set { SetValue(CustomViewEmptyBackgroundProperty, value); }
        }

        public static readonly BindableProperty TabScreenLocationProperty =
            BindableProperty.Create<TabbedPage, TabGravity>(p => p.TabScreenLocation, TabGravity.Bottom);

        public static readonly BindableProperty TextVisibleProperty =
            BindableProperty.Create<TabbedPage, bool>(p => p.TextVisible, true);

        public static readonly BindableProperty BadgeLocationProperty = BindableProperty.Create<TabbedPage, int>(p => p.BadgeLocation, -1);

        public static readonly BindableProperty ShowTabsProperty =
            BindableProperty.Create<TabbedPage, bool>(p => p.ShowTabs, true);

        public static readonly BindableProperty ShowIndicatorProperty =
            BindableProperty.Create<TabbedPage, bool>(p => p.ShowIndicator, true);

        public static readonly BindableProperty BadgeCountProperty =
            BindableProperty.Create<TabbedPage, int>(p => p.BadgeCount, 0);

        public static readonly BindableProperty SelectedTextColorProperty =
            BindableProperty.Create<TabbedPage, Color>(p => p.SelectedTextColor, Color.Default);

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create<TabbedPage, Color>(p => p.TextColor, Color.Default);

        public static readonly BindableProperty TabBackgroundColorProperty =
            BindableProperty.Create<TabbedPage, Color>(p => p.TabBackgroundColor, Color.Default);

        public static readonly BindableProperty BackgroundDrawableProperty =
            BindableProperty.Create<TabbedPage, FileImageSource>(p => p.BackgroundDrawable, null);

        public static readonly BindableProperty SelectedDrawableProperty =
            BindableProperty.Create<TabbedPage, FileImageSource>(p => p.SelectedDrawable, null);

        public static readonly BindableProperty CustomViewBackgroundProperty =
            BindableProperty.Create<TabbedPage, string>(p => p.CustomViewBackground, null);

        public static readonly BindableProperty CustomViewEmptyBackgroundProperty =
            BindableProperty.Create<TabbedPage, string>(p => p.CustomViewEmptyBackground, null);
    }
}
