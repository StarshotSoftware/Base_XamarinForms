using Android.Media;
using Android.Widget;
using BaseStarShot.Controls;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using System.ComponentModel;
using Android.Views;
using Android.App;

[assembly: ExportRenderer(typeof(BaseStarShot.Controls.VideoView), typeof(VideoViewRenderer))]
namespace BaseStarShot.Controls
{
    public class VideoViewRenderer : ViewRenderer<BaseStarShot.Controls.VideoView, Android.Widget.VideoView>
    {
        private Android.Widget.VideoView videoView = null;

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
                return;

            if (string.IsNullOrEmpty(Element.Source))
                throw new ArgumentException("Source is required.");

            videoView = new Android.Widget.VideoView(this.Context);

            Android.Net.Uri uri = null;
            if (Element.IsFromUrl)
            {
                uri = Android.Net.Uri.Parse(Element.Source);
            }
            else
            {
                uri = Android.Net.Uri.Parse("android.resource://" + this.Context.PackageName + "/raw/" + Element.Source);
            }
            videoView.SetVideoURI(uri);
            videoView.SetOnPreparedListener(new VideoLoop(Element.Repeat, this.Context as Activity, videoView));

            if (Element.ShowControl)
            {
                var mediaController = new MediaController(this.Context);
                mediaController.SetMediaPlayer(videoView);
                videoView.SetMediaController(mediaController);
                videoView.Start();
                mediaController.Show(2000);
            }
            else
            {
                videoView.SetMediaController(null);

                StartVideo(Element.Playing);
            }

            SetNativeControl(videoView);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Element == null || Control == null)
                return;

            if (e.PropertyName == "Playing")
            {
                StartVideo(Element.Playing);
            }

        }
        void StartVideo(bool start)
        {
            if (start)
            {
                videoView.Visibility = Android.Views.ViewStates.Visible;
                videoView.Start();
            }
            else
            {
                videoView.Visibility = Android.Views.ViewStates.Gone;
                videoView.StopPlayback();
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (videoView != null)
            {
                videoView.Dispose();
            }
        }

    }

    public class VideoLoop : Java.Lang.Object, Android.Media.MediaPlayer.IOnPreparedListener
    {
        private bool repeat;
        private Activity activity;
        private Android.Widget.VideoView videoView;

        public VideoLoop(bool repeat, Activity activity, Android.Widget.VideoView videoView)
        {
            this.repeat = repeat;
            this.activity = activity;
            this.videoView = videoView;
        }

        public void OnPrepared(MediaPlayer player)
        {
            player.Looping = this.repeat;

            //var windowManager = this.activity.WindowManager;

            //int videoWidth = player.VideoWidth;
            //int videoHeight = player.VideoHeight;
            //float videoProportion = (float)videoWidth / (float)videoHeight;
            //int screenWidth = windowManager.DefaultDisplay.Width;
            //int screenHeight = windowManager.DefaultDisplay.Height;
            //float screenProportion = (float)screenWidth / (float)screenHeight;
            //var lp = this.videoView.LayoutParameters;

            //if (videoProportion > screenProportion)
            //{
            //    lp.Width = screenWidth;
            //    lp.Height = (int)((float)screenWidth / videoProportion);
            //}
            //else
            //{
            //    lp.Width = (int)(videoProportion * (float)screenHeight);
            //    lp.Height = screenHeight;
            //}
            //videoView.LayoutParameters = lp;
        }

    }
}