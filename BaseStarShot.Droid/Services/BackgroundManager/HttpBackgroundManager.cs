using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BaseStarShot.Net;
using Java.Lang;
using Xamarin.Forms;
using Java.Util;
using System.IO;
using BaseStarShot.IO;
using System.Collections.Specialized;
using Base1902;

namespace BaseStarShot.Services
{
    public class HttpBackgroundManager : IBackgroundManager, IHttpBackgroundManagerClient
    {
        public event EventHandler<TransferBackgroundProgressArgs> OnTransferProgressEvent;
        public event EventHandler<TransferBackgroundCompleteArgs> OnTransferCompleteEvent;
        public event EventHandler<TransferBackgroundFailedArgs> OnTransferFailedEvent;

        public static IHttpClientManager manager { get; set; }
        public static HttpRequest request;
        public static object objectData;

        //Notification BackgroundManager
        public NotificationParamsDTO notificationParams;

        public Guid operationId;
        public Intent intent;


        HttpBackgroundManagerServiceConnection connection;

        static HttpBackgroundManager()
        {
            manager = new HttpClientManager();
            manager.Timeout = TimeSpan.FromDays(1);
        }

        public System.Threading.Tasks.Task ReattachActiveUploadsAsync<T>()
        {
            throw new NotImplementedException();
        }

        public void CreateRequest(List<KeyValuePair<string, string>> headers, string urlRequest, WebRequestMethod methodRequest, Guid? operationId = null)
        {
            UUID uid = UUID.RandomUUID();
            if (operationId.HasValue)
                this.operationId = operationId.Value;
            else
                this.operationId = Guid.Parse(uid.ToString());

            request = new HttpRequest
            {
                Url = urlRequest,
                Method = methodRequest,
                UseJsonPropertyEncoding = false
            };

            foreach (KeyValuePair<string, string> header in headers)
            {
                request.Headers[header.Key] = header.Value;
            }
        }

        public void CreateRequestAndUpload(List<KeyValuePair<string, string>> headers, string urlRequest, WebRequestMethod methodRequest, object fileRequest = null, Guid? operationId = null)
        {
            CreateRequest(headers, urlRequest, methodRequest, operationId);

            if (fileRequest != null)
                UploadInBackground<Guid>(fileRequest, null);
        }

        public void CreateSuccessNotification(string title, string message, string failedMessage, string imagePath, IDictionary<string, string> notificationParams, int additionalCount = 0)
        {
            this.notificationParams = new NotificationParamsDTO();
            this.notificationParams.Title = title;
            this.notificationParams.Message = message;
            this.notificationParams.FailedMessage = failedMessage;
            this.notificationParams.ImagePath = imagePath;
            this.notificationParams.Params = notificationParams;
			this.notificationParams.AdditionalCount = additionalCount;
        }

        public void UploadInBackground<T>(object data, System.Threading.CancellationTokenSource cts = null)
        {
            objectData = data;
            intent = new Intent(Forms.Context, typeof(HttpBackgroundManagerService));
            intent.PutExtra("OperationId", this.operationId.ToString());
            if (connection == null)
            {
                connection = new HttpBackgroundManagerServiceConnection(this);
                connection.ServiceConnected += connection_ServiceConnected;
                connection.ServiceDisconnected += connection_ServiceDisconnected;
                Forms.Context.BindService(intent, connection, Bind.AutoCreate);
            }
            else
            {
                Forms.Context.StartService(intent);
            }
        }

        void connection_ServiceDisconnected(object sender, EventArgs e)
        {
            BackgroundBinder.Service.OnTransferCompleteEvent -= Service_OnTransferCompleteEvent;
            BackgroundBinder.Service.OnTransferProgressEvent -= Service_OnTransferProgressEvent;
            BackgroundBinder.Service.OnTransferFailedEvent -= Service_OnTransferFailedEvent;
            Forms.Context.UnbindService(connection);
        }

        void connection_ServiceConnected(object sender, EventArgs e)
        {
            BackgroundBinder.Service.OnTransferCompleteEvent += Service_OnTransferCompleteEvent;
            BackgroundBinder.Service.OnTransferProgressEvent += Service_OnTransferProgressEvent;
            BackgroundBinder.Service.OnTransferFailedEvent += Service_OnTransferFailedEvent;
            Forms.Context.StartService(intent);
        }

        void Service_OnTransferFailedEvent(object sender, TransferBackgroundFailedArgs e)
        {
            OnTransferFailed(e);
        }

        void Service_OnTransferProgressEvent(object sender, TransferBackgroundProgressArgs e)
        {
            OnTransferProgress(e);
        }

        void Service_OnTransferCompleteEvent(object sender, TransferBackgroundCompleteArgs e)
        {
            OnTransferComplete(e);
        }

        public void OnTransferProgress(TransferBackgroundProgressArgs args)
        {
            if (OnTransferProgressEvent != null)
                OnTransferProgressEvent(this, args);
        }

        public void OnTransferComplete(TransferBackgroundCompleteArgs args)
        {
            args.notificationParams = this.notificationParams;
            this.notificationParams = null;
            if (OnTransferCompleteEvent != null)
                OnTransferCompleteEvent(this, args);
        }

        public void OnTransferFailed(TransferBackgroundFailedArgs args)
        {
            if (OnTransferFailedEvent != null)
                OnTransferFailedEvent(this, args);
        }

        public void CreateLocalNotification()
        {

        }

        public HttpBackgroundManagerBinder BackgroundBinder
        {
            get;
            set;
        }

    }

    [Service]
    public class HttpBackgroundManagerService : Service
    {
        public event EventHandler<TransferBackgroundProgressArgs> OnTransferProgressEvent;
        public event EventHandler<TransferBackgroundCompleteArgs> OnTransferCompleteEvent;
        public event EventHandler<TransferBackgroundFailedArgs> OnTransferFailedEvent;

        Guid uploadOperationGuid;
        public IDictionary<string, string> sNotificationParams;
        public Context CurrentContext { get; set; }
        public IBinder binder;

        int countSuccess = 0;
        int countFailed = 0;
        string responseString = "";

        List<Dictionary<string, object>> listOfForms;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            this.uploadOperationGuid = Guid.Parse(intent.GetStringExtra("OperationId"));
            listOfForms = new List<Dictionary<string, object>>();
            Task.Factory.StartNew(async () =>
            {
                while (binder == null)
                {
                    await Task.Delay(1);
                }

                await HandleUpload(this.uploadOperationGuid);
            });

            return StartCommandResult.NotSticky;
        }

        void CopyHeaders(object rootTo, NameValueCollection to, NameValueCollection from)
        {
            foreach (string header in from.AllKeys)
            {
                try
                {
                    if (!(WebHeaderCollection.IsRestricted(header)))
                        to.Add(header, from[header]);
                }
                catch
                {
                    try
                    {
                        rootTo.GetType().GetProperty(header.Replace("-", "")).SetValue(rootTo, from[header]);
                    }
                    catch { }
                }
            }
        }

        public override void OnTaskRemoved(Intent rootIntent)
        {
            base.OnTaskRemoved(rootIntent);
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

        }

        void NotifyResult()
        {
            if (countSuccess == 0)
            {
                OnTransferFailed(new TransferBackgroundFailedArgs
                (
                    uploadOperationGuid,
                    responseString
                ));
            }
            else
            {
                OnTransferComplete(new TransferBackgroundCompleteArgs(uploadOperationGuid, responseString, totalSuccessFileCount: countSuccess, totalFileCount: (listOfForms.Count() + countSuccess)));
            }
            countFailed = 0;
            countSuccess = 0;
        }

        async Task HandleUpload(Guid uid)
        {
            var startTime = DateTime.Now;
            OnTransferProgress(new TransferBackgroundProgressArgs(0, 100, uid));
            BaseStarShot.Net.HttpResponse response = null;
            var serviceManager = HttpBackgroundManager.manager;
            var serviceWebRequest = HttpBackgroundManager.request;
            object serviceObjectData = HttpBackgroundManager.objectData;
            var postData = ReflectionHelper.ToFlatDictionary(serviceObjectData);

            listOfForms.Clear();

            foreach (var pd in postData)
            {
                if (pd.Value is Base1902.IO.File)
                {
                    var filesDic = new Dictionary<string, object>();
                    filesDic.Add(pd.Key, pd.Value);
                    foreach (var otherData in postData)
                    {
                        if (!(otherData.Value is Base1902.IO.File))
                        {
                            filesDic.Add(otherData.Key, otherData.Value);
                        }
                    }
                    listOfForms.Add(filesDic);
                }
            }

            foreach (var form in listOfForms)
            {

                EventHandler<TransferProgressBytesArgs> onProgress = (s, e) =>
                {
                    OnTransferProgress(new TransferBackgroundProgressArgs(e.TransferredBytes, e.TotalBytes, uid));
                    Logger.DebugWrite("Uploading:", e.FileName + ":" + e.TransferredBytes + " / " + e.TotalBytes);
                };

                serviceManager.OnTransferProgressEvent += onProgress;

                var request = new HttpRequest
                {
                    Url = serviceWebRequest.Url,
                    Data = form,
                    AppendUrl = serviceWebRequest.AppendUrl,
                    IsJsonContent = serviceWebRequest.IsJsonContent,
                    Method = serviceWebRequest.Method,
                    Serialization = serviceWebRequest.Serialization,
                    Parameters = serviceWebRequest.Parameters,
                    Tag = uploadOperationGuid,
                    UseJsonPropertyEncoding = serviceWebRequest.UseJsonPropertyEncoding,
                    Headers = serviceWebRequest.Headers
                };

                try
                {
                    response = await serviceManager.GetResponseAsync(request);
                    foreach (var item in form)
                        if (item.Value is Base1902.IO.File)
                        {
                            var file = item.Value as Base1902.IO.File;
                            Logger.DebugWrite("Uploading:", file.Name + " completed");
                        }

                    if (response.IsSuccessStatusCode)
                        countSuccess++;
                    else
                        countFailed++;
                    responseString = await response.ReadAsStringAsync();
                }
                catch (WebException ex)
                {
                    Logger.DebugWriteErrorLog("Uploading:", ex);
                    countFailed++;
                }
                catch (System.Exception ex)
                {
                    Logger.DebugWriteErrorLog("Uploading:", ex);
                    countFailed++;
                }
                finally
                {
                    serviceManager.OnTransferProgressEvent -= onProgress;
                }
            }

            NotifyResult();
            StopSelf();
            var endTime = DateTime.Now;
            Logger.DebugWrite("Uploading:", "Finished in :" + (endTime - startTime).TotalMilliseconds);
        }

        public void OnTransferProgress(TransferBackgroundProgressArgs args)
        {
            if (OnTransferProgressEvent != null)
                OnTransferProgressEvent(this, args);
        }

        public void OnTransferComplete(TransferBackgroundCompleteArgs args)
        {
            if (OnTransferCompleteEvent != null)
                OnTransferCompleteEvent(this, args);
        }

        public void OnTransferFailed(TransferBackgroundFailedArgs args)
        {
            if (OnTransferFailedEvent != null)
                OnTransferFailedEvent(this, args);
        }


        public override IBinder OnBind(Intent intent)
        {
            binder = new HttpBackgroundManagerBinder(this);
            return binder;
        }
    }
}