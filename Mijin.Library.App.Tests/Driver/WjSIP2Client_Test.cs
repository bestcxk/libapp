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
    public class WjSIP2Client_Test
    {
        public WjSIP2Client wjSIP2Client;

        const string ip =  "11.176.26.77";
        const string port =  "2001";
        const string loginAccount = "250302admin";
        const string loginPw = "Wj123456";
        const int drivceId = 464;
        const string libCode = "800021250302";
        public WjSIP2Client_Test()
        {
            wjSIP2Client = new WjSIP2Client();
        }

        [Fact, TestPriority(1)]
        public void Connect_Test()
        {
            var res = wjSIP2Client.Connect(ip, port);
            Assert.True(res.success);
        }

        [Fact, TestPriority(2)]
        public void Login_Test()
        {
            Connect_Test();
            var res = wjSIP2Client.Login(loginAccount, loginPw, drivceId);
            Assert.True(res.success);
        }

        [Fact, TestPriority(2)]
        public void Regiester_Test()
        {
            Login_Test();
            var res = wjSIP2Client.RegiesterReader(new RegiesterInfo()
            {
                Name = "黄石测试用户",
                CardNo = "0123456789",
                Identity = "500113199706286714",
                Pw = "012345678",
                CreateReaderLibrary = libCode,
                Type = "01",
                Birth = "19910101",
            });
            Assert.True(res.success);
        }

        [Fact, TestPriority(2)]
        public void GetBookInfo_Test()
        {
            Login_Test();
            var res = wjSIP2Client.GetBookInfo("00000002");
            Assert.True(res.success);
        }

        [Fact, TestPriority(2)]
        public void GetReaderInfo_Test()
        {
            Login_Test();
            var res = wjSIP2Client.GetReaderInfo("012345678911");
            Assert.True(res.success);
        }

        [Fact, TestPriority(2)]
        public void LendBook_Test()
        {
            Login_Test();
            var res = wjSIP2Client.LendBook("00000002", "012345678911", libCode);
            Assert.True(res.success);
        }

        [Fact, TestPriority(2)]
        public void ContinueBook_Test()
        {
            Login_Test();
            var res = wjSIP2Client.ContinueBook("00000002", "012345678911", libCode);
            Assert.True(res.success);
        }

        [Fact, TestPriority(2)]
        public void BackBook_Test()
        {
            Login_Test();
            var res = wjSIP2Client.BackBook("00000002", libCode);
            Assert.True(res.success);
        }

    }
}
