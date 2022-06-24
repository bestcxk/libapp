using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IsUtil;
using IsUtil.Helpers;
using IsUtil.Maps;

namespace Mijin.Library.App.Driver
{
    public class CabinetLock : ICabinetLock
    {
        private SerialPort _serialPort = new SerialPort();
        private readonly static object openLockObj = new object();

        private readonly static object sendLockObj = new object();
        
        public event Action<WebViewSendModel<List<bool>>> OnLockEvent;


        // 锁孔数量
        private int controllerCount = 1;

        /// <summary>
        /// 串口是否打开
        /// </summary>
        public bool IsOpen
        {
            get => _serialPort.IsOpen;
        }


        public Task task;

        /// <summary>
        /// 数据接收缓存
        /// </summary>
        private byte[] buffer = new byte[100];

        /// <summary>
        /// 构造函数
        /// </summary>
        public CabinetLock()
        {
            task = Task.Run(LockEvent);
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~CabinetLock()
        {
            try
            {
                _serialPort.Close();
            }
            catch (Exception)
            {
            }

            try
            {
                _serialPort.Dispose();
            }
            catch (Exception)
            {
            }
        }

        public async void LockEvent()
        {
            while (true)
            {
                await Task.Delay(1500);
                if (IsOpen)
                {
                    try
                    {
                        var dt = GetLockStatus();
                        if (dt.response.Any(s => s == true))
                        {
                        }

                        if (dt.success)
                        {
                            OnLockEvent?.Invoke(new()
                            {
                                method = "OnLockEvent",
                                response = dt.response,
                                msg = "获取锁孔板状态成功"
                            });
                        }
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
        }

        /// <summary>
        /// 设置控制板数量
        /// </summary>
        /// <param name="Count"></param>
        /// <returns></returns>
        public MessageModel<bool> SetControllerCount(Int64 Count)
        {
            controllerCount = (int) Count;
            return new MessageModel<bool>()
            {
                success = true,
                msg = "设置成功"
            };
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="com"></param>
        /// <param name="baud"></param>
        /// <returns></returns>
        public MessageModel<bool> OpenSerialPort(string com, Int64 baud)
        {
            lock (openLockObj)
            {
                var data = new MessageModel<bool>();
                Console.WriteLine(@$"com:{com} baud:{baud}");
                if (_serialPort.IsOpen)
                    ClosePort();

                _serialPort.PortName = com;
                _serialPort.BaudRate = (int) baud;
                _serialPort.DataBits = 8; //数据位：8
                _serialPort.StopBits = StopBits.One; //停止位：1
                _serialPort.Parity = Parity.None;
                _serialPort.Encoding = Encoding.Default;
                //_serialPort.ReadBufferSize = 5;  // 读取缓冲区大小设置5
                //_serialPort.DataReceived += ReceivedData;
                try
                {
                    _serialPort.Open(); //打开串口
                }
                catch (Exception e)
                {
                    data.msg = e.ToString();
                    return data;
                }

                Task.Delay(1000).GetAwaiter().GetResult();
                if (_serialPort.IsOpen)
                {
                    data.success = true;
                    data.msg = "连接成功";
                }


                return data;
            }
        }

        /// <summary>
        /// 开指定柜号
        /// </summary>
        /// <param name="boxIndex">1开始</param>
        /// <returns></returns>
        public MessageModel<string> OpenBox(Int64 boxIndex)
        {
            var controller = 1;
            var index = boxIndex;

            while (index > 24)
            {
                controller++;
                index -= 24;
            }


            return OpenBox((byte) controller, index);
        }

        private MessageModel<string> OpenBox(byte controller, Int64 boxIndex)
        {
            var res = new MessageModel<string>();
            byte[] bytes = new byte[] {0x8a, controller, (byte) boxIndex, 0x11};

            var data = Send(bytes);
            if (!data.success)
            {
                return new(data);
            }

            if (data.response[3] != 0x11 && data.response[3] != 0)
            {
                res.msg = "开锁柜失败";
                return res;
            }

            res.msg = @$"开锁柜{boxIndex}成功";
            res.success = true;
            return res;
        }

        /// <summary>
        /// 获取锁控板锁状态
        /// </summary>
        /// <returns></returns>
        public MessageModel<List<bool>> GetLockStatus()
        {
            var res = new MessageModel<List<bool>>() {response = new List<bool>()};

            for (byte cont = 1; cont <= controllerCount; cont++)
            {
                for (byte i = 1; i <= 3; i++)
                {
                    var data = GetLockStatus(cont, i);
                    if (!data.success)
                        return new(data);
                    res.response.AddRange(data.response);
                }
            }

            res.msg = "获取成功";
            res.success = true;
            return res;
        }

        /// <summary>
        /// 获取指定锁控地址 柜门状态命令
        /// </summary>
        /// <param name="lockAddr">0X01：对应 1-8 号锁位     0X02：对应 9-16 号锁位      0X03：对应 17-24 号锁</param>
        /// <returns>true 为开 false 为关</returns>
        private MessageModel<List<bool>> GetLockStatus(byte controller, byte lockAddr)
        {
            var res = new MessageModel<List<bool>>() {response = new List<bool>()};
            byte[] bytes = new byte[] {0x81, controller, lockAddr, 0x33};

            if (!IsOpen)
            {
                res.msg = "串口未打开";
                return res;
            }

            try
            {
                var data = Send(bytes);

                if (!data.success)
                {
                    return new(data);
                }

                for (int i = 0; i < 8; i++)
                {
                    var isOpen = ((buffer[3] >> i) & 0x01) == 1;
                    res.response.Add(isOpen);
                }

                res.success = true;
                res.msg = "获取锁控板状态成功";
                return res;
            }
            catch (Exception e)
            {
                return new()
                {
                    success = false,
                    devMsg = e.ToString(),
                    msg = "获取锁孔板状态失败"
                };
            }
        }

        private MessageModel<byte[]> Send(byte[] sendData)
        {
            var res = new MessageModel<byte[]>();
            if (!IsOpen)
            {
                res.msg = "串口未打开";
                return res;
            }

            lock (sendLockObj)
            {
                sendData = sendData.Append(SerialPortHelper.Get_CheckXor(sendData)).ToArray(); // 添加校验和
                int timeOut = 0;
                int time = 3, len = 0;
                while (--time > 0)
                {
                    ClearTempRead();
                    try
                    {
                        _serialPort.Write(sendData, 0, sendData.Length);
                    }
                    catch (Exception)
                    {
                        continue;
                        //res.msg = "端口可能被占用，通讯失败";
                        //return res;
                    }

                    Task.Delay(50).GetAwaiter().GetResult();
                    while (_serialPort.BytesToRead < 5)
                    {
                        Task.Delay(10).GetAwaiter().GetResult();
                        timeOut++;
                        if (timeOut == 50)
                        {
                            break;
                            //res.msg = "读取超时";
                            //return res;
                        }
                    }

                    if (_serialPort.BytesToRead < 5)
                    {
                        continue;
                    }


                    _serialPort.ReadTimeout = 500;
                    len = _serialPort.Read(buffer, 0, buffer.Length);
                    break;
                }


                res.response = buffer.Take(len).ToArray();
                res.msg = "获取成功";
                res.success = true;
                return res;
            }
        }

        /// <summary>
        /// 清空串口缓冲区
        /// </summary>
        private void ClearTempRead()
        {
            var tempbuf = new byte[100];
            if (_serialPort.BytesToRead > 0)
            {
                try
                {
                    _serialPort.Read(tempbuf, 0, tempbuf.Length);
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 关闭端口
        /// </summary>
        private void ClosePort()
        {
            if (_serialPort.IsOpen)
            {
                try
                {
                    _serialPort.Close();
                }
                catch (Exception)
                {
                }
            }
        }
    }
}