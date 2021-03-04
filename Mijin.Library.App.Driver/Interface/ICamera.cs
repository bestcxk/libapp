using Mijin.Library.App.Model;
using System;

namespace Mijin.Library.App.Driver.Drivers.Camera
{
    public interface ICamera
    {
        /// <summary>
        /// 摄像头图片事件, response 是 string
        /// </summary>
        event Action<WebViewSendModel<string>> OnCameraGetImage;
        /// <summary>
        /// 关闭摄像头
        /// </summary>
        /// <returns></returns>
        MessageModel<bool> CloseCamera();
        /// <summary>
        /// 开启摄像头
        /// </summary>
        MessageModel<bool> OpenCamera();
    }
}