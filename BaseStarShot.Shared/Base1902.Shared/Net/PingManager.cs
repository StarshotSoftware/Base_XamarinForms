using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
#if !WINDOWS_PHONE && !NETFX_CORE
using System.Net.NetworkInformation;
#else
using Windows.Networking.Sockets;
#endif

namespace BaseStarShot.Net
{
    public class PingManager : IPingManager
    {
        public async Task<bool> PingURL(string host, string port)
        {
            if (string.IsNullOrEmpty(host) && string.IsNullOrEmpty(port))
                return false;

#if !WINDOWS_PHONE && !NETFX_CORE

			#if __IOS__
			return true; // TODO: create native implemention of ping
			#else
            Ping ping = new Ping();
            try
            {
                PingReply reply = ping.Send(host, 1000);
                return reply.Status == IPStatus.Success;
            }
            catch (Exception)
            {

            }
            return false;
			#endif


            //try
            //{
            //    return await Task.Run(() =>
            //    {
            //        using (var tcpClient = new System.Net.Sockets.Socket(System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp))
            //        {
            //            tcpClient.Connect(host, Convert.ToInt32(port));
            //            return true;
            //        }
            //    }); 
            //}
            //catch (Exception)
            //{
            //    return false;
            //}

#else
            try
            {
                using (var tcpClient = new StreamSocket())
                {
                    await tcpClient.ConnectAsync(
                        new global::Windows.Networking.HostName(host),
                        port,
                        SocketProtectionLevel.PlainSocket);

                    var localIp = tcpClient.Information.LocalAddress.DisplayName;
                    var remoteIp = tcpClient.Information.RemoteAddress.DisplayName;

                    tcpClient.Dispose();
                    return true;
                }
            }
            catch (Exception ex)
            {
                if (ex.HResult == -2147013895)
                {
                    //ConnectionAttemptInformation = "Error: No such host is known";
                }
                else if (ex.HResult == -2147014836)
                {
                    //ConnectionAttemptInformation = "Error: Timeout when connecting (check hostname and port)";
                }
                else
                {
                    //ConnectionAttemptInformation = "Error: Exception returned from network stack: " + ex.Message;
                }
                return false;
            }
#endif

        }
    }
}
