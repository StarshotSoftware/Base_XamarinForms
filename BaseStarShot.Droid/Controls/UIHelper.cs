using System;
using Android.Content;
using Android.Views;

using Xamarin.Forms;
using Android.Views.InputMethods;
using Android.Widget;
using System.Threading;

namespace BaseStarShot.Controls
{
    public static partial class UIHelper
    {
        public static Type Drawable;
        public static Type Layout;
        public static Type Id;
        public static Type Menu;
        public static Type Dimension;

        public static Context Context;

        public static int GetResource(Type resourceType, string name)
        {
            if (string.IsNullOrEmpty(name)) return -1;

            int resourceId = 0;
            var field = resourceType.GetField(name);
            if (field != null)
                resourceId = (int)field.GetValue(null);
            return resourceId;
        }

        public static int GetDrawableResource(FileImageSource source)
        {
            return source == null ? 0 : GetResource(Drawable, source.File);
        }

        public static int GetDimensionResource(string dimensionName)
        {
            return dimensionName == null ? 0 : GetResource(Dimension, dimensionName);
        }

        public static int GetLayoutResource(string name)
        {
            return GetResource(Layout, name);
        }

        public static int GetId(string name)
        {
            return GetResource(Id, name);
        }

        public static int GetMenuId(string name)
        {
            return GetResource(Menu, name);
        }

        public static void CloseSoftKeyboard(Android.Views.View view)
        {
            InputMethodManager imm = (InputMethodManager)view.Context.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(view.WindowToken, 0);
        }

        public static void OpenSoftKeyboard(Android.Views.View view)
        {
            view.RequestFocus();
            ThreadPool.QueueUserWorkItem(s =>
            {
                try
                {
                    Thread.Sleep(100);
                    InputMethodManager inputMethodManager = view.Context.GetSystemService(Context.InputMethodService) as InputMethodManager;
                    inputMethodManager.ShowSoftInput(view, ShowFlags.Forced);
                    inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);
                }
                catch(Exception e)
                {

                }
     

            });
        }

        public static Android.Graphics.Color Convert(Color color)
        {
            return Android.Graphics.Color.Argb(ConvertDoubleToByteColor(color.A),
                ConvertDoubleToByteColor(color.R),
                ConvertDoubleToByteColor(color.G),
                ConvertDoubleToByteColor(color.B));
        }

        public static Keycode? Convert(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.D1: return Keycode.Num1;
                case KeyCode.D2: return Keycode.Num2;
                case KeyCode.D3: return Keycode.Num3;
                case KeyCode.D4: return Keycode.Num4;
                case KeyCode.D5: return Keycode.Num5;
                case KeyCode.D6: return Keycode.Num6;
                case KeyCode.D7: return Keycode.Num7;
                case KeyCode.D8: return Keycode.Num8;
                case KeyCode.D9: return Keycode.Num9;
                case KeyCode.D0: return Keycode.Num0;

                case KeyCode.A: return Keycode.A;
                case KeyCode.B: return Keycode.B;
                case KeyCode.C: return Keycode.C;
                case KeyCode.D: return Keycode.D;
                case KeyCode.E: return Keycode.E;
                case KeyCode.F: return Keycode.F;
                case KeyCode.G: return Keycode.G;
                case KeyCode.H: return Keycode.H;
                case KeyCode.I: return Keycode.I;
                case KeyCode.J: return Keycode.J;
                case KeyCode.K: return Keycode.K;
                case KeyCode.L: return Keycode.L;
                case KeyCode.M: return Keycode.M;
                case KeyCode.N: return Keycode.N;
                case KeyCode.O: return Keycode.O;
                case KeyCode.P: return Keycode.P;
                case KeyCode.Q: return Keycode.Q;
                case KeyCode.R: return Keycode.R;
                case KeyCode.S: return Keycode.S;
                case KeyCode.T: return Keycode.T;
                case KeyCode.U: return Keycode.U;
                case KeyCode.V: return Keycode.V;
                case KeyCode.W: return Keycode.W;
                case KeyCode.X: return Keycode.X;
                case KeyCode.Y: return Keycode.Y;
                case KeyCode.Z: return Keycode.Z;

                case KeyCode.Back: return Keycode.Del;
                case KeyCode.Delete: return Keycode.ForwardDel;

                case KeyCode.Decimal: return Keycode.NumpadDot;
                case KeyCode.Add: return Keycode.Plus;
                case KeyCode.Subtract: return Keycode.Minus;
                case KeyCode.Multiply: return Keycode.NumpadMultiply;
                case KeyCode.Divide: return Keycode.NumpadDivide;

                case KeyCode.Comma: return Keycode.Comma;
                case KeyCode.At: return Keycode.At;
                case KeyCode.Slash: return Keycode.Slash;
                case KeyCode.Apostrophe: return Keycode.Apostrophe;
                case KeyCode.Backslash: return Keycode.Backslash;
                case KeyCode.LeftParenthesis: return Keycode.NumpadLeftParen;
                case KeyCode.RightParenthesis: return Keycode.NumpadRightParen;
                case KeyCode.Period: return Keycode.Period;
            }

            return Keycode.Break;
        }
    }
}