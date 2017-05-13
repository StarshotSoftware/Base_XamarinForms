using System;
using System.Threading.Tasks;
using BaseStarShot.Services;
using UIKit;
using Foundation;
using System.Collections.Generic;
using System.Threading;

namespace BaseStarShot
{
	public class NotificationServiceiOS : INotificationService
	{
		private Queue<IDictionary<string, string>> notifications;
		private ReaderWriterLockSlim nLocker;

		public NotificationServiceiOS()
		{
			notifications = new Queue<IDictionary<string, string>>();
			nLocker = new ReaderWriterLockSlim();
		}

        public bool AutoCancel { get; set; }
        public bool DisabledQueueing { get; set; }

		public void SetSenderIds (string[] SENDER_IDS)
		{
		
		}

		/// <summary>
		/// Registers notification channel with cloud service
		/// </summary>
		public async Task<bool> RegisterNotificationChannelAsync ()
		{	
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				var pushSettings = UIUserNotificationSettings.GetSettingsForTypes (
					UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
					new NSSet ());

				UIApplication.SharedApplication.RegisterUserNotificationSettings (pushSettings);
				UIApplication.SharedApplication.RegisterForRemoteNotifications ();
			} else {if (ClearBadgesEvent != null)
				ClearBadgesEvent (this, EventArgs.Empty);
				UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
				UIApplication.SharedApplication.RegisterForRemoteNotificationTypes (notificationTypes);
			}
			return true;
		}

		/// <summary>
		/// Unregisters notification channel with cloud service
		/// </summary>
		public async Task<bool> UnregisterNotificationChannelAsync ()
		{
			UIApplication.SharedApplication.UnregisterForRemoteNotifications ();
			return true;
		}

		/// <summary>
		/// Registers local notification channel
		/// </summary>
		public Task<bool> RegisterLocalNotificationChannelAsync()
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			{
				var notificationTypes = UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound;
				var userNoticationSettings = UIUserNotificationSettings.GetSettingsForTypes(notificationTypes, new NSSet(new string[] { }));
				UIApplication.SharedApplication.RegisterUserNotificationSettings(userNoticationSettings);
			}
			return Task.FromResult(true);
		}

		public event EventHandler ClearBadgesEvent;

		/// <summary>
		/// Clear badges
		/// </summary>
		public void ClearBadges ()
		{
			UIApplication.SharedApplication.ApplicationIconBadgeNumber = 0;
			UIApplication.SharedApplication.CancelAllLocalNotifications ();
			if (ClearBadgesEvent != null)
				ClearBadgesEvent (this, EventArgs.Empty);
		}

        public event EventHandler RefreshBadgesEvent;

        /// <summary>
        /// Refresh badges
        /// </summary>
        public void RefreshBadges()
        {
            if (RefreshBadgesEvent != null)
                RefreshBadgesEvent(this, EventArgs.Empty);
        }

		public event EventHandler<NotificationReceivedArgs> NotificationReceived;
		public event EventHandler<NotificationRegisteredArgs> NotificationRegistered;
		public event EventHandler<IDictionary<string, string>> NotificationQueued;

		public void OnError (string errorId)
		{
			
		}

		public void OnNotificationReceived(NotificationReceivedArgs args)
		{
            if (args.FromInactiveState && !DisabledQueueing)
			{
				AddNotification(args.Items);
				var queuedHandler = NotificationQueued;
				if (queuedHandler != null)
				{
					queuedHandler.Invoke(this, args.Items);
				}
			}
			else
			{
				if (NotificationReceived != null)
					NotificationReceived(this, args);
			}
		}

		/// <summary>
		/// Fires NotificationRegistered event.
		/// </summary>
		/// <param name="args"></param>
		public void OnNotificationRegistered(NotificationRegisteredArgs args){
			if (NotificationRegistered != null)
				NotificationRegistered (this, args);
		}

		void AddNotification(IDictionary<string, string> notification)
		{
			nLocker.EnterWriteLock();
			try
			{
				notifications.Enqueue(notification);
			}
			finally
			{
				if (nLocker.IsWriteLockHeld)
					nLocker.ExitWriteLock();
			}
		}

		public IDictionary<string, string> DequeueNotification()
		{
			nLocker.EnterWriteLock();
			try
			{
				if (notifications.Count > 0)
					return notifications.Dequeue();
			}
			finally
			{
				if (nLocker.IsWriteLockHeld)
					nLocker.ExitWriteLock();
			}
			return null;
		}
	}
}

