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
    /**
  * CustomMenu class
  *
  * This is the class that manages our menu items and the popup window.
  *
  * @category   Helper
  * @author     William J Francis (w.j.francis@tx.rr.com)
  * @copyright  Enjoy!
  * @version    1.0
  */
    public class CustomMenuPopUp : BaseStarShot.Controls.ICustomMenu
    {
        /**
         * Some global variables.
         */
        private readonly List<ImageView> imageViewList = new List<ImageView>();
        private readonly List<TextView> titleViewList = new List<TextView>();
        private List<TextView> badgeTextViewList = new List<TextView>();

        private List<CustomMenuItem> mMenuItems;
        private OnMenuItemSelectedListener mListener = null;
        private Context mContext = null;
        private LayoutInflater mLayoutInflater = null;
        private PopupWindow mPopupWindow = null;
        private static bool mIsShowing = false;
        private int mItemsCount = 4;
        private int selectedIndex = -1;
        private TableLayout table;
        private bool EnableClickingTab = true;

        private Color selectedTextColor, textColor;
        private string backgroundDrawable, selectedDrawable;

        /**
         * Use this method to determine if the menu is currently displayed to the user.
         * @return bool isShowing
         */
        public bool IsShowing() { return mIsShowing; }

        public void SetTextColor(Color textColor, Color selectedTextColor)
        {
            this.selectedTextColor = selectedTextColor;
            this.textColor = Color.White;
        }

        public void SetSelectedIndex(int selectedIndex)
        {
            this.selectedIndex = selectedIndex;

            if (this.selectedIndex < imageViewList.Count)
            {
                if (mMenuItems.ElementAt(selectedIndex).ActiveImageId > 0)
                    imageViewList.ElementAt(selectedIndex).SetImageResource(mMenuItems.ElementAt(selectedIndex).ActiveImageId);

                titleViewList.ElementAt(selectedIndex).SetTextColor(selectedTextColor);
            }

        }

        public void UpdateSelectedTab(Activity _activity, IList<string> activeIcons, List<string> icons)
        {
            if (imageViewList != null && titleViewList != null)
            {

                for (int i = 0; i < imageViewList.Count; i++)
                {
                    ImageView icon = imageViewList.ElementAt(i);
                    TextView title = titleViewList.ElementAt(i);

                    var ivParent = icon.Parent.Parent as Android.Widget.RelativeLayout;

                    if (selectedIndex == i)
                    {
                        if (activeIcons.Count > 0)
                            icon.SetImageResource(UIHelper.GetDrawableResource(activeIcons.ElementAt(i)));
                        title.SetTextColor(selectedTextColor);

                        if (!string.IsNullOrEmpty(selectedDrawable))
                            ivParent.Background = _activity.Resources.GetDrawable(UIHelper.GetResource(UIHelper.Drawable, selectedDrawable));
                    }
                    else
                    {
                        if (icons.Count > 0)
                            icon.SetImageResource(UIHelper.GetDrawableResource(icons.ElementAt(i)));
                        title.SetTextColor(textColor);

                        if (!string.IsNullOrEmpty(backgroundDrawable))
                            ivParent.Background = _activity.Resources.GetDrawable(UIHelper.GetResource(UIHelper.Drawable, backgroundDrawable));
                    }
                }
            }
        }

        /**
         * Use this method to decide how many of your menu items you'd like one a single line.
         * This setting in particular applied to portrait orientation.
         * @param int count
         * @return void
         */
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
        public CustomMenuPopUp(Context context, OnMenuItemSelectedListener listener, LayoutInflater lo)
        {
            mListener = listener;
            mMenuItems = new List<CustomMenuItem>();
            mContext = context;
            mLayoutInflater = lo;

        }

        private int badgeLocation;

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

        public int Count()
        {
            return mMenuItems.Count;
        }

        public void EnableTabs(bool enableTabs)
        {
            this.EnableClickingTab = enableTabs;
        }

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
            if (itemCount < 1) return; //no menu items to show
            if (mPopupWindow != null) return; //already showing
            if (imageViewList.Count > 0)
                imageViewList.Clear();

            if (badgeTextViewList.Count > 0)
                badgeTextViewList.Clear();

            if (titleViewList.Count > 0)
                titleViewList.Clear();

            View mView = mLayoutInflater.Inflate(UIHelper.GetLayoutResource("custom_menu"), null);
            table = (TableLayout)mView.FindViewById(UIHelper.GetId("custom_menu_table"));

            IWindowManager windowManager = mContext.GetSystemService(Context.WindowService) as IWindowManager;

            mPopupWindow = new PopupWindow(mView, LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent, false);
            mPopupWindow.AnimationStyle = Android.Resource.Style.AnimationDialog;
            mPopupWindow.ShowAtLocation(v, screenLocation, 0, 0);

            table.RemoveAllViews();

            TableRow row = null;
            TextView tv = null;
            TextView tvBadge = null;
            ImageView iv = null;
            //create headers
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

                tvBadge = (TextView)itemLayout.FindViewById(UIHelper.GetId("custom_badge"));

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

                if (!string.IsNullOrEmpty(selectedDrawable))
                {
                    var ivParent = (RelativeLayout)imageViewList[selectedIndex].Parent.Parent;
                    ivParent.Background = mContext.Resources.GetDrawable(UIHelper.GetResource(UIHelper.Drawable, selectedDrawable));
                }
            }

            table.AddView(row);

            mIsShowing = true;
        }

        private void Reshow()
        {
            table.Visibility = ViewStates.Visible;
        }

        public void Hide()
        {
            if (mPopupWindow != null)
            {
                table.Visibility = ViewStates.Gone;
                //mPopupWindow.ContentView.Visibility = ViewStates.Gone;

                //mPopupWindow.Dismiss();
                //mPopupWindow = null;

                mIsShowing = false;
            }

            return;
        }



        public View GetView()
        {
            return null;
        }


        public void SetBadgeLocation(int badgeLocation)
        {
            this.badgeLocation = badgeLocation;
        }

        public void SetBackgroundDrawable(string backgroundDrawable, string selectedDrawable)
        {
            this.backgroundDrawable = backgroundDrawable;
            this.selectedDrawable = selectedDrawable;
        }
    }
}
