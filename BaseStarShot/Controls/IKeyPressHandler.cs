using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Controls
{
    public interface IKeyPressHandler
    {
        void SetPerformKeyPress(Action<KeyCode> onKeyPress);

        void PerformKeyPress(KeyCode key);
    }
}
