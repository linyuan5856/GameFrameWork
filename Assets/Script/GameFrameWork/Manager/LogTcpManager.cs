using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFW
{
    public class LogTcpManager : HMMonoBehaviour
    {
        static public LogTcpManager Instance;

        protected TcpAsyncConnector mConnector;
        [SerializeField]
        protected string mTcpName;

       // static AtomicLong log_index = new AtomicLong(0);
        // private string clientId;
        //private string deviceModel;
        // private string screenSize;

        private float updateTime;

        void Awake()
        {
            Instance = this;
            mTcpName = "log_conn";

            //clientId = GameUtil.GetDeviceId();
            //deviceModel = SystemInfo.deviceModel;
            //screenSize = Screen.currentResolution.width + "x" + Screen.currentResolution.height;

            Connect(GameConfigPVO.Instance.logServerIp, GameConfigPVO.Instance.logServerPort);

            Application.logMessageReceived += OnLog;

            updateTime = Time.time;
        }
        private void Connect(string ip, int port)
        {
            if (mConnector != null && mConnector.Connected)
                mConnector.Disconnect();

            //
            mConnector = new TcpAsyncConnector(mTcpName, 40960, 1024);

            if (!mConnector.Connected)
                mConnector.Connect(ip, port, 2000);
        }

        private void Send(Packet pack)
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

        private bool OnMessage(Packet packet)
        {
            int msgCode = packet.Code;
            switch (msgCode)
            {
                case TcpAsyncConnector.C_DISCONNECT:
                case TcpAsyncConnector.C_CAN_NOT_CONNECT:
                case TcpAsyncConnector.C_NOT_CONNECTED:
                    Debug.LogWarning("log server dis connected...");
                    StartCoroutine(CO_Reconnnect());
                    break;
                case TcpAsyncConnector.C_CONNECTED:
                    Debug.LogWarning("log server connected...");
                    break;
            }
            return true;
        }


        private IEnumerator CO_Reconnnect()
        {
            yield return new WaitForSeconds(8f);

            Connect(GameConfigPVO.Instance.logServerIp, GameConfigPVO.Instance.logServerPort);
        }

        private Queue logQueue = Queue.Synchronized(new Queue());
        private void OnLog(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Error
                || type == LogType.Exception
                || type == LogType.Assert)
            {
                string content = condition + "~" + stackTrace.Replace("\n", "~");
                AddLog("Error:" + content);
            }
        }

        public void AddLog(string message)
        {
           // long index = log_index.IncrementAndGet();

            //int playerId = SceneSTable.Instance.SelfVO != null ? SceneSTable.Instance.SelfVO.playerId : -1;
            //string str = "";
            //string.Format(
            //    "{0} Log: | clientId:{1} | index:{2} | serverId:{3} | uid:{4} | roleId:{5} | deviceModel:{6} | screenSize:{7} | {8} | v={9}",
            //    DateTime.Now.ToLocalTime(), clientId, index, Session.SERVER_ID, Session.UID, playerId,
            //    deviceModel, screenSize, message, VersionManager.Instance.LocalVersion + "/" + VersionManager.Instance.Version);

           // logQueue.Enqueue(str);
        }

        protected override void OnUpdate()
        {
            float offsetTime = Time.time - updateTime;
            if (offsetTime > 2.0f) return;
            updateTime = Time.time;

            if (mConnector != null)
            {
                while (mConnector.RecvChannel.Count > 0)
                {
                    Packet pack = mConnector.RecvChannel.Read<Packet>();
                    if (pack != null)
                    {
                        OnMessage(pack);
                    }
                }

                if (mConnector.Connected)
                {                   
                    if (logQueue.Count > 0)
                    {
                        //string msg = string.Empty;
                        //msg = (string)logQueue.Dequeue();
                        //Packet pack = new Packet(MsgCode.C_LOG);
                        //Msg.CS_Log data = new Msg.CS_Log();
                        //data.log = msg;
                        //pack.WriteProtobuf(data);
                        //Send(pack);
                    }
                }
            }
        }
    }
}

