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
using Base1902.IO;

namespace BaseStarShot.Services
{
	public class BackgroundManager : IBackgroundManager, IBackgroundManagerClient
	{
		public event EventHandler<TransferBackgroundProgressArgs> OnTransferProgressEvent;
		public event EventHandler<TransferBackgroundCompleteArgs> OnTransferCompleteEvent;
		public event EventHandler<TransferBackgroundFailedArgs> OnTransferFailedEvent;

		public static WebRequestManager manager { get; set; }
		public static HttpWebRequest webRequest;
		public static object objectData;

		//Notification BackgroundManager
		public NotificationParamsDTO notificationParams;

		public Guid operationId;
		public Intent intent;


		BackgroundManagerServiceConnection connection;

		public System.Threading.Tasks.Task ReattachActiveUploadsAsync<T> ()
		{
			throw new NotImplementedException ();
		}

		public void CreateRequest (List<KeyValuePair<string, string>> headers, string urlRequest, WebRequestMethod methodRequest, Guid? operationId = null)
		{
			UUID uid = UUID.RandomUUID ();
			if (operationId.HasValue)
				this.operationId = operationId.Value;
			else
				this.operationId = Guid.Parse (uid.ToString ());
			manager = new WebRequestManager ();
			webRequest = manager.Create (urlRequest, method: methodRequest, useJsonPropertyEncoding: false);
			foreach (KeyValuePair<string, string> header in headers) {
				webRequest.Headers [header.Key] = header.Value;
			}
		}

		public void CreateRequestAndUpload (List<KeyValuePair<string, string>> headers, string urlRequest, WebRequestMethod methodRequest, object fileRequest = null, Guid? operationId = null)
		{
			UUID uid = UUID.RandomUUID ();
			if (operationId.HasValue)
				this.operationId = operationId.Value;
			else
				this.operationId = Guid.Parse (uid.ToString ());
			manager = new WebRequestManager ();
			webRequest = manager.Create (urlRequest, method: methodRequest, useJsonPropertyEncoding: false);
			foreach (KeyValuePair<string, string> header in headers) {
				webRequest.Headers [header.Key] = header.Value;
			}

			if (fileRequest != null)
				UploadInBackground<Guid> (fileRequest, null);
		}

		public void CreateSuccessNotification (string title, string message, string failedMessage, string imagePath, IDictionary<string, string> notificationParams, int additionalCount = 0)
		{
			this.notificationParams = new NotificationParamsDTO ();
			this.notificationParams.Title = title;
			this.notificationParams.Message = message;
			this.notificationParams.FailedMessage = failedMessage;
			this.notificationParams.ImagePath = imagePath;
			this.notificationParams.Params = notificationParams;
			this.notificationParams.AdditionalCount = additionalCount;
		}

		public void UploadInBackground<T> (object data, System.Threading.CancellationTokenSource cts = null)
		{
			objectData = data;
			intent = new Intent (Forms.Context, typeof (BackgroundManagerService));
			intent.PutExtra ("OperationId", this.operationId.ToString ());
			if (connection == null) {
				connection = new BackgroundManagerServiceConnection (this);
				connection.ServiceConnected += connection_ServiceConnected;
				connection.ServiceDisconnected += connection_ServiceDisconnected;
				Forms.Context.BindService (intent, connection, Bind.AutoCreate);
			} else {
				Forms.Context.StartService (intent);
			}
		}

		void connection_ServiceDisconnected (object sender, EventArgs e)
		{
			BackgroundBinder.Service.OnTransferCompleteEvent -= Service_OnTransferCompleteEvent;
			BackgroundBinder.Service.OnTransferProgressEvent -= Service_OnTransferProgressEvent;
			BackgroundBinder.Service.OnTransferFailedEvent -= Service_OnTransferFailedEvent;
			Forms.Context.UnbindService (connection);
		}

		void connection_ServiceConnected (object sender, EventArgs e)
		{
			BackgroundBinder.Service.OnTransferCompleteEvent += Service_OnTransferCompleteEvent;
			BackgroundBinder.Service.OnTransferProgressEvent += Service_OnTransferProgressEvent;
			BackgroundBinder.Service.OnTransferFailedEvent += Service_OnTransferFailedEvent;
			Forms.Context.StartService (intent);
		}

		void Service_OnTransferFailedEvent (object sender, TransferBackgroundFailedArgs e)
		{
			OnTransferFailed (e);
		}

		void Service_OnTransferProgressEvent (object sender, TransferBackgroundProgressArgs e)
		{
			OnTransferProgress (e);
		}

		void Service_OnTransferCompleteEvent (object sender, TransferBackgroundCompleteArgs e)
		{
			OnTransferComplete (e);
		}

		public void OnTransferProgress (TransferBackgroundProgressArgs args)
		{
			if (OnTransferProgressEvent != null)
				OnTransferProgressEvent (this, args);
		}

		public void OnTransferComplete (TransferBackgroundCompleteArgs args)
		{
			args.notificationParams = this.notificationParams;
			this.notificationParams = null;
			if (OnTransferCompleteEvent != null)
				OnTransferCompleteEvent (this, args);
		}

		public void OnTransferFailed (TransferBackgroundFailedArgs args)
		{
			if (OnTransferFailedEvent != null)
				OnTransferFailedEvent (this, args);
		}

		public void CreateLocalNotification ()
		{

		}

		public BackgroundManagerBinder BackgroundBinder {
			get;
			set;
		}

	}

	[Service]
	public class BackgroundManagerService : Service
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

		public override StartCommandResult OnStartCommand (Intent intent, StartCommandFlags flags, int startId)
		{
			this.uploadOperationGuid = Guid.Parse (intent.GetStringExtra ("OperationId"));
			listOfForms = new List<Dictionary<string, object>> ();
			Task.Factory.StartNew (async () => {
				while (binder == null) {
					await Task.Delay (1);
				}

				await HandleUpload (this.uploadOperationGuid);
			});

			return StartCommandResult.NotSticky;
		}

		void CopyHeaders (object rootTo, NameValueCollection to, NameValueCollection from)
		{
			foreach (string header in from.AllKeys) {
				try {
					if (!(WebHeaderCollection.IsRestricted (header)))
						to.Add (header, from [header]);
				} catch {
					try {
						rootTo.GetType ().GetProperty (header.Replace ("-", "")).SetValue (rootTo, from [header]);
					} catch { }
				}
			}
		}

		public override void OnTaskRemoved (Intent rootIntent)
		{
			base.OnTaskRemoved (rootIntent);
		}

		public override void OnDestroy ()
		{
			base.OnDestroy ();

		}

		void NotifyResult ()
		{
			if (countSuccess == 0) {
				OnTransferFailed (new TransferBackgroundFailedArgs
				(
					uploadOperationGuid,
					responseString
				));
			} else {
				OnTransferComplete (new TransferBackgroundCompleteArgs (uploadOperationGuid, responseString, totalSuccessFileCount: countSuccess, totalFileCount: (listOfForms.Count () + countSuccess)));
			}
			countFailed = 0;
			countSuccess = 0;
		}

		async Task HandleUpload (Guid uid)
		{
            var startTime = DateTime.Now;
			OnTransferProgress (new TransferBackgroundProgressArgs (0, 100, uid));
			BaseStarShot.Net.Response response = null;
			WebRequestManager serviceManager = BackgroundManager.manager;
			HttpWebRequest serviceWebRequest = BackgroundManager.webRequest;
			object serviceObjectData = BackgroundManager.objectData;
			var postData = ReflectionHelper.ToFlatDictionary (serviceObjectData);

			listOfForms.Clear ();


			foreach (var pd in postData) {
				if (pd.Value is Base1902.IO.File) {
					var filesDic = new Dictionary<string, object> ();
					filesDic.Add ("Files[0]", pd.Value);
					foreach (var otherData in postData) {
						if (!(otherData.Value is Base1902.IO.File)) {
							filesDic.Add (otherData.Key, otherData.Value);
						}
					}
					listOfForms.Add (filesDic);
				}
			}

			foreach (var form in listOfForms) {
				HttpWebRequest wsRequest = HttpWebRequest.CreateHttp (serviceWebRequest.RequestUri);
				wsRequest.Method = serviceWebRequest.Method;
				CopyHeaders (wsRequest, wsRequest.Headers, serviceWebRequest.Headers);
				wsRequest.AllowWriteStreamBuffering = true;
				//wsRequest.Timeout = 60000;
				//wsRequest.ReadWriteTimeout = 1200000;

				wsRequest.UserAgent = Globals.UserAgent;

				string boundary = "----------------------------" + DateTime.Now.Ticks.ToString ("x");
				wsRequest.ContentType = "multipart/form-data; boundary=" + boundary;

				byte [] boundarybytes = System.Text.Encoding.UTF8.GetBytes ("\r\n--" + boundary + "\r\n");
				string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\nContent-Type: text/plain; charset=UTF-8\r\n\r\n{1}";
				try {
					using (Stream loPostData = await wsRequest.GetRequestStreamAsync ()) {
						if (loPostData == null) {
							NotifyResult ();
							StopSelf ();
							return;
						}

						Dictionary<string, object> files = new Dictionary<string, object> ();

						// write all non file items first
						foreach (var pd in form) {
							if (pd.Value is Base1902.IO.File) {
								files.Add (pd.Key, pd.Value);
								continue;
							}
							string formitem = string.Format (formdataTemplate, pd.Key, EncodeUrlData (pd.Value));
							byte [] formitembytes = System.Text.Encoding.UTF8.GetBytes (formitem);
							loPostData.Write (formitembytes, 0, formitembytes.Length);
						}

						string headerTemplate = "Content-Disposition: attachment; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
						foreach (var fileItem in files) {
							var file = (Base1902.IO.File)fileItem.Value;

							loPostData.Write (boundarybytes, 0, boundarybytes.Length);

							string header = string.Format (headerTemplate, fileItem.Key, file.Name, file.GetMimeType ());

							byte [] headerbytes = System.Text.Encoding.UTF8.GetBytes (header);

							loPostData.Write (headerbytes, 0, headerbytes.Length);

							using (var stream = await file.GetStreamAsync ()) {
								byte [] buffer = new byte [8192];
								long size = (long)stream.Length;
								long totalSize = size;

								while (size > 0) {
									int length = await stream.ReadAsync (buffer, 0, buffer.Length);
									size -= length;
									loPostData.Write (buffer, 0, length);
									OnTransferProgress (new TransferBackgroundProgressArgs (size, totalSize, uid));
									Logger.DebugWrite ("Uploading:", file.Name + ":" + size + " bytes left");
								}
							}
						}
						byte [] oef = System.Text.Encoding.UTF8.GetBytes ("\r\n--" + boundary + "--\r\n");
						loPostData.Write (oef, 0, oef.Length);
					}
				} catch (WebException ex) {
					NotifyResult ();
					StopSelf ();
				} catch (System.Exception e) {
					NotifyResult ();
					StopSelf ();
				}

				BaseStarShot.Net.WebExceptionStatus status = BaseStarShot.Net.WebExceptionStatus.Success;
				try {
					using (var loWebResponse = (HttpWebResponse)await wsRequest.GetResponseAsync ()) {
						if (loWebResponse == null) {
							NotifyResult ();
							StopSelf ();
							return;
						}
						response = new BaseStarShot.Net.Response (loWebResponse, status);
						responseString = response.GetResponseAsString ();

						if (response.StatusCode != HttpStatusCode.OK) {
							countFailed++;
						} else {
							countSuccess++;
						}
					}
				} catch (System.Exception ex) {
					NotifyResult ();
					StopSelf ();
					return;
				}
				wsRequest = null;
			}
			NotifyResult ();
			StopSelf ();
            var endTime = DateTime.Now;
            Logger.DebugWrite("Uploading:", "Finished in :" + (endTime - startTime).TotalMilliseconds);
		}

		string EncodeUrlData (object value)
		{
			if (value == null) return string.Empty;
			DateTime? dtValue = null;
			if (value is DateTime)
				dtValue = (DateTime)value;
			if (!dtValue.HasValue) {
				var ntype = Nullable.GetUnderlyingType (value.GetType ());
				if (ntype != null && ntype.FullName == "System.DateTime")
					dtValue = (DateTime?)value;
			}
			if (dtValue.HasValue)
				return dtValue.Value.ToString (Globals.DateTimeFormat);
			return value.ToString ();
		}

		public void OnTransferProgress (TransferBackgroundProgressArgs args)
		{
			if (OnTransferProgressEvent != null)
				OnTransferProgressEvent (this, args);
		}

		public void OnTransferComplete (TransferBackgroundCompleteArgs args)
		{
			if (OnTransferCompleteEvent != null)
				OnTransferCompleteEvent (this, args);
		}

		public void OnTransferFailed (TransferBackgroundFailedArgs args)
		{
			if (OnTransferFailedEvent != null)
				OnTransferFailedEvent (this, args);
		}


		public override IBinder OnBind (Intent intent)
		{
			binder = new BackgroundManagerBinder (this);
			return binder;
		}
	}
}