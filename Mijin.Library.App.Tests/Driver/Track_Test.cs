using Mijin.Library.App.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    public class Track_Test
    {
        public Track track = new Track();
        public Track_Test()
        {
        }

        [Fact]
        public async void Test()
        {
            var res = track.Connect(3);
            track.Output(1,false);
            track.Output(2, false);
            await Task.Run(() =>
            {
                while (true) ;
            });
        }
    }
}
