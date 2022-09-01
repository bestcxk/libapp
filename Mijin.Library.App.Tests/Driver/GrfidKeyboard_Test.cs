using Mijin.Library.App.Driver.Drivers.RFID;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Mijin.Library.App.Tests.Driver;
public class GrfidKeyboard_Test
{
    public GrfidKeyboard grfidKeyboard { get; set; } = new GrfidKeyboard(null);

    [Fact]
    public void TestKeyPut()
    {
    }


}
