using System.Collections.Generic;
using System.Text;
using System;
using System.Runtime.InteropServices;

namespace Mijin.Library.App.Driver.Drivers.FaceValid.SDK
{
    /// <summary>
    /// 人脸检测
    /// </summary>
    public class NativeCWFaceDetection
    {
        private const string CloudWalkSDKDll = "CWFaceSDK.dll";

        /// <summary>
        /// 创建检测器句柄
        /// </summary>
        /// <param name="pConfigFile">模型参数配置文件</param>
        /// <param name="pLicence">授权码（仅用于安卓平台，PC端传空即可）</param>
        /// <returns>如果创建成功，返回detector句柄，否则返回空</returns>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwCreateDetHandle", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr cwCreateDetHandle(out cw_errcode_t errCode, string pConfigFile, string pLicence);


        /// <summary>
        /// 释放创建的检测器
        /// </summary>
        /// <param name="pDetector"></param>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwReleaseDetHandle", CallingConvention = CallingConvention.Cdecl)]
        public static extern void cwReleaseDetHandle(IntPtr pDetector);


        /// <summary>
        /// 获取检测器参数
        /// </summary>
        /// <param name="pDetector"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwGetFaceParam", CallingConvention = CallingConvention.Cdecl)]
        public static extern cw_errcode_t cwGetFaceParam(IntPtr pDetector, out cw_det_param_t param);


        /// <summary>
        /// 设置检测器参数（必须先调用cwGetFaceParam再使用此函数）
        /// </summary>
        /// <param name="pDetector"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwSetFaceParam", CallingConvention = CallingConvention.Cdecl)]
        public static extern cw_errcode_t cwSetFaceParam(IntPtr pDetector, ref cw_det_param_t param);




        /// <summary>
        /// 人脸检测跟踪接口
        /// </summary>
        /// <param name="pDetector"></param>
        /// <param name="pFrameImg">被检测图像. 如果传入的数据只有bgr,则nImageChannel 固定为3，如果pimageData为其他格式，则还应有一个参数来表示图像格式</param>
        /// <param name="pFaceBuffer">检测到的人脸，该缓冲区初始化必须足够大</param>
        /// <param name="iBuffLen">最大检测到人脸个数，主要定义了pFaceBuffer的初始化大小；如果实际人脸个数多于此值则只能返回nMaxFaceNumber个人脸.</param>
        /// <param name="nFaceNum">实际检测到的人脸个数</param>
        /// <param name="iOp">人脸检测接口可以进行的操作（POS是默认操作，其他项可选；返回结果和操作选项是对应的）,具体参考DET_OP定义.</param>
        /// <returns></returns>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwFaceDetection", CallingConvention = CallingConvention.Cdecl)]
        public static extern cw_errcode_t cwFaceDetection(IntPtr pDetector, ref cw_img_t pFrameImg, IntPtr pFaceBuffer, int iBuffLen, ref int nFaceNum, int iOp);


        /// <summary>
        /// 清除检测跟踪状态信息函数
        /// </summary>
        /// <param name="pDetector">检测器句柄</param>
        /// <returns>成功返回CW_OK，失败返回其他</returns>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwResetDetTrackState", CallingConvention = CallingConvention.Cdecl)]
        public static extern cw_errcode_t cwResetDetTrackState(IntPtr pDetector);

    }
}