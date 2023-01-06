using Bing.Extensions;
using System;
using Xunit;

namespace Mijin.Library.App.Tests.Driver;
public class Base_Test
{

    [Fact]
    public void Test()
    {
        var epc = "01010998765432100000ab66";
        if (epc.IsEmpty())
            return;
        var isTagLabel = (epc.Substring(epc.Length - 4, 4) == "AB66") || (epc.Substring(epc.Length - 4, 4) == "ab66");

        if (isTagLabel != true)
            return;

        try
        {
            var serialLen = epc.Substring(4, 2).ToInt();
            var serial = epc.Substring(6, serialLen);

            //Task.Run(async () =>
            //{
            //    await keyboardSettings.PutValue(serial);
            //});
        }
        catch (Exception)
        {

        }

    }
}
