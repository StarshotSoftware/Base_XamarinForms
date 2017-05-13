using BaseStarShot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace BaseStarShot.Controls {
    public class PullToRefresh: ContentView
    {
        public static readonly BindableProperty IsRefreshingProperty =
            BindableProperty.Create<PullToRefresh, bool>(
                p => p.IsRefreshing, false);

        public bool IsRefreshing
        {
            get { return (bool)GetValue(IsRefreshingProperty); }
            set { SetValue(IsRefreshingProperty, value); }
        }

        public static readonly BindableProperty RefreshCommandProperty =
            BindableProperty.Create<PullToRefresh, ICommand>(
                p => p.RefreshCommand, null);

        public ICommand RefreshCommand
        {
            get { return (ICommand)GetValue(RefreshCommandProperty); }
            set { SetValue(RefreshCommandProperty, value); }
        }

        /// <param name="widthConstraint">The available width for the element to use.</param>
        /// <param name="heightConstraint">The available height for the element to use.</param>
        /// <summary>
        /// Optimization as we can get the size here of our content all in DIP
        /// </summary>
        protected override SizeRequest OnSizeRequest(double widthConstraint, double heightConstraint)
        {
            if (Content == null)
                return new SizeRequest(new Size(100, 100));

            return Content.GetSizeRequest(widthConstraint, heightConstraint);
        }
    }
}