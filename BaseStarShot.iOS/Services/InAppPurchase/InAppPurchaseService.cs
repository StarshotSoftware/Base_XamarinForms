using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseStarShot.Services.InAppPurchase;
using BaseStarShot.Services;
using Foundation;
using System.Threading;
using StoreKit;
using System.Diagnostics;
using System.Linq;

namespace BaseStarShot.Services
{
	public class InAppPurchaseService : SKPaymentTransactionObserver, IInAppPurchaseService
	{
		PurchaseManager purchaseManager;
		IList<string> productIdentfiers;
		SKPaymentTransaction currentTransaction;

		TaskCompletionSource<List<Product>> productListTcs;
		TaskCompletionSource<PurchaseResponse> paymentTransactionTcs;

		public event EventHandler<EventArgs> ConnectedEvent;
		public event EventHandler<EventArgs> DisconnectedEvent;
		public event EventHandler<InAppPurchasedEventArgs> PurchasedProductEvent;
		public event EventHandler<InAppPurchasedEventArgs> ConsumedProductEvent;

		public InAppPurchaseService()
		{
			productIdentfiers = new List<string>();

			purchaseManager = new PurchaseManager();

			SKPaymentQueue.DefaultQueue.AddTransactionObserver(this);
		}

		#region SKPaymentTransactionObserver

		public override void PaymentQueueRestoreCompletedTransactionsFinished(SKPaymentQueue queue)
		{
			
		}

		public override void RestoreCompletedTransactionsFailedWithError(SKPaymentQueue queue, NSError error)
		{
			
		}

		public override void UpdatedTransactions (SKPaymentQueue queue, SKPaymentTransaction[] transactions)
		{
			if (paymentTransactionTcs == null)
				return;

			foreach (SKPaymentTransaction transaction in transactions)
			{
				switch (transaction.TransactionState)
				{
					case SKPaymentTransactionState.Purchased:
						paymentTransactionTcs.TrySetResult(new PurchaseResponse {
							ProductId = transaction.Payment.ProductIdentifier,
							Receipt = transaction.TransactionReceipt.ToString(),
							TransactionId = transaction.TransactionIdentifier,
							Status = PurchaseStatus.Succeeded
						});
						currentTransaction = transaction;
						break;
					case SKPaymentTransactionState.Failed:
						paymentTransactionTcs.TrySetResult(new PurchaseResponse {
							ProductId = transaction.Payment.ProductIdentifier,
							Receipt = null,
							TransactionId = transaction.TransactionIdentifier,
							Status = PurchaseStatus.NotPurchased
						});
						break;
					case SKPaymentTransactionState.Restored:
						paymentTransactionTcs.TrySetResult(new PurchaseResponse {
							ProductId = transaction.Payment.ProductIdentifier,
							Receipt = null,
							TransactionId = transaction.TransactionIdentifier,
							Status = PurchaseStatus.NotFulfilled
						});
						break;
					default:
						break;
				}
			}
		}

		#endregion

		#region IInAppPurchaseService

		public object GetService()
		{
			throw new NotImplementedException();
		}

		public void Connect()
		{
			foreach (var transaction in SKPaymentQueue.DefaultQueue.Transactions)
			{
				SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
			}

			if (ConnectedEvent != null)
				ConnectedEvent(this, EventArgs.Empty);
		}

		public void Disconnect()
		{
			if (DisconnectedEvent != null)
				DisconnectedEvent(this, EventArgs.Empty);
		}

		public Task<PurchaseResponse> PurchaseProduct(string productId)
		{
			paymentTransactionTcs = new TaskCompletionSource<PurchaseResponse>();

//			#if DEBUG
//			Task.Run(async () => 
//			{
//				await Task.Delay(3000);
//				paymentTransactionTcs.SetResult(new PurchaseResponse
//				{
//					ProductId = productId,
//					TransactionId = productId,
//					Receipt = productId,
//					Status = PurchaseStatus.Succeeded
//				});
//			});
//			#else
			SKPayment payment = SKPayment.PaymentWithProduct(productId);
			SKPaymentQueue.DefaultQueue.AddPayment(payment);
//			#endif

			return paymentTransactionTcs.Task;
		}

		public Task<ProductConsumeResult> ConsumeProduct(string productId, string transactionId, object transactionObject = null)
		{
			var tcs = new TaskCompletionSource<ProductConsumeResult>();
			try
			{
//				#if !DEBUG
				var transaction = SKPaymentQueue.DefaultQueue.Transactions.FirstOrDefault(x => x.TransactionIdentifier == transactionId);
				SKPaymentQueue.DefaultQueue.FinishTransaction(transaction);
				currentTransaction = null;
//				#endif
				tcs.SetResult(ProductConsumeResult.Succeeded);
			}
			catch (Exception)
			{
				tcs.SetResult(ProductConsumeResult.Error);
			}
			return tcs.Task;
		}

		public Task<List<Product>> LoadProductList(IEnumerable<string> productIds = null)
		{
			productListTcs = new TaskCompletionSource<List<Product>>();
			//purchaseManager.RequestProductData(productIdentfiers);

//			#if DEBUG
//			Task.Run(async () => 
//			{
//				await Task.Delay(3000);
//
//				var productids = new[] { "remove_ads", "remove_ads_180", "remove_ads_365", "multiplayer", 
//					"multiplayer_180", "multiplayer_365", "chat", "chat_180", "chat_365", "view_stats", "view_stats_180",
//					"view_stats_365", "dice_yellow", "dice_blue", "dice_green" };
//
//				var products = new List<Product>();
//				foreach (var productId in productids)
//				{
//					var product = new Product();
//					product.FormattedPrice = "1.00";
//					product.ProductName = productId;
//					product.ProductId = productId;
//
//					products.Add(product);
//				}
//
//				productListTcs.SetResult(products);
//			});
//			#else
			NSString[] array = productIds.Select (pId => (NSString)pId).ToArray();
			NSSet productIdentifiers = NSSet.MakeNSObjectSet<NSString>(array);

			//set up product request for in-app purchase
			var productsRequest = new SKProductsRequest(productIdentifiers);
			productsRequest.ReceivedResponse += ProductsRequest_ReceivedResponse;
			productsRequest.RequestFailed += ProductsRequest_RequestFailed;
			productsRequest.Start();
//			#endif

			return productListTcs.Task;
		}

		void ProductsRequest_ReceivedResponse (object sender, SKProductsRequestResponseEventArgs e)
		{
			var products = new List<Product>();

			foreach (var skProduct in e.Response.Products)
			{	
				var product = new Product();
				product.Price = skProduct.Price.DoubleValue;
				product.FormattedPrice = skProduct.PriceLocale.CurrencySymbol + skProduct.Price.ToString();
				product.ProductName = skProduct.Description;
				product.ProductId = skProduct.ProductIdentifier;

				//					if (skProduct. == InAppProductType.Consumable)
				//						product.Type = ProductType.Consumable;
				//					else if (skProduct.ProductType == InAppProductType.NonConsumable)
				//						product.Type = ProductType.NonConsumable;
				//					else
				//						product.Type = ProductType.Unknown;

				products.Add(product);
			}

			productListTcs.SetResult(products);
		}

		void ProductsRequest_RequestFailed (object sender, SKRequestErrorEventArgs e)
		{
			productListTcs.TrySetResult(new List<Product>());
		}

		public Task<List<UnfulfilledProduct>> LoadUnfulfilledProductList()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}

