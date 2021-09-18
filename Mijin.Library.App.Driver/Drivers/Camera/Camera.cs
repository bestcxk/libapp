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
        private Capture _capture = null;           // 摄像头操作对象
        private bool _taskIsRunning = false;       // 线程是否启动

        //private Thread _threadFaceDet = null;     // 检测线程 
        private Task _task = null;                  // 检测线程

        /// <summary>
        /// 摄像头图片事件, response 是 string
        /// </summary>
        public event Action<WebViewSendModel<string>> OnCameraGetImage;

        public static int gcCount = 0;

        public Camera()
        {
        }

        /// <summary>
        /// 开启摄像头
        /// </summary>
        public MessageModel<bool> OpenCamera()
        {
            // 第一次启动获取摄像头操作对象
            if (_capture == null)
                _capture = new Capture();

            _task = new Task(GetPicOnCameraHandle);
            _task.Start();
            _taskIsRunning = true;
            //if (_taskIsRunning != true)
            //{
            //    _taskIsRunning = true;
            //    _task = new Task(GetPicOnCameraHandle);
            //    _task.Start();

            //    //_threadFaceDet = new Thread(new ThreadStart(SendPic));
            //    //_taskIsRunning = true;
            //    //_threadFaceDet.Start();
            //}
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
                Task.Delay(30).GetAwaiter();

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

                    if (++gcCount >= 500)
                    {
                        gcCount = 0;
                        System.GC.Collect();

                    }

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
            return ToBase64Str(bitmap);
        }

        /// <summary>
        /// Bitmap 转 base64字符串
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns></returns>
        private string ToBase64Str(Bitmap bmp)
        {
            if (bmp.IsNull())
            {
                return null;
            }
            try
            {
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                String strbaser64 = Convert.ToBase64String(arr);
                return strbaser64;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("ImgToBase64String 转换失败 Exception:" + ex.Message);
                return "";
            }
        }
    }
}
