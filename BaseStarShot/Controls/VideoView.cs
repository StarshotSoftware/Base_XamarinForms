using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class VideoView : View
    {
        public static readonly BindableProperty ShowControlProperty =
            BindableProperty.Create<VideoView, bool>(p => p.ShowControl, false);

        public static readonly BindableProperty IsFromUrlProperty =
            BindableProperty.Create<VideoView, bool>(p => p.IsFromUrl, false);

        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create<VideoView, string>(p => p.Source, null);

        public static readonly BindableProperty PlayingProperty =
            BindableProperty.Create<VideoView, bool>(p => p.Playing, false);

        public static readonly BindableProperty RepeatProperty =
            BindableProperty.Create<VideoView, bool>(p => p.Repeat, false);

        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public bool Playing
        {
            get { return (bool)GetValue(PlayingProperty); }
            set { SetValue(PlayingProperty, value); }
        }

        public bool Repeat
        {
            get { return (bool)GetValue(RepeatProperty); }
            set { SetValue(RepeatProperty, value); }
        }

        public bool IsFromUrl
        {
            get { return (bool)GetValue(IsFromUrlProperty); }
            set { SetValue(IsFromUrlProperty, value); }
        }

        public bool ShowControl
        {
            get { return (bool)GetValue(ShowControlProperty); }
            set { SetValue(ShowControlProperty, value); }
        }
        
    }
}