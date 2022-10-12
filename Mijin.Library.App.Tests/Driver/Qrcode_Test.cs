using Mijin.Library.App.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mijin.Library.App.Tests.Driver;
public class Qrcode_Test
{
    QrCode qrcode = new QrCode();

    [Fact]
    public async void Test()
    {
        var res = qrcode.AutoConnect();


        res = qrcode.WatchQrCode(true);

        qrcode.OnScanQrCode += Qrcode_OnScanQrCode;

        await Task.Delay(10000);
    }

    private void Qrcode_OnScanQrCode(Model.WebViewSendModel<string> obj)
    {
    }
}
