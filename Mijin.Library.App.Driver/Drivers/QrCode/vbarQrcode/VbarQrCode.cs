using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bing.Extensions;
using Mijin.Library.App.Model;
using Txq_csharp_sdk;
using Util.Dependency;

namespace Mijin.Library.App.Driver
{
    public class VbarQrCode : IVbarQrCode
    {
        public event Action<WebViewSendModel<string>> OnScanQrCode;

        public bool isOpen { get; set; }

        public bool watching { get; set; } = false;

        public Task task { get; set; } = null;

        private async Task DataReceived()
        {
            while (true)
            {
                if (!watching)
                {
                    await Task.Delay(50);
                    continue;
                }

                var decoderesult = Vbarapi.Decoder();
                if (!decoderesult.IsEmpty())
                {
                    OnScanQrCode?.Invoke(new WebViewSendModel<string>()
                    {
                        msg = "获取成功",
                        response = decoderesult,
                        success = true,
                        method = nameof(OnScanQrCode),
                    });
                }
                await Task.Delay(50);
            }
        }

        public MessageModel<string> AutoConnect(string baud = "115200")
        {
            var res = new MessageModel<string>();

            if (isOpen)
            {
                res.msg = "已连接设备，无需重复连接";
                res.success = isOpen;
                return res;
            }

            Vbarapi.closeDevice();
            if (!Vbarapi.openDevice())
            {
                res.msg = "连接Vbar二维码失败";
            }
            else
            {
                res.success = true;
                res.msg = "连接Vbar二维码成功";
                task = DataReceived();
                //task.Start();
            }
            isOpen = res.success;
            return res;

        }

        public MessageModel<bool> SerialPortIsOpen()
        {
            return new MessageModel<bool>()
            {
                msg = isOpen ? "已连接设备" : "未连接设备",
                success = isOpen,
            };
        }

        public MessageModel<string> WatchQrCode(bool watch)
        {
            watching = watch;
            return new MessageModel<string>()
            {
                msg = "设置成功",
                success = true
            };
        }


    }
}
