using BaseStarShot.Services;
using CoreGraphics;
using Foundation;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using System.Linq;
using Base1902;

namespace BaseStarShot.Controls
{
    public class ListViewRenderer : Xamarin.Forms.Platform.iOS.ListViewRenderer
    {
        // TODO: implement XChanged as an event from listview.
        //private readonly Lazy<IScrollListener> scrollListener =
        //    new Lazy<IScrollListener>(() => Resolver.Get<IScrollListener>());

        CGPoint _lastContentOffset;
        private INotifyCollectionChanged collectionChanged = null;
        private PaddedLabel emptyLabel;
        bool isScrolledUp = false;
        private UIGestureRecognizer panGesture;
        private CGPoint startSwipePoint;
        protected BaseStarShot.Controls.ListView BaseControl { get { return ((BaseStarShot.Controls.ListView)this.Element); } }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var baseElement = Element as ListView;
            SetEmptyText(baseElement);
        }

        private bool isScrolling = false;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null || this.Element == null)
                return;

            var baseElement = Element as ListView;

            if (baseElement != null)
            {
                baseElement.PropertyChanged += async (s, ev) =>
                {
                    if (Element == null || Control == null)
                        return;

                    var element = (ListView)s;
                    switch (ev.PropertyName)
                    {
                        case "ItemsSource":
                            {
                                SetItemsSource();
                                //if (Element != null && Element.RowHeight < 0)
                                //    Control.RowHeight = 30;
                            }
                            break;
                        //case "ShowEmptyView": ShowEmptyLabel(element); break;
                        case "EmptyText":
                            SetEmptyText(element);
                            break;
                        case "EmptyTextFontStyle":
                            SetEmptyTextFont(element);
                            break;
                        case "EmptyTextFontSize":
                            SetEmptyTextFont(element);
                            break;
                        case "EmptyTextColor":
                            SetEmptyTextColor(element);
                            break;
                        case "Clickable": SetAllowsSelection(element); break;
                        // Hack: force to scroll to top when refreshing
                        case "IsRefreshing":
                            if (emptyLabel != null)
                            {
                                UpdateEmptyLabelVisibility();
                            }
                            //                            if (!Element.IsRefreshing)
                            //                                ScrollToTop();

                            break;

                        case "ScrollToTopChange":
                            ScrollToTop();
                            break;

                        case "ScrollToIndex":
                            if (BaseControl.ScrollToIndex > 0)
                            {
                                if (isScrolling)
                                    return;

                                isScrolling = true;
                                await Task.Delay(500);
                                var index = NSIndexPath.FromRowSection(BaseControl.ScrollToIndex, (nint)0);
                                Control.ScrollToRow(index, UITableViewScrollPosition.Bottom, true);
                                isScrolling = false;
                            }
                            break;
                    }
                };

                //SetEmptyLabel(baseElement);
                //ShowEmptyLabel(baseElement);
                SetAllowsSelection(baseElement);
                SetSwipeEnabled(baseElement);
            }
            //Control.BackgroundColor = Color.Transparent.ToUIColor();
            if (baseElement.DismissKeyboardOnDrag && Control != null)
                Control.KeyboardDismissMode = UIScrollViewKeyboardDismissMode.OnDrag;

            SetItemsSource();

            if (!baseElement.UseCustomRenderer && Control != null)
            {

                if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
                {
                    Control.Scrolled += Control_Scrolled;
                    Control.DraggingStarted += Control_DraggingStarted;
                    Control.DraggingEnded += Control_DraggingEnded;
                }
                else
                {
                    panGesture = new UIPanGestureRecognizer(recognizer =>
                    {
                        if (recognizer.VelocityInView(Control).Y > 0)
                        {
                            if (Control.ContentOffset.Y == 0)
                            {
                                BaseControl.OnScrolledTop();
                            }
                        }
                        else
                        {
                            if (Control.ContentOffset.Y + Control.Frame.Height > Control.ContentSize.Height)
                            {
                            }
                        }
                    });
                    panGesture.Delegate = new RecognizerDelegate();
                    Control.AddGestureRecognizer(panGesture);


                    // For IOS 7  use gesture recognizer
                }
            }

            BaseControl.ScrollToBottomEvent += BaseControl_ScrollToBottom;
            //            BaseControl.ScrollToTopEvent += BaseControl_ScrollToTopEvent;

            Control.TableFooterView = new UIKit.UIView();
        }

        private void BaseControl_ScrollToBottom(object sender, int e)
        {
            if (Control == null || isScrolling)
                return;

            var index = NSIndexPath.FromRowSection((nint)e, (nint)0);
            Control.ScrollToRow(index, UITableViewScrollPosition.Bottom, false);
            //var rect = new CGRect(0, 0, Control.ContentSize.Width, Control.ContentSize.Height);
            //Control.ScrollRectToVisible(rect, false);
        }

        private void BaseControl_ScrollToTopEvent(object sender, EventArgs e)
        {
            if (Control == null)
                return;

            NSIndexPath index = NSIndexPath.FromRowSection(0, 0);
            Control.ScrollToRow(index, UITableViewScrollPosition.Top, true);
        }

        private void Control_DraggingEnded(object sender, DraggingEventArgs e)
        {
            if (_lastContentOffset.Y < (int)Control.ContentOffset.Y)
            {
                isScrolledUp = false;
            }

            if (_lastContentOffset.Y > (int)Control.ContentOffset.Y)
            {
                isScrolledUp = true;
            }
        }

        private void Control_DraggingStarted(object sender, EventArgs e)
        {
            _lastContentOffset = Control.ContentOffset;
        }
        private void Control_Scrolled(object sender, EventArgs e)
        {
			try {
            if (Element == null)
                return;

            var baseElement = Element as ListView;

            if (Control.ContentOffset.Y == 0)
            {
                if (isScrolledUp)
                {
                    baseElement.OnScrolledTop();
                }
            }
            else
            {
                //CGPoint offset = Control.ContentOffset;
                //CGRect bounds = Control.Bounds;
                //CGSize size = Control.ContentSize;
                //UIEdgeInsets inset = Control.ContentInset;
                //nfloat y = offset.Y + bounds.Size.Height - inset.Bottom;
                //nfloat h = size.Height;

                //float reload_distance = 10;
                //if (y > h + reload_distance)
                //{

                //}
            }

				if (baseElement != null && Control != null && Control.VisibleCells != null && Control.VisibleCells.Count() > 0 && Control.IndexPathsForVisibleRows.Count () > 0)
					baseElement.LastVisiblePosition = Control.IndexPathForCell (Control.VisibleCells [(int)(Control.VisibleCells.Count () - 1)]).Row;
			} catch (Exception ex) {
			}
        }

        private async void ScrollToTop()
        {
            if (Control == null)
                return;

            //await Task.Delay(1000);
            //NSIndexPath index = NSIndexPath.FromRowSection(0, 0);
            //Control.ScrollToRow(index, UITableViewScrollPosition.Top, true);

            //await Task.Delay(150);
            //if (Control != null)
            //    Control.SetContentOffset(new CGPoint(0, 0), true);
            if (BaseControl.ScrollToTopChange)
            {
                NSIndexPath index = NSIndexPath.FromRowSection(0, 0);
                Control.ScrollToRow(index, UITableViewScrollPosition.Top, true);
                BaseControl.ScrollToTopChange = false;
            }


        }

        private void SetAllowsSelection(ListView element)
        {
            Control.AllowsSelection = element.Clickable;
            //            if (element.Selector == ListViewSelector.None)
            //            {
            //
            //            }
        }

        private void SetEmptyText(ListView element)
        {
            if (Superview != null && Superview.Frame.Width > 0)
            {
                if (emptyLabel == null)
                {
                    emptyLabel = new PaddedLabel();
                    emptyLabel.Center = Center;
                    emptyLabel.Bounds = Bounds;
                    emptyLabel.TextAlignment = UITextAlignment.Center;
                    emptyLabel.LineBreakMode = UILineBreakMode.WordWrap;
                    emptyLabel.Lines = 10;
                    SetEmptyTextFont(element);
                    SetEmptyTextColor(element);
                    UpdateEmptyLabelVisibility();

                    Control.BackgroundView = emptyLabel;
                }

                emptyLabel.Text = element.EmptyText;
            }
        }

        private void SetEmptyTextFont(ListView element)
        {
            if (emptyLabel != null)
            {
                var fontService = Resolver.Get<IFontService>();
                if (fontService != null)
                {
                    var font = UIFont.FromName(fontService.GetFontName(element.EmptyTextFontStyle), (nfloat)element.EmptyTextFontSize);
                    if (font != null)
                        emptyLabel.Font = font;
                }
            }
        }

        private void SetEmptyTextColor(ListView element)
        {
            if (emptyLabel != null)
            {
                emptyLabel.TextColor = element.EmptyTextColor.ToUIColor();
            }
        }

        private void SetItemsSource()
        {
            var element = Element;
            if (Element == null || Element.ItemsSource == null)
            {
                if (emptyLabel != null)
                    emptyLabel.Hidden = false;
                return;
            }

            if (collectionChanged == null)
            {
                if (!(Element.ItemsSource is INotifyPropertyChanged))
                {
                    if (emptyLabel != null)
                        emptyLabel.Hidden = false;

                    return;
                }
                collectionChanged = Element.ItemsSource as INotifyCollectionChanged;

                int itemCount = 0;
                collectionChanged.CollectionChanged += (s, ev) =>
                {
					if (s == null || Control == null || Element == null || Element.ItemsSource == null)
						return;
					
                    itemCount = ev.NewStartingIndex;

                    if (emptyLabel == null)
                        return;

                    if (ev.Action == NotifyCollectionChangedAction.Add)
                    {
                        if (!emptyLabel.Hidden)
                            emptyLabel.Hidden = true;
                    }
                    else if (ev.Action == NotifyCollectionChangedAction.Remove)
                    {
                        if (ev.NewItems != null && ev.OldItems != null)
                            if (ev.NewItems.Count - ev.OldItems.Count < 0)
                            {
                                if (!Element.IsRefreshing)
                                    emptyLabel.Hidden = false;
                            }
                    }
                    else if (ev.Action == NotifyCollectionChangedAction.Reset)
                    {
                        if (Element != null && !Element.IsRefreshing)
                            emptyLabel.Hidden = false;
                    }
                };
            }

            UpdateEmptyLabelVisibility();
        }

        //            UIView.Animate(0.3,
        //                () => emptyLabel.Alpha = element.ShowEmptyView ? 1 : 0,
        //                () => { emptyLabel.Hidden = !element.ShowEmptyView; });
        //        }
        //    }
        //}
        private void SetSwipeEnabled(ListView element)
        {
            if (element.SwipeEnabled)
            {
                // TODO: implement XChanged as an event from listview.
                //panGesture = new UIPanGestureRecognizer(recognizer =>
                //{
                //    if (recognizer.State == UIGestureRecognizerState.Began)
                //        startSwipePoint = recognizer.LocationInView(Control);
                //    else if (recognizer.State == UIGestureRecognizerState.Changed)
                //    {
                //        var endPoint = recognizer.LocationInView(Control);
                //        scrollListener.Value.SetX((int)startSwipePoint.X, (int)endPoint.X);
                //        startSwipePoint = endPoint;
                //    }
                //});
                //panGesture.Delegate = new RecognizerDelegate();
                //Control.AddGestureRecognizer(panGesture);
            }
            else
            {
                if (panGesture != null)
                    Control.RemoveGestureRecognizer(panGesture);
            }
        }

        private void UpdateEmptyLabelVisibility()
        {
            if (Element.IsRefreshing)
            {
                if (emptyLabel != null)
                {
                    if (Element.IsRefreshing)
                        emptyLabel.Hidden = true;
                }
                return;
            }

            if (Element.ItemsSource == null && !Element.IsRefreshing)
            {
                if (emptyLabel != null)
                {
                    if (Element.ItemsSource == null)
                        emptyLabel.Hidden = false;
                }
                return;
            }

            var prop = Element.ItemsSource.GetType().GetRuntimeProperty("Count");

            if (prop != null && prop.GetValue(Element.ItemsSource) != null)
            {
                var count = int.Parse(prop.GetValue(Element.ItemsSource).ToString());

                if (emptyLabel != null)
                    emptyLabel.Hidden = (count > 0);
            }
        }

        public class RecognizerDelegate : UIGestureRecognizerDelegate
        {
            public override bool ShouldRecognizeSimultaneously(UIGestureRecognizer gestureRecognizer, UIGestureRecognizer otherGestureRecognizer)
            {
                return true;
            }
        }

        public class PaddedLabel : UILabel
        {
            public override void DrawText(CGRect rect)
            {
                var insets = new UIEdgeInsets(0, 20, 0, 20);
                base.DrawText(insets.InsetRect(rect));
            }
        }
    }
}