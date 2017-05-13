using System;
using StoreKit;
using Foundation;

namespace BaseStarShot.Services.InAppPurchase
{
	internal class CustomPaymentObserver : SKPaymentTransactionObserver 
	{
		PurchaseManager purchaseManager;

		public CustomPaymentObserver(PurchaseManager purchaseManager)
		{
			this.purchaseManager = purchaseManager;
		}

		// called when the transaction status is updated
		public override void UpdatedTransactions (SKPaymentQueue queue, SKPaymentTransaction[] transactions)
		{
			Console.WriteLine ("UpdatedTransactions");
			foreach (SKPaymentTransaction transaction in transactions)
			{
				switch (transaction.TransactionState)
				{
					case SKPaymentTransactionState.Purchased:
						purchaseManager.CompleteTransaction(transaction);
						break;
					case SKPaymentTransactionState.Failed:
						purchaseManager.FailedTransaction(transaction);
						break;
					case SKPaymentTransactionState.Restored:
						purchaseManager.RestoreTransaction(transaction);
						break;
					default:
						break;
				}
			}
		}

		public override void PaymentQueueRestoreCompletedTransactionsFinished(SKPaymentQueue queue)
		{
			// Restore succeeded
			Console.WriteLine(" ** RESTORE PaymentQueueRestoreCompletedTransactionsFinished ");
		}

		public override void RestoreCompletedTransactionsFailedWithError(SKPaymentQueue queue, NSError error)
		{
			// Restore failed somewhere...
			Console.WriteLine(" ** RESTORE RestoreCompletedTransactionsFailedWithError " + error.LocalizedDescription);
		}
	}
}

