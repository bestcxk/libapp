using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mijin.Library.App.Common.Domain
{
    /// <summary>
    /// 端口转发
    /// </summary>
    public class NetWorkTranspond : IDisposable
    {
        public int localPort { get; set; }
        public string localIp { get; set; }
        public int TargetPort { get; set; }
        public string TargetHost { get; set; }

        public string OriginHostPort { get; set; }

        private Socket _socket { get; set; }

        private Thread _thread { get; set; }
        public NetWorkTranspond(string localIp, int localPort, string TargetIp, int TargetPort)
        {
            this.localIp = localIp;
            this.localPort = localPort;
            this.TargetHost = TargetIp;
            this.TargetPort = TargetPort;
        }

        public NetWorkTranspond(int localPort, string TargetIp, int TargetPort)
        {
            this.localIp = "0.0.0.0";
            this.localPort = localPort;
            this.TargetHost = TargetIp;
            this.TargetPort = TargetPort;
        }

        public NetWorkTranspond(string TargetIp, int TargetPort)
        {
            this.localIp = "0.0.0.0";
            this.localPort = 80;
            this.TargetHost = TargetIp;
            this.TargetPort = TargetPort;
        }

        public void Start()
        {
            //服务器IP地址  
            IPAddress ip = IPAddress.Parse(localIp);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(new IPEndPoint(ip, localPort));
            _socket.Listen(10000);
            Console.WriteLine("启动监听{0}成功", _socket.LocalEndPoint.ToString());
            _thread = new Thread(Listen);
            _thread.Start(_socket);
        }

        public void Stop()
        {
            _thread?.Abort();
            _socket?.Dispose();
        }


        //监听客户端连接
        private void Listen(object obj)
        {
            Socket serverSocket = (Socket)obj;

            IPAddress ip = null;
            try
            {
                var address = Dns.GetHostEntry(TargetHost)?.AddressList;
            }
            catch (Exception e)
            {
                ip = IPAddress.Parse(TargetHost);
            }

            while (true)
            {
                Socket tcp1 = serverSocket.Accept();
                Socket tcp2 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                tcp2.Connect(new IPEndPoint(ip, TargetPort));
                //目标主机返回数据
                ThreadPool.QueueUserWorkItem(new WaitCallback(SwapMsg), new thSock
                {
                    tcp1 = tcp2,
                    tcp2 = tcp1
                });
                //中间主机请求数据
                ThreadPool.QueueUserWorkItem(new WaitCallback(SwapMsg), new thSock
                {
                    tcp1 = tcp1,
                    tcp2 = tcp2
                });
            }
        }
        ///两个 tcp 连接 交换数据，一发一收
        public void SwapMsg(object obj)
        {
            thSock mSocket = (thSock)obj;
            while (true)
            {
                try
                {
                    byte[] result = new byte[1024];
                    int num = mSocket.tcp2.Receive(result, result.Length, SocketFlags.None);
                    if (num == 0) //接受空包关闭连接
                    {
                        if (mSocket.tcp1.Connected)
                        {
                            mSocket.tcp1.Close();
                        }
                        if (mSocket.tcp2.Connected)
                        {
                            mSocket.tcp2.Close();
                        }
                        break;
                    }
                    mSocket.tcp1.Send(result, num, SocketFlags.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    if (mSocket.tcp1.Connected)
                    {
                        mSocket.tcp1.Close();
                    }
                    if (mSocket.tcp2.Connected)
                    {
                        mSocket.tcp2.Close();
                    }
                    break;
                }
            }
        }
        private class thSock
        {
            public Socket tcp1 { get; set; }
            public Socket tcp2 { get; set; }
        }

        public void Dispose()
        {
            Stop();
        }

        public override string ToString()
        {
            return @$"{localIp}:{localPort}—{TargetHost}:{TargetPort}";
        }

        public string ToTargetString()
        {
            return @$"{TargetHost}:{TargetPort}";
        }
    }


}
