using Mijin.Library.App.Model.Model;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver.Lock
{
    /// <summary>
    /// 柜锁
    /// </summary>
    public class CabinetLock
    {
        /// <summary>
        /// 柜号索引 16个柜子
        /// </summary>
        public static byte[] lockNos = { 0xa1, 0xa2, 0xa3, 0xa4, 0xa5, 0xa6, 0xa7, 0xa8, 0xb1, 0xb2, 0xb3, 0xb4, 0xb5, 0xb6, 0xb7, 0xb8 };

        private SerialPort _serialPort = new SerialPort();

        /// <summary>
        /// 串口是否打开
        /// </summary>
        public bool IsOpen { get => _serialPort.IsOpen; }

        /// <summary>
        /// 锁状态事件
        /// </summary>
        public event Action<List<bool>> lockStatusEvent;

        /// <summary>
        /// 数据接收缓存
        /// </summary>
        public byte[] buffer = new byte[100];

        /// <summary>
        /// 构造函数
        /// </summary>
        public CabinetLock()
        {
            //Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        try
            //        {
            //            if (_serialPort.IsOpen)
            //            {
            //                byte[] data = new byte[4];
            //                //Console.WriteLine($@"开始接收数据");
            //                _serialPort.Read(data, 0, data.Length);
            //                Console.WriteLine($@"收到数据：{string.Join(" ",data.ToString())}");
            //                if (data[0] == 0x08 && data[1] == 0x81)
            //                {
            //                    var lockStatus1 = new List<bool>();
            //                    var lockStatus2 = new List<bool>();
            //                    for (byte i = 0; i < 8; i++)
            //                    {
            //                        byte dt = (byte)(0x01 << i);
            //                        lockStatus1.Add((data[2] & dt) != dt); // false:未打开  true:打开
            //                        lockStatus2.Add((data[3] & dt) != dt);
            //                    }
            //                    lockStatus1.AddRange(lockStatus2);

            //                    lockStatusEvent.Invoke(lockStatus1);
            //                }
            //            }

            //        }
            //        catch (TimeoutException)
            //        { 

            //        }
            //        catch (Exception e)
            //        {
            //            Console.WriteLine(e.ToString());
            //        }
            //    }
            //});
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


        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="com"></param>
        /// <param name="baud"></param>
        /// <returns></returns>
        public MessageModel<bool> OpenSerialPort(string com, int baud = 115200)
        {
            var data = new MessageModel<bool>();
            Console.WriteLine(@$"com:{com} baud:{baud}");
            if (_serialPort.IsOpen)
                ClosePort();

            _serialPort.PortName = com;
            _serialPort.BaudRate = baud;
            _serialPort.DataBits = 8;//数据位：8
            _serialPort.StopBits = StopBits.One;//停止位：1
            _serialPort.Parity = Parity.None;
            _serialPort.Encoding = Encoding.Default;

            _serialPort.DataReceived += ReceivedData;

            try
            {
                _serialPort.Open();//打开串口
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
                    _serialPort.DataReceived -= ReceivedData;
                }
                catch (Exception)
                {
                }
            }
        }

        /// <summary>
        /// 接收数据处理
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private void ReceivedData(object obj, SerialDataReceivedEventArgs args)
        {
            var lockStatusList = ReceivedDataHandle();

            if (lockStatusList != null)
                lockStatusEvent.Invoke(lockStatusList);

        }

        /// <summary>
        /// 数据接收处理
        /// </summary>
        /// <returns>失败时返回Null</returns>
        private List<bool> ReceivedDataHandle()
        {
            Task.Delay(1000).GetAwaiter().GetResult();
            if (!_serialPort.IsOpen)
                return null;

            int len = _serialPort.Read(buffer, 0, buffer.Length);
            if (len == 4)
            {
                if (buffer[0] == 0x08 && buffer[1] == 0x81) // 检测头帧
                {
                    var lockStatus1 = new List<bool>();
                    var lockStatus2 = new List<bool>();
                    for (byte i = 0; i < 8; i++)
                    {
                        byte dt = (byte)(0x01 << i);
                        lockStatus1.Add((buffer[2] & dt) != dt); // false:未打开  true:打开
                        lockStatus2.Add((buffer[3] & dt) != dt);
                    }
                    lockStatus1.AddRange(lockStatus2);

                    return lockStatus1;
                }
            }

            return null;
        }

        /// <summary>
        /// 打开柜门
        /// </summary>
        /// <param name="lockIndex">柜号 1开始</param>
        /// <returns></returns>
        public MessageModel<bool> OpenBox(int lockIndex)
        {
            var data = new MessageModel<bool>();

            if (!_serialPort.IsOpen)
            {
                data.msg = "串口未打开";
                return data;
            }

            try
            {
                _serialPort.DataReceived -= ReceivedData;

                var sendBytes = new byte[] { lockNos[lockIndex - 1] };
                Console.WriteLine(@$"发送数据：{string.Join(" ", sendBytes.Select(s => s.ToString()))}");
                _serialPort.Write(sendBytes, 0, sendBytes.Length);
                _serialPort.ReadTimeout = 500;
                var lockStatusList = ReceivedDataHandle();

                // 开柜失败
                if (lockStatusList == null || lockStatusList[lockIndex - 1] == false)
                {
                    data.msg = "开柜失败";
                    return data;
                }
            }
            catch (Exception e)
            {
                data.msg = e.ToString();
                return data;
            }
            finally
            {
                _serialPort.DataReceived += ReceivedData;
            }
            data.success = true;
            data.msg = "开柜成功";

            return data;
        }
    }
}
