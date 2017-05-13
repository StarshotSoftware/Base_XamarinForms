﻿using System;
using Xamarin.Forms;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("TwinTechsLib.iOS")]
[assembly: InternalsVisibleTo("TwinTechsLib.Droid")]
namespace TwinTechs.Controls
{
    /*
     * a view which can be used as a cell in order to get optimum performance 
     */
    public abstract class FastCell : ViewCell
    {
        //public static BindableProperty DefaultRowHeightProperty =
        //   BindableProperty.Create<FastCell, double>(p => p.DefaultRowHeight, 0);

        //public double DefaultRowHeight
        //{
        //    get { return (double)GetValue(DefaultRowHeightProperty); }
        //    set { SetValue(DefaultRowHeightProperty, value); }
        //}

        public bool IsInitialized
        {
            get;
            private set;
        }

        //		public Layout<Xamarin.Forms.View> Content { get; set; }

        /// <summary>
        /// Initializes the cell.
        /// </summary>
        public void PrepareCell()
        {
            InitializeCell();
            if (BindingContext != null)
            {
                SetupCell(false);
            }
            IsInitialized = true;
        }

        public void PrepareCell(Size cellSize)
        {
            InitializeCell();
            if (BindingContext != null)
            {
                SetupCell(false);
            }
            IsInitialized = true;
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (IsInitialized)
            {
                SetupCell(true);
            }
        }

        /// <summary>
        /// Setups the cell. You should call InitializeComponent in here
        /// </summary>
        protected abstract void InitializeCell();

        /// <summary>
        /// Do your cell setup using the binding context in here.
        /// </summary>
        /// <param name="isRecycled">If set to <c>true</c> is recycled.</param>
        protected abstract void SetupCell(bool isRecycled);

        internal object OriginalBindingContext;

    }
}

