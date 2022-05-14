using Mijin.Library.App.Driver.Interface;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Bing.Conversions;
using Bing.Drawing;
using Bing.Extensions;
using Bing.Helpers;
using Bing.Utils.Timing;
using Microsoft.AspNetCore.Http.Internal;
using Mijin.Library.App.Model;
using Util.Logs;
using Util.Logs.Extensions;
using static Mijin.Library.App.Driver.Drivers.DhCamera.DhSdk;

namespace Mijin.Library.App.Driver.Drivers.DhCamera
{
    /// <summary>
    /// 大华摄像头
    /// </summary>
    public class DhCamera : IDhPeopleInoutCamera
    {
        #region 参数

        /// <summary>
        /// 人流量统计事件
        /// </summary>
        public event Action<WebViewSendModel<(int In, int Out)>> OnDhPeopleInOut;

        /// <summary>
        /// 截取人脸事件
        /// </summary>
        public event Action<WebViewSendModel<string>> OnDhGetFaceImageBase64;

        public bool _type { get; set; }

        //声明静态委托，普通委托可能会出现回调之前就将其释放的错误
        /// <summary>
        /// 事件数据回调函数
        /// </summary>
        private DhSdk.fAnalyzerDataCallBack m_AnalyzerDataCallBack { get; set; }

        /// <summary>
        /// 断线回调函数
        /// </summary>
        private DhSdk.fDisConnectCallBack m_DisConnectCallBack { get; set; }

        private event Action DeviceDisconnected;

        /// <summary>
        /// 视频统计摘要信息回调函数
        /// </summary>
        private DhSdk.fVideoStatSumCallBack m_VideoStatSumCallBack { get; set; }

        /// <summary>
        /// 登录句柄
        /// </summary>
        private IntPtr m_LoginID { get; set; } = IntPtr.Zero;

        private int m_GroupID { get; set; } = 0;
        private IntPtr m_AttactID { get; set; } = IntPtr.Zero;
        private IntPtr m_PlayID { get; set; } = IntPtr.Zero;
        private IntPtr m_AnalyzerID = IntPtr.Zero;
        private DhStruct.NET_DEVICEINFO_Ex m_DevInfo = new();

        private IntPtr realLoadPictureHandle = IntPtr.Zero;
        private IntPtr hWnd = IntPtr.Zero;

        private bool isInit = false;

        private string networkCutbase64 = null;

        private fSnapRevCallBack _SnapRevCallBack;

        /// <summary>
        /// 人脸图片保存地址
        /// </summary>
        private string path { get; set; }

        #endregion


        public DhCamera()
        {
            m_VideoStatSumCallBack = VideoStatSumCallBack;
            m_DisConnectCallBack = DisConnectCallBack;
            m_AnalyzerDataCallBack = AnalyzerDataCallBack;
        }

        #region 回调方法

        /// <summary>
        /// 远程抓图回调
        /// </summary>
        /// <param name="lLoginID"></param>
        /// <param name="pBuf"></param>
        /// <param name="RevLen"></param>
        /// <param name="EncodeType"></param>
        /// <param name="CmdSerial"></param>
        /// <param name="dwUser"></param>
        private void SnapRevCallBack(IntPtr lLoginID, IntPtr pBuf, uint RevLen, uint EncodeType, uint CmdSerial,
            IntPtr dwUser)
        {
            if (EncodeType == 10) //.jpg
            {
                byte[] data = new byte[RevLen];
                Marshal.Copy(pBuf, data, 0, (int) RevLen);
                networkCutbase64 = data.ToBase64String();
            }
        }

        /// <summary>
        /// 人流量统计回调方法
        /// </summary>
        /// <param name="lAttachHandle"></param>
        /// <param name="pBuf"></param>
        /// <param name="dwBufLen"></param>
        /// <param name="dwUser"></param>
        private void VideoStatSumCallBack(IntPtr lAttachHandle, IntPtr pBuf, uint dwBufLen, IntPtr dwUser)
        {
            if (lAttachHandle != m_AttactID)
            {
                // TODO: 可能需要抛出异常，目前不做处理
                // return new DhStruct.NET_VIDEOSTAT_SUMMARY();
            }

            DhStruct.NET_VIDEOSTAT_SUMMARY info =
                (DhStruct.NET_VIDEOSTAT_SUMMARY) Marshal.PtrToStructure(pBuf, typeof(DhStruct.NET_VIDEOSTAT_SUMMARY))!;
            OnDhPeopleInOut?.Invoke(new WebViewSendModel<(int In, int Out)>()
            {
                success = true,
                method = nameof(OnDhPeopleInOut),
                response = (info.stuEnteredSubtotal.nTotal, info.stuExitedSubtotal.nTotal),
                msg = "获取成功"
            });
        }

        private void OnDeviceDisconnected()
        {
            DeviceDisconnected?.Invoke();
        }

        /// <summary>
        /// 断线回调方法
        /// </summary>
        /// <param name="lLoginID"></param>
        /// <param name="pchDVRIP"></param>
        /// <param name="nDVRPort"></param>
        /// <param name="dwUser"></param>
        private void DisConnectCallBack(IntPtr lLoginID, IntPtr pchDVRIP, int nDVRPort, IntPtr dwUser)
        {
            DhSdk.Logout(m_LoginID);
            m_LoginID = IntPtr.Zero;
            if (m_PlayID != IntPtr.Zero)
            {
                DhSdk.RenderPrivateData(m_PlayID, false);
                DhSdk.StopRealPlay(m_PlayID);
                m_PlayID = IntPtr.Zero;
            }

            if (m_AnalyzerID != IntPtr.Zero)
            {
                DhSdk.StopLoadPic(m_AnalyzerID);
                m_AnalyzerID = IntPtr.Zero;
            }

            OnDeviceDisconnected();
        }


        private int getFaceBase64Count = 0;

        /// <summary>
        /// 订阅事件回调
        /// </summary>
        /// <param name="lAnalyzerHandle"></param>
        /// <param name="dwEventType"></param>
        /// <param name="pEventInfo"></param>
        /// <param name="pBuffer"></param>
        /// <param name="dwBufSize"></param>
        /// <param name="dwUser"></param>
        /// <param name="nSequence"></param>
        /// <param name="reserved"></param>
        /// <returns></returns>
        private int AnalyzerDataCallBack(IntPtr lAnalyzerHandle, uint dwEventType, IntPtr pEventInfo, IntPtr pBuffer,
            uint dwBufSize, IntPtr dwUser, int nSequence, IntPtr reserved)
        {
            if (m_AnalyzerID != lAnalyzerHandle) return 0;

            DhStruct.NET_DEV_EVENT_FACEDETECT_INFO info =
                (DhStruct.NET_DEV_EVENT_FACEDETECT_INFO) Marshal.PtrToStructure(pEventInfo,
                    typeof(DhStruct.NET_DEV_EVENT_FACEDETECT_INFO));
            if (m_GroupID != info.stuObject.nRelativeID)
                return 0;

            if (++getFaceBase64Count <= 1)
            {
                return 0;
            }

            switch (dwEventType)
            {
                //截取人脸
                case (uint) DhStruct.EM_EVENT_IVS_TYPE.FACEDETECT:
                {
                    byte[] personFaceInfo = new byte[dwBufSize];

                    Marshal.Copy(pBuffer, personFaceInfo, 0, (int) dwBufSize);
                    using MemoryStream stream = new MemoryStream(personFaceInfo);


                    Image bitmap = Image.FromStream(stream);

#if DEBUG
                    try
                    {
                        bitmap?.Save(@$"imgs/face_event_{new DateTime().ToTimeStamp()}");
                    }
                    catch (Exception e)
                    {
                    }
#endif


                    OnDhGetFaceImageBase64?.Invoke(new WebViewSendModel<string>()
                    {
                        success = true,
                        response = bitmap.ToBase64String(ImageFormat.Png),
                        method = nameof(OnDhGetFaceImageBase64),
                        msg = "获取成功"
                    });
                }
                    break;
            }

            getFaceBase64Count = 0;
            return 0;
        }

        #endregion
        
        #region MyRegion

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="ip">设备ip地址</param>
        /// <param name="netPort">设备端口号默认37777</param>
        /// <param name="name">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public MessageModel<string> Login(string ip, string netPort, string name, string password)
        {
            var res = new MessageModel<string>();
            if (IntPtr.Zero == m_LoginID)
            {
                ushort port = 0;
                try
                {
                    port = Convert.ToUInt16(netPort.Trim());
                }
                catch
                {
                    res.msg = "输入的端口有误";
                    return res;
                }

                //登录
                m_LoginID = DhSdk.LoginWithHighLevelSecurity(ip, port, name, password,
                    DhStruct.EM_LOGIN_SPAC_CAP_TYPE.TCP, IntPtr.Zero, ref m_DevInfo);
                if (IntPtr.Zero == m_LoginID)
                {
                    res.msg = DhSdk.GetLastError();
                    return res;
                }

                //判断设备是否有通道
                if (m_DevInfo.nChanNum == 0)
                {
                    bool ret = DhSdk.Logout(m_LoginID);
                    if (!ret)
                    {
                        res.msg = DhSdk.GetLastError();
                        return res;
                    }

                    m_LoginID = IntPtr.Zero;
                    res.msg = "Cannot get channel info 获取不到通道号！\n Device unspport 设备不支持";
                    return res;
                }

                _SnapRevCallBack = new fSnapRevCallBack(SnapRevCallBack);
                DhSdk.SetSnapRevCallBack(_SnapRevCallBack, IntPtr.Zero);
                // if (m_PlayID == IntPtr.Zero)
                // {
                //     m_PlayID = DhSdk.RealPlay(m_LoginID, 0, hWnd);
                //     if (IntPtr.Zero == m_PlayID)
                //     {
                //         res.msg = "开启监视失败";
                //         return res;
                //     }
                //
                //     // bool resData = DhSdk.RenderPrivateData(m_PlayID, true);
                //     // if (!resData)
                //     // {
                //     //     res.msg = DhSdk.GetLastError();
                //     //     return res;
                //     // }
                // }
            }

            res.msg = "登录成功";
            res.success = true;
            return res;
        }

        public MessageModel<string> Init()
        {
            var res = new MessageModel<string>();

            if (isInit)
            {
                res.success = true;
                res.msg = "已经完成初始化，无需再次初始化";
                return res;
            }

            //初始化
            if (!DhSdk.Init(m_DisConnectCallBack, IntPtr.Zero, null))
            {
                res.msg = "初始化失败";
                return res;
            }


            isInit = true;

            res.msg = "初始化成功";
            res.success = true;

            return res;
        }


        private bool _isRegisterCutFaceEvent = false;

        /// <summary>
        /// 截取人脸
        /// </summary>
        /// <exception cref="Exception"></exception>
        public MessageModel<string> RegisterCutFaceEvent()
        {
            var res = new MessageModel<string>();

            if (_isRegisterCutFaceEvent)
            {
                res.success = true;
                res.msg = "已经注册时间，无需再次注册";
                return res;
            }

            //订阅人脸检测事件
            realLoadPictureHandle = DhSdk.RealLoadPicture(m_LoginID, 0,
                (uint) DhStruct.EM_EVENT_IVS_TYPE.FACEDETECT, true, m_AnalyzerDataCallBack, IntPtr.Zero, IntPtr.Zero);
            m_AnalyzerID = realLoadPictureHandle;
            if (IntPtr.Zero == realLoadPictureHandle)
            {
                res.msg = DhSdk.GetLastError();
                return res;
            }

            _isRegisterCutFaceEvent = true;
            res.success = true;
            res.msg = "注册成功";
            return res;
        }

        private bool _isRegisterPeopleInoutEvent = false;

        /// <summary>
        /// 人流量统计
        /// </summary>
        public MessageModel<string> RegisterPeopleInoutEvent()
        {
            var res = new MessageModel<string>();

            if (_isRegisterPeopleInoutEvent)
            {
                res.success = true;
                res.msg = "已经注册时间，无需再次注册";
                return res;
            }

            if (m_AttactID != IntPtr.Zero)
            {
                res.msg = "空指针异常";
                return res;
            }

            DhStruct.NET_IN_ATTACH_VIDEOSTAT_SUM inParam = new DhStruct.NET_IN_ATTACH_VIDEOSTAT_SUM
            {
                dwSize = (uint) Marshal.SizeOf(typeof(DhStruct.NET_IN_ATTACH_VIDEOSTAT_SUM)),
                nChannel = 0,
                cbVideoStatSum = m_VideoStatSumCallBack
            };

            DhStruct.NET_OUT_ATTACH_VIDEOSTAT_SUM outParam = new DhStruct.NET_OUT_ATTACH_VIDEOSTAT_SUM
            {
                dwSize = (uint) Marshal.SizeOf(typeof(DhStruct.NET_OUT_ATTACH_VIDEOSTAT_SUM))
            };
            //订阅人流量统计信息
            m_AttactID = DhSdk.AttachVideoStatSummary(m_LoginID, ref inParam, ref outParam, 5000);
            if (IntPtr.Zero == m_AttactID)
            {
                res.msg = DhSdk.GetLastError();
                return res;
            }

            _isRegisterPeopleInoutEvent = true;
            res.success = true;
            res.msg = "注册成功";
            return res;
        }


        private static object lockobj = new object();


        public MessageModel<string> CutCameraBase64Image()
        {
            var res = new MessageModel<string>();

            var fileName = @"C:\Users\zy\Desktop\temp\CutCameraImage.jpg";

            lock (lockobj)
            {
                NET_SNAP_PARAMS asyncSnap = new NET_SNAP_PARAMS();
                asyncSnap.Channel = 0;
                asyncSnap.Quality = 1;
                asyncSnap.ImageSize = 2;
                asyncSnap.mode = 0;
                asyncSnap.InterSnap = 0;
                networkCutbase64 = "";

                bool ret = default;
                try
                {
                    ret = DhSdk.SnapPictureEx(m_LoginID, asyncSnap, IntPtr.Zero);
                }
                catch (Exception e)
                {
                    res.success = false;
                    res.msg = "远程抓图请求异常";
                    res.devMsg = @$"exception msg : {e.ToString()} " + "\r\n" + @$"StackTrace: {e.StackTrace}";

                    e.Log(Log.GetLog());
                    return res;
                }


                if (!ret)
                {
                    res.msg = "截取图片失败";
                    return res;
                }

                while (networkCutbase64.IsEmpty())
                {
                    Task.Delay(10).GetAwaiter().GetResult();
                }

#if DEBUG
                try
                {
                    ImageHelper.FromBase64String(networkCutbase64)?.Save(@$"imgs/cut_{new DateTime().ToTimeStamp()}");
                }
                catch (Exception e)
                {
                }
#endif

                res.response = networkCutbase64;
                res.success = true;
                res.msg = "获取成功";
                return res;
            }


            // lock (lockobj)
            // {
            //     
            //     var cutRes =  DhSdk.CapturePicture(m_PlayID, fileName, DhSdk.EM_NET_CAPTURE_FORMATS.JPEG_50);
            //
            //     if (File.Exists(fileName))
            //     {
            //         res.response = Bitmap.FromFile(fileName).ToBase64String(ImageFormat.Jpeg);
            //     }
            //
            //     res.success = !res.response.IsEmpty();
            //
            //     res.msg = res.success ? "获取成功" : "获取失败";
            //     return res;
            // }
        }

        #endregion
    }
}