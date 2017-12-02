using System;
using System.Collections.Generic;

namespace Pandora
{
    public class MsgManager : Singleton<MsgManager>
    {
        private Dictionary<int, Action<Packet>> messageProcessor = new Dictionary<int, Action<Packet>>();

        public void Init()
        {

        }

        public bool OnMessage(Packet pack)
        {
            if (this.messageProcessor.ContainsKey(pack.Code))
            {
                Action<Packet> mp = this.messageProcessor[pack.Code];
                if (mp != null)
                {
                    mp(pack);

                    this.DoMsgCallBack(pack);
                }
                return true;
            }

            return false;
        }

        private void RegistMsg<T>() where T : IMsgProcesser
        {        
            T t = default(T);
            t = Activator.CreateInstance<T>();
            RegisterMessage(t);
        }

        public void RegisterMessage(IMsgProcesser mp)
        {
            if (mp == null)
                return;
            MsgRegistInfo[] msgRegistInfos = mp.GetMsgCodes();
            for (int i = 0; i < msgRegistInfos.Length; i++)
            {
                MsgRegistInfo info = msgRegistInfos[i];
                if (this.messageProcessor.ContainsKey(info.MsgCode))
                    this.messageProcessor[info.MsgCode] = info.Callback;
                else
                    this.messageProcessor.Add(info.MsgCode, info.Callback);
            }
        }


        private Dictionary<short, IMsgCallBack> SeverPack__Dictionary = new Dictionary<short, IMsgCallBack>();

        public void AddMsgCallBack(short msg, IMsgCallBack icallback)
        {
            if (this.SeverPack__Dictionary.ContainsKey(msg))
            {
                this.SeverPack__Dictionary[msg] = icallback;
                return;
            }

            this.SeverPack__Dictionary.Add(msg, icallback);
        }

        public void RemoveMsgCallBack(short msg)
        {
            if (this.SeverPack__Dictionary.ContainsKey(msg))
            {
                this.SeverPack__Dictionary.Remove(msg);
            }
        }

        private void DoMsgCallBack(Packet pack)
        {
            IMsgCallBack callback = null;

            if (this.SeverPack__Dictionary.TryGetValue((short)pack.Code, out callback))
            {              
                if (callback != null)
                {               
                  callback.MsgCallBack(pack);                                    
                }
            }      
        }

        public void Clear()
        {
            this.SeverPack__Dictionary.Clear();
        }
    }

}
