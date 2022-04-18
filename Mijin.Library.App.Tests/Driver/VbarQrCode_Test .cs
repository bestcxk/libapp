using Mijin.Library.App.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    public class VbarQrCode_Test
    {
        VbarQrCode qrcode;
        public VbarQrCode_Test()
        {
            qrcode = new VbarQrCode();
        }

        [Fact]
        public async void TestCamera()
        {
            var res = qrcode.AutoConnect();
            res = qrcode.WatchQrCode(true);
            qrcode.OnScanQrCode += onGetData;
            await Task.Delay(3000);

        }

        private void onGetData(Model.WebViewSendModel<string> obj)
        {
            
        }
    }

    
}
