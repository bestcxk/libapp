using Mijin.Library.App.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    [TestCaseOrderer("Mijin.Library.App.Tests.PriorityOrderer", "Mijin.Library.App.Tests")]
    public class TuChuangSIP2Client_Test
    {
        public TuChuangSIP2Client tuChuangSIP2Client;
        public TuChuangSIP2Client_Test()
        {
            tuChuangSIP2Client = new TuChuangSIP2Client();
        }

        [Fact, TestPriority(1)]
        public void Connect_Test()
        {
            var res = tuChuangSIP2Client.Connect("222.89.181.140", "2001");
            Assert.True(res.success);
        }

        [Fact, TestPriority(2)]
        public void Login_Test()
        {
            {
                var res = tuChuangSIP2Client.Connect("222.89.181.140", "2001");
            }
            //{
            //    var res = tuChuangSIP2Client.LendBook("0200002270", "411002020100117");
            //}
            //{
            //    var res = tuChuangSIP2Client.ContinueBook("0200002270", "411002020100117");
            //}
            //{
            //    var res = tuChuangSIP2Client.BackBook("0200002270");
            //}
            //{
            //    var res = tuChuangSIP2Client.GetBookInfo("0200002270");
            //}
            {
                //var res = tuChuangSIP2Client.LendBook("0200079862", "411002020100015");
                //var res1 = tuChuangSIP2Client.GetBookInfo("020001996");
                //var res2 = tuChuangSIP2Client.GetBookInfo("020001959");
                //var res3 = tuChuangSIP2Client.GetBookInfo("020002038");
            }



        }
    }
}
