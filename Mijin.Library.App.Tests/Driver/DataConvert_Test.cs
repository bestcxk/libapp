using Mijin.Library.App.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    public class DataConvert_Test
    {
        private DataConvert _dataConvert = new DataConvert();

        [Fact]
        public void RFID_Decode96bit_Test()
        {
            var epc = "C2000E326E23140745153D00";
            var res = _dataConvert.RFID_Decode96bit(epc);

            Assert.True(res.success);

        }
    }
}
