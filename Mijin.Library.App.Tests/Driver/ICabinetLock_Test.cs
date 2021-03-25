using Mijin.Library.App.Driver.Drivers.Lock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    public class ICabinetLock_Test
    {
        CabinetLock _cabinetLockNew;
        public ICabinetLock_Test()
        {
            _cabinetLockNew = new CabinetLock();
        }

        [Fact]
        public void Test()
        {
            var rs = _cabinetLockNew.OpenSerialPort("COM7",9600);
            var ass = _cabinetLockNew.GetLockStatus();
            var dt = _cabinetLockNew.OpenBox(1);
        }
    }
}
