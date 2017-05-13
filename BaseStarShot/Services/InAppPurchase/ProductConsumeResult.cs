using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Services
{
    public enum ProductConsumeResult
    {
        /// <summary>
        ///  The in-app consumable purchase was fulfilled.
        /// </summary>
        Succeeded = 0,
        /// <summary>
        ///     The specified transaction ID has been passed or the consumables assoc transaction
        ///     ID has already been fulfilled.
        /// </summary>
        NothingToFulfill = 1,
        /// <summary>
        ///     The purchase has not yet cleared. At this point it is still possible for
        ///     the transaction to be reversed due to provider failures and/or risk checks.
        /// </summary>
        PurchasePending = 2,
        /// <summary>
        ///     The purchase request has been reverted.
        /// </summary>
        PurchaseReverted = 3,
        /// <summary>
        ///     There was an issue receiving fulfillment status.
        /// </summary>
        ServerError = 4,
        /// <summary>
        ///     Error occured
        /// </summary>
        Error = 5,

    }
}
