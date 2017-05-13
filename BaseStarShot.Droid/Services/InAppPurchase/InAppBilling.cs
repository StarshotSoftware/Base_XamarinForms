using Android.App;
using Android.Views.InputMethods;
using System;
using Xamarin.InAppBilling;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace BaseStarShot.Services
{
    public class InAppBilling : IInAppPurchaseService
    {
        #region IInAppPurchaseService Members
        private InAppBillingServiceConnection _serviceConnection;

        static string unifyKey = "";

        public static void SetLicenseKey(string[] key)
        {
            unifyKey = Unify(key);
        }

        public static void SetLicenseKey(string key)
        {
            unifyKey = key;
        }

        public static string Unify(string[] element)
        {
            string empty = string.Empty;
            foreach (string s in element)
            {
                empty = string.Concat(empty, s);
            }
            return empty;
        }

        TaskCompletionSource<PurchaseResponse> purchaseTcs;
        string purchaseProductId;
        public async System.Threading.Tasks.Task<PurchaseResponse> PurchaseProduct(string productId)
        {
            purchaseTcs = new TaskCompletionSource<PurchaseResponse>();
            purchaseProductId = productId;
            if (_serviceConnection != null && _serviceConnection.Connected)
            {

                IList<Xamarin.InAppBilling.Product> _products = await _serviceConnection.BillingHandler.QueryInventoryAsync(new List<string> { productId }, ItemType.Product);

                Xamarin.InAppBilling.Product product = _products.Where(x => x.ProductId == productId).FirstOrDefault();
                _serviceConnection.BillingHandler.BuyProduct(product);
            }
            else {
                Logger.Write("InAppBilling", "service is not connected");
                purchaseTcs.TrySetCanceled();
            }
            var result = await purchaseTcs.Task;
            purchaseTcs = null;
            purchaseProductId = null;
            return result;
        }

        TaskCompletionSource<ProductConsumeResult> consumeTcs;
        string consumeProductId, consumeTransactionId;
        public async System.Threading.Tasks.Task<ProductConsumeResult> ConsumeProduct(string productId, string transactionId, object transactionObject = null)
        {
            consumeTcs = new TaskCompletionSource<ProductConsumeResult>();
            consumeProductId = productId;
            consumeTransactionId = transactionId;
            if (_serviceConnection != null && _serviceConnection.Connected)
            {
                var purchases = _serviceConnection.BillingHandler.GetPurchases(ItemType.Product);
                bool response = false;
                foreach (Purchase p in purchases)
                {
                    if (p.OrderId == transactionId && p.ProductId == productId) 
                    {
                        response = _serviceConnection.BillingHandler.ConsumePurchase(p);
                    }
                }
                if (!response)
                {
                    consumeTcs.TrySetResult(ProductConsumeResult.NothingToFulfill);
                }
            }
            else
            {
                Logger.Write("InAppBilling", "service is not connected");
                consumeTcs.TrySetResult(ProductConsumeResult.Error);
            }
            var result = await consumeTcs.Task;
            consumeTcs = null;
            consumeProductId = null;
            consumeTransactionId = null;
            return result;
        }

        public async Task<List<Product>> LoadProductList(IEnumerable<string> productIds = null)
        {
            System.Collections.Generic.List<Product> listOfProduct = new System.Collections.Generic.List<Product>();
            if (_serviceConnection != null)
            {
                if (_serviceConnection.Connected)
                {
                    try
                    {
                        // Old skuList
                        //new List<string>   {
                        //    ReservedTestProductIDs.Purchased,
                        //    ReservedTestProductIDs.Canceled,
                        //    ReservedTestProductIDs.Refunded,
                        //    ReservedTestProductIDs.Unavailable
                        //};
                        IList<Xamarin.InAppBilling.Product> _products = await _serviceConnection.BillingHandler.QueryInventoryAsync(productIds.ToList(), ItemType.Product);

                        if (_products != null)
                        {
                            if (_products.Count > 0)
                            {
                                foreach (Xamarin.InAppBilling.Product product in _products)
                                {
                                    Product productEntity = new Product();
                                    productEntity.ProductId = product.ProductId;
                                    productEntity.ProductName = product.Description;
                                    productEntity.Type = ProductType.Consumable;
                                    productEntity.FormattedPrice = product.Price;
                                    listOfProduct.Add(productEntity);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        return new List<Product>();
                    }
                }
            }
            return listOfProduct;
        }

        public async System.Threading.Tasks.Task<System.Collections.Generic.List<UnfulfilledProduct>> LoadUnfulfilledProductList()
        {
            System.Collections.Generic.List<UnfulfilledProduct> listOfProduct = new System.Collections.Generic.List<UnfulfilledProduct>();
            if (_serviceConnection != null)
            {
                if (_serviceConnection.Connected)
                {
                    try
                    {
                        await System.Threading.Tasks.Task.Run(() =>
                                   {
                                       var purchases = _serviceConnection.BillingHandler.GetPurchases(ItemType.Product);
                                       foreach (Purchase p in purchases)
                                       {
                                           UnfulfilledProduct uProduct = new UnfulfilledProduct();
                                           uProduct.ProductId = p.ProductId;
                                           uProduct.TransactionId = p.OrderId;
                                           listOfProduct.Add(uProduct);
                                       }
                                   });
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return listOfProduct;
        }

        public void Connect()
        {
               _serviceConnection = new InAppBillingServiceConnection((Activity)BaseStarShot.Controls.UIHelper.Context, unifyKey);
               _serviceConnection.Connect();
               _serviceConnection.OnConnected += _serviceConnection_OnConnected;
               _serviceConnection.OnDisconnected += _serviceConnection_OnDisconnected;
        }

        public void Disconnect()
        {
            if (_serviceConnection != null && _serviceConnection.Connected)
            {
                OnDisconnectedEvent(new EventArgs());
                _serviceConnection.Disconnect();
            }
            else {
                _serviceConnection = null;
            }
        }

        void _serviceConnection_OnDisconnected()
        {
            if (_serviceConnection != null)
            {
                if (_serviceConnection.BillingHandler != null)
                {
                    _serviceConnection.BillingHandler.OnProductPurchased -= BillingHandler_OnProductPurchased;
                    _serviceConnection.BillingHandler.OnProductPurchasedError -= BillingHandler_OnProductPurchasedError;
                    _serviceConnection.BillingHandler.OnPurchaseFailedValidation -= BillingHandler_OnPurchaseFailedValidation;
                    _serviceConnection.BillingHandler.OnUserCanceled -= BillingHandler_OnUserCanceled;
                    _serviceConnection.BillingHandler.InAppBillingProcesingError -= BillingHandler_InAppBillingProcesingError;
                    _serviceConnection.BillingHandler.BuyProductError -= BillingHandler_BuyProductError;
                }
                _serviceConnection = null;
            }
        }

        void _serviceConnection_OnConnected()
        {
            OnConnectedEvent(new EventArgs());
            if (_serviceConnection != null && _serviceConnection.BillingHandler != null)
            {
                _serviceConnection.BillingHandler.OnProductPurchased += BillingHandler_OnProductPurchased;
                _serviceConnection.BillingHandler.OnProductPurchasedError += BillingHandler_OnProductPurchasedError;
                _serviceConnection.BillingHandler.OnPurchaseFailedValidation += BillingHandler_OnPurchaseFailedValidation;
                _serviceConnection.BillingHandler.OnUserCanceled += BillingHandler_OnUserCanceled;
                _serviceConnection.BillingHandler.InAppBillingProcesingError += BillingHandler_InAppBillingProcesingError;
                _serviceConnection.BillingHandler.BuyProductError += BillingHandler_BuyProductError;
                _serviceConnection.BillingHandler.OnPurchaseConsumed += BillingHandler_OnPurchaseConsumed;
                _serviceConnection.BillingHandler.OnPurchaseConsumedError += BillingHandler_OnPurchaseConsumedError;
            }
        }

        private void BillingHandler_OnPurchaseConsumedError(int responseCode, string token)
        {
            if (consumeTcs != null)
            {
                consumeTcs.TrySetResult(ProductConsumeResult.Error);
            }
        }

        private void BillingHandler_OnPurchaseConsumed(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                OnConsumedProduct(new InAppPurchasedEventArgs(consumeProductId, consumeTransactionId, token));
            }
            if (consumeTcs != null)
            {
                consumeTcs.TrySetResult(ProductConsumeResult.Succeeded);
            }
        }

        private void BillingHandler_BuyProductError(int responseCode, string sku)
        {
            Logger.Write("InAppBilling", "Buy product error:" + sku);
            if (purchaseTcs != null)
            {
                var response = new PurchaseResponse { ProductId = sku, Status = PurchaseStatus.NotPurchased };
                // BILLING_RESPONSE_RESULT_ITEM_ALREADY_OWNED
                if (responseCode == 7)
                {
                    response.Status = PurchaseStatus.AlreadyPurchased;
                    var purchase = GetPurchase(sku);
                    if (purchase != null)
                    {
                        response.TransactionId = purchase.OrderId;
                        response.Receipt = purchase.DeveloperPayload;
                    }
                    else
                    {
                        response.TransactionId = PurchaseStatus.AlreadyPurchased.ToString();
                        response.Receipt = PurchaseStatus.AlreadyPurchased.ToString();
                    }
                }
                purchaseTcs.TrySetResult(response);
            }
        }

        Purchase GetPurchase(string sku)
        {
            if (_serviceConnection != null && _serviceConnection.Connected)
            {
                var purchases = _serviceConnection.BillingHandler.GetPurchases(ItemType.Product);
                if (purchases != null)
                {
                    return purchases.FirstOrDefault(p => p.ProductId == sku);
                }
            }
            return null;
        }

        private void BillingHandler_InAppBillingProcesingError(string message)
        {
            Logger.Write("InAppBilling", "Processing error:" + message);
            if (purchaseTcs != null)
            {
                purchaseTcs.TrySetResult(new PurchaseResponse { ProductId = purchaseProductId, Status = PurchaseStatus.NotPurchased });
            }
        }

        private void BillingHandler_OnUserCanceled()
        {
            Logger.Write("InAppBilling", "User cancelled");
            if (purchaseTcs != null)
            {
                purchaseTcs.TrySetResult(new PurchaseResponse { ProductId = purchaseProductId, Status = PurchaseStatus.NotPurchased });
            }
        }

        private void BillingHandler_OnPurchaseFailedValidation(Purchase purchase, string purchaseData, string purchaseSignature)
        {
            Logger.Write("InAppBilling", "Purchased Failed Validation:" + purchase.ProductId);
            if (purchaseTcs != null)
            {
                purchaseTcs.TrySetResult(new PurchaseResponse { ProductId = purchase.ProductId, Status = PurchaseStatus.NotPurchased });
            }
        }

        private void BillingHandler_OnProductPurchased(int response, Purchase purchase, string purchaseData, string purchaseSignature)
        {
            PurchaseResponse purchaseResponse = new PurchaseResponse();
            purchaseResponse.ProductId = purchase.ProductId;
            purchaseResponse.TransactionId = purchase.OrderId;
            purchaseResponse.Receipt = purchaseData;

            //fire event on  purchased product
            OnPurchasedProduct(new InAppPurchasedEventArgs(purchaseResponse.ProductId, purchaseResponse.TransactionId, purchaseResponse.Receipt));

            //set result
            Logger.Write("InAppBilling", "Purchased:" + purchase.ProductId);
            if (purchaseTcs != null)
            {
                purchaseTcs.TrySetResult(purchaseResponse);
            }
        }

        private void BillingHandler_OnProductPurchasedError(int responseCode, string sku)
        {
            Logger.Write("InAppBilling", "Purchased Error:" + sku);
            if (purchaseTcs != null)
            {
                var response = new PurchaseResponse { ProductId = sku, Status = PurchaseStatus.NotPurchased };
                // BILLING_RESPONSE_RESULT_ITEM_ALREADY_OWNED
                if (responseCode == 7)
                {
                    response.Status = PurchaseStatus.AlreadyPurchased;
                    var purchase = GetPurchase(sku);
                    if (purchase != null)
                    {
                        response.TransactionId = purchase.OrderId;
                        response.Receipt = purchase.DeveloperPayload;
                    }
                    else
                    {
                        response.TransactionId = PurchaseStatus.AlreadyPurchased.ToString();
                        response.Receipt = PurchaseStatus.AlreadyPurchased.ToString();
                    }
                }
                purchaseTcs.TrySetResult(response);
            }
        }

        public object GetService()
        {
            return _serviceConnection;
        }

        #endregion

        #region IInAppPurchaseService Members


        public event EventHandler<InAppPurchasedEventArgs> PurchasedProductEvent;

        public void OnPurchasedProduct(InAppPurchasedEventArgs args)
        {
            if (PurchasedProductEvent != null)
                PurchasedProductEvent(this, args);
        }

        public event EventHandler<InAppPurchasedEventArgs> ConsumedProductEvent;

        public void OnConsumedProduct(InAppPurchasedEventArgs args)
        {
            if (ConsumedProductEvent != null)
                ConsumedProductEvent(this, args);
        }

        public event EventHandler<EventArgs> ConnectedEvent;

        public void OnConnectedEvent(EventArgs args)
        {
            if (ConnectedEvent != null)
                ConnectedEvent(this, args);
        }

        public event EventHandler<EventArgs> DisconnectedEvent;

        public void OnDisconnectedEvent(EventArgs args)
        {
            if (DisconnectedEvent != null)
                DisconnectedEvent(this, args);
        }


        #endregion
    }
}