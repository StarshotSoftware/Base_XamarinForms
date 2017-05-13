using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Services
{
    public class CameraOptions : AbstractOptions
    {
        public CameraDevice Camera { get; set; }


        public CameraOptions()
        {
            this.Camera = CameraDevice.Rear;
        }
    }
}
