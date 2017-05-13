using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Services
{
    public enum PurchaseStatus
    {
        // Summary:
        //     The purchase succeeded and the user has been informed.
        Succeeded = 0,
        //
        // Summary:
        //     The purchase did not occur because this in-app purchase has already been
        //     purchased by the user, and it cannot be purchased again.
        AlreadyPurchased = 1,
        //
        // Summary:
        //     The purchase did not occur because a previous purchase of this consumable
        //     has not been fulfilled.
        NotFulfilled = 2,
        //
        // Summary:
        //     The purchase did not occur because the user decided not to complete the purchase
        //     (or the purchase failed for other reasons).
        NotPurchased = 3,
    }
}
