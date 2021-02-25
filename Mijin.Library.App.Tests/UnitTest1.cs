using Mijin.Library.App.Driver;
using System;
using Xunit;

namespace Mijin.Library.App.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            WonteReader wonteReader = new WonteReader();

            var data = wonteReader.ReadHFCardNo();
            Console.WriteLine(data.msg);
            var data1 = wonteReader.Test();
            Console.WriteLine(data1);
        }
    }
}
