using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using IsUtil;
using Bing.Extensions;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// SIP2客户端通信协议基类
    /// </summary>
    public abstract class baseTcpClient
    {
        private TcpClient _tcpClient = new TcpClient();

        /// <summary>
        /// 是否连接成功
        /// </summary>
        public bool Connected { get => _tcpClient.Connected; }

        public string host { get; set; }
        public string port { get; set; }

        public int ReadTimeOut { get => _tcpClient.GetStream().ReadTimeout; set => _tcpClient.GetStream().ReadTimeout = value; }

        public baseTcpClient()
        {

        }
        /// <summary>
        /// 初始化 baseSIP2Client 并连接
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        public baseTcpClient(string host, string port)
        {
            Connect(host, port);
        }
        /// <summary>
        /// 释放client
        /// </summary>
        ~baseTcpClient()
        {
            if (_tcpClient != null)
            {
                try
                {
                    // fw 4.5.2不让自己释放
                    //_tcpClient.Dispose();

                }
                catch (Exception)
                {
                }
            }
        }

        public TcpClient GetTcpClient()
        {
            return _tcpClient;
        }
        /// <summary>
        /// 建立TCP连接
        /// </summary>
        /// <returns></returns>
        public virtual MessageModel<bool> Connect(string host, string port)
        {
            var result = new MessageModel<bool>();
            if (_tcpClient != null)
            {
                try
                {
                    _tcpClient.Close();
                    _tcpClient = new TcpClient();
                }
                catch (Exception)
                {
                }

                _tcpClient.Connect(host, port.ToInt());
                _tcpClient.ReceiveBufferSize = 1024 * 1024;
                _tcpClient.SendBufferSize = 1024 * 1024;
                if (_tcpClient.Connected)
                    ReadTimeOut = 3000;
            }
            result.success = _tcpClient.Connected;
            result.msg = result.success ? "连接成功" : "连接失败";
            if (result.success)
            {
                this.host = host;
                this.port = port;
            }
            return result;


        }
        /// <summary>
        /// Read timeOut 时会触发异常
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ReadTimeOut"></param>
        /// <returns>返回接收的信息</returns>
        internal virtual string Send(string message)
        {
            if (!Connected)
            {
                throw new Exception("未连接到socket");
            }
            var sendBytes = UTF8Encoding.UTF8.GetBytes(message + "\r\n");
            using var stream = _tcpClient.GetStream();

            stream.Write(sendBytes, 0, sendBytes.Length);
            stream.Flush();

            var data = new byte[1024 * 1024];
            stream.Read(data, 0, data.Length);
            return UTF8Encoding.UTF8.GetString(data);
        }

        /// <summary>
        /// Read timeOut 时会触发异常
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ReadTimeOut"></param>
        /// <returns>返回接收的信息</returns>
        internal virtual async Task<string> SendAsync(string message)
        {
            if (!Connected)
            {

                if (!this.host.IsEmpty())
                {
                    var res = this.Connect(host, port);
                    if (!res.success)
                    {
                        throw new Exception("未连接到socket");
                    }
                }
                else
                    throw new Exception("未连接到socket");
            }
            var sendBytes = UTF8Encoding.UTF8.GetBytes(message + "\r\n");
            using var stream = _tcpClient.GetStream();

            stream.Write(sendBytes, 0, sendBytes.Length);
            stream.Flush();

            var data = new byte[1024 * 1024];
            var len = await stream.ReadAsync(data, 0, data.Length);
            return UTF8Encoding.UTF8.GetString(data.Take(len).ToArray());
        }
    }
}
