using Mijin.Library.App.Driver.Interface;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace Mijin.Library.App.Driver.Drivers.DhCamera
{
    /// <summary>
    /// 大华摄像头
    /// </summary>
    public class DhCamera : IDhCamera
    {
        /// <summary>
        /// 人流量统计事件
        /// </summary>
        public event Action<(int In, int Out)> OnPeopleInOut;

        /// <summary>
        /// 截取人脸事件
        /// </summary>
        public event Action<Image> OnGetFaceImage;

        //声明静态委托，普通委托可能会出现回调之前就将其释放的错误
        /// <summary>
        /// 事件数据回调函数
        /// </summary>
        private static DhSdk.fAnalyzerDataCallBack m_AnalyzerDataCallBack { get; set; }
        /// <summary>
        /// 断线回调函数
        /// </summary>
        private static DhSdk.fDisConnectCallBack m_DisConnectCallBack { get; set; }

        private event Action DeviceDisconnected;

        /// <summary>
        /// 视频统计摘要信息回调函数
        /// </summary>
        private static DhSdk.fVideoStatSumCallBack m_VideoStatSumCallBack { get; set; }
        /// <summary>
        /// 登录句柄
        /// </summary>
        private IntPtr m_LoginID { get; set; } = IntPtr.Zero;

        private int m_GroupID { get; set; } = 0;
        private IntPtr m_AttactID { get; set; } = IntPtr.Zero;
        private IntPtr m_PlayID { get; set; } = IntPtr.Zero;
        private IntPtr m_AnalyzerID = IntPtr.Zero;
        private DhStruct.NET_DEVICEINFO_Ex m_DevInfo = new();

        /// <summary>
        /// 人脸图片保存地址
        /// </summary>
        private string path { get; set; }

        public DhCamera()
        {
            m_VideoStatSumCallBack = VideoStatSumCallBack;
            m_DisConnectCallBack = DisConnectCallBack;
            m_AnalyzerDataCallBack = AnalyzerDataCallBack;
            //初始化
            if (!DhSdk.Init(m_DisConnectCallBack, IntPtr.Zero, null))
            {
                throw new Exception("初始化失败");
            }
        }

        #region 回调方法

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
            DhStruct.NET_VIDEOSTAT_SUMMARY info = (DhStruct.NET_VIDEOSTAT_SUMMARY)Marshal.PtrToStructure(pBuf, typeof(DhStruct.NET_VIDEOSTAT_SUMMARY))!;
            OnPeopleInOut?.Invoke((info.stuEnteredSubtotal.nTotal, info.stuExitedSubtotal.nTotal));
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
        private int AnalyzerDataCallBack(IntPtr lAnalyzerHandle, uint dwEventType, IntPtr pEventInfo, IntPtr pBuffer, uint dwBufSize, IntPtr dwUser, int nSequence, IntPtr reserved)
        {
            if (m_AnalyzerID != lAnalyzerHandle) return 0;
            switch (dwEventType)
            {
                //截取人脸
                case (uint)DhStruct.EM_EVENT_IVS_TYPE.FACEDETECT:
                    {
                        byte[] personFaceInfo = new byte[dwBufSize];

                        Marshal.Copy(pBuffer, personFaceInfo, 0, (int)dwBufSize);
                        using MemoryStream stream = new MemoryStream(personFaceInfo);
                        Image bitmap = Image.FromStream(stream);
                        OnGetFaceImage?.Invoke(bitmap);
                    }
                    break;
            }
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
        public void Login(string ip, string netPort, string name, string password)
        {
            if (IntPtr.Zero == m_LoginID)
            {
                ushort port = 0;
                try
                {
                    port = Convert.ToUInt16(netPort.Trim());
                }
                catch
                {
                    throw new Exception("输入的端口有误");
                }

                //登录
                m_LoginID = DhSdk.LoginWithHighLevelSecurity(ip, port, name, password, DhStruct.EM_LOGIN_SPAC_CAP_TYPE.TCP, IntPtr.Zero, ref m_DevInfo);
                if (IntPtr.Zero == m_LoginID)
                {
                    throw new Exception(DhSdk.GetLastError());
                }
                //判断设备是否有通道
                if (m_DevInfo.nChanNum == 0)
                {
                    bool ret = DhSdk.Logout(m_LoginID);
                    if (!ret)
                    {
                        throw new Exception(DhSdk.GetLastError());
                    }

                    m_LoginID = IntPtr.Zero;
                    throw new Exception("Cannot get channel info 获取不到通道号！\n Device unspport 设备不支持");
                }
            }
        }

        /// <summary>
        /// 截取人脸
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void GetFace()
        {
            //订阅人脸检测事件
            IntPtr realLoadPictureHandle = DhSdk.RealLoadPicture(m_LoginID, 0, (uint)DhStruct.EM_EVENT_IVS_TYPE.FACEDETECT, true, m_AnalyzerDataCallBack, IntPtr.Zero, IntPtr.Zero);
            m_AnalyzerID = realLoadPictureHandle;
            if (IntPtr.Zero == realLoadPictureHandle)
            {
                throw new Exception(DhSdk.GetLastError());
            }
        }

        /// <summary>
        /// 人流量统计
        /// </summary>
        public void HumanSum()
        {
            if (m_AttactID != IntPtr.Zero) return;

            DhStruct.NET_IN_ATTACH_VIDEOSTAT_SUM inParam = new DhStruct.NET_IN_ATTACH_VIDEOSTAT_SUM
            {
                dwSize = (uint)Marshal.SizeOf(typeof(DhStruct.NET_IN_ATTACH_VIDEOSTAT_SUM)),
                nChannel = 0,
                cbVideoStatSum = m_VideoStatSumCallBack
            };

            DhStruct.NET_OUT_ATTACH_VIDEOSTAT_SUM outParam = new DhStruct.NET_OUT_ATTACH_VIDEOSTAT_SUM
            {
                dwSize = (uint)Marshal.SizeOf(typeof(DhStruct.NET_OUT_ATTACH_VIDEOSTAT_SUM))
            };
            //订阅人流量统计信息
            m_AttactID = DhSdk.AttachVideoStatSummary(m_LoginID, ref inParam, ref outParam, 5000);
            if (IntPtr.Zero == m_AttactID)
            {
                throw new Exception(DhSdk.GetLastError());
            }
        }

        #endregion
    }
}
