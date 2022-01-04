using System;
using System.Threading.Tasks;
using Mijin.Library.App.Model;

namespace Mijin.Library.App.Driver
{
    public interface IVbarQrCode
    {
        bool isOpen { get; set; }
        Task task { get; set; }
        bool watching { get; set; }

        event Action<WebViewSendModel<string>> OnScanQrCode;

        MessageModel<string> AutoConnect(string baud = "115200");
        MessageModel<bool> SerialPortIsOpen();
        MessageModel<string> WatchQrCode(bool watch);
    }
}