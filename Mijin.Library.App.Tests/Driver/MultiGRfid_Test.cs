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
            _multiGrfid.Connected();
        }
    }
}
