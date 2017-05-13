using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BaseStarShot.Controls
{
    /// <summary>
    /// Provides ability to create a Binding object in XAML attributes.
    /// </summary>
    [ContentProperty("Path")]
    public class BindingExtension : IMarkupExtension
    {
        public string Path { get; set; }

        public BindingMode BindingMode { get; set; }

        public IValueConverter Converter { get; set; }

        public object ConverterParameter { get; set; }

        public string StringFormat { get; set; }

        public object Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return new Binding(Path, mode: BindingMode, converter: Converter, converterParameter: ConverterParameter,
                stringFormat: StringFormat, source: Source);
        }
    }
}
