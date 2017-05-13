using System;
using BaseStarShot.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using BaseStarShot;
using System.Threading;
using Foundation;
using UIKit;
using System.IO;
using BaseStarShot.IO;
using BaseStarShot.Net;
using System.Linq;
using Base1902;
using Base1902.IO;

namespace BaseStarShot.Services
{
	public class BackgroundManager : IBackgroundManager
	{
		public event EventHandler<TransferBackgroundProgressArgs> OnTransferProgressEvent;
		public event EventHandler<TransferBackgroundCompleteArgs> OnTransferCompleteEvent;
		public event EventHandler<TransferBackgroundFailedArgs> OnTransferFailedEvent;

		const int DefaultBufferSize = 8192;

		NSUrlSession session;
		NSMutableUrlRequest request;
		Guid operationId;
		string boundary;
		string successMessage;
		string failedMessage;
		int additionalCount;
		IDictionary<string, string> notificationParams;
		IDictionary<Guid, UploadItem> pendingUploads = new Dictionary<Guid, UploadItem> ();
		List<KeyValuePair<string, string>> headers;
		string url;
		WebRequestMethod methodRequest;

		public Task ReattachActiveUploadsAsync<T> ()
		{
			throw new NotImplementedException ();
		}

		public void CreateRequestAndUpload (List<KeyValuePair<string, string>> headers, string url, WebRequestMethod methodRequest, object fileRequest = null, Guid? operationId = null)
		{
			this.headers = headers;
			this.url = url;
			this.methodRequest = methodRequest;

			if (operationId.HasValue)
				this.operationId = operationId.Value;
			else
				this.operationId = Guid.NewGuid ();

			NSUrlSessionConfiguration configuration = null;

			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				configuration = NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration (this.operationId.ToString ());
			} else {
				configuration = NSUrlSessionConfiguration.BackgroundSessionConfiguration (this.operationId.ToString ());
			}
			configuration.SessionSendsLaunchEvents = true;
			configuration.HttpMaximumConnectionsPerHost = 20;
			session = NSUrlSession.FromConfiguration (configuration, (NSUrlSessionDelegate)new SessionDelegate (this), new NSOperationQueue ());

			boundary = "----------------------------" + DateTime.Now.Ticks.ToString ("x");

			if (fileRequest != null)
				UploadInBackground<Guid> (fileRequest, null);
		}

		public void CreateRequest (List<KeyValuePair<string, string>> headers, string url, WebRequestMethod methodRequest, Guid? operationId = null)
		{
			this.headers = headers;
			this.url = url;
			this.methodRequest = methodRequest;

			if (operationId.HasValue)
				this.operationId = operationId.Value;
			else
				this.operationId = Guid.NewGuid ();

			NSUrlSessionConfiguration configuration = null;

			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				configuration = NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration (this.operationId.ToString ());
			} else {
				configuration = NSUrlSessionConfiguration.BackgroundSessionConfiguration (this.operationId.ToString ());
			}
			configuration.SessionSendsLaunchEvents = true;
			configuration.HttpMaximumConnectionsPerHost = 20;
			session = NSUrlSession.FromConfiguration (configuration, (NSUrlSessionDelegate)new SessionDelegate (this), new NSOperationQueue ());

			boundary = "----------------------------" + DateTime.Now.Ticks.ToString ("x");

		}

		public void CreateSuccessNotification (string title, string message, string failedMessage, string imagePath, IDictionary<string, string> notificationParams, int additionalCount = 0)
		{
			this.successMessage = message;
			this.failedMessage = failedMessage;
			this.notificationParams = notificationParams;
			this.additionalCount = additionalCount;
		}

		public async void UploadInBackground<T> (object data, CancellationTokenSource cts = null)
		{
			var postData = ReflectionHelper.ToFlatDictionary (data);

			byte [] boundarybytes = System.Text.Encoding.UTF8.GetBytes ("\r\n--" + boundary + "\r\n");
			string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\nContent-Type: text/plain; charset=UTF-8\r\n\r\n{1}";

			var formItems = new List<string> ();
			var files = new Dictionary<string, object> ();

			foreach (var pd in postData) {
				if (pd.Value is Base1902.IO.File) {
					files.Add (pd.Key, pd.Value);
					continue;
				}
				string formitem = string.Format (formdataTemplate, pd.Key, EncodeUrlData (pd.Value));
				formItems.Add (formitem);
			}

			pendingUploads.Add (operationId, new UploadItem { NumberOfExpectedUpload = files.Count });

			string headerTemplate = "Content-Disposition: attachment; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
			foreach (var fileItem in files) {
				using (Stream loPostData = new MemoryStream ()) {
					var uploadUrl = NSUrl.FromString (url);
					request = new NSMutableUrlRequest (uploadUrl);
					request.HttpMethod = methodRequest.ToString ();
					foreach (var header in headers) {
						request [header.Key] = header.Value;
					}
					request ["Content-Type"] = "multipart/form-data; boundary=" + boundary;

					foreach (var formitem in formItems) {
						byte [] formitembytes = System.Text.Encoding.UTF8.GetBytes (formitem);
						loPostData.Write (formitembytes, 0, formitembytes.Length);
					}

					var file = (Base1902.IO.File)fileItem.Value;

					loPostData.Write (boundarybytes, 0, boundarybytes.Length);

					string header2 = string.Format (headerTemplate, fileItem.Key, file.Name, file.GetMimeType ());

					byte [] headerbytes = System.Text.Encoding.UTF8.GetBytes (header2);

					loPostData.Write (headerbytes, 0, headerbytes.Length);

					using (var stream = await file.GetStreamAsync ()) {
						byte [] buffer = new byte [DefaultBufferSize];
						long size = (long)stream.Length;
						while (size > 0) {
							int length = await stream.ReadAsync (buffer, 0, DefaultBufferSize);
							size -= length;
							loPostData.Write (buffer, 0, length);
						}
					}

					byte [] oef = System.Text.Encoding.UTF8.GetBytes ("\r\n--" + boundary + "--\r\n");
					loPostData.Write (oef, 0, oef.Length);

					loPostData.Seek (0, SeekOrigin.Begin);

					if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
						request.Body = NSData.FromStream (loPostData);
						Logger.DebugWrite ("Uploading", file.FullPath + " <> ");
						NSUrlSessionUploadTask uploadTask = session.CreateUploadTask (request);
						uploadTask.Resume ();

					} else {
						var documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
						var fileName = DateTime.Now.ToMilliseconds () + "";
						var filePath = Path.Combine (documentsPath, fileName);
						var fileStream = System.IO.File.Create (Path.Combine (documentsPath, fileName));
						loPostData.CopyTo (fileStream);

						//request.Body = NSData.FromFile(Path.Combine (documentsPath, fileName));
						Logger.DebugWrite ("Uploading", file.FullPath + " <> ");
						NSUrlSessionUploadTask uploadTask = session.CreateUploadTask (request, new NSUrl ("file://" + filePath));
						uploadTask.Resume ();
					}
				}
			}
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
			UploadItem uploadItem;
			if (pendingUploads.TryGetValue (args.OperationGUID, out uploadItem)) {
				uploadItem.NumberOfSuccessfulUpload++;
				TryNotify (args.OperationGUID, uploadItem);
			}

			if (OnTransferCompleteEvent != null)
				OnTransferCompleteEvent (this, args);
		}

		public void OnTransferFailed (TransferBackgroundFailedArgs args)
		{
			UploadItem uploadItem;
			if (pendingUploads.TryGetValue (args.OperationGUID, out uploadItem)) {
				uploadItem.NumberOfFailedUpload++;
				TryNotify (args.OperationGUID, uploadItem);
			}

			if (OnTransferFailedEvent != null)
				OnTransferFailedEvent (this, args);
		}

		void TryNotify (Guid guid, UploadItem uploadItem)
		{
			if (uploadItem.NumberOfSuccessfulUpload + uploadItem.NumberOfFailedUpload == uploadItem.NumberOfExpectedUpload) {
				if (uploadItem.NumberOfFailedUpload > 0) {
					if (!string.IsNullOrEmpty (failedMessage)) {
						Resolver.Get<IDispatcherService> ().BeginInvokeOnMainThread (() => {
							var notification = new UILocalNotification ();
							notification.AlertBody = string.Format (failedMessage, uploadItem.NumberOfSuccessfulUpload + additionalCount, uploadItem.NumberOfExpectedUpload + additionalCount);
							notification.FireDate = NSDate.FromTimeIntervalSinceNow (1);
							notification.UserInfo = NSDictionary.FromObjectsAndKeys (notificationParams.Values.ToArray (), notificationParams.Keys.ToArray ());
							UIApplication.SharedApplication.ScheduleLocalNotification (notification);
						});
					}
				} else {
					if (!string.IsNullOrEmpty (successMessage)) {
						Resolver.Get<IDispatcherService> ().BeginInvokeOnMainThread (() => {
							var notification = new UILocalNotification ();
							notification.AlertBody = string.Format (successMessage, uploadItem.NumberOfSuccessfulUpload + additionalCount, uploadItem.NumberOfExpectedUpload + additionalCount);
							notification.FireDate = NSDate.FromTimeIntervalSinceNow (1);
							notification.UserInfo = NSDictionary.FromObjectsAndKeys (notificationParams.Values.ToArray (), notificationParams.Keys.ToArray ());
							UIApplication.SharedApplication.ScheduleLocalNotification (notification);
						});
					}
				}
				pendingUploads.Remove (guid);
			}
		}

		public class SessionDelegate : NSUrlSessionDataDelegate
		{
			IBackgroundManager manager;
			object responseData;
			ManualResetEventSlim manualReset;

			public SessionDelegate (IBackgroundManager manager)
			{
				this.manager = manager;

				manualReset = new ManualResetEventSlim ();
			}

			public override void DidCompleteWithError (NSUrlSession session, NSUrlSessionTask task, NSError error)
			{
				if (error != null) {

					manager.OnTransferFailed (new TransferBackgroundFailedArgs (Guid.Parse (session.Configuration.Identifier), error.ToString ()));
				} else {
					var response = task.Response as NSHttpUrlResponse;
					if (response != null && response.StatusCode != 200) {
						manager.OnTransferFailed (new TransferBackgroundFailedArgs (Guid.Parse (session.Configuration.Identifier), response.ToString ()));
					} else {
						manualReset.Wait ();
						manager.OnTransferComplete (new TransferBackgroundCompleteArgs (Guid.Parse (session.Configuration.Identifier), responseData));
					}
				}
				task.Dispose ();
				session.Dispose ();
				this.Dispose ();
			}

			//			public override void DidSendBodyData(NSUrlSession session, NSUrlSessionTask task, long bytesSent, long totalBytesSent, long totalBytesExpectedToSend)
			//			{
			//				manager.OnTransferProgress(new TransferBackgroundProgressArgs(totalBytesSent, totalBytesExpectedToSend, operationId));
			//			}

			public override void DidReceiveData (NSUrlSession session, NSUrlSessionDataTask dataTask, NSData data)
			{
				if (data != null) {
					responseData = data.ToString ();
				}

				manualReset.Set ();
			}
		}

		public class UploadItem
		{
			public int NumberOfExpectedUpload { get; set; }
			public int NumberOfSuccessfulUpload { get; set; }
			public int NumberOfFailedUpload { get; set; }
		}
	}
}

