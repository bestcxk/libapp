using Mijin.Library.App.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    // 按顺序单元测试需要填入 排序器的 类文件全名 和 当前程序集
    [TestCaseOrderer("Mijin.Library.App.Tests.PriorityOrderer", "Mijin.Library.App.Tests")]
    public class Rfid_Test
    {
        IRfid _rfid { get; set; }
        public Rfid_Test()
        {
            _rfid = new GRfid(null);
        }

        /// <summary>
        /// 测试232自动连接
        /// </summary>
        [Fact, TestPriority(1)]
        public void Auto232Connect_Test()
        {
            var result = _rfid.Auto232Connect();
            Assert.True(result.success);
            
        }
        
        /// <summary>
        /// 测试232自动连接
        /// </summary>
        [Fact, TestPriority(1)]
        public void TcpConnect_Test()
        {
            var result = _rfid.Connect("232","COM7:115200");
            Assert.True(result.success);
            
        }

        /// <summary>
        /// 测试 开启读
        /// </summary>
        [Fact, TestPriority(4)]
        public void Read_Test()
        {
            var result = _rfid.Read();
            Assert.True(result.success);
        }

        /// <summary>
        /// 测试 停止读
        /// </summary>
        [Fact, TestPriority(5)]
        public void Stop_Test()
        {
            var result = _rfid.Stop();
            Assert.True(result.success);
        }

        /// <summary>
        /// 测试 只读一次
        /// </summary>
        [Fact, TestPriority(6)]
        public void ReadOnce_Test()
        {
            var result = _rfid.ReadOnce();
            Assert.True(result.success);
        }

        /// <summary>
        /// 测试 获取当前读写器频段
        /// </summary>
        [Fact, TestPriority(7)]
        public void GetFreqRange_Test()
        {
            var result = _rfid.GetFreqRange();
            Assert.True(result.success);
        }

        /// <summary>
        /// 测试 设置频段
        /// </summary>
        [Fact, TestPriority(8)]
        public void SetFreqRange_Test()
        {
            var result = _rfid.SetFreqRange("1");
            Assert.True(result.success);
        }

        /// <summary>
        /// 测试 获取当前功率
        /// </summary>
        [Fact, TestPriority(9)]
        public void GetPower_Test()
        {
            var result = _rfid.GetPower();
            Assert.True(result.success);
        }

        /// <summary>
        /// 测试 设置当前功率
        /// </summary>
        [Fact, TestPriority(10)]
        public void SetPower_Test()
        {
            var powResult = _rfid.GetPower();
            Assert.True(powResult.success);
            var result = _rfid.SetPower(powResult.response);
            Assert.True(result.success);
        }
    }
}
