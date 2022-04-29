using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mijin.Library.App.Driver.Drivers.DhCamera;
using Xunit;
using Xunit.Abstractions;

namespace Mijin.Library.App.Tests.Driver
{
    public class DhCamera_Test
    {
        ITestOutputHelper output;
        public DhCamera_Test(ITestOutputHelper output)
        {
            this.output = output;
        }

        #region 参数

        DhCamera dh = new();
        private string ip = "192.168.1.108";
        private string port = "37777";
        private string name = "admin";
        private string password = "admin123";

        #endregion

        [Fact]
        public void GetFace_Test()
        {
            dh.Login(ip, port, name, password);
            var task = new Task(() =>
            {
                dh.GetFace();
            });
            task.Start();

            dh.OnGetFaceImage += (image) =>
            {
                Assert.NotNull(image);
                output.WriteLine("已获取到图片");
            };

            while (true)
            {
                
            }
        }

        [Fact]
        public void HumanSum_Test()
        {
            dh.Login(ip, port, name, password);

            var task = new Task(() =>
            {
                dh.HumanSum();
            });
            task.Start();

            dh.OnPeopleInOut += (info) =>
            {
                output.WriteLine($"进{info.In}, 出{info.Out}");
            };

            while (true)
            {

            }
        }


    }
}
