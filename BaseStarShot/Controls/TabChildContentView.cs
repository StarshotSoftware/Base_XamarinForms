using System;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class TabChildContentView : ContentView
    {
        internal void Show()
        {
            OnSelected();
        }

        internal void Hide()
        {
            OnDeselected();
        }

        protected virtual void OnSelected()
        {

        }

        protected virtual void OnDeselected()
        {

        }
    }
}

