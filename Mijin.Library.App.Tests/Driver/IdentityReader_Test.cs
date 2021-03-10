using Mijin.Library.App.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    // 按顺序单元测试需要填入 排序器的 类文件全名 和 当前程序集
    [TestCaseOrderer("Mijin.Library.App.Tests.PriorityOrderer", "Mijin.Library.App.Tests")]
    public class IdentityReader_Test
    {
        public IdentityReader IdReader { get; set; }
        public ISystemFunc sysFunc { get; set; }

        public IdentityReader_Test()
        {
            sysFunc = new SystemFunc()
            {
                LibrarySettings = new Model.Setting.LibrarySettings()
                {
                    IcSettings = new Model.Setting.IcSettings()
                }
            };
            IdReader = new WonteReader(sysFunc);

            
        }
        /// <summary>
        /// 身份证读卡器 读高频卡测试   需要和 当前其他测试分开测试
        /// </summary>
        /// <param name="com">端口号</param>
        [Fact]
        public void ReadHFCardNo_Test()
        {
            var result = IdReader.ReadHFCardNo();
            Assert.True(result.success);
            Assert.True(!result.response.IsEmpty());
        }

        /// <summary>
        /// 读身份证信息    需要和 当前其他测试分开测试
        /// </summary>
        [Fact]
        public void ReadIdentity_Test()
        {
            var result = IdReader.ReadIdentity();
            Assert.True(result.success);
            Assert.True(!result.response.Identity.IsEmpty());
        }
    }
}
