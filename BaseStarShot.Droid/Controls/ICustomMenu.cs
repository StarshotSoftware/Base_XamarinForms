using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
namespace BaseStarShot.Controls
{
    public interface ICustomMenu
    {
        int Count();
        void EnableTabs(bool enableTabs);
        void Hide();
        bool IsShowing();
        void SetItemsCountInLine(int count);
        void SetMenuItems(System.Collections.Generic.List<CustomMenuItem> items);
        void SetSelectedIndex(int selectedIndex);
        void SetTextColor(Android.Graphics.Color textColor, Android.Graphics.Color selectedTextColor);
        void Show(Android.Views.View v, Android.Views.GravityFlags screenLocation, bool enableSelectedIndicator);
        void UpdateBadgeCount(int badgeCount);
        void UpdateSelectedTab(Android.App.Activity _activity, System.Collections.Generic.IList<string> activeIcons, System.Collections.Generic.List<string> icons);
        View GetView();
        void SetBadgeLocation(int badgeLocation);
        void SetBackgroundDrawable(string backgroundDrawable, string selectedDrawable);
    }

    public interface OnMenuItemSelectedListener
    {
        void MenuItemSelectedEvent(CustomMenuItem selection, List<ImageView> icons, List<TextView> titleTextView);
    }
}
