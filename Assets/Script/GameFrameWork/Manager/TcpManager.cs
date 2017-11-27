using System;
using UnityEngine;

namespace GFW
{
    public class TcpManager : HMMonoBehaviour
    {
        public static TcpManager Instance;

        protected TcpAsyncConnector mConnector;
        [SerializeField] protected string mTcpName;


        void Awake()
        {
            Instance = this;
            mTcpName = "game_connect";       
        }

        protected override void OnUpdate()
        {
            this.EnterBoost();
        }

        public void EnterBoost()
        {       
            if (mConnector == null) return;

            while (mConnector.RecvChannel.Count > 0)
            {
                Packet pack = mConnector.RecvChannel.Read<Packet>();
                if (pack != null)
                {                  
                    try
                    {
                        MsgManager.Instance.OnMessage(pack);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex.ToString());
                    }
                }
            }
        }

        private string to16string(int code)
        {
            return (code < 0 ? "-0x" : "0x") + Convert.ToString(Math.Abs(code), 16);
        }

        public void Connect(string ip, int port)
        {
            if (mConnector != null && mConnector.Connected) return;
            
            mConnector = new TcpAsyncConnector(mTcpName, 40960, 1024);

            if (!mConnector.Connected)
                mConnector.Connect(ip, port, 2000);
        }

        public void Send(short msgCode, ProtoBuf.IExtensible info = null)
        {
            Packet pack = new Packet(msgCode);
            if (info != null)
                pack.WriteProtobuf(info);
            Send(pack);
        }

        void Send(Packet pack)
        {
            mConnector.Send(pack);        
        }

        public void Disconnect()
        {
            if (mConnector != null && mConnector.Connected)
                mConnector.Disconnect();

            mConnector = null;
        }

        public bool IsConnected
        {
            get
            {
                if (mConnector != null)
                {
                    return mConnector.Connected;
                }
                return false;
            }
        }
    }
}