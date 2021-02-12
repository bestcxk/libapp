using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver.Drivers.FaceValid.SDK
{
    /// <summary>
    /// 检测功能开关选项
    /// </summary>
    public enum DetectTrackOperationType
    {
        /// <summary>
        /// 进行人脸检测，并返回人脸矩形位置，默认开启
        /// </summary>
        CW_OP_DET = 0,
        /// <summary>
        /// 进行人脸跟踪，并返回人脸跟踪的ID
        /// </summary>
        CW_OP_TRACK = 2,
        /// <summary>
        /// 进行人脸关键点检测开关
        /// </summary>
        CW_OP_KEYPT = 4,
        /// <summary>
        /// 进行人脸图像对齐，并返回对齐后的人脸图像，用来提取特征
        /// </summary>
        CW_OP_ALIGN = 8,
        /// <summary>
        /// 人脸质量评估开关（质量分子项开关在配置文件中配置）
        /// </summary>
        CW_OP_QUALITY = 16,
        /// <summary>
        /// （所有开关综合）总开关
        /// </summary>
        CW_OP_ALL = 30,
    };

    /// <summary>
    /// 通用错误码
    /// </summary>
    public enum cw_errcode_t
    {
        CW_OK = 0,							// 成功 or 合法

        CW_UNKNOWN_ERR = 20000,		        // 未知错误
        CW_DETECT_INIT_ERR,					// 初始化人脸检测器失败:如加载模型失败等
        CW_KEYPT_INIT_ERR,					// 初始化关键点检测器失败：如加载模型失败等
        CW_QUALITY_INIT_ERR,			    // 初始化跟踪器失败：如加载模型失败等

        CW_DET_ERR,						    // 检测失败
        CW_TRACK_ERR,						// 跟踪失败
        CW_KEYPT_ERR,						// 提取关键点失败
        CW_ALIGN_ERR,						// 对齐人脸失败
        CW_QUALITY_ERR,					    // 质量评估失败

        CW_EMPTY_FRAME_ERR,		            // 空图像
        CW_UNSUPPORT_FORMAT_ERR,			// 图像格式不支持
        CW_ROI_ERR,						    // ROI设置失败
        CW_UNINITIALIZED_ERR,				// 尚未初始化
        CW_MINMAX_ERR,						// 最小最大人脸设置失败
        CW_OUTOF_RANGE_ERR,                 // 数据范围错误
        CW_UNAUTHORIZED_ERR,				// 未授权
        CW_METHOD_UNAVAILABLE,			    // 方法无效
        CW_PARAM_INVALID,                   // 参数无效
        CW_BUFFER_EMPTY,					// 缓冲区空

        CW_FILE_UNAVAILABLE,                // 文件不存在：如加载的模型不存在等.
        CW_DEVICE_UNAVAILABLE,    			// 设备不存在
        CW_DEVICE_ID_UNAVAILABLE, 		    // 设备id不存在
        CW_EXCEEDMAXHANDLE_ERR,		        // 超过授权最大句柄数

        CW_RECOG_FEATURE_MODEL_ERR,		    // 加载特征识别模型失败   
        CW_RECOG_ALIGNEDFACE_ERR,		    // 对齐图片数据错误
        CW_RECOG_MALLOCMEMORY_ERR,          // 预分配特征空间不足  

        CW_RECOG_FEATUREDATA_ERR,		    // 特征数据错误
        CW_RECOG_EXCEEDMAXFEASPEED,		    // 超过授权最大提特征速度
        CW_RECOG_EXCEEDMAXCOMSPEED,		    // 超过授权最大比对速度
        CW_RECOG_GROUPSIZE_ERR,             // 特征比对特征数N超过最大授权数
        CW_RECOG_CONVERT_ERR,               // 特征转换失败
        CW_RECOG_NOFACEDET,                 // 未检测到人脸

        CW_LICENCE_JSON_CREATE_ERR,         // Json操作失败
        CW_LICENCE_DECRYPT_ERR,             // 加密失败
        CW_LICENCE_HTTP_ERROR,              // HTTP失败
        CW_LICENCE_MALLOCMEMORY_ERR,        // 授权内存分配不足
        CW_LICENCE_KEY_DEVICE_ERR,          // 获取设备文件错误
        CW_LICENCE_KEY_LICENSE_ERR,         // 获取授权文件错误
        CW_LICENCE_KEY_INSTALL_ERR,         // 安装授权文件错误

        CW_ATTRI_AGEGENDER_MODEL_ERR,       //加载年龄性别模型失败
        CW_ATTRI_EVAL_AGE_ERR,              //年龄识别失败
        CW_ATTRI_EVAL_GENDER_ERR,           //性别识别失败
        CW_ATTRI_EVAL_RACE_ERR,             //种族识别失败
    }

    /// <summary>
    /// 质量分检测错误码
    /// </summary>
    public enum cw_quality_errcode_t
    {
        CW_QUALITY_OK = 0,				    // 质量分数据有效
        CW_QUALITY_NO_DATA = 20150,		    // 质量分数据无效，原因：尚未检测
        CW_QUALITY_ERROR_UNKNOWN,           // 未知错误
    }

    /// <summary>
    /// 接口功能参数
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct cw_det_param_t
    {
        public int roiX;						   // roi, 默认整帧图像0, 0, 0, 0 若设置为异常值检测阶段将恢复默认
        public int roiY;
        public int roiWidth;
        public int roiHeight;

        public int minSize;							// 检测人脸尺寸范围： pc端默认[48,600];移动端默认[100,400]
        public int maxSize;

        public string pConfigFile;                  // 内部参数配置文件路径，此参数只能设置(set);从句柄内部获取出来的一律无效-------
    }

    /// <summary>
    /// 图像旋转角度（逆时针）
    /// </summary>
    public enum cw_img_angle_t
    {
        CW_IMAGE_ANGLE_0 = 0,
        CW_IMAGE_ANGLE_90,
        CW_IMAGE_ANGLE_180,
        CW_IMAGE_ANGLE_270
    }

    /// <summary>
    /// 图像镜像
    /// </summary>
    public enum cw_img_mirror_t
    {
        CW_IMAGE_MIRROR_NONE = 0,        // 不镜像  
        CW_IMAGE_MIRROR_HOR,             // 水平镜像
        CW_IMAGE_MIRROR_VER,             // 垂直镜像
        CW_IMAGE_MIRROR_HV               // 垂直和水平镜像
    }

    /// <summary>
    /// 图像格式
    /// </summary>
    public enum cw_img_form_t
    {
        CW_IMAGE_GRAY8 = 0,
        CW_IMAGE_BGR888,
        CW_IMAGE_BGRA8888,
        CW_IMAGE_RGB888,
        CW_IMAGE_RGBA8888,
        CW_IMAGE_YUV420P,
        CW_IMAGE_YV12,
        CW_IMAGE_NV12,
        CW_IMAGE_NV21,
        CW_IMAGE_BINARY,
    }

    /***************
	* 关键点信息
	*/
    public struct cw_point
    {  
        public float keypoint_x;
        public float keypoint_y;
    }

    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct cw_keypt
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 68)]
        public cw_point[] points;					                         // 关键点  
        public int nkeypt;											    // 关键点个数
        public float keyptScore;										// 关键点得分,推荐阈值为0.7
    }

    /// <summary>
    /// 图像
    /// </summary>
    public struct cw_img_t
    {
        public Int64 frameId;                           // 帧号
        public IntPtr data;								// 图像数据（必须预分配足够的空间）
        public int dataLen;                             // 数据长度，CW_IMAGE_BINARY格式必须设置，其他格式可不设
        public int width;							    // 宽，CW_IMAGE_BINARY格式可不设，其他格式必须设置
        public int height;							    // 高，CW_IMAGE_BINARY格式可不设，其他格式必须设置
        public cw_img_form_t format;					// 图像格式
        public cw_img_angle_t angle;					// 旋转角度
        public cw_img_mirror_t mirror;					// 镜像
    }

    /// <summary>
    /// 人脸框
    /// </summary>
    public struct cw_facepos_rect_t
    {
        public int x;
        public int y;
        public int width;
        public int height;
    }

    /// <summary>
    /// 对齐人脸
    /// </summary>
    public struct cw_aligned_face_t
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128 * 128)]
        public byte[] data;		                                    //图像数据,固定大小128 * 128
        public int width;											//宽
        public int height;										    //高
        public int nChannels;										//图像通道
    }

    /// <summary>
    /// 人脸质量分
    /// </summary>
    public struct cw_quality_t
    {
        public cw_quality_errcode_t errcode;	               // 质量分析错误码

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public float[] scores;                                 /* 质量分分数项，具体含义（根据数据下标顺序）:
															   * 0 - 人脸质量总分，0.65-1.0 (在启用第16个项mog分数的总分时，此分数为常数1.0，请忽略)
															   * 1 - 清晰度，越大表示越清晰，推荐范围0.65-1.0
															   * 2 - 亮度，越大表示越亮，推荐范围0.2-0.8
															   * 3 - 人脸角度，左转为正，右转为负
															   * 4 - 人脸角度，抬头为正，低头为负
															   * 5 - 人脸角度，顺时针为正，逆时针为负
															   * 6 - 肤色接近真人肤色程度，越大表示越真实，推荐范围0.5-1.0
															   * 7 - 戴黑框眼镜置信度，越大表示戴黑框眼镜的可能性越大，推荐范围0.0-0.5
															   * 8 - 戴墨镜的置信分，越大表示戴墨镜的可能性越大，推荐范围0.0-0.5
															   */
    }

    /// <summary>
    /// 人脸综合信息
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct cw_face_res_t
    {
        public Int64 frameId;                       // 人脸所在帧号

        public int detected;			            // 0: 跟踪到的人脸; 1: 检测到的人脸; 2:检测到但不会被进行后续计算(关键点)的人脸; 
        // 3: 可能是静态误检框；4:大角度人脸; 5:关键点错误; 6:不需再处理的人脸（只有标记为1的人脸，关键点、
        // 对齐、质量分才有效；但除0之外其他都可能有口罩分）7:被估计为低质量人脸

        public int trackId;			                // 人脸ID（ID<0表示没有进入跟踪）

        public cw_facepos_rect_t faceRect;			// 人脸框

        public cw_keypt keypt;             // 关键点

        public cw_aligned_face_t faceAligned;		// 对齐人脸

        public cw_quality_t quality;			    // 人脸质量
    }

    public enum cw_recog_pattern_t
    {
        CW_FEATURE_EXTRACT = 0,                     //  特征提取
        CW_RECOGNITION = 1                          //  识    别
    }


    //////////////////////////////////////////////////////////////////////////红外活体

    /***************
     * 红外活体检测结果返回值
     */
    public enum cw_nirliv_det_rst_t
    {
        CW_NIR_LIV_DET_LIVE = 0,				// 以阈值0.5判断为活体
        CW_NIR_LIV_DET_UNLIVE,				    // 以阈值0.5判断为非活体
        CW_NIR_LIV_DET_DIST_FAILED,				// 人脸距离检测未通过
        CW_NIR_LIV_DET_SKIN_FAILED,				// 人脸肤色检测未通过
        CW_NIR_LIV_DET_NO_PAIR_FACE,			// 未匹配到人脸
        CW_NIR_LIV_DET_IS_INIT					// 红外活体检测结果初始值
    }

    /***************
     * 红外活体检测错误码
     */
    public enum cw_nirliveness_err_t
    {
        CW_NIRLIV_OK = 0,						// 成功返回
        CW_NIRLIV_ERR_CREATE_HANDLE = 26000,	// 创建红外活体检测句柄失败
        CW_NIRLIV_ERR_FREE_HANDLE,				// 释放红外活体检测句柄失败
        CW_NIRLIV_ERR_FACE_PAIR,	            // 人脸匹配初始化失败
        CW_NIRLIV_ERR_CREAT_LOG_DIR,	        // 创建日志路径失败
        CW_NIRLIV_ERR_MODEL_NOTEXIST,			// 输入模型不存在
        CW_NIRLIV_ERR_MODEL_FAILED,			    // 输入模型初始化失败
        CW_NIRLIV_ERR_INPUT_UNINIT,			    // 输入未初始化
        CW_NIRLIV_ERR_NIR_NO_FACE,				// 输入红外图片没有人脸
        CW_NIRLIV_ERR_VIS_NO_FACE,				// 输入可见光图片没有人脸
        CW_NIRLIV_ERR_NO_PAIR_FACE,			    // 输入可见光和红外图片人脸未能匹配
        CW_NIRLIV_ERR_PUSH_DATA,		        // 输入数据失败
        CW_NIRLIV_ERR_NUM_LANDMARKS,			// 输入可见光图片和红外图片关键点个数不等
        CW_NIRLIV_ERR_NO_LANDMARKS,			    // 输入红外图片没有人脸关键点
        CW_NIRLIV_ERR_INPUT_IMAGE,		        // 输入红外图片或者可见光图片不是多通道
        CW_NIRLIV_ERR_UNAUTHORIZED,			    // 没有license（未授权）
        CW_NIRLIV_ERR_FACE_NUM_ERR,			    // 未开启人脸匹配开关时，可见光或红外图像人脸大于1
        CW_NIRLIV_ERR_CAM_UNCW,		            // 非云从定制摄像头
        CW_NIRLIV_ERR_UNKNOWN,					// 未知结果
        CW_NIRLIV_ERR_MAXHANDLE,			    // 超过最大红外活体最大授权句柄数
        CW_NIRLIV_ERR_NIRIMAGE,			        // 输入红外图片数据错误
        CW_NIRLIV_ERR_VISIMAGE,			        // 输入可见光图片数据错误
    }

    /***************
     * 红外活体检测结果
     */
    public struct cw_nirliv_res_t
    {
        public cw_nirliv_det_rst_t livRst;			         // 输出红外活体检测结果返回值
        public float score;			                         // 输出红外活体检测得分，非活体的时候为0
    }



}
