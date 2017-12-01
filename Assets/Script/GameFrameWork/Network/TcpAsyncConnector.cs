// RingBuffer.cs --- This is where you apply your OCD.
//
// Copyright (C) 2016 Damon Kwok
//
// Author: gww <damon-kwok@outlook.com>
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
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using System.Diagnostics;

namespace Pandora
{
    public class TcpAsyncConnector : Connector
    {
        //客户端本地消息0-100
        public const short C_DISCONNECT = -1;
        public const short C_NOT_CONNECTED = -2;
        public const short C_CAN_NOT_CONNECT = -3;
        public const short C_CONNECTED = -9;
        public const short C_PLUGIN_MSG = -7;

        private string name = "tcp_conn1";
        private Socket fd;
        private IPEndPoint hostEndPoint;
        private AtomicBoolean connected = new AtomicBoolean(false);
        private int recv_buf_size;
        private int send_buf_size;
        private RingBuffer ringbuf;
        private byte[] rcvbuf;
        private BlockChannel chan_sent;
        private Channel chan_recv;


        public AtomicLong recv_begin = new AtomicLong(0);
        public AtomicLong send_begin = new AtomicLong(0);
        private bool has_idle_thread;
        private DateTime time_stamp_begin = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(2016, 1, 1));
        private const int SendIdleInternal = 40;
        private const int IdleCheckMaxTime = 90;

        public TcpAsyncConnector(string name, int rcv_buf, int snd_buf)
        {
            this.name = name;
            this.fd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            this.recv_buf_size = rcv_buf;
            this.send_buf_size = snd_buf;
            this.fd.ReceiveBufferSize = this.recv_buf_size;
            this.fd.SendBufferSize = this.send_buf_size * 100;
            this.ringbuf = new RingBuffer(this.recv_buf_size * 10, this.recv_buf_size / 2);
            this.rcvbuf = new byte[this.recv_buf_size];
            this.chan_sent = new BlockChannel();
            this.chan_recv = new Channel();
        }

        public string Name { get { return this.name; } }

        public bool Connected { get { return this.connected.Get(); } }

        public bool NotConnected { get { return this.connected.Get() == false; } }

        public Channel RecvChannel
        {
            get
            {
                return this.chan_recv;
            }
        }

        public virtual void SetKeepAlive(ulong keepalive_time, ulong keepalive_interval)
        {
            int bytes_per_long = 32 / 8;
            byte[] keep_alive = new byte[3 * bytes_per_long];
            ulong[] input_params = new ulong[3];
            int i1;
            int bits_per_byte = 8;

            if (keepalive_time == 0 || keepalive_interval == 0)
                input_params[0] = 0;
            else
                input_params[0] = 1;
            input_params[1] = keepalive_time;
            input_params[2] = keepalive_interval;
            for (i1 = 0; i1 < input_params.Length; i1++)
            {
                keep_alive[i1 * bytes_per_long + 3] = (byte)(input_params[i1] >> ((bytes_per_long - 1) * bits_per_byte) & 0xff);
                keep_alive[i1 * bytes_per_long + 2] = (byte)(input_params[i1] >> ((bytes_per_long - 2) * bits_per_byte) & 0xff);
                keep_alive[i1 * bytes_per_long + 1] = (byte)(input_params[i1] >> ((bytes_per_long - 3) * bits_per_byte) & 0xff);
                keep_alive[i1 * bytes_per_long + 0] = (byte)(input_params[i1] >> ((bytes_per_long - 4) * bits_per_byte) & 0xff);
            }

            this.fd.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, keep_alive);
        }

        public override void Connect(String ip, int port, int timeout_ms)
        {
            this.chan_sent.Clear();

            this.hostEndPoint = new IPEndPoint(IPAddress.Parse(ip), Convert.ToInt32(port));

            SocketAsyncEventArgs connectArgs = new SocketAsyncEventArgs();

            connectArgs.UserToken = this.fd;
            connectArgs.RemoteEndPoint = this.hostEndPoint;
            connectArgs.Completed += new EventHandler<SocketAsyncEventArgs>(delegate (object sender, SocketAsyncEventArgs e)
            {

                this.connected.Set(e.SocketError == SocketError.Success);

                if (this.connected.Get())
                {
                    GameLogger.Log("[socket] tcp connected:(" + ip + ":" + port + ")");
                    this.start_io_thread();
                    this.chan_recv.Write(new Packet(TcpAsyncConnector.C_CONNECTED));


                    long now = (long)(DateTime.Now - time_stamp_begin).TotalSeconds;
                    this.recv_begin.Set(now);
                    this.send_begin.Set(now);
                }
                else
                {
                    this.chan_recv.Write(new Packet(TcpAsyncConnector.C_CAN_NOT_CONNECT));
                    GameLogger.LogWarn("[socket] tcp can't connect:(" + ip + ":" + port + ")");
                }
            });
            this.fd.ConnectAsync(connectArgs);


            SocketError errorCode = connectArgs.SocketError;
            if (errorCode != SocketError.Success)
            {
                throw new SocketException((Int32)errorCode);
            }
        }

        public override void Disconnect()
        {
            this.disconnectAndCallback("local closed!");
        }

        public Packet ReadPacket()
        {
            if (this.RecvChannel.Count > 0)
                return this.RecvChannel.Read<Packet>();
            return null;
        }

        public void PutLocalPacket(Packet pack)
        {
            if (pack != null)
                this.chan_recv.Write(pack);
        }

        public override void Send(Packet pack)
        {
            if (pack == null || !this.connected.Get())
            {
                GameLogger.LogError("[socket] send failed");
                return;
            }
            this.chan_sent.Write(pack);
        }

        public void disconnectAndCallback(string info)
        {
            if (!this.connected.GetAndSet(false))
                return;

            GameLogger.LogWarn(" [socket] disconnect: " + info);

            this.chan_recv.Write(new Packet(TcpAsyncConnector.C_DISCONNECT));
            this.chan_sent.Close();

            try
            {
                this.fd.Disconnect(true);
                this.fd.Close();
            }
            catch (Exception ex)
            {
                GameLogger.LogError(ex.Message);
            }
        }


        private void start_io_thread()
        {
            GameLogger.Log(" starting io thread...");

            Thread thread_recv = new Thread(new ThreadStart(this.thread_callback_recv));
            thread_recv.IsBackground = true;
            thread_recv.Start();

            Thread thread_sent = new Thread(new ThreadStart(this.thread_callback_send));
            thread_sent.IsBackground = true;
            thread_sent.Start();

            if (!has_idle_thread)
            {
                has_idle_thread = true;
                Thread thread_idle = new Thread(new ThreadStart(this.thread_callback_idle));
                thread_idle.IsBackground = true;
                thread_idle.Start();
            }
        }

        private void thread_callback_idle()
        {
            GameLogger.Log(" idle thread is startup!");
            //Packet pack = new Packet(MsgCode.C_IDLE);
            Packet pack = new Packet(-1);//todo
            while (this.connected.Get())
            {
                Thread.Sleep(1000);

                //sned check
                long now = (long)(DateTime.Now - time_stamp_begin).TotalSeconds;
                if (now - this.send_begin.Get() > SendIdleInternal)
                {
                    this.Send(pack);
                }

                //recv check
                if (now - this.recv_begin.Get() > IdleCheckMaxTime)
                {
                    this.disconnectAndCallback("idle: recv timeout!");
                    break;
                }
            }
            GameLogger.Log(" idle thread is free!");
        }

        private void thread_callback_send()
        {
            GameLogger.Log(" [socket] send thread is startup!");
            while (this.connected.Get())
            {
                Packet pack = this.chan_sent.Read<Packet>();
                if (pack == null)
                    break;

                try
                {
                    pack.Flush();
                    this.fd.Send(pack.GetBytes(), pack.DataSize, SocketFlags.None);
                    long now = (long)(DateTime.Now - time_stamp_begin).TotalSeconds;
                    this.send_begin.Set(now);
                    //Logger.Log("[socket] send-> packet: " + pack.Code + " (" + GameUtil.to16string(pack.Code) + ")  发出->" + MsgCode.GetInfo(pack.Code));
                }
                catch (Exception e)
                { //ObjectDisposedException
                   // this.disconnectAndCallback("send packet field (not connected) message code:"  + " (" + to16string(pack.Code) + ")  发出->" + MsgCode.GetInfo(pack.Code) + "|" + e.Message);
                   GameLogger.LogError(e.Message);
                }
            }
            GameLogger.Log(" send thread is free!");
        }
     
        private void thread_callback_recv()
        {
            Packet pack = null;
             GameLogger.Log("[socket] recv thread is startup!");
           
            while (this.connected.Get())
            {
                try
                {
                    //Receive方法中会一直等待服务端回发消息  
                    //如果没有回发会一直在这里等着。fd.Close会中断等待
                    int recv_len = this.fd.Receive(this.rcvbuf);
                    if (recv_len <= 0)
                    {
                        this.disconnectAndCallback("recv thread, remote closed: recv data len:" + recv_len);
                        break;
                    }

                    this.ringbuf.WriteByteArray(this.rcvbuf, recv_len);
                    while (this.ringbuf.DataSize > 0)
                    {
                        if (pack == null)
                            pack = new Packet();

                        pack.ReadFromBuffer(this.ringbuf);

                        if (pack.IsReadCompleted)
                        {
                            long now = (long)(DateTime.Now - time_stamp_begin).TotalSeconds;
                            this.recv_begin.Set(now);
                            this.chan_recv.Write(pack);
                            //Logger.Log(" recv<- packet: " + GameUtil.to16string(pack.Code)+ " (" + to16string(pack.Code) + ") 收到<-" + MsgCode.GetInfo(pack.Code));

                            pack = null;
                        }
                    }//while

                }
                catch (SocketException e)
                {
                    int errorCode = e.ErrorCode;
                    this.disconnectAndCallback("recv thread, socket errno:" + errorCode + ", msg:" + e.Message);
                    break;
                }
                catch (Exception e)
                {
                    this.disconnectAndCallback("recv thread, exception: " + e.Message);
                    break;
                }
            }  //while
            GameLogger.Log("[[socket]] recv thread is free!");
        }

    }
}