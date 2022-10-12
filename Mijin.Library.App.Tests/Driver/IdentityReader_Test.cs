using Mijin.Library.App.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bing.Extensions;
using IsUtil;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    // 按顺序单元测试需要填入 排序器的 类文件全名 和 当前程序集
    [TestCaseOrderer("Mijin.Library.App.Tests.PriorityOrderer", "Mijin.Library.App.Tests")]
    public class IdentityReader_Test
    {
        public IdentityReader wonteReader { get; set; }
        public IdentityReader m513Reader { get; set; }

        public ISystemFunc sysFunc { get; set; }

        public IHFReader hFReader { get; set; }

        public IdentityReader_Test()
        {
            sysFunc = new SystemFunc()
            {
                LibrarySettings = new Model.Setting.LibrarySettings()
                {
                    IcSettings = new Model.Setting.IcSettings()
                }
            };
            wonteReader = new WonteReader(sysFunc);
            hFReader = new BlackHFReader(sysFunc);
            m513Reader = new M513Reader();


        }
        /// <summary>
        /// 身份证读卡器 读高频卡测试   需要和 当前其他测试分开测试
        /// </summary>
        /// <param name="com">端口号</param>
        [Fact]
        public void ReadHFCardNo_Test()
        {
            var result = wonteReader.ReadHFCardNo();
            Assert.True(result.success);
            Assert.True(!result.response.IsEmpty());
        }

        /// <summary>
        /// 读身份证信息    需要和 当前其他测试分开测试
        /// </summary>
        [Fact]
        public void WonteReadIdentity_Test()
        {
            var result = wonteReader.ReadIdentity();
            Assert.True(result.success);
            Assert.True(!result.response.Identity.IsEmpty());
        }

        [Fact]
        public void M53ReadIdentity_Test()
        {
            var result = m513Reader.ReadIdentity();
            Assert.True(result.success);
            Assert.True(!result.response.Identity.IsEmpty());
        }



        [Fact]
        public void LongTeng_Test()
        {
            var dt = "3ad5b852".ToUpper();
            //var hfRes = hFReader.ReadCardNo();
            //var card = Convert.ToString(hfRes.response.ToInt(), 16);
            //var str = "";
            //for (int i = 0; i < card.Length; i += 2)
            //{
            //    string dt = card[i].ToString() + card[i + 1].ToString();
            //    str = str.Insert(0, dt);
            //}
            //var idRes = IdReader.ReadHFCardNo(14);
        }
    }
}
