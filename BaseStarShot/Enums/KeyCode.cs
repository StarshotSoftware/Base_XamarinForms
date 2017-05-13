using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot
{
    /// <summary>
    /// A key enumertion that has an exact mapping to System.Windows.Input.Key.
    /// </summary>
    public enum KeyCode
    {
        /// <summary>
        /// A special value indicating no key.
        /// </summary>
        None = 0,
        /// <summary>
        /// The BACKSPACE key.
        /// </summary>
        Back = 1,
        /// <summary>
        /// The TAB key.
        /// </summary>
        Tab = 2,
        /// <summary>
        /// The ENTER key.
        /// </summary>
        Enter = 3,
        /// <summary>
        /// The SHIFT key.
        /// </summary>
        Shift = 4,
        /// <summary>
        /// The CTRL (control) key.
        /// </summary>
        Ctrl = 5,
        /// <summary>
        /// The ALT key.
        /// </summary>
        Alt = 6,
        /// <summary>
        /// The CAPSLOCK key.
        /// </summary>
        CapsLock = 7,
        /// <summary>
        /// The ESC (also known as ESCAPE) key.
        /// </summary>
        Escape = 8,
        /// <summary>
        /// The SPACE key.
        /// </summary>
        Space = 9,
        /// <summary>
        /// The PAGEUP key.
        /// </summary>
        PageUp = 10,
        /// <summary>
        /// The PAGEDOWN key.
        /// </summary>
        PageDown = 11,
        /// <summary>
        /// The END key.
        /// </summary>
        End = 12,
        /// <summary>
        /// The HOME key.
        /// </summary>
        Home = 13,
        /// <summary>
        /// The left arrow key.
        /// </summary>
        Left = 14,
        /// <summary>
        /// The up arrow key.
        /// </summary>
        Up = 15,
        /// <summary>
        /// The right arrow key.
        /// </summary>
        Right = 16,
        /// <summary>
        /// The down arrow key.
        /// </summary>
        Down = 17,
        /// <summary>
        /// The INSERT key.
        /// </summary>
        Insert = 18,
        /// <summary>
        /// The DEL (also known as DELETE) key.
        /// </summary>
        Delete = 19,
        /// <summary>
        /// The 0 (zero) key.
        /// </summary>
        D0 = 20,
        /// <summary>
        /// The 1 key.
        /// </summary>
        D1 = 21,
        /// <summary>
        /// The 2 key.
        /// </summary>
        D2 = 22,
        /// <summary>
        /// The 3 key.
        /// </summary>
        D3 = 23,
        /// <summary>
        /// The 4 key.
        /// </summary>
        D4 = 24,
        /// <summary>
        /// The 5 key.
        /// </summary>
        D5 = 25,
        /// <summary>
        /// The 6 key.
        /// </summary>
        D6 = 26,
        /// <summary>
        /// The 7 key.
        /// </summary>
        D7 = 27,
        /// <summary>
        /// The 8 key.
        /// </summary>
        D8 = 28,
        /// <summary>
        /// The 9 key.
        /// </summary>
        D9 = 29,
        /// <summary>
        /// The A key.
        /// </summary>
        A = 30,
        /// <summary>
        /// The B key.
        /// </summary>
        B = 31,
        /// <summary>
        /// The C key.
        /// </summary>
        C = 32,
        /// <summary>
        /// The D key.
        /// </summary>
        D = 33,
        /// <summary>
        /// The E key.
        /// </summary>
        E = 34,
        /// <summary>
        /// The F key.
        /// </summary>
        F = 35,
        /// <summary>
        /// The G key.
        /// </summary>
        G = 36,
        /// <summary>
        /// The H key.
        /// </summary>
        H = 37,
        /// <summary>
        /// The I key.
        /// </summary>
        I = 38,
        /// <summary>
        /// The J key.
        /// </summary>
        J = 39,
        /// <summary>
        /// The K key.
        /// </summary>
        K = 40,
        /// <summary>
        /// The L key.
        /// </summary>
        L = 41,
        /// <summary>
        /// The M key.
        /// </summary>
        M = 42,
        /// <summary>
        /// The N key.
        /// </summary>
        N = 43,
        /// <summary>
        /// The O key.
        /// </summary>
        O = 44,
        /// <summary>
        /// The P key.
        /// </summary>
        P = 45,
        /// <summary>
        /// The Q key.
        /// </summary>
        Q = 46,
        /// <summary>
        /// The R key.
        /// </summary>
        R = 47,
        /// <summary>
        /// The S key.
        /// </summary>
        S = 48,
        /// <summary>
        /// The T key.
        /// </summary>
        T = 49,
        /// <summary>
        /// The U key.
        /// </summary>
        U = 50,
        /// <summary>
        /// The V key.
        /// </summary>
        V = 51,
        /// <summary>
        /// The W key.
        /// </summary>
        W = 52,
        /// <summary>
        /// The X key.
        /// </summary>
        X = 53,
        /// <summary>
        /// The Y key.
        /// </summary>
        Y = 54,
        /// <summary>
        /// The Z key.
        /// </summary>
        Z = 55,
        /// <summary>
        /// The F1 key.
        /// </summary>
        F1 = 56,
        /// <summary>
        /// The F2 key.
        /// </summary>
        F2 = 57,
        /// <summary>
        /// The F3 key.
        /// </summary>
        F3 = 58,
        /// <summary>
        /// The F4 key.
        /// </summary>
        F4 = 59,
        /// <summary>
        /// The F5 key.
        /// </summary>
        F5 = 60,
        /// <summary>
        /// The F6 key.
        /// </summary>
        F6 = 61,
        /// <summary>
        /// The F7 key.
        /// </summary>
        F7 = 62,
        /// <summary>
        /// The F8 key.
        /// </summary>
        F8 = 63,
        /// <summary>
        /// The F9 key.
        /// </summary>
        F9 = 64,
        /// <summary>
        /// The F10 key.
        /// </summary>
        F10 = 65,
        /// <summary>
        /// The F11 key.
        /// </summary>
        F11 = 66,
        /// <summary>
        /// The F12 key.
        /// </summary>
        F12 = 67,
        /// <summary>
        /// The 0 key on the number pad.
        /// </summary>
        NumPad0 = 68,
        /// <summary>
        /// The 1 key on the number pad.
        /// </summary>
        NumPad1 = 69,
        /// <summary>
        /// The 2 key on the number pad.
        /// </summary>
        NumPad2 = 70,
        /// <summary>
        /// The 3 key on the number pad.
        /// </summary>
        NumPad3 = 71,
        /// <summary>
        /// The 4 key on the number pad.
        /// </summary>
        NumPad4 = 72,
        /// <summary>
        /// The 5 key on the number pad.
        /// </summary>
        NumPad5 = 73,
        /// <summary>
        /// The 6 key on the number pad.
        /// </summary>
        NumPad6 = 74,
        /// <summary>
        /// The 7 key on the number pad.
        /// </summary>
        NumPad7 = 75,
        /// <summary>
        /// The 8 key on the number pad.
        /// </summary>
        NumPad8 = 76,
        /// <summary>
        /// The 9 key on the number pad.
        /// </summary>
        NumPad9 = 77,
        /// <summary>
        /// The * (MULTIPLY) key.
        /// </summary>
        Multiply = 78,
        /// <summary>
        /// The + (ADD) key.
        /// </summary>
        Add = 79,
        /// <summary>
        /// The - (SUBTRACT) key.
        /// </summary>
        Subtract = 80,
        /// <summary>
        /// The . (DECIMAL) key.
        /// </summary>
        Decimal = 81,
        /// <summary>
        /// The / (DIVIDE) key.
        /// </summary>
        Divide = 82,
        /// <summary>
        /// The , comma key.
        /// </summary>
        Comma = 128,
        /// <summary>
        /// The @ key.
        /// </summary>
        At = 129,
        /// <summary>
        /// The / key.
        /// </summary>
        Slash = 130,
        /// <summary>
        /// The \ key.
        /// </summary>
        Backslash = 131,
        /// <summary>
        /// The ' key.
        /// </summary>
        Apostrophe = 132,
        /// <summary>
        /// The ( key.
        /// </summary>
        LeftParenthesis = 133,
        /// <summary>
        /// The ) key.
        /// </summary>
        RightParenthesis = 134,
        /// <summary>
        /// The . key.
        /// </summary>
        Period = 135,
        /// <summary>
        /// A special value indicating the key is out of range of this enumeration.
        /// </summary>
        Unknown = 255,
    }
}
