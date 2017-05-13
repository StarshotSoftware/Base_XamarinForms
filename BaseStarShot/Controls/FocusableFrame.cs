using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace BaseStarShot.Controls
{
    /// <summary>
    /// A focusable frame.
    /// </summary>
    public class FocusableFrame : Frame
    {
        public event EventHandler Tapped = delegate { };

        public FocusableFrame()
        {
            // Need to set background so it will become a hit target in win phone;
            this.BackgroundColor = Color.Transparent;

            this.Focused += FocusableFrame_Focused;

            var tapRecognizer = new TapGestureRecognizer();
            tapRecognizer.Command = new RelayCommand(() =>
            {
                Tapped(this, EventArgs.Empty);
            });
            this.GestureRecognizers.Add(tapRecognizer);
        }

        /// <summary>
        /// Only fired on android to execute the tap gesture command.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void FocusableFrame_Focused(object sender, FocusEventArgs e)
        {
            if (e.IsFocused)
                foreach (var gr in this.GestureRecognizers)
                {
                    var tapGr = gr as TapGestureRecognizer;
                    if (tapGr != null)
                    {
                        if (tapGr.Command != null && tapGr.Command.CanExecute(null))
                            tapGr.Command.Execute(null);
                    }
                }
        }
    }
}