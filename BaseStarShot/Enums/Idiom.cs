using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot
{
    public enum Idiom
    {
        // Summary:
        //     (Unused) Indicates that Forms is running on an unsupported device.
        Unsupported = 0,
        //
        // Summary:
        //     Indicates that Forms is running on a Phone, or a device similar in shape
        //     and capabilities.
        Phone = 1,
        //
        // Summary:
        //     Indicates that Forms is running on a Tablet.
        Tablet = 2,
        //
        // Summary:
        //     (Unused) Indicates that Forms is running on a computer.
        Desktop = 3,
    }
}
