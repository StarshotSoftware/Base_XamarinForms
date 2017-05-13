using System;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using System.Threading;

#if NETFX_CORE
using Windows.System.Threading;
#endif

namespace BaseStarShot.Net
{
    public class HttpWebRequestManager : WebRequestManager
    {
        const int DefaultTimeout = 120000;

        protected override async Task<System.IO.Stream> GetRequestStreamAsync(HttpWebRequest webRequest)
        {
            System.IO.Stream stream = null;
            WebException webEx = null;
            ManualResetEvent allDone = new ManualResetEvent(false);

            int timeout = DefaultTimeout;
#if !WINDOWS_PHONE && !NETFX_CORE
            timeout = webRequest.Timeout;
#endif
            var t = Task.Factory.FromAsync<System.IO.Stream>(webRequest.BeginGetRequestStream, webRequest.EndGetRequestStream, null);
            t.ContinueWith(task =>
            {
                try
                {
                    stream = task.Result;
                }
                catch (AggregateException ex)
                {
                    webEx = ex.InnerExceptions.FirstOrDefault() as WebException;
				}
                finally
                {
                    allDone.Set();
                }
            });
#if !NETFX_CORE
            ThreadPool.RegisterWaitForSingleObject(((IAsyncResult)t).AsyncWaitHandle, new WaitOrTimerCallback(TimeoutCallback), webRequest, timeout, true);
#else
            Task.Run(() =>
            {
                if (!t.Wait(timeout))
                {
                    TimeoutCallback(webRequest, true);
                }
            });
#endif
            await Task.Run(() => allDone.WaitOne());

            if (webEx != null)
                throw webEx;

            return stream;
        }


        protected override async Task<Response> GetResponseAsync(HttpWebRequest webRequest)
        {
            Response response = null;
            WebExceptionStatus status = WebExceptionStatus.Success;
            ManualResetEvent allDone = new ManualResetEvent(false);

            int timeout = DefaultTimeout;
#if !WINDOWS_PHONE && !NETFX_CORE
            timeout = webRequest.Timeout;
#endif
            var t = Task.Factory.FromAsync<WebResponse>(webRequest.BeginGetResponse, webRequest.EndGetResponse, null);
            t.ContinueWith(task =>
            {
                HttpWebResponse loWebResponse = null;
                try
                {
                    loWebResponse = (HttpWebResponse)task.Result;
                }
                catch (AggregateException ex)
                {
                    WebException webEx = ex.InnerExceptions.FirstOrDefault() as WebException;
                    if (webEx != null)
                    {
                        status = (WebExceptionStatus)webEx.Status;
                        loWebResponse = (HttpWebResponse)webEx.Response;

//                        var w = webEx.Response;
//                        using (System.IO.Stream respStream = w.GetResponseStream())
//                        using (System.IO.StreamReader respReader = new System.IO.StreamReader(respStream))
//                        {
//                            var error = respReader.ReadToEnd();
//                        }
                    }
                    else
                        status = WebExceptionStatus.ConnectFailure;
                }
                finally
                {
                    response = new Response(loWebResponse, status);
                    allDone.Set();
                }
            });
#if !NETFX_CORE
            ThreadPool.RegisterWaitForSingleObject(((IAsyncResult)t).AsyncWaitHandle, new WaitOrTimerCallback(TimeoutCallback), webRequest, timeout, true);
#else
            Task.Run(() =>
            {
                if (!t.Wait(timeout))
                {
                    TimeoutCallback(webRequest, true);
                }
            });
#endif
            await Task.Run(() => allDone.WaitOne());

            return response;
        }

        private static void TimeoutCallback(object state, bool timedOut)
        {
            if (timedOut)
            {
                HttpWebRequest request = (HttpWebRequest)state;
                if (state != null)
                {
                    Logger.Write("HttpWebRequestManager", "Timeout: " + request.Method + " " + request.RequestUri.ToString());
                    request.Abort();
                }
            }
        }

        //protected override async Task<Response> GetResponseAsync(HttpWebRequest request)
        //{
        //    try
        //    {
        //        ManualResetEvent sendEvent = new ManualResetEvent(false);
        //        Response result = null;

        //        var sendTask = Task.Run(async () =>
        //        {
        //            try
        //            {
        //                result = await base.GetResponseAsync(request);
        //            }
        //            finally
        //            {
        //                sendEvent.Set();
        //            }
        //        });

        //        await Task.Run(() =>
        //        {
        //            sendEvent.WaitOne(request.Timeout);
        //            if (!sendTask.IsCompleted)
        //                request.Abort();
        //        });

        //        return result;
        //    }
        //    catch
        //    {
        //        return null;
        //    }
        //}
    }
}

