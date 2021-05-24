using Mijin.Library.App.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    public class Keyboard_Test
    {
        IKeyboard _keyboard;
        public Keyboard_Test()
        {
            _keyboard = new Keyboard();
        }

        [Fact]
        public void Open_Test()
        {
            _keyboard.Open();
            _keyboard.Close();
        }
    }
}
