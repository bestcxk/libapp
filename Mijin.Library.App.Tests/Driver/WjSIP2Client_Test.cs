using Mijin.Library.App.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Mijin.Library.App.Tests.Driver
{
    [TestCaseOrderer("Mijin.Library.App.Tests.PriorityOrderer", "Mijin.Library.App.Tests")]
    public class WjSIP2Client_Test
    {
        private readonly ITestOutputHelper _tempOutput;
        public WjSIP2Client wjSIP2Client;

        const string ip = "113.209.194.191";
        const string port = "2329";
        const string loginAccount = "ACS01";
        const string loginPw = "Acs123456";
        const int drivceId = 1;
        const string libCode = "30008200001";
        public WjSIP2Client_Test(ITestOutputHelper tempOutput)
        {
            _tempOutput = tempOutput;
            wjSIP2Client = new WjSIP2Client();
        }

        /// <summary>
        /// 连接
        /// </summary>
        [Fact, TestPriority(1)]
        public void Connect_Test()
        {
            var res = wjSIP2Client.Connect(ip, port);
            Assert.True(res.success);
        }

        /// <summary>
        /// 登陆
        /// </summary>
        [Fact, TestPriority(2)]
        public void Login_Test()
        {
            Connect_Test();
            var res = wjSIP2Client.Login(loginAccount, loginPw, drivceId);
            Assert.True(res.success);
        }

        /// <summary>
        /// 注册
        /// </summary>
        [Fact, TestPriority(2)]
        public void Register_Test()
        {
            Login_Test();
            var res = wjSIP2Client.RegiesterReader(new RegiesterInfo()
            {
                Name = "MJ测试用户",
                CardNo = "3336636077131",
                Identity = "500113199706286715",
                Pw = "012345678",
                CreateReaderLibrary = libCode,
                Type = "01",
                Birth = "19910101",
            });
            Assert.True(res.success);
        }

        /// <summary>
        /// 获取书籍信息
        /// </summary>
        [Fact, TestPriority(2)]
        public void GetBookInfo_Test()
        {
            Login_Test();
            var res = wjSIP2Client.GetBookInfo("XS0001962", libCode);
            Assert.True(res.success);
        }

        /// <summary>
        /// 获取读者信息
        /// </summary>
        [Fact, TestPriority(2)]
        public void GetReaderInfo_Test()
        {
            Login_Test();
            var res = wjSIP2Client.GetReaderInfo("2198008157566");
            Assert.True(res.success);
        }

        /// <summary>
        /// 借阅书籍
        /// </summary>
        [Fact, TestPriority(2)]
        public void LendBook_Test()
        {
            Login_Test();
            var res = wjSIP2Client.LendBook("XS0001962", "2198008157566", libCode);
            _tempOutput.WriteLine(res.msg);
            Assert.True(res.success);
        }

        /// <summary>
        /// 续借书籍
        /// </summary>
        [Fact, TestPriority(2)]
        public void ContinueBook_Test()
        {
            Login_Test();
            var res = wjSIP2Client.ContinueBook("XS0001962", "2198008157566", libCode);
            _tempOutput.WriteLine(res.msg);
            Assert.True(res.success);
        }

        /// <summary>
        /// 归还数据
        /// </summary>
        [Fact, TestPriority(2)]
        public void BackBook_Test()
        {
            Login_Test();
            var res = wjSIP2Client.BackBook("XS0001962", libCode);
            _tempOutput.WriteLine(res.msg);
            Assert.True(res.success);
        }

        //[Fact, TestPriority(2)]
        //public void GetBookInfos_Test()
        //{
        //    Login_Test();

            
        //}

    }
}
