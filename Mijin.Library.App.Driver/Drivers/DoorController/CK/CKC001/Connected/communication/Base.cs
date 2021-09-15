using PublicAPI.CKC001.MessageObj;
using PublicAPI.CKC001.MessageObj.MsgObj;
using PublicAPI.CKC001.Others;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


//链接  通信
namespace PublicAPI.CKC001.Connected.communication
{
    internal class Base
    {
        private delegateMessageReceived dMessageReceived;//接收
        public delegateDisconnected dDisconnected;//断开连接
        protected RingBuffer receivedRingBuffer;//环形缓冲区
        protected object SyncLock;//互斥锁?
        protected byte[] readTempBuffer;//读缓冲
        internal bool IsReceived;//是否接收

        public string DevIP;//设备ip
        public System.IO.Ports.SerialPort serialPort;//串口号
        public Socket socket;//socket

        protected Base()
        {
            receivedRingBuffer = new RingBuffer(1024 * 1024 * 10);//new一个环形缓冲区
            SyncLock = new object();
            readTempBuffer = new byte[1024 * 256];//读缓冲区
            IsReceived = false;//是否接收完  
        }

        public void ReceivedCombineMethod(delegateMessageReceived dMsgReceived1)
        {
            delegateMessageReceived dMsgReceived2;
            delegateMessageReceived dMsgReceived = this.dMessageReceived;
            do
            {
                dMsgReceived2 = dMsgReceived;
                delegateMessageReceived dMsgReceived3 = (delegateMessageReceived)Delegate.Combine(dMsgReceived2, dMsgReceived1);
                dMsgReceived = Interlocked.CompareExchange<delegateMessageReceived>(ref this.dMessageReceived, dMsgReceived3, dMsgReceived2);
            }
            while (dMsgReceived != dMsgReceived2);
        }

        public void ReceivedRemoveMethod(delegateMessageReceived dMsgReceived1)
        {
            delegateMessageReceived dMsgReceived2;
            delegateMessageReceived dMsgReceived = this.dMessageReceived;
            do
            {
                dMsgReceived2 = dMsgReceived;
                delegateMessageReceived dMsgReceived3 = (delegateMessageReceived)Delegate.Remove(dMsgReceived2, dMsgReceived1);
                dMsgReceived = Interlocked.CompareExchange<delegateMessageReceived>(ref this.dMessageReceived, dMsgReceived3, dMsgReceived2);
            }
            while (dMsgReceived != dMsgReceived2);
        }

        public void DisconnectedCombineMethod(delegateDisconnected dDisconnected1)
        {
            delegateDisconnected dDisconnected2;
            delegateDisconnected dDisconnected = this.dDisconnected;
            do
            {
                dDisconnected2 = dDisconnected;
                delegateDisconnected dDisconnected3 = (delegateDisconnected)Delegate.Combine(dDisconnected2, dDisconnected1);
                dDisconnected = Interlocked.CompareExchange<delegateDisconnected>(ref this.dDisconnected, dDisconnected3, dDisconnected2);
            }
            while (dDisconnected != dDisconnected2);
        }

        public void DisconnectedRemoveMethod(delegateDisconnected dDisconnected1)
        {
            delegateDisconnected dDisconnected2;
            delegateDisconnected dDisconnected = this.dDisconnected;
            do
            {
                dDisconnected2 = dDisconnected;
                delegateDisconnected dDisconnected3 = (delegateDisconnected)Delegate.Remove(dDisconnected2, dDisconnected1);
                dDisconnected = Interlocked.CompareExchange<delegateDisconnected>(ref this.dDisconnected, dDisconnected3, dDisconnected2);
            }
            while (dDisconnected != dDisconnected2);
        }

        public void CallDelegateDiconnected(string msg)
        {
            try
            {
                if (this.dDisconnected != null)
                {
                    this.dDisconnected(msg);
                }
            }
            catch { }
        }

        public void CallDelegateRecived(MsgObjBase msg)
        {
            try
            {
                if (this.dMessageReceived != null)
                {
                    //消息接收
                    this.dMessageReceived(msg);
                }
            }
            catch { }
        }

        public void ReceivedDataSplit_AddToThreadPool()
        {
            try
            {
                Task taskSplit = new Task(ReceivedDataSplit);
                taskSplit.Start();
                //ThreadPool.QueueUserWorkItem(new WaitCallback(ReceivedDataSplit));
            }
            catch { }
        }
        [CompilerGenerated]
        protected void ReceivedDataSplit() //接收数据分帧
        {
            while (this.IsReceived)
            {
                byte[] receivedBytes = null;
                lock (this.SyncLock)
                {
                    try
                    {
                        if (this.receivedRingBuffer.DataCount < 14)
                        {
                            Monitor.Wait(this.SyncLock);
                            continue;
                        }
                        if (this.receivedRingBuffer[0] != 0x16 && this.receivedRingBuffer[1] != 0x98)
                        {
                            this.receivedRingBuffer.Clear(1);
                            continue;
                        }
                        ushort dataLen = DataConverts.Bytes_To_Ushort(new byte[2] { this.receivedRingBuffer[10], this.receivedRingBuffer[11] });
                        int cmdLen = 14 + dataLen;
                        if (cmdLen < 605535)
                        {
                            if (this.receivedRingBuffer.DataCount < cmdLen)
                            {
                                Monitor.Wait(this.SyncLock);
                                continue;
                            }
                            receivedBytes = new byte[cmdLen];
                            this.receivedRingBuffer.ReadFromRingBuffer(receivedBytes, 0, cmdLen);
                            this.receivedRingBuffer.Clear(cmdLen);
                            Monitor.Pulse(this.SyncLock);
                        }
                    }
                    catch
                    { }
                }
                if (receivedBytes != null)
                {
                    //CRC校验
                    MsgObjBase msg = new MsgObjBase(receivedBytes);
                    if (msg.CheckCRC())
                    {
                        CallDelegateRecived(msg);//处理
                    }
                }
                Thread.Sleep(5);
            }
        }
        internal virtual bool Open(string readerName, int timeOut = 2000, string _8n1 = "8:n:1") { return false; }
        internal virtual bool Open(string readerName) { return false; }
        //internal virtual bool Open(string readerName,int timeOut=2000) { return false; }
        internal virtual bool Open(Socket _socket) { return false; }
        /// <summary>
        ///虚拟方法2，发送
        /// </summary>
        internal virtual void Send(byte[] sendBytes)
        {

        }
        /// <summary>
        /// 虚拟方法2，发送
        /// </summary>
        internal virtual void Send(MsgObjBase msg)
        {
            //if (msg != null)
            //{
            //    this.Send(msg.CmdToBytes());
            //}
        }
        internal virtual void Closed() { }

    }
}
