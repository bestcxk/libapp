using GDotnet.Reader.Api.Utils;
using IsUtil;

using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Logs.Extensions;

namespace Mijin.Library.App.Driver
{
    public class QrCode : IQrCode
    {
        private readonly string vid = "0103";
        private readonly string pid = "6061";

        private SerialPort serialPort;

        private bool watchQrCode = false;

        public event Action<WebViewSendModel<string>> OnScanQrCode;

        public QrCode()
        {
            serialPort = new SerialPort();
            serialPort.DataBits = 8;
            serialPort.StopBits = (StopBits)1;
            serialPort.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);//添加事件
        }



        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            String str_HEX = "";
            string str = serialPort.ReadExisting().Replace("\r\n","");//字符串方式读
            if (!watchQrCode) return;
            OnScanQrCode?.Invoke(new WebViewSendModel<string>()
            {
                msg = "获取成功",
                response = str,
                success = true,
                method = nameof(OnScanQrCode)
            });


        }

        public MessageModel<string> AutoConnect(string baud = "115200")
        {
            var res = new MessageModel<string>();
            // 如果com口已经打开，则先关闭
            if (serialPort.IsOpen)
            {
                serialPort.Close();
            }
            var comName = PublicFun.GetPortNameFormVidPid(vid, pid);
            if (comName.IsNull())
            {
                res.msg = "未连接二维码设备,无法获取到com口";
                return res;
            }
            serialPort.PortName = comName;
            serialPort.BaudRate = baud.ToInt();

            

            try
            {
                serialPort.Open();
            }
            catch (Exception e)
            {
                e.Log(Util.Logs.Log.GetLog().Caption("IQrCode.AutoConnect"));
                res.devMsg = e.ToString();
                res.msg = "端口打开失败，端口可能已被占用";
                return res;
            }
            res.msg = "打开二维码设备成功";
            res.success = true;
            return res;
        }


        public MessageModel<string> WatchQrCode(bool watch)
        {
            watchQrCode = watch;
            return new MessageModel<string>()
            {
                success = true,
                msg = "设置成功"
            };
        }

        public MessageModel<bool> SerialPortIsOpen()
        {
            return new MessageModel<bool>()
            {
                success = true,
                response = serialPort.IsOpen,
                msg = "获取成功"
            };
        }
    }
}
