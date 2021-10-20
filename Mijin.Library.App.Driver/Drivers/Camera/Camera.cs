using Emgu.CV;
using Emgu.CV.Structure;
using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IsUtil;
using Util.Logs;
using Util.Logs.Extensions;

namespace Mijin.Library.App.Driver
{
    public class Camera : ICamera
    {
        private Capture _capture = null; // 摄像头操作对象
        private bool _taskIsRunning = false; // 线程是否启动

        //private Thread _threadFaceDet = null;     // 检测线程 
        private Task _task = null; // 检测线程

        /// <summary>
        /// 摄像头图片事件, response 是 string
        /// </summary>
        public event Action<WebViewSendModel<string>> OnCameraGetImage;

        public static int gcCount = 0;

        public ISystemFunc _systemFunc { get; }

        ~Camera()
        {
            _taskIsRunning = false;
        }

        public Camera(ISystemFunc systemFunc)
        {
            _systemFunc = systemFunc;
        }

        /// <summary>
        /// 开启摄像头
        /// </summary>
        public MessageModel<bool> OpenCamera()
        {
            // 第一次启动获取摄像头操作对象
            if (_capture == null)
                _capture = new Capture(_systemFunc.ClientSettings.CameraIndex);

            if(_task == null)
            {
                _task = new Task(GetPicOnCameraHandle);
                _task.Start();
            }
            _taskIsRunning = true;

            return new MessageModel<bool>()
            {
                msg = "开启摄像头成功",
                success = true,
            };
        }

        /// <summary>
        /// 关闭摄像头
        /// </summary>
        /// <returns></returns>
        public MessageModel<bool> CloseCamera()
        {
            _taskIsRunning = false;
            _task = null;
            return new MessageModel<bool>()
            {
                msg = "关闭摄像头成功",
                success = true,
            };
        }


        /// <summary>
        /// 线程任务，从摄像头中获取图片
        /// </summary>
        private void GetPicOnCameraHandle()
        {
            while (true)
            {
                Task.Delay(200).GetAwaiter().GetResult();

                if (!_taskIsRunning) return;
                try
                {
                    //MessageModel<Dictionary<string, string>> data = new MessageModel<Dictionary<string, string>>() { response = new Dictionary<string, string>() };
                    //data.response["method"] = "OnCameraGetImage";
                    //data.response["image"] = GetCameraImageForBase64();

                    var SendModel = new WebViewSendModel<string>()
                    {
                        msg = "获取成功",
                        success = true,
                        response = GetCameraImageForBase64(),
                        method = "OnCameraGetImage"
                    };
                    this.OnCameraGetImage.Invoke(SendModel);

                }
                catch (Exception e)
                {
                    e.GetBaseException().Log(Log.GetLog().Caption(@$"摄像头{nameof(GetPicOnCameraHandle)}异常"));
                }
            }
        }

        /// <summary>
        /// 从摄像头中获取base64图片
        /// </summary>
        /// <returns></returns>
        private string GetCameraImageForBase64()
        {
            //var image = _capture.QueryFrame().ToImage<Bgr, byte>().ToBitmap();
            var bitmap = _capture.QueryFrame()?.Bitmap;
            return ImageHelper.ToBase64Str(bitmap);
        }
    }
}