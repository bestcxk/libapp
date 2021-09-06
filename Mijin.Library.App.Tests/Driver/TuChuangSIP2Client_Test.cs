using Mijin.Library.App.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    [TestCaseOrderer("Mijin.Library.App.Tests.PriorityOrderer", "Mijin.Library.App.Tests")]
    public class TuChuangSIP2Client_Test
    {
        public TuChuangSIP2Client tuChuangSIP2Client;
        public TuChuangSIP2Client_Test()
        {
            tuChuangSIP2Client = new TuChuangSIP2Client();
        }

        [Fact, TestPriority(1)]
        public void Connect_Test()
        {
            // 阳西  自助机账号 002042ACS
            var res = tuChuangSIP2Client.Connect("47.107.119.27", "2042");
            //var bkRes = tuChuangSIP2Client.GetBookInfo("0420069486");
            var regiesterRes = tuChuangSIP2Client.RegiesterReader(new RegiesterInfo()
            {
                Identity = "500113199706286714",
                Pw="123456",
                Name="米进测试用户",
                CreateReaderLibrary= "002042",
                Moeny = 0,
                Type = "999_SFZYH2C"
            });

            var getReaderRes = tuChuangSIP2Client.GetReaderInfo("500113199706286714");

        }

        [Fact, TestPriority(2)]
        public void Login_Test()
        {
            {
                var res = tuChuangSIP2Client.Connect("222.89.181.140", "2001");
                var dt = tuChuangSIP2Client.LendBook("0200082261", "411002020100117", "wdqacs011");
                var dt1 = tuChuangSIP2Client.GetReaderInfo("411002020100117");
            }
        }
    }
}
