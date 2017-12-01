// RingBuffer.cs --- This is where you apply your OCD.
//
// Copyright (C) 2016 Damon Kwok
//
// Author: gww <DamonKwok@outlook.com>
// Date: 2015-09-01
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
//Code:
using System.Collections.Generic;
using System.IO;

namespace Pandora
{
    public class Packet : LineBuffer
    {
        private enum PacketState
        {
            NotRead,
            Reading,
            Completed
        }

        private enum ChecktState
        {
            NotCheck,
            Invalid,
            Valid
        }

        public BufferHandler head;
        public BufferHandler body;
        public const int HEAD_LEN = 10;

        public Packet(int code = 0)
            : base(4096, 256)
        {
            this.arrts.Add("len", 0);
            this.arrts.Add("sum", 0);
            this.arrts.Add("code", code);

            this.WriteInt32BE(0);
            this.WriteInt32BE(0);
            this.WriteInt16BE(code);
        }

        private PacketState state = PacketState.NotRead;
        private ChecktState check = ChecktState.NotCheck;
        private Dictionary<string, object> arrts = new Dictionary<string, object>();

        public TYPE GetArrt<TYPE>(string arrt)
        {
            if (!this.arrts.ContainsKey(arrt))
                return default(TYPE);
            return (TYPE)this.arrts[arrt];
        }

        public int ErrorCode
        {
            get { return this.GetArrt<int>("sum"); }
        }

        public int Code
        {
            get { return this.GetArrt<int>("code"); }
        }

        public bool IsNotRead
        {
            get
            {
                return this.state == PacketState.NotRead;
            }
        }

        public bool IsReading
        {
            get
            {
                return this.state == PacketState.Reading;
            }
        }

        public bool IsReadCompleted
        {
            get
            {
                return this.state == PacketState.Completed;
            }
        }

        public bool Validity
        {
            get
            {
                return this.IsReadCompleted && this.check == ChecktState.Valid;
            }
        }

        public void ReadFromBuffer(NetBuffer rbuf)
        {
            if (this.state == PacketState.Completed)
                return;

            if (this.state == PacketState.NotRead)
            {
                this.Clear();
                if (rbuf.DataSize >= Packet.HEAD_LEN)
                {
                    this.WriteBuffer(rbuf, HEAD_LEN);
                    BufferHandler bh = new BufferHandler(this.GetBytes(), HEAD_LEN, 0);
                    int len = bh.ReadInt32BE();
                    int sum = bh.ReadInt32BE();
                    int code = bh.ReadInt16BE();

                    this.arrts["len"] = len;
                    this.arrts["sum"] = sum;
                    this.arrts["code"] = code;

                    //Log.LogDebug("len:" + len + "  sum:" + sum + "  code:" + code);

                    if (rbuf.DataSize >= len - HEAD_LEN)
                    {
                        this.WriteBuffer(rbuf, (len - Packet.HEAD_LEN));
                        this.state = PacketState.Completed;
                        this.recv_complete();
                    }
                    else
                    {
                        this.state = PacketState.Reading;
                        this.WriteBuffer(rbuf, rbuf.DataSize);
                    }
                }// end if
            }
            else if (this.state == PacketState.Reading)
            {
                int residual = this.GetArrt<int>("len") - this.DataSize;

                if (rbuf.DataSize >= residual)
                {
                    this.WriteBuffer(rbuf, residual);
                    this.state = PacketState.Completed;
                    this.recv_complete();
                }
                else
                {
                    this.WriteBuffer(rbuf, rbuf.DataSize);
                }
            }
        }


        private int calculateSum()
        {
            int sum = 0x77;
            int pos = HEAD_LEN;//(HEAD_LEN - 2);
            int len = this.BufferSize - HEAD_LEN;//(HEAD_LEN - 2);
            while (pos < len)
            {
                sum += this.GetBytes()[pos++] & 0xff;
            }
            return (sum) & 0x7F7F;
        }

        private void checkPacket()
        {
            int sum = this.GetArrt<int>("sum");
            this.check = (sum == this.calculateSum()) ? ChecktState.Valid : ChecktState.Invalid;
        }

        public void Flush()
        {
            int sum = this.calculateSum();
            BufferHandler bh = new BufferHandler(this.GetBytes(), HEAD_LEN, 0);
            bh.WriteInt32BE(this.DataSize);
            bh.WriteInt32BE(sum);

            this.arrts["len"] = this.DataSize;
            this.arrts["sum"] = sum;
        }

        private object mCachedProto;
        public T ReadProtobuf<T>()
        {
            if (mCachedProto != null)
            {
                GameLogger.LogWarn("Pack Proto Read From Cache:" + typeof(T));
                return (T)mCachedProto;
            }

            byte[] bytes = this.ReadByteArray(this.DataSize);
            MemoryStream ms = new MemoryStream(bytes);
            T context = ProtoBuf.Serializer.Deserialize<T>(ms);
            mCachedProto = context;
            return context;
        }

        public void WriteProtobuf(ProtoBuf.IExtensible info)
        {
            if (info == null)
                return;

            MemoryStream ms = new MemoryStream();
            ProtoBuf.Serializer.Serialize(ms, info);
            byte[] buf = ms.ToArray();
            this.WriteByteArray(buf, buf.Length);
        }

        public void recv_complete()
        {
            // 1-1 非打印模式
            this.ReadInt32BE();
            this.ReadInt32BE();
            this.ReadInt16BE();

            //1-2 打印模式
            //		int len = this.ReadInt32BE ();
            //		int sum = this.ReadInt32BE ();
            //		int code = this.ReadInt16BE ();
            //		if (code != -0x205 && code != -0x206)
            //			Log.LogDebug ("[Packet] recved packet | len:" + len + "  sum:" + sum + "  code:" + code + "(0x" + Convert.ToString (Math.Abs(code), 16) + ")");

            // 2 check sum
            //this.checkPacket ();
        }
    }
}