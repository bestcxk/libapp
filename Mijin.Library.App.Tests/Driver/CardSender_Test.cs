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
    public class CardSender_Test
    {
        public ISystemFunc systemFunc;
        public CardSender cardSender;

        public CardSender_Test()
        {
            systemFunc = new SystemFunc();
            cardSender = new CardSender(systemFunc);
        }

        [Fact, TestPriority(1)]
        public void Init_Test()
        {
            var res = cardSender.Init("COM3");
            Assert.True(res.success);
        }

        [Fact, TestPriority(2)]
        public void SpitCard_Test()
        {
            {
                var res = cardSender.Init("COM3");
                Assert.True(res.success);
            }
            {
                var res = cardSender.SpitCard();
                Assert.True(res.success);
            }

        }
    }
}
