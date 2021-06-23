using Mijin.Library.App.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    public class HFRfid_Test
    {
        RRfid _rRfid = new RRfid();

        [Fact]
        public void AutoAutoOpenComPort_Test()
        {
            var res = _rRfid.AutoOpenComPort();
            Assert.True(res.success);
        }

        [Fact]
        public void NewScan_Test()
        {
            {
                var res = _rRfid.AutoOpenComPort();
                Assert.True(res.success);
            }
            {
                //var res = _rRfid.NewScan(0,2);
                //Assert.True(res.success);
            }

        }
    }
}
