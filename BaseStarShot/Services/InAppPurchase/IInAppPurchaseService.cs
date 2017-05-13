using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Services
{
    public interface IInAppPurchaseService
    {
        /// <summary>
        /// Get Service for Android
        /// </summary>
        object GetService();

        /// <summary>
        /// Connect to the service needed. Only for Android
        /// </summary>
        void Connect();


        /// <summary>
        /// DisconnectService after use.  Only for Android
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Purchase Product via productId
        /// </summary>
        Task<PurchaseResponse> PurchaseProduct(string productId);

        /// <summary>
        /// Consume Product purchased in order to buy again consumable products
        /// <param>productId</param>
        /// <param>transactionId</param>
        /// </summary>
        Task<ProductConsumeResult> ConsumeProduct(string productId, string transactionId, object transactionObject = null);

        /// <summary>
        /// Load available Product List of the app.
        /// <param>productIds</param>
        /// </summary>
        Task<List<Product>> LoadProductList(IEnumerable<string> productIds = null);

        /// <summary>
        /// Load unfulfilled Products of the app.
        /// </summary>
        Task<List<UnfulfilledProduct>> LoadUnfulfilledProductList();

        /// <summary>
        /// Added On Purchased Item event
        /// </summary>
        event EventHandler<InAppPurchasedEventArgs> PurchasedProductEvent;

        /// <summary>
        /// Added On Consumed Item event
        /// </summary>
        event EventHandler<InAppPurchasedEventArgs> ConsumedProductEvent;

        /// <summary>
        /// Event where detect service if connected
        /// </summary>
        event EventHandler<EventArgs> ConnectedEvent;

        /// <summary>
        /// Event where detect service if connected
        /// </summary>
        event EventHandler<EventArgs> DisconnectedEvent;
    }

    public class InAppPurchasedEventArgs : EventArgs
    {
        public string ProductId { get; protected set; }
        public string TransactionId { get; protected set; }
        public string Receipt { get; protected set; }

        public InAppPurchasedEventArgs(string productId, string transactionId, string receipt)
        {
            this.ProductId = productId;
            this.TransactionId = transactionId;
            this.Receipt = receipt;
        }
    }
}
