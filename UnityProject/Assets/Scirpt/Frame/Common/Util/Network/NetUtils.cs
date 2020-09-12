using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace GameFrame.Utils
{
    public class NetUtils
    {
        public static int GetAvailablePort()
        {
            int port = 0;
            try
            {
                var s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                s.Bind(new IPEndPoint(IPAddress.Any, 0));
                port = (s.LocalEndPoint as IPEndPoint).Port;
                s.Close();
            }
            catch (Exception e)
            {
                throw e;
            }

            return port;
        }

        /// <summary>
        /// 网络可用
        /// </summary>
        public static bool NetAvailable => Application.internetReachability != NetworkReachability.NotReachable;

        /// <summary>
        /// 是否是无线
        /// </summary>
        public static bool IsWifi =>
            Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
        
    }
}