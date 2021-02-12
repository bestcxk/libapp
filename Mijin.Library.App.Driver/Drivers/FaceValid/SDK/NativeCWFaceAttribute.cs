using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System;

namespace Mijin.Library.App.Driver.Drivers.FaceValid.SDK
{
    /// <summary>
    /// 人脸检测
    /// </summary>
    public class NativeCWFaceAttribute
    {
        private const string CloudWalkSDKDll = "CWFaceSDK.dll";

        /// <summary>
        /// 创建属性句柄
        /// </summary>
        /// <param name="pConfigFile">模型参数配置文件</param>
        /// <param name="pLicence">授权码
        /// <returns>如果创建成功，返回Attribute句柄，否则返回空</returns>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwCreateAttributeHandle", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr cwCreateAttributeHandle(out cw_errcode_t errCode, string pConfigFile, string pLicence);


        /// <summary>
        /// 释放属性句柄
        /// </summary>
        /// <param name="pAttributeHandle"></param>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwReleaseAttributeHandle", CallingConvention = CallingConvention.Cdecl)]
        public static extern void cwReleaseAttributeHandle(IntPtr pAttributeHandle);


        /// <summary>
        /// 年龄段估计
        /// </summary>
        /// <param name="pAttributeHandle"></param>
        /// <param name="alignedFace">对齐人脸</param>
        /// <param name="pAge">年龄段估计值，0 小孩  1 成年人 2 老人</param>
        /// <param name="confidence">置信分数 0-1之间的一个值，置信度越高</param>
        /// <returns></returns>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwGetAgeEval", CallingConvention = CallingConvention.Cdecl)]
        public static extern cw_errcode_t cwGetAgeEval(IntPtr pAttributeHandle, ref cw_aligned_face_t alignedFace, out int pAge, out float confidence);


        /// <summary>
        /// 性别估计
        /// </summary>
        /// <param name="pAttributeHandle"></param>
        /// <param name="alignedFace">对齐人脸</param>
        ///  <param name="pGender">性别估计值，0 女性, 1 男性</param>
        /// <returns></returns>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwGetGenderEval", CallingConvention = CallingConvention.Cdecl)]
        public static extern cw_errcode_t cwGetGenderEval(IntPtr pAttributeHandle, ref cw_aligned_face_t alignedFace, out int pGender, out float confidence);


        /// <summary>
        /// 人种估计
        /// </summary>
        /// <param name="pAttributeHandle"></param>
        /// <param name="alignedFace">对齐人脸</param>
        /// <param nem="pRace">人种估计值，0 黑人 1 白人 2 黄人</param>
        /// <returns></returns>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwGetRaceEval", CallingConvention = CallingConvention.Cdecl)]
        public static extern cw_errcode_t cwGetRaceEval(IntPtr pAttributeHandle, ref cw_aligned_face_t alignedFace, out int pRace, out float confidence);

    }
}