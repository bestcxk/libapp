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
            //var res = tuChuangSIP2Client.Connect("222.89.181.140", "2001");
            var res = tuChuangSIP2Client.Connect("4", "2042");
            var bkRes = tuChuangSIP2Client.GetBookInfo("0420020575");
            var getReaderRes = tuChuangSIP2Client.GetReaderInfo("00000464");
            var regiesterRes = tuChuangSIP2Client.RegiesterReader(new RegiesterInfo()
            {
                CardNo = "445281199805091111",
                Identity = "445281199805091111",
                Pw ="123456",
                Name = "napama",
                CreateReaderLibrary= "002042",
                Moeny = 0,
                Type = "999_SFZYH2C"
            });

        }

        [Fact, TestPriority(2)]
        public void Login_Test()
        {
            {
                //var res = tuChuangSIP2Client.Connect("222.89.181.140", "2001");//连接
                //var res = tuChuangSIP2Client.Connect("47.107.119.27", "2042");
                var res = tuChuangSIP2Client.Connect("218.14.113.25", "2001");
                var gbRes = tuChuangSIP2Client.GetBookInfo("75240010007856");
                //var getReaderRes = tuChuangSIP2Client.GetReaderInfo("75240000000063");
               
                var dt = tuChuangSIP2Client.LendBook("75240010007856", "75240000000063", "BZJ_005");//借书
                
                
                var bkRes = tuChuangSIP2Client.BackBook("75240010007856", "");
                var cbt = tuChuangSIP2Client.ContinueBook("75240010022850", "75240000000063", "BZJ_005");//续借书
            }
        }
    }
}
