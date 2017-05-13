using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;
using Foundation;
using System.IO;
using MediaPlayer;
using AVFoundation;
using CoreGraphics;

namespace BaseStarShot.Controls
{
    public class VideoViewRenderer : ViewRenderer<VideoView, UIView>
    {
        MPMoviePlayerController movieController;
        AVPlayer player;
        AVPlayerLayer playerLayer;
        AVAsset asset;
        AVPlayerItem playerItem;

        NSObject playerItemReachEndNotitication;
        UIButton playButton = null;

        protected override void OnElementChanged(ElementChangedEventArgs<VideoView> e)
        {
            if (e.NewElement != null)
            {
                if (string.IsNullOrEmpty(Element.Source))
                    throw new ArgumentException("Source is required.");

                if (base.Control == null)
                {
                    var view = new UIView();
                    SetNativeControl(view);
                    
                    if (Element.ShowControl)
                    {
                        if (Element.IsFromUrl)
                        {
                            if (movieController == null)
                            {
                                var url = NSUrl.FromString(Element.Source);
                                movieController = new MPMoviePlayerController();
                                movieController.ShouldAutoplay = true;
                                movieController.Fullscreen = true;
                                movieController.ControlStyle = MPMovieControlStyle.Embedded;
                                movieController.PrepareToPlay();
                                movieController.SourceType = MPMovieSourceType.Streaming;
                                movieController.ContentUrl = url;
                                view.Add(movieController.View);

                                movieController.Play();
                                movieController.SetFullscreen(true, true);
                            }
                        }
                    }
                    else
                    {
                        if (playerItem == null)
                        {
                            asset = AVAsset.FromUrl(NSUrl.FromFilename(Element.Source));
                            playerItem = new AVPlayerItem(asset);
                            //					AVPlayerItem.DidPlayToEndTimeNotification;

                            player = new AVPlayer(playerItem);
                            player.ActionAtItemEnd = AVPlayerActionAtItemEnd.None;
                            playerLayer = AVPlayerLayer.FromPlayer(player);


                            view.Layer.AddSublayer(playerLayer);


                            SetRepeat();
                            SetPlaying();
                        }
                    }
                }
            }

            base.OnElementChanged(e);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if (Control.Frame.Width > 0 && playerLayer != null)
            {
                //				movieController.Play();
                //				player.Play();
                playerLayer.Frame = Control.Bounds;
            }
            if (movieController != null && Control.Frame.Width > 0)
            {
                movieController.View.Frame = Control.Bounds;
            }
        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Element == null || Control == null)
                return;

            switch (e.PropertyName)
            {
                case "Repeat":
                    if (!Element.ShowControl)
                        SetRepeat();
                    break;
                case "Playing":
                    if (!Element.ShowControl)
                        SetPlaying();
                    break;
                case "Renderer":
                    if (Element.Height > 0)
                    {
                        Dispose();
                    }
                    break;
                default:
                    break;
            }
        }

        void PlayerItemDidReachEnd(NSNotification notification)
        {
            ((AVPlayerItem)notification.Object).Seek(CoreMedia.CMTime.Zero);
        }

        void SetRepeat()
        {
            if (Element.Repeat)
            {
                //				movieController.RepeatMode = MPMovieRepeatMode.One;
                playerItemReachEndNotitication = NSNotificationCenter.DefaultCenter.AddObserver(AVPlayerItem.DidPlayToEndTimeNotification, PlayerItemDidReachEnd);
            }
            else
            {
                //				movieController.RepeatMode = MPMovieRepeatMode.None;
                if (playerItemReachEndNotitication != null)
                {
                    NSNotificationCenter.DefaultCenter.RemoveObserver(playerItemReachEndNotitication);
                    playerItemReachEndNotitication = null;
                }
            }
        }

        void SetPlaying()
        {
            if (Element.Playing)
                player.Play();
            //				movieController.Play();
            else
                player.Pause();
            //				movieController.Stop();
        }

        protected override void Dispose(bool disposing)
        {
            DisposeItem();
            base.Dispose(disposing);
        }
        public void DisposeItem()
        {
            if (playerItemReachEndNotitication != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(playerItemReachEndNotitication);
                playerItemReachEndNotitication = null;
            }

            if (movieController != null)
            {
                movieController.Stop();
                movieController.Dispose();
            }
        }
    }
}

