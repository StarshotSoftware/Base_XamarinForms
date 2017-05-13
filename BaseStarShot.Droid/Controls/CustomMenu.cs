using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;
using Android.OS;
using Android.Views;
using Android.Widget;
using BaseStarShot.Helpers;
using BaseStarShot.Services;
using Java.Lang.Reflect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaseStarShot.Controls
{
    public class CustomMenu : BaseStarShot.Controls.ICustomMenu
    {
        private readonly List<ImageView> imageViewList = new List<ImageView>();
        private readonly List<TextView> titleViewList = new List<TextView>();
        private readonly List<TextView> indicatorList = new List<TextView>();
        private List<TextView> badgeTextViewList = new List<TextView>();

        private List<CustomMenuItem> mMenuItems;
        private OnMenuItemSelectedListener mListener = null;
        private Context mContext = null;
        private LayoutInflater mLayoutInflater = null;
        private static bool mIsShowing = false;
        private int mItemsCount = 4;
        private int selectedIndex = -1;
        private TableLayout table;
        private bool EnableClickingTab = true;
        private LinearLayout tableParent;
        private string backgroundDrawable, selectedDrawable;

        private Color selectedTextColor, textColor;

        public bool IsShowing() { return mIsShowing; }

        private int badgeLocation;

        public void SetTextColor(Color textColor, Color selectedTextColor)
        {
            this.selectedTextColor = selectedTextColor;
            this.textColor = textColor;
        }

        public void SetBadgeLocation(int badgeLocation)
        {
            if (badgeLocation == -1) return;
            this.badgeLocation = badgeLocation;
        }

        public void SetBackgroundDrawable(string backgroundDrawable, string selectedDrawable)
        {
            this.backgroundDrawable = backgroundDrawable;
            this.selectedDrawable = selectedDrawable;
        }

        public int Count()
        {
            return mMenuItems.Count;
        }

        public void SetSelectedIndex(int selectedIndex)
        {
            this.selectedIndex = selectedIndex;

            if (this.selectedIndex < imageViewList.Count)
            {
                if (mMenuItems.ElementAt(selectedIndex).ActiveImageId > 0)
                    imageViewList.ElementAt(selectedIndex).SetImageResource(mMenuItems.ElementAt(selectedIndex).ActiveImageId);

                titleViewList.ElementAt(selectedIndex).SetTextColor(selectedTextColor);
                indicatorList.ElementAt(selectedIndex).SetBackgroundColor(selectedTextColor);
            }

        }

        public void UpdateSelectedTab(Activity _activity, IList<string> activeIcons, List<string> icons)
        {
            if (table != null)
                table.Visibility = ViewStates.Visible;

            if (imageViewList != null && titleViewList != null)
            {

                for (int i = 0; i < titleViewList.Count; i++)
                {
                    ImageView icon = imageViewList.ElementAt(i);
                    TextView title = titleViewList.ElementAt(i);
                    TextView indicator = indicatorList.ElementAt(i);

                    var ivParent = icon.Parent.Parent as Android.Widget.RelativeLayout;

                    if (selectedIndex == i)
                    {
                        if (activeIcons.Count > 0)
                            icon.SetImageResource(UIHelper.GetDrawableResource(activeIcons.ElementAt(i)));
                        title.SetTextColor(selectedTextColor);
                        indicator.SetBackgroundColor(selectedTextColor);
                        if (!string.IsNullOrEmpty(selectedDrawable))
                            ivParent.Background = _activity.Resources.GetDrawable(UIHelper.GetResource(UIHelper.Drawable, selectedDrawable));
                        //ivParent.Background = _activity.Resources.GetDrawable(UIHelper.GetResource(UIHelper.Drawable, "my_menu_item_pressed"));
                    }
                    else
                    {
                        if (icons.Count > 0)
                            icon.SetImageResource(UIHelper.GetDrawableResource(icons.ElementAt(i)));
                        title.SetTextColor(textColor);
                        indicator.SetBackgroundColor(Color.Transparent);
                        if (!string.IsNullOrEmpty(backgroundDrawable))
                            ivParent.Background = _activity.Resources.GetDrawable(UIHelper.GetResource(UIHelper.Drawable, backgroundDrawable));
                        //ivParent.Background = _activity.Resources.GetDrawable(UIHelper.GetResource(UIHelper.Drawable, "segment_button_unpressed"));
                    }
                }
            }
        }

        public void SetItemsCountInLine(int count) { mItemsCount = count; }

        /**
         * Use this method to assign your menu items. You can only call this when the menu is hidden.
         * @param ArrayList<CustomMenuItem> items
         * @return void
         * @throws Exception "Menu list may not be modified while menu is displayed."
         */
        public void SetMenuItems(List<CustomMenuItem> items)
        {
            try
            {
                if (mIsShowing)
                {
                    throw new Exception("Menu list may not be modified while menu is displayed.");
                }
                mMenuItems = items;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /**
         * This is our constructor.  Note we require a layout inflater.  There is probably a way to
         * grab one of these from the local context but I failed to find it.
         * @param Context context
         * @param OnMenuItemSelectedListener listener
         * @param LayoutInflater lo
         * @return void
         */
        public CustomMenu(Context context, OnMenuItemSelectedListener listener, LayoutInflater lo)
        {
            mListener = listener;
            mMenuItems = new List<CustomMenuItem>();
            mContext = context;
            mLayoutInflater = lo;

        }

        public void UpdateBadgeCount(int badgeCount)
        {
            if (badgeTextViewList.Count <= 0) return;

            if (badgeCount <= 0)
            {
                badgeTextViewList[badgeLocation].Visibility = ViewStates.Gone;
                return;
            }

            if (badgeTextViewList[badgeLocation].Visibility == ViewStates.Gone)
                badgeTextViewList[badgeLocation].Visibility = ViewStates.Visible;

            badgeTextViewList[badgeLocation].Text = badgeCount.ToString();

        }

        public void EnableTabs(bool enableTabs)
        {
            this.EnableClickingTab = enableTabs;
        }

        View mView;
        /**
         * Display your menu. Not we require a view from the parent.  This is so we can get
         * a window token.  It doesn't matter which view on the parent is passed in.
         * @param View v
         * @return void
         */
        public void Show(View v, GravityFlags screenLocation, bool enableSelectedIndicator)
        {
            if (table != null && table.Visibility == ViewStates.Gone)
            {
                Reshow();
                return;
            }

            if (mIsShowing) return;

            int itemCount = mMenuItems.Count;
            if (itemCount < 1) return;

            if (imageViewList.Count > 0) imageViewList.Clear();
            if (badgeTextViewList.Count > 0) badgeTextViewList.Clear();
            if (titleViewList.Count > 0) titleViewList.Clear();
            if (indicatorList.Count > 0) indicatorList.Clear();

            mView = mLayoutInflater.Inflate(UIHelper.GetResource(UIHelper.Layout, "custom_menu"), null);
            tableParent = mView.FindViewById(UIHelper.GetResource(UIHelper.Id, "table_parent")) as LinearLayout;

            if (mView == null) return;

            var tableId = UIHelper.GetResource(UIHelper.Id, "custom_menu_table");
            table = (TableLayout)mView.FindViewById(tableId);
            if (table == null) return;

            var windowManager = (mContext as Activity).WindowManager;

            table.RemoveAllViews();

            TableRow row = null;
            TextView tv = null;
            TextView tvBadge = null;
            TextView selected_item_indicator = null;

            ImageView iv = null;

            row = new TableRow(mContext);
            row.LayoutParameters = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.WrapContent);
            for (int j = 0; j < mItemsCount; j++)
            {
                if (j >= itemCount) break;

                CustomMenuItem cmi = mMenuItems.ElementAt(j);
                View itemLayout = mLayoutInflater.Inflate(UIHelper.GetLayoutResource("custom_menu_item"), null);
                tv = (TextView)itemLayout.FindViewById(UIHelper.GetId("custom_menu_item_caption"));
                tv.SetTextColor(this.textColor);

                if (String.IsNullOrEmpty(cmi.Caption))
                {
                    tv.Visibility = ViewStates.Gone;
                }
                else
                {
                    tv.Text = cmi.Caption;
                    tv.Visibility = ViewStates.Visible;
                }
                titleViewList.Add(tv);

                iv = (ImageView)itemLayout.FindViewById(UIHelper.GetId("custom_menu_item_icon"));

                if (cmi.ImageResourceId > 0)
                    iv.SetImageResource(cmi.ImageResourceId);

                iv.Tag = cmi.Id;
                imageViewList.Add(iv);

                selected_item_indicator = itemLayout.FindViewById(UIHelper.GetId("selected_item_indicator")) as TextView;
                if (enableSelectedIndicator)
                    selected_item_indicator.Visibility = ViewStates.Visible;

                indicatorList.Add(selected_item_indicator);

                tvBadge = itemLayout.FindViewById(UIHelper.GetId("custom_badge")) as TextView;

                if (String.IsNullOrEmpty(cmi.Badge))
                {
                    tvBadge.Visibility = ViewStates.Gone;
                }
                else
                {
                    int badgeCount = int.Parse(cmi.Badge);
                    if (badgeCount > 0)
                    {
                        tvBadge.Text = badgeCount.ToString();
                        //tvBadge.SetBackgroundDrawable(drawbg(tvBadge));
                        tvBadge.Visibility = ViewStates.Visible;

                        //if (Build.VERSION.SdkInt <= Android.OS.BuildVersionCodes.JellyBean)
                        //{
                        //    //TODO: Fix x location of badge
                        //    tvBadge.SetX(iv.GetX() + iv.Drawable.IntrinsicWidth);
                        //}
                    }
                }
                badgeTextViewList.Add(tvBadge);

                itemLayout.Click += delegate(object sender, EventArgs e)
                {
                    if (EnableClickingTab)
                    {
                        selectedIndex = cmi.Id;
                        mListener.MenuItemSelectedEvent(cmi, imageViewList, titleViewList);
                    }
                };

                row.AddView(itemLayout);
            }

            if (selectedIndex >= 0)
            {
                if (mMenuItems.ElementAt(selectedIndex).ActiveImageId > 0)
                    imageViewList.ElementAt(selectedIndex).SetImageResource(mMenuItems.ElementAt(selectedIndex).ActiveImageId);

                titleViewList.ElementAt(selectedIndex).SetTextColor(selectedTextColor);
                indicatorList.ElementAt(selectedIndex).SetBackgroundColor(selectedTextColor);

                //var ivParent = (RelativeLayout)imageViewList[selectedIndex].Parent.Parent;
                //ivParent.Background = mContext.Resources.GetDrawable(UIHelper.GetDrawableResource("my_menu_item_pressed"));
            }

            table.AddView(row);

            mIsShowing = true;
        }

        public View GetView()
        {
            return tableParent;
        }

        private void Reshow()
        {
            table.Visibility = ViewStates.Visible;
        }

        public void Hide()
        {
            if (table != null)
            {
                table.Visibility = ViewStates.Gone;
                mIsShowing = false;
            }

            return;
        }

    }
}
