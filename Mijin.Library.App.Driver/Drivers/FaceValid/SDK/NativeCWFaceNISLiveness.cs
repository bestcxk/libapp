using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System;

namespace Mijin.Library.App.Driver.Drivers.FaceValid.SDK
{
    /// <summary>
    /// 人脸检测
    /// </summary>
    public class CWFaceNisLiveness
    {
        private const string CloudWalkSDKDll = "CWFaceSDK.dll";

        /// <summary>
        /// 创建活体检测句柄
        /// </summary>
        /// <param name="pNirModelPath">红外活体检测器模型文件</param>
        /// <param name="pRecogModelPath">红外活体识别比对模型文件</param>
        /// <param name="pPairFilePath">匹配文件路径</param>
        /// <param name="skinThresh">肤色阈值（根据不同的前端版本设置）</param>
        /// <param name="pLicence">授权码
        /// <returns>如果创建成功，返回Attribute句柄，否则返回空</returns>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwCreateNirLivenessHandle", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr cwCreateNirLivenessHandle(out cw_nirliveness_err_t errCode, string pNirModelPath, string pRecogModelPath, string pPairFilePath, string pLogPath, float skinThresh, string pLicence);


        /// <summary>
        /// 释放活体检测句柄
        /// </summary>
        /// <param name="pHandle">红外活体句柄</param>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwReleaseNirLivenessHandle", CallingConvention = CallingConvention.Cdecl)]
        public static extern void cwReleaseNirLivenessHandle(IntPtr pHandle);


        /// <summary>
        /// 红外活体检测接口
        /// </summary>
        /// <param name="pDetector">人脸检测句柄</param>
        /// <param name="pNirHandle">红外活体句柄</param>
        /// <param name="pImgVis">输入可见光图片数据</param>
        /// <param name="pImgNir">输入红外光图片数据</param>
        /// <param name="pNirLivRes">存放红外活体检测结果，需事先分配内存</param>
        /// <returns></returns>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwFaceNirByImageData", CallingConvention = CallingConvention.Cdecl)]
        public static extern cw_nirliveness_err_t cwFaceNirByImageData(IntPtr pDetector, IntPtr pNirHandle, ref cw_img_t pImgVis, ref cw_img_t pImgNir, out cw_nirliv_res_t pNirLivRes);

    }
}