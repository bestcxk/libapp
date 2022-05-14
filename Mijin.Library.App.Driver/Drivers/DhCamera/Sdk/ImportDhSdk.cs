using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver.Drivers.DhCamera
{
    /// <summary>
    /// 大华DLLImport
    /// </summary>
    public static class ImportDhSdk
    {
        private const string LIBRARYNETSDK = "dhnetsdk.dll";
        private const string LIBRARYCONFIGSDK = "dhconfigsdk.dll";
        
        /// <summary>
        /// 抓图
        /// </summary>
        /// <param name="hPlayHandle"></param>
        /// <param name="pchPicFileName"></param>
        /// <param name="eFormat"></param>
        /// <returns></returns>
        [DllImport(LIBRARYNETSDK)]
        public static extern bool CLIENT_CapturePictureEx(IntPtr hPlayHandle, string pchPicFileName, DhSdk.EM_NET_CAPTURE_FORMATS eFormat);
        
        [DllImport(LIBRARYNETSDK)]
        public static extern void CLIENT_SetSnapRevCallBack(DhSdk.fSnapRevCallBack OnSnapRevMessage, IntPtr dwUser);

        /// <summary>
        /// 监视
        /// </summary>
        /// <param name="lLoginID"></param>
        /// <param name="nChannelID"></param>
        /// <param name="hWnd"></param>
        /// <param name="rType"></param>
        /// <returns></returns>
        [DllImport(LIBRARYNETSDK)]
        public static extern IntPtr CLIENT_RealPlayEx(IntPtr lLoginID, int nChannelID, IntPtr hWnd, DhStruct.EM_RealPlayType rType);
        
        /// <summary>
        /// 网络抓图请求
        /// </summary>
        /// <param name="lLoginID"></param>
        /// <param name="par"></param>
        /// <param name="reserved"></param>
        /// <returns></returns>
        [DllImport(LIBRARYNETSDK)]
        public static extern bool CLIENT_SnapPictureEx(IntPtr lLoginID, ref NET_SNAP_PARAMS par, IntPtr reserved);
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="cbDisConnect"></param>
        /// <param name="dwUser"></param>
        /// <param name="lpInitParam"></param>
        /// <returns></returns>
        [DllImport(LIBRARYNETSDK)]
        public static extern bool CLIENT_InitEx(DhSdk.fDisConnectCallBack cbDisConnect, IntPtr dwUser, IntPtr lpInitParam);

        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="lLoginID"></param>
        /// <returns></returns>
        [DllImport(LIBRARYNETSDK)]
        public static extern bool CLIENT_Logout(IntPtr lLoginID);

        [DllImport(LIBRARYNETSDK)]
        public static extern bool CLIENT_RenderPrivateData(IntPtr lPlayHandle, bool bTrue);

        /// <summary>
        /// 异常
        /// </summary>
        /// <returns></returns>
        [DllImport(LIBRARYNETSDK)]
        public static extern int CLIENT_GetLastError();

        /// <summary>
        /// 关闭实时监视
        /// </summary>
        /// <param name="lRealHandle"></param>
        /// <returns></returns>
        [DllImport(LIBRARYNETSDK)]
        public static extern bool CLIENT_StopRealPlayEx(IntPtr lRealHandle);

        /// <summary>
        /// 取消订阅事件
        /// </summary>
        /// <param name="lAnalyzerHandle"></param>
        /// <returns></returns>
        [DllImport(LIBRARYNETSDK)]
        public static extern bool CLIENT_StopLoadPic(IntPtr lAnalyzerHandle);

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="pstInParam"></param>
        /// <param name="pstOutParam"></param>
        /// <returns></returns>
        [DllImport(LIBRARYNETSDK)]
        public static extern IntPtr CLIENT_LoginWithHighLevelSecurity(ref DhStruct.NET_IN_LOGIN_WITH_HIGHLEVEL_SECURITY pstInParam, ref DhStruct.NET_OUT_LOGIN_WITH_HIGHLEVEL_SECURITY pstOutParam);

        /// <summary>
        /// 订阅事件
        /// </summary>
        /// <param name="lLoginID"></param>
        /// <param name="nChannelID"></param>
        /// <param name="dwAlarmType"></param>
        /// <param name="bNeedPicFile"></param>
        /// <param name="cbAnalyzerData"></param>
        /// <param name="dwUser"></param>
        /// <param name="reserved"></param>
        /// <returns></returns>
        [DllImport(LIBRARYNETSDK)]
        public static extern IntPtr CLIENT_RealLoadPictureEx(IntPtr lLoginID, int nChannelID, uint dwAlarmType, bool bNeedPicFile, DhSdk.fAnalyzerDataCallBack cbAnalyzerData, IntPtr dwUser, IntPtr reserved);

        /// <summary>
        /// 人流统计订阅
        /// </summary>
        /// <param name="lLoginID"></param>
        /// <param name="pInParam"></param>
        /// <param name="pOutParam"></param>
        /// <param name="nWaitTime"></param>
        /// <returns></returns>
        [DllImport(LIBRARYNETSDK)]
        public static extern IntPtr CLIENT_AttachVideoStatSummary(IntPtr lLoginID, ref DhStruct.NET_IN_ATTACH_VIDEOSTAT_SUM pInParam, ref DhStruct.NET_OUT_ATTACH_VIDEOSTAT_SUM pOutParam, int nWaitTime);

    }
}
