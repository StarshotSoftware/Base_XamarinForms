using System;
using System.Threading.Tasks;
using BaseStarShot.Services;
using PushSharp.Client;
using Android.Content;
using Android.App;
using System.Collections.Generic;
using System.Threading;

namespace BaseStarShot
{
    public class NotificationServiceAndroid<TService> : INotificationService, IPushClient
        where TService : PushHandlerServiceBase
    {
        private PushHandlerServiceConnection pushHandlerConnection;
        private Queue<IDictionary<string, string>> notifications;
        private ReaderWriterLockSlim nLocker;

        public string[] SENDER_IDS = new string[] { "" };

        public bool AutoCancel { get; set; }
        public bool DisabledQueueing { get; set; }

        public PushHandlerServiceBinder PushHandlerBinder { get; set; }

        public NotificationServiceAndroid()
        {
            notifications = new Queue<IDictionary<string, string>>();
            nLocker = new ReaderWriterLockSlim();
        }

        public void SetSenderIds(string[] SENDER_IDS)
        {
            this.SENDER_IDS = SENDER_IDS;
        }

        /// <summary>
        /// Registers notification channel with cloud service
        /// </summary>
        public async Task<bool> RegisterNotificationChannelAsync()
        {
            PushClient.Register(Android.App.Application.Context, SENDER_IDS);
            return true;
        }

        /// <summary>
        /// Unregisters notification channel with cloud service
        /// </summary>
        public async Task<bool> UnregisterNotificationChannelAsync()
        {
            PushClient.UnRegister(Android.App.Application.Context);
            return true;
        }

        public event EventHandler ClearBadgesEvent;

        /// <summary>
        /// Clear badges
        /// </summary>
        public void ClearBadges()
        {
            string ns = Context.NotificationService;
            NotificationManager nMgr = (NotificationManager)Application.Context.GetSystemService(ns);
            nMgr.CancelAll();
            if (PushHandlerBinder != null)
                PushHandlerBinder.Service.ClearBadges();
            if (ClearBadgesEvent != null)
                ClearBadgesEvent(this, EventArgs.Empty);
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


        public void OnError(string errorId)
        {

        }

        public void OnNotificationReceived(NotificationReceivedArgs args)
        {
            if (NotificationReceived != null)
                NotificationReceived(this, args);
            OnNotificationQueued(args.Items);
        }

        /// <summary>
        /// Fires NotificationRegistered event.
        /// </summary>
        /// <param name="args"></param>
        public void OnNotificationRegistered(NotificationRegisteredArgs args)
        {
            if (pushHandlerConnection == null)
            {
                pushHandlerConnection = new PushHandlerServiceConnection(this);
                pushHandlerConnection.ServiceConnected += pushHandlerConnection_ServiceConnected;
                var pushHandlerServiceIntent = new Intent(Application.Context, typeof(TService));
                Application.Context.BindService(pushHandlerServiceIntent, pushHandlerConnection, Bind.AutoCreate);
            }
            if (NotificationRegistered != null)
                NotificationRegistered(this, args);
        }

        void pushHandlerConnection_ServiceConnected(object sender, EventArgs e)
        {
            PushHandlerBinder.Service.NotificationRegistered += Service_NotificationRegistered;
            PushHandlerBinder.Service.NotificationReceived += Service_NotificationReceived;
        }

        void Service_NotificationRegistered(object sender, NotificationRegisteredArgs e)
        {
            OnNotificationRegistered(e);
        }

        void Service_NotificationReceived(object sender, NotificationReceivedArgs e)
        {
            OnNotificationReceived(e);
        }

        #region INotificationService Members


        public event EventHandler<System.Collections.Generic.IDictionary<string, string>> NotificationQueued;

        public void OnNotificationQueued(System.Collections.Generic.IDictionary<string, string> args)
        {
            if (!DisabledQueueing)
            {
                AddNotification(args);
                if (NotificationQueued != null)
                    NotificationQueued(this, args);
            }
        }

        public async Task<bool> RegisterLocalNotificationChannelAsync()
        {
            //throw new NotImplementedException();
            await Task.Delay(1000);
            return false;
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

        #endregion
    }
}

