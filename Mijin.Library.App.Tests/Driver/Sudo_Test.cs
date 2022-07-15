using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mijin.Library.App.Driver;
using Mijin.Library.App.Model;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    public class Sudo_Test
    {
        ISudo sudo = new Sudo(new SystemFunc()
        {
            ClientSettings = new ClientSettings()
            {
                SudoPSAMMode = true
            }
        });

        private const int comNumber = 3;

        public Sudo_Test()
        {
        }

        [Fact]
        public void Connect_Test()
        {
            var res = sudo.Connect(comNumber, 115200);
        }

        [Fact]
        public void ReadIdentity_Test()
        {
            {
                var res = sudo.Connect(comNumber, 115200);
            }
            {
                var res = sudo.ReadIdentity();
            }
        }

        [Fact]
        public void Qrcode_Test()
        {
            {
                var res = sudo.Connect(3, 115200);
            }
            {
                var res = sudo.ReadQrcode();
            }
        }

        [Fact]
        public void Read_SSC_Test()
        {
            {
                var res = sudo.Connect(comNumber, 115200);
            }
            {
                var res = sudo.Read_SSC();
            }
        }
    }
}