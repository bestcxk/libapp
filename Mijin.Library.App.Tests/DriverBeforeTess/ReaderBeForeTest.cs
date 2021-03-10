using Mijin.Library.App.Driver;
using Mijin.Library.App.Model.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mijin.Library.App.Tests.DriverBeforeTess
{
    // 按顺序单元测试需要填入 排序器的 类文件全名 和 当前程序集
    [TestCaseOrderer("Mijin.Library.App.Tests.PriorityOrderer", "Mijin.Library.App.Tests")]
    public class ReaderBeForeTest
    {
        public ISystemFunc sysFunc { get; set; }
        public ReaderBeForeTest()
        {
            sysFunc = new SystemFunc()
            {
                LibrarySettings = new Model.Setting.LibrarySettings()
                {
                    IcSettings = new Model.Setting.IcSettings()
                }
            };
        }

        /// <summary>
        /// 补左边字符串
        /// </summary>
        [Fact, TestPriority(2)]
        public void DataHandleLeft_Test()
        {
            string result = null;

            var data = "123456789";
            sysFunc.LibrarySettings.IcSettings.ICAddDirection = DirectionEnum.left;
            sysFunc.LibrarySettings.IcSettings.ICLength = 10;
            sysFunc.LibrarySettings.IcSettings.ICAddStr = "ASD";

            result = IcSettings.DataHandle(data, sysFunc.LibrarySettings.IcSettings);
            Assert.Equal(@$"ASD{data}", result);
        }

        /// <summary>
        /// 补右边字符串
        /// </summary>
        [Fact, TestPriority(3)]
        public void DataHandleRight_Test()
        {
            string result = null;

            var data = "123456789";
            sysFunc.LibrarySettings.IcSettings.ICAddDirection = DirectionEnum.right;
            sysFunc.LibrarySettings.IcSettings.ICLength = 10;
            sysFunc.LibrarySettings.IcSettings.ICAddStr = "ASD";

            result = IcSettings.DataHandle(data, sysFunc.LibrarySettings.IcSettings);
            Assert.Equal(@$"{data}ASD", result);
        }

        /// <summary>
        /// 不补字符串
        /// </summary>
        [Fact, TestPriority(4)]
        public void DataHandleNo_Test()
        {
            string result = null;

            var data = "123456789";
            sysFunc.LibrarySettings.IcSettings.ICAddDirection = DirectionEnum.right;
            sysFunc.LibrarySettings.IcSettings.ICLength = 9;
            sysFunc.LibrarySettings.IcSettings.ICAddStr = "ASD";

            result = IcSettings.DataHandle(data, sysFunc.LibrarySettings.IcSettings);
            Assert.Equal(data, result);

            sysFunc.LibrarySettings.IcSettings.ICAddStr = null;
            result = IcSettings.DataHandle(data, sysFunc.LibrarySettings.IcSettings);
            Assert.Equal(data, result);

            sysFunc.LibrarySettings.IcSettings.ICLength = 0;
            result = IcSettings.DataHandle(data, sysFunc.LibrarySettings.IcSettings);
            Assert.Equal(data, result);


            data = null;
            sysFunc.LibrarySettings.IcSettings.ICLength = 100;
            result = IcSettings.DataHandle(data, sysFunc.LibrarySettings.IcSettings);
            Assert.Equal(data, result);
        }
    }
}
