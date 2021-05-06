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
    public class WenHuaSIP2Client_Test
    {
        public WenhuaSIP2Client wenhuaSIP2Client;
        public WenHuaSIP2Client_Test()
        {
            wenhuaSIP2Client = new WenhuaSIP2Client();
        }

        [Fact, TestPriority(1)]
        public void Connect_Test()
        {
            var res = wenhuaSIP2Client.Connect("222.89.181.140", "2001");
            Assert.True(res.success);
        }

        [Fact, TestPriority(2)]
        public void Login_Test()
        {
            {
                var res = wenhuaSIP2Client.Connect("222.89.181.140", "2001");
                Assert.True(res.success);
            }
            {
                var res = wenhuaSIP2Client.Login("wdqacs012", "111111");
                Assert.True(res.success);
            }

        }
    }
}
