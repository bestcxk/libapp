using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bing.Extensions;
using Bing.Text;
using Mijin.Library.App.Driver;
using Mijin.Library.App.Driver.Drivers.Sudo;
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
                SudoPSAMMode = false
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
                Stopwatch st = Stopwatch.StartNew();
                var res = sudo.Read_SSC();
                st.Stop();

                var times = st.ElapsedMilliseconds;

            }
        }

        [Fact]
        public async void WatchQrcode_Test()
        {

            sudo.OnSudoQrcode += model =>
            {

            };

            {
                var res = sudo.Connect(comNumber, 115200);
            }
            {
                var res = sudo.StartWatchQrcode();
            }

            await Task.Delay(10000);

        }

    }
}