using BaseStarShot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class Switch : Xamarin.Forms.Switch
    {
        public static readonly BindableProperty TextOnProperty = BindableProperty.Create<Switch, string>(p => p.TextOn, "");
        public static readonly BindableProperty TextOffProperty = BindableProperty.Create<Switch, string>(p => p.TextOff, "");
        public static BindableProperty TrackProperty = BindableProperty.Create<Switch, FileImageSource>(p => p.SwitchTrack, null);
        public static BindableProperty TrackDrawableProperty = BindableProperty.Create<Switch, FileImageSource>(p => p.TrackDrawable, null);
        public static BindableProperty ThumbDrawableProperty = BindableProperty.Create<Switch, FileImageSource>(p => p.ThumbDrawable, null);
        //
        public string TextOn
        {
            get { return (string)GetValue(TextOnProperty); }
            set { SetValue(TextOnProperty, value); }
        }

        public string TextOff
        {
            get { return (string)GetValue(TextOffProperty); }
            set { SetValue(TextOffProperty, value); }
        }

        public FileImageSource SwitchTrack
        {
            get { return (FileImageSource)GetValue(TrackProperty); }
            set { SetValue(TrackProperty, value); }
        }

        public FileImageSource TrackDrawable
        {
            get { return (FileImageSource)GetValue(TrackDrawableProperty); }
            set { SetValue(TrackDrawableProperty, value); }
        }

        public FileImageSource ThumbDrawable
        {
            get { return (FileImageSource)GetValue(ThumbDrawableProperty); }
            set { SetValue(ThumbDrawableProperty, value); }
        }
    }
}
