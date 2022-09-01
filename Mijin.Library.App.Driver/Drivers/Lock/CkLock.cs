using Mijin.Library.App.Driver.Interface;
using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bing.Collections;
using IsUtil.Helpers;

namespace Mijin.Library.App.Driver.Drivers.Lock;
public class CkLock : ICkLock
{
    public bool IsOpen => _serialPort.IsOpen;

    private SerialPort _serialPort = new SerialPort();

    private readonly static object sendLockObj = new();
    private readonly static object openLockObj = new();

    // 锁孔数量
    private int controllerCount = 1;
    public Task task;

    /// <summary>
    /// 数据接收缓存
    /// </summary>
    private byte[] buffer = new byte[100];

    public event Action<WebViewSendModel<List<bool>>> OnLockEvent;

    public CkLock()
    {
        task = Task.Run(LockEvent);
    }

    ~CkLock(){
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
                    if (dt.response.Any(s => s))
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
        controllerCount = (int)Count;
        return new MessageModel<bool>()
        {
            success = true,
            msg = "设置成功"
        };
    }

    /// <summary>
    /// 获取锁控板状态
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public MessageModel<List<bool>> GetLockStatus()
    {
        var res = new MessageModel<List<bool>>() { response = new List<bool>() };

        for (byte cont = 1; cont <= controllerCount; cont++)
        {
            var data = GetLockStatus(cont);
            if (!data.success) return new(data);
            res.response.AddRange(data.response);
        }

        res.msg = "获取成功";
        res.success = true;
        return res;
    }

    /// <summary>
    /// 获取指定锁控地址 柜门状态命令
    /// </summary>
    /// <returns>true 为开 false 为关</returns>
    private MessageModel<List<bool>> GetLockStatus(byte controller)
    {
        var res = new MessageModel<List<bool>>() { response = new List<bool>() };
        byte[] bytes = { 0x5A, 0x08, (byte)(controller - 1), 0x03, 0x00, 0x00 };

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

            int integer = data.response[4] << 8;
            integer |= data.response[5];

            var temp = integer;
            for (int i = 0; i < 12; i++)
            {
                res.response.Add((temp & 1) == 1);
                temp >>= 1;
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
            sendData = sendData.Append((byte)0x0D).ToArray();

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
                while (_serialPort.BytesToRead < 8)
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

                if (_serialPort.BytesToRead < 8)
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
    /// 打开柜子
    /// </summary>
    /// <param name="boxIndex"></param>
    /// <returns></returns>
    public MessageModel<string> OpenBox(long boxIndex)
    {
        var controller = 1;
        var index = boxIndex;

        while (index > 12)
        {
            controller++;
            index -= 12;
        }


        return OpenBox((byte)controller, (int)index);
    }

    private MessageModel<string> OpenBox(byte controller, int boxIndex)
    {
        var res = new MessageModel<string>();

        var tempByte = 1;
        tempByte <<= boxIndex - 1;

        byte[] bytes = { 0x5A, 0x08, (byte)(controller - 1), 0x01, (byte)(tempByte >> 8), (byte)tempByte };

        var data = Send(bytes);
        if (!data.success)
        {
            return new(data);
        }

        if (data.response[3] != 0x02 && data.response[3] != 0)
        {
            res.msg = "开锁柜失败";
            return res;
        }

        res.msg = @$"开锁柜{boxIndex}成功";
        res.success = true;
        return res;
    }

    /// <summary>
    /// 打开串口
    /// </summary>
    /// <param name="com">串口号</param>
    /// <param name="baud">波特率</param>
    /// <returns></returns>
    public MessageModel<bool> OpenSerialPort(string com, long baud)
    {
        lock (openLockObj)
        {
            var data = new MessageModel<bool>();
            Console.WriteLine(@$"com:{com} baud:{baud}");
            if (_serialPort.IsOpen)
                ClosePort();

            _serialPort.PortName = com;
            _serialPort.BaudRate = (int)baud;
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
    /// 关闭串口
    /// </summary>
    private void ClosePort()
    {
        if (!_serialPort.IsOpen) return;

        try
        {
            _serialPort.Close();
        }
        catch (Exception)
        {
        }
    }
}
