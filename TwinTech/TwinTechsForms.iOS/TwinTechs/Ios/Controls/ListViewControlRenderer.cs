//using System;
//using UIKit;
//using System.Reflection;
//using Foundation;
//using System.Linq;
//using TwinTechs.Controls;
//using Xamarin.Forms;
//using Base1902.Controls;
//using System.Collections.Generic;
//using System.Collections;

//namespace TwinTechsLib.Ios.TwinTechs.Ios.Controls
//{

//    public class ListViewControlRenderer : Base1902.Controls.ListViewRenderer
//    {
//        protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<Xamarin.Forms.ListView> e)
//        {
//            base.OnElementChanged(e);

//            if (Element == null || e.OldElement != null)
//                return;

//            if ((Element is Base1902.Controls.ListView))
//            {
//                var element = (Base1902.Controls.ListView)Element;

//                if (!Element.IsGroupingEnabled && (element).UseCustomRenderer)
//                {
//                    Control.Source = new ListViewDataSourceWrapper(this.GetFieldValue<UITableViewSource>(typeof(Xamarin.Forms.Platform.iOS.ListViewRenderer), "dataSource"), Element);
//                }
//                //this will enable header scrolling
//                //if (Element.IsGroupingEnabled && element.ScrollSectionHeader)
//                {
//                    //Control.SectionHeaderHeight = 40;
//                    //nfloat sectionHeaderHeight = 40; //Control.SectionHeaderHeight;

//                    //Control.Scrolled += (s, ev) =>
//                    //{
//                    //    if (Control.ContentOffset.Y <= sectionHeaderHeight && Control.ContentOffset.Y >= 0)
//                    //    {
//                    //        Control.ContentInset = new UIEdgeInsets(-Control.ContentOffset.Y, 0, 0, 0);
//                    //    }
//                    //    else if (Control.ContentOffset.Y >= sectionHeaderHeight)
//                    //    {
//                    //        Control.ContentInset = new UIEdgeInsets(-sectionHeaderHeight, 0, 0, 0);
//                    //    }
//                    //};

//                }
//            }

//            Control.TableFooterView = new UIKit.UIView();
//        }

//    }

//    public class TableViewDelegate : UITableViewDelegate
//    {
//        private nfloat DefaultHeight = 0;

//        //public TableViewDelegate(nfloat DefaultHeight)
//        //{
//        //    this.DefaultHeight = DefaultHeight;
//        //}
//        private Xamarin.Forms.ListView Element;

//        public TableViewDelegate(Xamarin.Forms.ListView Element)
//        {
//            this.Element = Element;
//        }

//        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
//        {
//            var item = Element.ItemsSource.Cast<object>().ElementAt(indexPath.Row);
//            this.Element.SelectedItem = item;
//        }

//        public override nfloat GetHeightForRow(UITableView tableView, Foundation.NSIndexPath indexPath)
//        {
//            return DefaultHeight;
//        }

//        public void SetDefaultHeight(UITableView tableView, Foundation.NSIndexPath indexPath, nfloat DefaultHeight)
//        {
//            this.DefaultHeight = DefaultHeight;

//            GetHeightForRow(tableView, indexPath);
//        }
//    }

//    public class ListViewDataSourceWrapper : UITableViewSource
//    {
//        private readonly UITableViewSource underlyingTableSource;
//        private readonly Xamarin.Forms.ListView Element;

//        public ListViewDataSourceWrapper(UITableViewSource underlyingTableSource, Xamarin.Forms.ListView Element)
//        {
//            this.underlyingTableSource = underlyingTableSource;
//            this.Element = Element;
//        }

//        public override void Scrolled(UIScrollView scrollView)
//        {

//        }

//        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
//        {
//            return this.underlyingTableSource.GetCell(tableView, indexPath);
//        }

//        public override nint RowsInSection(UITableView tableview, nint section)
//        {
//            return this.underlyingTableSource.RowsInSection(tableview, section);
//        }

//        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
//        {
//            return this.underlyingTableSource.GetHeightForHeader(tableView, section);
//        }

//        public override UIView GetViewForHeader(UITableView tableView, nint section)
//        {
//            return this.underlyingTableSource.GetViewForHeader(tableView, section);
//        }

//        public override nint NumberOfSections(UITableView tableView)
//        {
//            return this.underlyingTableSource.NumberOfSections(tableView);
//        }

//        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
//        {
//            if (!Element.IsGroupingEnabled)
//            {
//                var item = Element.ItemsSource.Cast<object>().ElementAt(indexPath.Row);
//                this.Element.SelectedItem = item;
//                this.underlyingTableSource.RowSelected(tableView, indexPath);
//            }
//            else
//            {
//                var item = (IList)Element.ItemsSource.Cast<object>().ElementAt(indexPath.Section);
//                var childItem = item[indexPath.Row];
//                this.Element.SelectedItem = childItem;
//                this.underlyingTableSource.RowSelected(tableView, indexPath);
//            }
//        }

//        public override string[] SectionIndexTitles(UITableView tableView)
//        {
//            return this.underlyingTableSource.SectionIndexTitles(tableView);
//        }

//        public override string TitleForHeader(UITableView tableView, nint section)
//        {
//            return this.underlyingTableSource.TitleForHeader(tableView, section);
//        }

//        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
//        {
//            if (tableView.Delegate == null)
//            {
//                tableView.Delegate = new TableViewDelegate(Element);
//            }
//            var uiCell = this.GetCellInternal(tableView, indexPath);
//            var count = tableView.VisibleCells;

//            uiCell.SetNeedsLayout();
//            uiCell.LayoutIfNeeded();

//            //var viewCell0 = uiCell.GetPropertyValue<Xamarin.Forms.ViewCell>(uiCell.GetType(), "ViewCell");
//            var viewCell = uiCell.GetPropertyValue<UIView>(uiCell.GetType(), "ContentView");

//            //uiCell.AddConstraint(NSLayoutConstraint.Create(uiCell, NSLayoutAttribute.Height, NSLayoutRelation.Equal, viewCell, NSLayoutAttribute.Height, 1, 0));

//            (tableView.Delegate as TableViewDelegate).SetDefaultHeight(tableView, indexPath, viewCell.Bounds.Height);
//            return viewCell.Bounds.Height;

//            //return (float)viewCell.RenderHeight;
//        }

//        //public void UpdateRowHeight(UITableView tableView, NSIndexPath indexPath, nfloat rowHeight)
//        //{
//        //    //GetHeightForRow(tableView, indexPath);
//        //}

//        private UITableViewCell GetCellInternal(UITableView tableView, NSIndexPath indexPath)
//        {
//            return this.underlyingTableSource.GetCell(tableView, indexPath);
//        }
//    }

//    public static class PrivateExtensions
//    {
//        public static T GetFieldValue<T>(this object @this, Type type, string name)
//        {
//            var field = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
//            return (T)field.GetValue(@this);
//        }

//        public static T GetPropertyValue<T>(this object @this, Type type, string name)
//        {
//            var property = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
//            return (T)property.GetValue(@this);
//        }
//    }
//}
