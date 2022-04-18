using System.Threading.Tasks;
using Mijin.Library.App.Driver;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    public class CKDoorController_Test
    {
        private CkDoorController _doorController;
        public CKDoorController_Test()
        {
            _doorController = new CkDoorController(null);
        }

        [Fact]
        public async void test()
        {
            await Task.Run(() =>
            {
                while (true)
                {

                }
            });
        }
    }
}