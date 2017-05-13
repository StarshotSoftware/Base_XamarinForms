using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseStarShot.Services
{
    public interface INotificationService
    {
        /// <summary>
        /// Sets if default notification should be prevented if app is in activate state.
        /// </summary>
        bool AutoCancel { get; set; }

        /// <summary>
        /// Sets if notification queueing from inactive state should be disabled.
        /// Not yet implemented for Android.
        /// </summary>
        bool DisabledQueueing { get; set; }

        event EventHandler ClearBadgesEvent;
        event EventHandler RefreshBadgesEvent;
        event EventHandler<NotificationReceivedArgs> NotificationReceived;
        event EventHandler<NotificationRegisteredArgs> NotificationRegistered;
        event EventHandler<IDictionary<string, string>> NotificationQueued;

        /// <summary>
        /// Initializes Sender IDs Server Side for Android.
        /// Sets allowed base Urls for tile back background image for WinPhone.
        /// </summary>
        void SetSenderIds(string[] SENDER_IDS);

        /// <summary>
        /// Registers notification channel with cloud service
        /// </summary>
        Task<bool> RegisterNotificationChannelAsync();

        /// <summary>
        /// Unregisters notification channel with cloud service
        /// </summary>
        Task<bool> UnregisterNotificationChannelAsync();

        /// <summary>
        /// Registers notification channel with cloud service
        /// </summary>
        Task<bool> RegisterLocalNotificationChannelAsync();

        /// <summary>
        /// Clear badges
        /// </summary>
        void ClearBadges();

        /// <summary>
        /// Refresh badges
        /// </summary>
        void RefreshBadges();

        /// <summary>
        /// Fires NotificationReceived event.
        /// </summary>
        /// <param name="args"></param>
        void OnNotificationReceived(NotificationReceivedArgs args);

        /// <summary>
        /// Fires NotificationRegistered event.
        /// </summary>
        /// <param name="args"></param>
        void OnNotificationRegistered(NotificationRegisteredArgs args);

        IDictionary<string, string> DequeueNotification();
    }

    public class NotificationReceivedArgs : EventArgs
    {
        public IDictionary<string, string> Items { get; protected set; }

        /// <summary>
        /// Cancels default handling of notification. Supported on WinRT.
        /// </summary>
        public bool Cancel { get; set; }

        public string Text1 { get; protected set; }

        public string Text2 { get; protected set; }

        public bool FromInactiveState { get; set; }

        public NotificationReceivedArgs(IDictionary<string, string> items)
        {
            this.Items = items;
        }

        public NotificationReceivedArgs(IDictionary<string, string> items, string text1)
            : this(items)
        {
            this.Text1 = text1;
        }

        public NotificationReceivedArgs(IDictionary<string, string> items, string text1, string text2)
            : this(items, text1)
        {
            this.Text2 = text2;
        }
    }

    public class NotificationRegisteredArgs : EventArgs
    {
        public string Token { get; protected set; }
        public string ItemId { get; protected set; }
        public bool UploadingSuccess { get; set; }

        public NotificationRegisteredArgs(string token)
        {
            this.Token = token;
        }

        public NotificationRegisteredArgs(string token, string itemId)
            : this(token)
        {
            this.ItemId = itemId;
        }
    }
}
