using Mijin.Library.App.Model;
using System;

namespace Mijin.Library.App.Driver
{
    public interface IQrCode
    {

        event Action<WebViewSendModel<string>> OnScanQrCode;

        MessageModel<string> AutoConnect(string baud = "115200");

        MessageModel<bool> SerialPortIsOpen();
    }
}