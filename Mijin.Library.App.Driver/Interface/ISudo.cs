using System;
using Mijin.Library.App.Model;

namespace Mijin.Library.App.Driver
{
    public interface ISudo
    {
        MessageModel<string> Connect(long port, long baud);
        MessageModel<IdentityInfo> ReadIdentity();
        MessageModel<string> Read_SSC(bool getAllStr = false);
        MessageModel<string> ReadQrcode();
        void Close();
        MessageModel<string> StartWatchQrcode();
        MessageModel<string> StopWatchQrcode();
        event Action<WebViewSendModel<SudoQrcode>> OnSudoQrcode;
    }
}