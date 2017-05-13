using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using Android.Text;
using BaseStarShot.Services;
using Android.Graphics.Drawables;
using Android.Graphics.Drawables.Shapes;

[assembly: ExportRenderer(typeof(BaseStarShot.Controls.RoundedBoxView), typeof(BaseStarShot.Controls.RoundedBoxViewRenderer))]
namespace BaseStarShot.Controls
{
    public class RoundedBoxViewRenderer : BoxRenderer
    {
        //Paint paint;
        Paint circlePaint;

        protected BaseStarShot.Controls.RoundedBoxView Base { get { return (BaseStarShot.Controls.RoundedBoxView)Element; } }

        public RoundedBoxViewRenderer()
        {
            //paint = new Paint();
            circlePaint = new Paint();
            this.SetWillNotDraw(false);
        }

        public override void Draw(Canvas canvas)
        {
            //RoundedBoxView rbv = (RoundedBoxView)this.Element;

            //int value;
            //try
            //{
            //    value = int.Parse(rbv.Text.Trim());
            //}
            //catch (Exception)
            //{
            //    value = 0;
            //}

            //if (value <= 0) return;

            //if (value < 10)
            //{
            //    rbv.WidthRequest = 20;
            //}
            //else if (value >= 10 && value < 100)
            //{
            //    rbv.WidthRequest = 25;
            //}
            //else
            //{
            //    rbv.WidthRequest = 30;
            //}

            //Rect rc = new Rect(0, 0, UIHelper.ConvertDPToPixels(rbv.WidthRequest), UIHelper.ConvertDPToPixels(rbv.HeightRequest));

            //GetDrawingRect(rc);

            //Rect interior = rc;
            //interior.Inset((int)rbv.StrokeThickness, (int)rbv.StrokeThickness);

            ////            paint.Color = Android.Graphics.Color.White;
            ////            paint.TextSize = UIHelper.ConvertDPToPixels(rbv.TextSize);
            ////            paint.AntiAlias = true;
            ////            paint.TextAlign = Paint.Align.Center;
            ////
            ////            Rect bounds = new Rect();
            ////            paint.GetTextBounds(rbv.Text, 0, rbv.Text.Length, bounds);

            //int radius = UIHelper.ConvertDPToPixels(rbv.CornerRadius);
            ////int radius = bounds.Width();

            circlePaint.Color = Base.BackgroundColor.ToAndroid();
            circlePaint.AntiAlias = true;
            circlePaint.SetStyle(Paint.Style.Fill);

            //canvas.DrawRoundRect(new RectF(interior), radius, radius, circlePaint);
            ////canvas.DrawCircle(x, y - (bounds.Height() / 2), radius, circlePaint);

            //circlePaint.Color = rbv.Stroke.ToAndroid();
            //circlePaint.StrokeWidth = (float)rbv.StrokeThickness;
            //circlePaint.SetStyle(Paint.Style.Stroke);

            //canvas.DrawRoundRect(new RectF(interior), radius, radius, circlePaint);
            ////canvas.DrawCircle(x, y - (bounds.Height() / 2), radius, circlePaint); //FOR the border

            ////canvas.DrawText(rbv.Text, rc.CenterX(), (UIHelper.ConvertDPToPixels(rbv.HeightRequest) / 2) + (bounds.Height() / 2), paint);

            RectF rect = new RectF(0, 0, this.Width, this.Height);
            int cornerRadius = UIHelper.ConvertDPToPixels(Base.CornerRadius);
            canvas.DrawRoundRect(rect, cornerRadius, cornerRadius, circlePaint);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
        {
            base.OnElementChanged(e);


        }

        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == RoundedBoxView.CornerRadiusProperty.PropertyName ||
                e.PropertyName == RoundedBoxView.StrokeProperty.PropertyName ||
                e.PropertyName == RoundedBoxView.StrokeThicknessProperty.PropertyName ||
                e.PropertyName == BoxView.ColorProperty.PropertyName)
            {
                Invalidate();
            }
        }
    }
}