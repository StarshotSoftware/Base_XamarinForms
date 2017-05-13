using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;

namespace PushSharp.Client
{
    using Base1902;
    using BaseStarShot.Services;

    [Android.Runtime.Preserve(AllMembers = true)]
    public abstract class PushHandlerServiceBase : IntentService
    {
        const string TAG = "GCMBaseIntentService";

        const string WAKELOCK_KEY = "GCM_LIB";
        static PowerManager.WakeLock sWakeLock;

        static object LOCK = new object();
        static int serviceId = 1;

        static string[] SenderIds = new string[] { };

        //int sCounter = 1;
        Random sRandom = new Random();

        const int MAX_BACKOFF_MS = 3600000; //1 hour

        string TOKEN = "";
        const string EXTRA_TOKEN = "token";

        public event EventHandler<NotificationReceivedArgs> NotificationReceived = delegate { };
        public event EventHandler<NotificationRegisteredArgs> NotificationRegistered = delegate { };

        protected string ApplicationName { get; private set; }

        protected PushHandlerServiceBase() : base() { }

        public PushHandlerServiceBase(params string[] senderIds)
            : base("GCMIntentService-" + (serviceId++).ToString())
        {
            SenderIds = senderIds;
        }

        public override IBinder OnBind(Intent intent)
        {
            var binder = new PushHandlerServiceBinder(this);
            return binder;
        }

        public override void OnCreate()
        {
            base.OnCreate();

            Android.Content.PM.PackageInfo pInfo = this.ApplicationContext.PackageManager.GetPackageInfo(this.ApplicationContext.PackageName, 0);
            ApplicationName = this.ApplicationContext.PackageManager.GetApplicationLabel(pInfo.ApplicationInfo);
        }

        public virtual void ClearBadges()
        {

        }

        protected virtual void OnDeletedMessages(Context context, int total)
        {
        }

        protected virtual bool OnRecoverableError(Context context, string errorId)
        {
            return true;
        }

        protected abstract void OnError(Context context, string errorId);

        protected abstract bool OnRegistered(Context context, string registrationId);

        protected abstract void OnUnRegistered(Context context, string registrationId);


        protected override void OnHandleIntent(Intent intent)
        {
            try
            {
                var context = this.ApplicationContext;
                var action = intent.Action;

                if (action.Equals(GCMConstants.INTENT_FROM_GCM_REGISTRATION_CALLBACK))
                {
                    handleRegistration(context, intent);
                }
                else if (action.Equals(GCMConstants.INTENT_FROM_GCM_MESSAGE))
                {
                    // checks for special messages
                    var messageType = intent.GetStringExtra(GCMConstants.EXTRA_SPECIAL_MESSAGE);
                    if (messageType != null)
                    {
                        if (messageType.Equals(GCMConstants.VALUE_DELETED_MESSAGES))
                        {
                            var sTotal = intent.GetStringExtra(GCMConstants.EXTRA_TOTAL_DELETED);
                            if (!string.IsNullOrEmpty(sTotal))
                            {
                                int nTotal = 0;
                                if (int.TryParse(sTotal, out nTotal))
                                {
                                    Log.Verbose(TAG, "Received deleted messages notification: " + nTotal);
                                    OnDeletedMessages(context, nTotal);
                                }
                                else
                                    Log.Error(TAG, "GCM returned invalid number of deleted messages: " + sTotal);
                            }
                        }
                        else
                        {
                            // application is not using the latest GCM library
                            Log.Error(TAG, "Received unknown special message: " + messageType);
                        }
                    }
                    else
                    {
                        Dictionary<string, string> items = new Dictionary<string, string>();
                        Bundle bundle = intent.Extras;

                        foreach (string key in bundle.KeySet())
                        {
							if(!key.Contains("google.sent_time"))
                            	items.Add(key, intent.GetStringExtra(key));
                        }
                        var text1 = "";
                        if (items.ContainsKey("alert"))
                            text1 = items["alert"];
                        var ev = new NotificationReceivedArgs(items, text1);
                        NotificationReceived(this, ev);
                        if (!ev.Cancel)
                            CreateNotification(context, ev);
                    }
                }
                else if (action.Equals(GCMConstants.INTENT_FROM_GCM_LIBRARY_RETRY))
                {
                    var token = intent.GetStringExtra(EXTRA_TOKEN);

                    if (!string.IsNullOrEmpty(token) && !TOKEN.Equals(token))
                    {
                        // make sure intent was generated by this class, not by a
                        // malicious app.
                        Log.Error(TAG, "Received invalid token: " + token);
                        return;
                    }

                    // retry last call
                    if (PushClient.IsRegistered(context))
                        PushClient.internalUnRegister(context);
                    else
                        PushClient.internalRegister(context, SenderIds);
                }
            }
            finally
            {
                // Release the power lock, so phone can get back to sleep.
                // The lock is reference-counted by default, so multiple
                // messages are ok.

                // If OnMessage() needs to spawn a thread or do something else,
                // it should use its own lock.
                lock (LOCK)
                {
                    //Sanity check for null as this is a public method
                    if (sWakeLock != null)
                    {
                        Log.Verbose(TAG, "Releasing Wakelock");
                        sWakeLock.Release();
                    }
                    else
                    {
                        //Should never happen during normal workflow
                        Log.Error(TAG, "Wakelock reference is null");
                    }
                }
            }
        }

        protected abstract void CreateNotification(Context context, NotificationReceivedArgs args);

        internal static void RunIntentInService(Context context, Intent intent, Type classType)
        {
            lock (LOCK)
            {
                if (sWakeLock == null)
                {
                    // This is called from BroadcastReceiver, there is no init.
                    var pm = PowerManager.FromContext(context);
                    sWakeLock = pm.NewWakeLock(WakeLockFlags.Partial, WAKELOCK_KEY);
                }
            }

            Log.Verbose(TAG, "Acquiring wakelock");
            sWakeLock.Acquire();
            //intent.SetClassName(context, className);
            intent.SetClass(context, classType);

            context.StartService(intent);
        }

        private void handleRegistration(Context context, Intent intent)
        {
            var registrationId = intent.GetStringExtra(GCMConstants.EXTRA_REGISTRATION_ID);
            var error = intent.GetStringExtra(GCMConstants.EXTRA_ERROR);
            var unregistered = intent.GetStringExtra(GCMConstants.EXTRA_UNREGISTERED);

            Log.Debug(TAG, "handleRegistration: registrationId = " + registrationId + ", error = " + error + ", unregistered = " + unregistered);

            // registration succeeded
            if (registrationId != null)
            {
                PushClient.ResetBackoff(context);
                PushClient.SetRegistrationId(context, registrationId);
                if (OnRegistered(context, registrationId))
                {
                    var ev = new NotificationRegisteredArgs(registrationId);
                    var notificationService = Resolver.Get<INotificationService>();
                    if (notificationService != null)
                        notificationService.OnNotificationRegistered(ev);
                    else
                        NotificationRegistered(this, ev);
                }
                return;
            }

            // unregistration succeeded
            if (unregistered != null)
            {
                // Remember we are unregistered
                PushClient.ResetBackoff(context);
                var oldRegistrationId = PushClient.ClearRegistrationId(context);
                OnUnRegistered(context, oldRegistrationId);
                return;
            }

            // last operation (registration or unregistration) returned an error;
            Log.Debug(TAG, "Registration error: " + error);
            // Registration failed
            if (GCMConstants.ERROR_SERVICE_NOT_AVAILABLE.Equals(error))
            {
                var retry = OnRecoverableError(context, error);

                if (retry)
                {
                    int backoffTimeMs = PushClient.GetBackoff(context);
                    int nextAttempt = backoffTimeMs / 2 + sRandom.Next(backoffTimeMs);

                    Log.Debug(TAG, "Scheduling registration retry, backoff = " + nextAttempt + " (" + backoffTimeMs + ")");

                    var retryIntent = new Intent(GCMConstants.INTENT_FROM_GCM_LIBRARY_RETRY);
                    retryIntent.PutExtra(EXTRA_TOKEN, TOKEN);

                    var retryPendingIntent = PendingIntent.GetBroadcast(context, 0, retryIntent, PendingIntentFlags.OneShot);

                    var am = AlarmManager.FromContext(context);
                    am.Set(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + nextAttempt, retryPendingIntent);

                    // Next retry should wait longer.
                    if (backoffTimeMs < MAX_BACKOFF_MS)
                    {
                        PushClient.SetBackoff(context, backoffTimeMs * 2);
                    }
                }
                else
                {
                    Log.Debug(TAG, "Not retrying failed operation");
                }
            }
            else
            {
                // Unrecoverable error, notify app
                OnError(context, error);
            }
        }

    }

    public interface IPushClient
    {
        PushHandlerServiceBinder PushHandlerBinder { get; set; }
    }

    public class PushHandlerServiceBinder : Binder
    {
        public PushHandlerServiceBase Service { get; protected set; }

        public bool IsBound { get; set; }

        public PushHandlerServiceBinder(PushHandlerServiceBase service)
        {
            this.Service = service;
        }
    }

    public class PushHandlerServiceConnection : Java.Lang.Object, IServiceConnection
    {
        IPushClient client;

        public event EventHandler ServiceConnected = delegate { };

        public PushHandlerServiceConnection(IPushClient client)
        {
            if (client != null)
                this.client = client;
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            var serviceBinder = service as PushHandlerServiceBinder;
            if (serviceBinder != null)
            {
                this.client.PushHandlerBinder = serviceBinder;
                this.client.PushHandlerBinder.IsBound = true;

                ServiceConnected(this, EventArgs.Empty);
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            this.client.PushHandlerBinder.IsBound = false;
        }
    }
}