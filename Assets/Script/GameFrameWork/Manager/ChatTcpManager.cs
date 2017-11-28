using System;
using System.Collections;
using UnityEngine;

namespace GFW
{
    public class ChatTcpManager : MonoSingleton<ChatTcpManager>
    {
        
        protected TcpAsyncConnector mConnector;
        [SerializeField]
        protected string mTcpName;

        protected override void Init()
        {
            base.Init();
            mTcpName = "chatsocket_connect";
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
                    OnMessage(pack);
                }
            }
        }

        private bool OnMessage(Packet packet)
        {
            int msgCode = packet.Code;
            switch (msgCode)
            {
                case TcpAsyncConnector.C_DISCONNECT:
                case TcpAsyncConnector.C_CAN_NOT_CONNECT:
                case TcpAsyncConnector.C_NOT_CONNECTED:
                    GameLogger.LogWarn("chat server dis connected...");
                    StartCoroutine(CO_Reconnnect());
                    break;
                case TcpAsyncConnector.C_CONNECTED:
                    GameLogger.LogWarn("chat server connected...");             
                    break;             
            }
            return true;
        }

        private IEnumerator CO_Reconnnect()
        {
            yield return new WaitForSeconds(8f);

          // Connect(MainGame.Instance.chatServerIp, MainGame.Instance.chatServerPort);
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

        public void Send(Packet pack)
        {
            mConnector.Send(pack);
        }

        public void Disconnect()
        {
            if (mConnector != null && mConnector.Connected)
                mConnector.Disconnect();

            mConnector = null;
        }

    }
}

