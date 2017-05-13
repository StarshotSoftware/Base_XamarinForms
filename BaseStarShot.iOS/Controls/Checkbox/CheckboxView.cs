using Foundation;
using UIKit;
using CoreGraphics;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
	/// <summary>
	/// Class CheckBoxView.
	/// </summary>
	[Register("CheckBoxView")]
	public class CheckBoxView : UIButton
	{
        //private double widthRequest;
        //private double heightRequest;
        //private double? spacing;

        private CheckBox checkBox;

        private string checkedImage;
        private string uncheckImage;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckBoxView"/> class.
        /// </summary>
        public CheckBoxView()
		{
			Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CheckBoxView"/> class.
		/// </summary>
		/// <param name="bounds">The bounds.</param>
		public CheckBoxView(CGRect bounds, string checkedImage, string uncheckImage, CheckBox checkBox/**, double widthRequest, double heightRequest, double? spacing**/)
			: base(bounds)
		{
            this.checkedImage = checkedImage;
            this.uncheckImage = uncheckImage;

            this.checkBox = checkBox;
            //this.widthRequest = widthRequest;
            //this.heightRequest = heightRequest;
            //this.spacing = spacing;

			Initialize();
		}

		/// <summary>
		/// Sets the checked title.
		/// </summary>
		/// <value>The checked title.</value>
		public string CheckedTitle
		{
			set
			{
				SetTitle(value + " ", UIControlState.Selected);
			}
		}

		/// <summary>
		/// Sets the unchecked title.
		/// </summary>
		/// <value>The unchecked title.</value>
		public string UncheckedTitle
		{
			set
			{
				SetTitle(value + " ", UIControlState.Normal);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="CheckBoxView"/> is checked.
		/// </summary>
		/// <value><c>true</c> if checked; otherwise, <c>false</c>.</value>
		public bool Checked
		{
			set { Selected = value; }
			get { return Selected; }
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		void Initialize()
		{
			AdjustEdgeInsets();
			ApplyStyle();

			TouchUpInside += (sender, args) => Selected = !Selected;
		}

		/// <summary>
		/// Adjusts the edge insets.
		/// </summary>
		void AdjustEdgeInsets()
		{
			float Inset = 8f;

            if (this.checkBox.Spacing.HasValue)
                Inset = (float)this.checkBox.Spacing.Value;

			HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
			ImageEdgeInsets = new UIEdgeInsets(0f, 0f, 0f, 0f);
			TitleEdgeInsets = new UIEdgeInsets(0f, Inset * 2, 0f, 0f);
		}

		/// <summary>
		/// Applies the style.
		/// </summary>
		void ApplyStyle()
		{
            //todo: update this, Images/checkbox_checked.png is currently hardcoded image location

            if (!string.IsNullOrWhiteSpace(this.checkedImage) && !string.IsNullOrWhiteSpace(this.uncheckImage))
            {

                var selectedImage = UIImage.FromBundle(checkedImage);
                var normaleImage = UIImage.FromBundle(uncheckImage);

                if (this.checkBox.ImageWidth > 0 && this.checkBox.ImageHeight > 0)
                {
                    SetCheckBoxImage(selectedImage, UIControlState.Selected);
                    SetCheckBoxImage(normaleImage, UIControlState.Normal);
                }
                else
                {
                    SetImage(selectedImage, UIControlState.Selected);
                    SetImage(normaleImage, UIControlState.Normal);
                }
            }
		}

        void SetCheckBoxImage(UIImage image, UIControlState state)
        {
            UIGraphics.BeginImageContextWithOptions(new CGSize(this.checkBox.ImageWidth, this.checkBox.ImageHeight), false, 0);
            image.Draw(new CGRect(0, 0, this.checkBox.ImageWidth, this.checkBox.ImageHeight));
            var resizedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
            SetImage(resizedImage, state);
        }
	}
}

