using PublicAPI.CKC001.Others;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace PublicAPI.CKC001.Connected.communication
{

    internal class TypeTcpServer 
    {
        internal Others.delegateInternalTcpClientConnected delegateInternal;
        public Socket ListenSocket;//监听socket
        public bool IsListen { get; private set; }
        private int ListenMaxNum;//最大链接

        public TypeTcpServer() 
        {
            this.ListenMaxNum = 100;
        }

        public bool Open(int listenPort)
        {
            if (ListenSocket == null)
            {
                try
                {
                    this.IsListen = true;
                    this.ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    this.ListenSocket.Bind(new IPEndPoint(IPAddress.Any, listenPort));//绑定端口
                    this.ListenSocket.Listen(this.ListenMaxNum);//监听
                    //ThreadPool.QueueUserWorkItem(new WaitCallback(ListenClient));//工作队列?
                    Task taskClient = new Task(ListenClient);
                    taskClient.Start();
                    return true;
                }
                catch {
                    this.Closed();
                }
            }
            return false;
        }
                
        [CompilerGenerated]
        private void ListenClient()
        {
            while (this.IsListen)
            {
                try
                {
                    Socket currentSocket = this.ListenSocket.Accept();
                    TypeTcpClient tcpClient = new TypeTcpClient() 
                    {
                        ipAddress = ((IPEndPoint)currentSocket.RemoteEndPoint).Address,
                        ipPort = ((IPEndPoint)currentSocket.RemoteEndPoint).Port,
                        socket = currentSocket
                    };
                    if (tcpClient.Open(currentSocket))
                    {
                        this.CallDisconnected(tcpClient);
                    }
                    else
                    {
                        currentSocket.Close();
                        tcpClient.Closed();
                    }
                    //continue;
                }
                catch(Exception ex)
                {
                    Console.WriteLine("后台监听异常："+ex.Message);
                    //continue;
                }
                Thread.Sleep(5);
            }
        }

        internal void CallDisconnected(TypeTcpClient tcpClient)
        {
            try {
                if (this.delegateInternal != null)
                {
                    this.delegateInternal(tcpClient);
                }
            } catch {  }
        }

        public void Closed()
        {
            try {
                this.IsListen = false;
                if (this.ListenSocket != null)
                {
                    this.ListenSocket.Close();
                    this.ListenSocket = null;
                }
            } catch { }
        }

    }
}
