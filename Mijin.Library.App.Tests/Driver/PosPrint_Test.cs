using Mijin.Library.App.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    public class PosPrint_Test
    {
        IPosPrint _posPrint;
        public PosPrint_Test()
        {
            _posPrint = new MyPosPrint();
        }

        [Fact]
        public void Print_Test()
        {
            var res = _posPrint.Print(new PrintInfo()
            {
                Action = ActionType.借阅,
                CreateTime = DateTime.Now,
                BookInfos = new List<BookInfo>() { 
                    new BookInfo(){ 
                        Title = "test",
                        Isbn = "123456789112"
                    }
                },
                UserName = "test",
                UserCardNo = "test",
                UserIdentity = "test"
            });

            Assert.True(res.success);
        }
    }



}
