using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System;

namespace Mijin.Library.App.Driver.Drivers.FaceValid.SDK
{
    /// <summary>
    /// 人脸检测
    /// </summary>
    public class NativeCWFaceVersion
    {
        private const string CloudWalkSDKDll = "CWFaceSDK.dll";

        /// <summary>
        /// 功能：获取SDK版本信息
        /// </summary>
        /// <param name="pVersion">版本信息，需事先分配内存</param>
        /// <param name="iBuffLen">输出buf分配字节长度</param>>
        /// <returns>成功返回CW_OK，失败返回其他</returns>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwGetSDKVersion", CallingConvention = CallingConvention.Cdecl)]
        public static extern cw_errcode_t cwGetSDKVersion(StringBuilder pVersion, int iBuffLen);


        /// <summary>
        /// 功能：获取设备码
        /// </summary>
        /// <param name="pDeviceInfo">设备唯一码，需事先分配内存，不低于160字节</param>
        /// <param name="iBuffLen">输出buf分配字节长度，不低于160字节</param>
        /// <param name="iUseLen">输出的设备码的长度</param>
        /// <returns>成功返回CW_OK，失败返回其他</returns>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwGetDeviceInfo", CallingConvention = CallingConvention.Cdecl)]
        public static extern cw_errcode_t cwGetDeviceInfo(StringBuilder pDeviceInfo, int iBuffLen, ref int iUseLen);



        /// <summary>
        /// 功能：安装授权
        /// </summary>
        /// <param name="sAppKey">授权AppKey，需从云从科技获取</param>
        /// <param name="sAppSecret">授权AppSecret，需从云从科技获取</param>
        /// <param name="sProductId">授权ProductId，需从云从科技获取</param>
        /// <returns>授权并发数</returns>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwInstallLicence", CallingConvention = CallingConvention.Cdecl)]
        public static extern cw_errcode_t cwInstallLicence(string sAppKey, string sAppSecret, string sProductId);

    }
}