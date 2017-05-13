using System;
using System.Windows.Input;
using UIKit;
using Xamarin.Forms;
using XLabs.Forms.Controls;

namespace TwinTechsLib.Ios
{
    public class FormsUIRefreshControl : UIRefreshControl
    {
        private GridCollectionView gridView;

        public FormsUIRefreshControl (GridCollectionView gridView)
        {
            this.gridView = gridView;

            this.ValueChanged += (object sender, EventArgs e) => {
                var command = RefreshCommand;
                if (command == null)
                    return;
                command.Execute (null);
            };
        }

        private bool isRefreshing;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is refreshing.
        /// </summary>
        /// <value><c>true</c> if this instance is refreshing; otherwise, <c>false</c>.</value>
        public bool IsRefreshing {
            get { return isRefreshing; }
            set {
                isRefreshing = value;
                if (isRefreshing)
                {
                    BeginRefreshing();

                    this.gridView.SetContentOffset(new CoreGraphics.CGPoint(0, -this.Frame.Size.Height), true);
        }
                else
                    EndRefreshing();
            }
        }

        /// <summary>
        /// Gets or sets the refresh command.
        /// </summary>
        /// <value>The refresh command.</value>
        public ICommand RefreshCommand { get; set; }
    }
}

