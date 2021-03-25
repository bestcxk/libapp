using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;
using Util.Helpers;
using Util.Maps;

namespace Mijin.Library.App.Driver.Drivers.Lock
{
    public class CabinetLock : ICabinetLock
    {
        private SerialPort _serialPort = new SerialPort();

        /// <summary>
        /// 串口是否打开
        /// </summary>
        public bool IsOpen { get => _serialPort.IsOpen; }

        /// <summary>
        /// 数据接收缓存
        /// </summary>
        private byte[] buffer = new byte[100];
        /// <summary>
        /// 构造函数
        /// </summary>
        public CabinetLock()
        {

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
        public MessageModel<bool> OpenSerialPort(string com, Int64 baud)
        {
            var data = new MessageModel<bool>();
            Console.WriteLine(@$"com:{com} baud:{baud}");
            if (_serialPort.IsOpen)
                ClosePort();

            _serialPort.PortName = com;
            _serialPort.BaudRate = (int)baud;
            _serialPort.DataBits = 8;//数据位：8
            _serialPort.StopBits = StopBits.One;//停止位：1
            _serialPort.Parity = Parity.None;
            _serialPort.Encoding = Encoding.Default;
            //_serialPort.ReadBufferSize = 5;  // 读取缓冲区大小设置5
            //_serialPort.DataReceived += ReceivedData;
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
        /// 开指定柜号
        /// </summary>
        /// <param name="boxIndex">1开始</param>
        /// <returns></returns>
        public MessageModel<string> OpenBox(Int64 boxIndex)
        {
            var res = new MessageModel<string>();
            byte[] bytes = new byte[] { 0x8a, 0x01, (byte)boxIndex, 0x11 };

            var data = Send(bytes);
            if (!data.success)
            {
                return new(data);
            }

            if (data.response[3] != 0x11)
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
            var res = new MessageModel<List<bool>>() { response = new List<bool>() };

            for (byte i = 1; i <= 3; i++)
            {
                var data = GetLockStatus(i);
                if (!data.success)
                {
                    return new(data);
                }
                res.response.AddRange(data.response);
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
        private MessageModel<List<bool>> GetLockStatus(byte lockAddr)
        {
            var res = new MessageModel<List<bool>>() { response = new List<bool>() };
            byte[] bytes = new byte[] { 0x81, 0x01, lockAddr, 0x33 };

            if (!IsOpen)
            {
                res.msg = "串口未打开";
                return res;
            }


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

        private MessageModel<byte[]> Send(byte[] sendData)
        {
            int timeOut = 0;
            var res = new MessageModel<byte[]>();
            if (!IsOpen)
            {
                res.msg = "串口未打开";
                return res;
            }

            sendData = sendData.Append(SerialPortHelper.Get_CheckXor(sendData)).ToArray(); // 添加校验和
            try
            {
                _serialPort.Write(sendData, 0, sendData.Length);

            }
            catch (Exception)
            {
                res.msg = "端口可能被占用，通讯失败";
                return res;
            }
            _serialPort.ReadTimeout = 500;
            while (_serialPort.BytesToRead < 5)
            {
                Task.Delay(5).GetAwaiter().GetResult();
                timeOut++;
                if (timeOut == 200)
                {
                    res.msg = "读取超时";
                    return res;
                }
            }
            int len = _serialPort.Read(buffer, 0, buffer.Length);

            res.response = buffer.Take(len).ToArray();
            res.msg = "获取成功";
            res.success = true;
            return res;
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
