using Bing.Extensions;
using Mijin.Library.App.Driver.Drivers.DhCamera;
using System.Threading.Tasks;
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
        private string ip = "192.168.0.108";
        private string port = "37777";
        private string name = "admin";
        private string password = "admin123";

        #endregion

        [Fact]
        public void GetFace_Test()
        {
            dh.Init();
            dh.Login(ip, port, name, password);
            var task = new Task(() => { dh.RegisterCutFaceEvent(); });
            task.Start();

            dh.OnDhGetFaceImageBase64 += (image) =>
            {
                Assert.NotNull(image);
                output.WriteLine("已获取到图片");
            };

            while (true)
            {
            }
        }

        [Fact]
        public void TestImageCat()
        {
            var bitmap = dh.CutFace(@$"C:\Users\zy\Desktop\temp\test\下载.png");
            bitmap.Dispose();
        }

        [Fact]
        public void HumanSum_Test()
        {
            dh.Login(ip, port, name, password);

            var task = new Task(() => { dh.RegisterPeopleInoutEvent(); });
            task.Start();

            dh.OnDhPeopleInOut += (info) => { output.WriteLine($"进{info.response.In}, 出{info.response.Out}"); };

            while (true)
            {
            }
        }

        [Fact]
        public void CutCameraBase64Image_Test()
        {
            var res = dh.Init();
            Assert.True(res.success);
            res = dh.Login(ip, port, name, password);
            Assert.True(res.success);


            int count = 10;
            while (--count > 0)
            {
                res = dh.CutCameraBase64Image();
                Assert.True(res.success);
                Assert.True(!res.response.IsEmpty());
            }
        }
    }
}