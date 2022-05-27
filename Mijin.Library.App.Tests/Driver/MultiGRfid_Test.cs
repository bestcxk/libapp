using Mijin.Library.App.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    public class MultiGRfid_Test
    {
        MultiGrfid _multiGrfid = new MultiGrfid();
        public MultiGRfid_Test()
        {

        }

        [Fact]
        public void Connected_Test()
        {
            var res = _multiGrfid.ConnectRfids(new List<MultiGrfidProp>()
            {
                new MultiGrfidProp()
                {
                    ConnectStr = "192.168.0.168:8160",
                },
                new MultiGrfidProp()
                {
                    ConnectStr = "192.168.1.168:8160",
                }
            });

            var readRes = _multiGrfid.ReadByNoTid();

        }
    }
}
