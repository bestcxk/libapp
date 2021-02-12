using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver.Drivers.FaceValid.SDK
{
    /// <summary>
    /// 
    /// </summary>
    public class NativeCWFaceRecognition
    {
        private const string CloudWalkSDKDll = "CWFaceSDK.dll";

        /// <summary>
        ///功能：创建识别句柄
        /// </summary>
        /// <param name="pRecogHandle">创建的识别句柄</param>
        /// <param name="configure_path">配置文件路径</param>
        /// <param name="recog_pattern">创建的句柄类型</param>
        /// <returns></returns>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwCreateRecogHandle", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr cwCreateRecogHandle(out cw_errcode_t errCode, string pConfigurePath, string pLicence, cw_recog_pattern_t emRecogPattern);


        /// <summary>
        /// 释放通道
        /// </summary>
        /// <param name="pRecogHandle">识别句柄</param>
        /// <returns></returns>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwReleaseRecogHandle", CallingConvention = CallingConvention.Cdecl)]
        public static extern void cwReleaseRecogHandle(IntPtr pRecogHandle);


        /// <summary>
        /// 功能：获取特征值长度
        /// </summary>
        /// <param name="pRecogHandle">识别句柄</param>
        /// <returns></returns>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwGetFeatureLength", CallingConvention = CallingConvention.Cdecl)]
        public static extern int cwGetFeatureLength(IntPtr pRecogHandle);


        /// <summary>
        /// 功能：提取人脸特征值
        /// </summary>
        /// <param name="pRecogHandle">识别句柄</param>
        /// <param name="alignedFace">对齐人脸数据指针</param>
        /// <param name="featueData">返回的特征数，需要预先分配足够空间</param>
        /// <returns></returns>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwGetFaceFeature", CallingConvention = CallingConvention.Cdecl)]
        public static extern cw_errcode_t cwGetFaceFeature(IntPtr pRecogHandle, ref cw_aligned_face_t alignedFace, byte[] featueData);



        /// <summary>
        /// 功能：计算2个特征与N个特征的相似度，返回的相似度scores的个数为N个
        /// </summary>
        /// <param name="pRecogHandle">识别句柄</param>
        /// <param name="pFea1">特征1，只能是一个特征</param>
        /// <param name="pFea2">特征2，可以是N个特征</param>
        /// <param name="pFea2Num">特征2的个数</param>
        /// <param name="scores">返回的相似度分数数组，长度为Fea2num，需要预先分配空间 </param>
        /// <returns></returns>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwComputeMatchScore", CallingConvention = CallingConvention.Cdecl)]
        public static extern cw_errcode_t cwComputeMatchScore(IntPtr pRecogHandle, byte[] pFea1, byte[] pFea2, int Fea2Num, float[] scores);


        /// <summary>
        /// 功能：比对两个人脸图片中最大人脸特征，获取相似度
        /// </summary>
        /// <param name="pDetector">检测句柄</param>
        /// <param name="pRecogHandle">识别句柄</param>
        /// <param name="pFrameImg1">图片1数据</param>
        /// <param name="pFrameImg2">图片2数据</param>
        ///  <param name="scores">比对分数</param>
        /// <returns></returns>
        [DllImport(CloudWalkSDKDll, EntryPoint = "cwVerifyImageData", CallingConvention = CallingConvention.Cdecl)]
        public static extern cw_errcode_t cwVerifyImageData(IntPtr pDetector, IntPtr pRecogHandle, cw_img_t[] pFrameImg1, cw_img_t[] pFrameImg2, float[] scores);
        
    }
}
