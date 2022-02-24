using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mijin.Library.App.Driver;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    public class WriteCxDb_Test
    {
        public WriteCxDb writeCxDb { get; set; } = new WriteCxDb();


        [Fact]
        public void Write_Test()
        {
            var entity = new CxEntity();
            entity.sn_code = "test";
            var res = writeCxDb.WriteDb(entity);
            Assert.True(res.success);
        }

    }
}
