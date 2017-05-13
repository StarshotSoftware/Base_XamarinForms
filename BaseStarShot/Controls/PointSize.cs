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
    /// Enables uniform sizing by point size.
    /// </summary>
    public class PointSize
    {
        public static readonly double Resolution = ForPlatform.Get(160, 160, 240, 240, 240);
        public static double Multiplier = 1d;

        /// <summary>
        /// The point size.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// The size in inches. This will override the value in the Value property.
        /// </summary>
        public double Inches { get; set; }

        public static implicit operator double(PointSize pointSize)
        {
            if (pointSize.Inches > 0d)
                return pointSize.GetSizeByInches();
            return pointSize.GetSize();
        }

        //public static implicit operator int(PointSize pointSize)
        //{
        //    return (int)(double)pointSize;
        //}

        static PointSize()
        {
            //Multiplier = Device.Idiom == TargetIdiom.Tablet ? 1.5d : 1d;
            //Multiplier = 1d;
        }

        public PointSize()
        {

        }

        public PointSize(double value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Converts point size uniformly across platforms.
        /// </summary>
        /// <param name="pointSize"></param>
        /// <returns></returns>
        public virtual double GetSize()
        {
            return Value * Resolution / 160d * Multiplier;
        }

        /// <summary>
        /// Gets a size that is visually number of inches.
        /// </summary>
        /// <param name="inches"></param>
        /// <returns></returns>
        public double GetSizeByInches()
        {
            
            return Inches * Resolution;
        }

        public override string ToString()
        {
            return this.GetSize().ToString();
        }
    }

    /// <summary>
    /// Enables unifrom sizing of font by point size. 
    /// </summary>
    public class PointFontSize : PointSize
    {
        public PointFontSize()
        {

        }

        public PointFontSize(double value)
        {
            this.Value = value;
        }

        public static implicit operator double(PointFontSize pointSize)
        {
            if (pointSize.Inches > 0d)
                return pointSize.GetSizeByInches();
            return pointSize.GetSize();
        }

        /// <summary>
        /// Converts font point size uniformly across platforms.
        /// </summary>
        /// <param name="pointSize"></param>
        /// <returns></returns>
        public override double GetSize()
        {
            return Value * Resolution / 72d * Multiplier;
        }
    }

    /// <summary>
    /// Enables uniform sizing of grid length by point size.
    /// </summary>
    public class PointGridLength : PointSize
    {
        public PointGridLength()
        {

        }

        public PointGridLength(double value)
        {
            this.Value = value;
        }

        public static implicit operator GridLength(PointGridLength pointGridLength)
        {
            if (pointGridLength.Inches > 0d)
                return new GridLength(pointGridLength.GetSizeByInches(), GridUnitType.Absolute);
            return new GridLength(pointGridLength.GetSize(), GridUnitType.Absolute);
        }
    }

    /// <summary>
    /// Enables uniform sizing of thickness by point size.
    /// </summary>
    public class PointThickness
    {
        /// <summary>
        /// The thickness in point size.
        /// </summary>
        public Thickness Value { get; set; }

        /// <summary>
        /// Get or sets whether to use inches when converting the size.
        /// </summary>
        public bool IsInches { get; set; }

        public PointThickness()
        {

        }

        public PointThickness(double uniformSize)
        {
            Value = new Thickness(uniformSize);
        }

        public PointThickness(double horizontalSize, double verticalSize)
        {
            Value = new Thickness(horizontalSize, verticalSize);
        }

        public PointThickness(double left, double top, double right, double bottom)
        {
            Value = new Thickness(left, top, right, bottom);
        }

        public static implicit operator Thickness(PointThickness pointThickness)
        {
            var leftSize = new PointSize(pointThickness.Value.Left);
            var topSize = new PointSize(pointThickness.Value.Top);
            var rightSize = new PointSize(pointThickness.Value.Right);
            var bottomSize = new PointSize(pointThickness.Value.Bottom);
            if (pointThickness.IsInches)
            {
                leftSize.Inches = leftSize.Value;
                topSize.Inches = topSize.Value;
                rightSize.Inches = rightSize.Value;
                bottomSize.Inches = bottomSize.Value;
            }

            return new Thickness(leftSize, topSize, rightSize, bottomSize);
        }
    }

    /// <summary>
    /// Provides the ability to use point size in XAML attributes.
    /// </summary>
    [ContentProperty("Value")]
    public class PointSizeExtension : IMarkupExtension
    {
        public double Value { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return new PointSize(Value);
        }
    }

    /// <summary>
    /// Provides the ability to use point size in XAML attributes.
    /// </summary>
    [ContentProperty("Value")]
    public class PointSizeIntExtension : IMarkupExtension
    {
        public int Value { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return (int)(double)new PointSize(Value);
        }
    }

    /// <summary>
    /// Provides the ability to use point size grid length in XAML attributes.
    /// </summary>
    [ContentProperty("Value")]
    public class PointGridLengthExtension : IMarkupExtension
    {
        public double Value { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return new PointGridLength(Value);
        }
    }

    /// <summary>
    /// Provides the ability to use point size thickness in XAML attributes.
    /// </summary>
    [ContentProperty("Value")]
    public class PointThicknessExtension : IMarkupExtension
    {
        public string Value { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(Value)) return new Thickness();
            string value = Value;
            while (value.Contains("  "))
                value = value.Replace("  ", " ");
            var thickness = (Thickness)new ThicknessTypeConverter().ConvertFromInvariantString(value.Replace(" ", ","));

            return (Thickness)new PointThickness() { Value = thickness };
        }
    }


    /// <summary>
    /// Enables uniform sizing by point size.
    /// </summary>
    public class NullablePointSize
    {
        public static readonly double Resolution = ForPlatform.Get(160, 160, 240, 240, 240);
        public static readonly double Multiplier;

        /// <summary>
        /// The point size.
        /// </summary>
        public double? Value { get; set; }

        /// <summary>
        /// The size in inches. This will override the value in the Value property.
        /// </summary>
        public double? Inches { get; set; }

        public static implicit operator double?(NullablePointSize pointSize)
        {
            if (pointSize.Inches > 0d)
                return pointSize.GetSizeByInches();
            return pointSize.GetSize();
        }

        //public static implicit operator int(PointSize pointSize)
        //{
        //    return (int)(double)pointSize;
        //}

        static NullablePointSize()
        {
            //Multiplier = Device.Idiom == TargetIdiom.Tablet ? 1.5d : 1d;
            Multiplier = 1d;
        }

        public NullablePointSize()
        {

        }

        public NullablePointSize(double? value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Converts point size uniformly across platforms.
        /// </summary>
        /// <param name="pointSize"></param>
        /// <returns></returns>
        public virtual double? GetSize()
        {
            return Value * Resolution / 160d * Multiplier;
        }

        /// <summary>
        /// Gets a size that is visually number of inches.
        /// </summary>
        /// <param name="inches"></param>
        /// <returns></returns>
        public double? GetSizeByInches()
        {
            return Inches * Resolution;
        }
    }

    /// <summary>
    /// Provides the ability to use point size in XAML attributes.
    /// </summary>
    [ContentProperty("Value")]
    public class NullablePointSizeExtension : IMarkupExtension
    {
        public double? Value { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return new NullablePointSize(Value);
        }
    }

    // <summary>
    /// Provides the ability to use point size in XAML attributes.
    /// </summary>
    [ContentProperty("Value")]
    public class PointFontSizeExtension : IMarkupExtension
    {
        public double Value { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            return new PointFontSize(Value);
        }
    }

}
