using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace BaseStarShot.Controls
{
    public class CustomMenuItem : INotifyPropertyChanged
    {
        private string mCaption = null;
        private string mBadge = null;
        private int mImageResourceId = -1;
        private int mActiveImageId = -1;
        private int mId = -1;

        public string Caption
        {
            get { return mCaption; }
            set
            {
                if (value != mCaption)
                {
                    mCaption = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string Badge
        {
            get { return mBadge; }
            set
            {
                if (value != mBadge)
                {
                    mBadge = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int ImageResourceId
        {
            get { return mImageResourceId; }
            set
            {
                if (value != mImageResourceId)
                {
                    mImageResourceId = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int ActiveImageId
        {
            get { return mActiveImageId; }
            set
            {
                if (value != mActiveImageId)
                {
                    mActiveImageId = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int Id
        {
            get { return mId; }
            set
            {
                if (value != mId)
                {
                    mId = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
