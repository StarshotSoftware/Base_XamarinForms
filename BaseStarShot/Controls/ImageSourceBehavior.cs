using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    public class ImageSourceBehavior : Behavior<Image>
    {
        public static BindableProperty SourceAttachedProperty =
            BindableProperty.CreateAttached("Source", typeof(string), typeof(ImageSourceBehavior), default(string), propertyChanged: SourceChanged);

        public string Source { get; set; }

        static void SourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var image = bindable as Image;
            if (image != null)
            {
                image.Source = null;
                image.Source = (string)newValue;
            }
        }

        protected override void OnAttachedTo(Image bindable)
        {
            base.OnAttachedTo(bindable);

            bindable.SetBinding(SourceAttachedProperty, new Binding(Source));
        }

        protected override void OnDetachingFrom(Image bindable)
        {
            base.OnDetachingFrom(bindable);

            bindable.RemoveBinding(SourceAttachedProperty);
        }
    }
}
