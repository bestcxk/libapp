using Mijin.Library.App.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    public class Camera_Test
    {
        ICamera _camera;
        public Camera_Test()
        {
            _camera = new Camera();
        }

        [Fact]
        public async void TestCamera()
        {
            _camera.OnCameraGetImage += _camera_OnCameraGetImage;
            var open = _camera.OpenCamera();
            await Task.Delay(3000);
            var close = _camera.CloseCamera();
        }

        private void _camera_OnCameraGetImage(Model.WebViewSendModel<string> obj)
        {
            
        }
    }

    
}
