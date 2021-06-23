using Mijin.Library.App.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    // 按顺序单元测试需要填入 排序器的 类文件全名 和 当前程序集
    public class RRfid_Test
    {
        IRRfid _rRfid;
        public RRfid_Test()
        {
            _rRfid = new RRfid();
        }

        [Fact]
        public void Scan_Test()
        {
            {
                var res = _rRfid.AutoOpenComPort();
                Assert.True(res.success);
            }
            {
                var res = _rRfid.NewScan();
                Assert.True(res.success);
            }
        }

        [Fact]
        public async void ReadAndStop_Test()
        {
            {
                var res = _rRfid.AutoOpenComPort();
                Assert.True(res.success);
            }
            {
                var res = _rRfid.Read();
                Assert.True(res.success);

                _rRfid.OnReadHFLabels += _rRfid_OnReadHFLabels;
                await Task.Delay(3000);
            }
            {
                var res = _rRfid.Stop();
                Assert.True(res.success);
            }
        }

        [Fact]
        public void WriteLabel_Test()
        {
            {
                var res = _rRfid.AutoOpenComPort();
                Assert.True(res.success);
            }
            {
                _rRfid.SetActionLabelPara(0,16,4,0x00);
                var uidRes = _rRfid.NewScan();
                Assert.True(uidRes.success);

                var res = _rRfid.WriteLabelByAscii(uidRes.response.First().UidHexStr,"0101071234567AB66");
                //Assert.True(res.success);

                var readRes = _rRfid.ReadOnce();
                Assert.True(readRes.success);
            }

        }
        [Fact]
        public void Test()
        {

        }


        private void _rRfid_OnReadHFLabels(Model.WebViewSendModel<List<ScanDataModel>> obj)
        {

        }
    }
}
