using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mijin.Library.App.Driver;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    public class Sudo_Test
    {
        ISudo sudo = new Sudo();
        public Sudo_Test()
        {
        }

        [Fact]
        public void Connect_Test()
        {
            var res = sudo.Connect(4, 115200);
        }

        [Fact]
        public void ReadIdentity_Test()
        {
            {
                var res = sudo.Connect(4, 115200);
            }
            {
                var res = sudo.ReadIdentity();
            }
        }

        [Fact]
        public void Read_SSC_Test()
        {
            {
                var res = sudo.Connect(4, 115200);
            }
            {
                var res = sudo.Read_SSC();

            }
        }
    }
}
