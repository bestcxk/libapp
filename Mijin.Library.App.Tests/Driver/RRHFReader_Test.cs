using Mijin.Library.App.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mijin.Library.App.Driver.Interface;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    public class RRHFReader_Test
    {
        IRRHFReader _rRHFReader;
        public RRHFReader_Test()
        {
            _rRHFReader = new RRHFReader(new SystemFunc());
        }

        [Fact]
        public void TestCamera()
        {
            {
                var res = _rRHFReader.Init();
                Assert.True(res.success);
            }
            //{
            //    var res = _rRHFReader.ChangeToISO14443A();
            //    Assert.True(res.success);
            //}
            {
                var res = _rRHFReader.ReadCardNo();
                Assert.True(res.success);
            }
            {
                var res = _rRHFReader.ReadBlock(0,1);
                Assert.True(res.success);
            }
        }
    }

    
}
