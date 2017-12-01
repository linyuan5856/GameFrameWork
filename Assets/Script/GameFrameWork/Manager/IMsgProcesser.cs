using System;

namespace Pandora
{
    public interface IMsgProcesser
    {
        MsgRegistInfo[] GetMsgCodes();
    }

    public interface IMsgCallBack
    {      
        void MsgCallBack(Packet pack);
    }


    public class MsgRegistInfo
    {
        private short msgCode;
        private Action<Packet> callback;

        public MsgRegistInfo(short msgCode, Action<Packet> callback)
        {
            this.msgCode = msgCode;
            this.callback = callback;
        }

        public short MsgCode
        {
            get { return msgCode; }
        }

        public Action<Packet> Callback
        {
            get { return callback; }
        }
    }

}
