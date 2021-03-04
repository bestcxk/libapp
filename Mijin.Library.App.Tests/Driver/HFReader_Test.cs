﻿using Mijin.Library.App.Driver;
using Mijin.Library.App.Tests;
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
            Assert.True(result.response.IsEmpty());
        }

        /// <summary>
        /// 测试读卡号左边添加内容
        /// </summary>
        [Fact, TestPriority(2)]
        public void ReadCardNoAddLeft_Test()
        {
            sysFunc.LibrarySettings.IcSettings.ICAddDirection = Model.Setting.DirectionEnum.left;
            sysFunc.LibrarySettings.IcSettings.ICAddStr = "ASD";
            var result = _hfReader.ReadCardNo();
            Assert.True(result.success);
            Assert.True(result.response.IsEmpty());
            Assert.True(result.response[0] == 'A' && result.response[1] == 'S' && result.response[2] == 'D');
        }

        /// <summary>
        /// 测试读卡号右边添加内容
        /// </summary>
        [Fact, TestPriority(3)]
        public void ReadCardNoAddRight_Test()
        {
            sysFunc.LibrarySettings.IcSettings.ICAddDirection = Model.Setting.DirectionEnum.right;
            sysFunc.LibrarySettings.IcSettings.ICAddStr = "ASD";
            var result = _hfReader.ReadCardNo();
            Assert.True(result.success);
            Assert.True(result.response.IsEmpty());
            var len = result.response.Length;
            Assert.True(result.response[len-3] == 'A' && result.response[len-2] == 'S' && result.response[len-1] == 'D');
        }

    }
}
