using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot
{
    public static class KeyCodeExtensions
    {
        public static char? ToChar(this KeyCode key)
        {
            switch (key)
            {
                case KeyCode.D1: return '1';
                case KeyCode.D2: return '2';
                case KeyCode.D3: return '3';
                case KeyCode.D4: return '4';
                case KeyCode.D5: return '5';
                case KeyCode.D6: return '6';
                case KeyCode.D7: return '7';
                case KeyCode.D8: return '8';
                case KeyCode.D9: return '9';
                case KeyCode.D0: return '0';

                case KeyCode.Delete:
                case KeyCode.Back: return '\b';
            }

            return null;
        }
    }
}
