//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.ComponentModel;
//using Xamarin.Forms.Platform.Android;
//using Android.Graphics;
//using BaseStarShot.Controls;
//using Android.Views;
//using Android.OS;
//using Xamarin.Forms;

//[assembly: ExportRenderer(typeof(CircleImage), typeof(CircleImageRenderer))]

//namespace BaseStarShot.Controls
//{
//    public class CircleImageRenderer : ImageControlRenderer
//    {
//        private Paint paint;
//        private Path path1, path2;

//        protected override bool DrawChild(Canvas canvas, Android.Views.View child, long drawingTime)
//        {
//            var circleImage = (CircleImage)Element;
//            if (!circleImage.RenderCircle)
//            {
//                return base.DrawChild(canvas, child, drawingTime);
//            }

//            int num = Math.Min(base.Width, base.Height) / 2;
//            const int num2 = 0;
//            num -= num2 / 2;
//            if (path1 == null)
//            {
//                path1 = new Path();
//                path1.AddCircle((float)(base.Width / 2), (float)(base.Height / 2), (float)num, Path.Direction.Ccw);
//            }
//            canvas.Save();
//            canvas.ClipPath(path1);
//            bool result = base.DrawChild(canvas, child, drawingTime);
//            canvas.Restore();
//            if (circleImage.BorderThickness > 0 && circleImage.BorderColor != Xamarin.Forms.Color.Transparent)
//            {
//                if (path2 == null)
//                {
//                    path2 = new Path();
//                    path2.AddCircle((float)(base.Width / 2), (float)(base.Height / 2), (float)num, Path.Direction.Ccw);
//                }
//                if (paint == null)
//                {
//                    paint = new Paint();
//                    paint.AntiAlias = true;
//                    paint.SetStyle(Paint.Style.Stroke);
//                }
//                paint.StrokeWidth = (float)circleImage.BorderThickness;
//                paint.Color = circleImage.BorderColor.ToAndroid();
//                canvas.DrawPath(path2, paint);
//            }
//            return result;
//        }

//        protected override void OnElementChanged(ElementChangedEventArgs<Image> e)
//        {
//            base.OnElementChanged(e);
//            if (e.OldElement == null && Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
//            {
//                var circleImage = (CircleImage)Element;
//                if (circleImage.RenderCircle)
//                    SetLayerType(LayerType.Software, null);

//                circleImage.PropertyChanged += circleImage_PropertyChanged;
//            }
//        }

//        void circleImage_PropertyChanged(object sender, PropertyChangedEventArgs e)
//        {
//            if (e.PropertyName == "RenderCircle")
//            {
//                var circleImage = (CircleImage)Element;
//                if (circleImage != null)
//                    SetLayerType(circleImage.RenderCircle ? LayerType.Software : LayerType.Hardware, null);
//            }
//        }

//        protected override void Dispose(bool disposing)
//        {
//            base.Dispose(disposing);

//            if (paint != null)

//                paint.Dispose();
//            if (path1 != null)
//                path1.Dispose();
//            if (path2 != null)
//                path2.Dispose();
//        }
//    }
//}