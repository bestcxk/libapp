using Mijin.Library.App.Driver;
using Mijin.Library.App.Model;
using Mijin.Library.App.Model.Setting;
using Mijin.Library.App.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using IsUtil;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    // 按顺序单元测试需要填入 排序器的 类文件全名 和 当前程序集
    [TestCaseOrderer("Mijin.Library.App.Tests.PriorityOrderer", "Mijin.Library.App.Tests")]
    public class HFReader_Test
    {
        public IHFReader _hfReader { get; }
        public ISystemFunc sysFunc { get; set; }
        public HFReader_Test()
        {
            sysFunc = new SystemFunc()
            {
                LibrarySettings = new Model.Setting.LibrarySettings() {
                IcSettings = new Model.Setting.IcSettings()
                }
            };
            _hfReader = new BlackHFReader(sysFunc);
        }

        /// <summary>
        /// 测试读卡号
        /// </summary>
        [Fact, TestPriority(1)]
        public void ReadCardNo_Test()
        {
            var result = _hfReader.ReadCardNo();
            Assert.True(result.success);
            Assert.True(!result.response.IsEmpty());
        }


        /// <summary>
        /// 测试读块
        /// </summary>
        [Fact, TestPriority(2)]
        public void ReadCardBlock_Test()
        {
            var result = _hfReader.ReadBlock("1");
            Assert.True(result.success);
            Assert.True(!result.response.IsEmpty());
        }



    }
}
