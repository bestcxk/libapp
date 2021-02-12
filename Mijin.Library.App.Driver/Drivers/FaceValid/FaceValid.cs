using Mijin.Library.App.Driver.Drivers.FaceValid.SDK;
using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Util;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// 1.需要使用到opencv，需要安装c++常用库
    /// 2. 仅可在windows平台下运行，可支持32 64,拷贝不同的SDK即可
    /// 3.release debug 模式都可以
    /// </summary>
    public class FaceValid : IFaceValid
    {
        private int m_iShrelhod = 85;               // 比对阈值

        private string m_sLicence = "";             // 云从授权licence
        private IntPtr m_pDet = IntPtr.Zero;        // 检测句柄，用于检测线程
        private IntPtr m_pDetVerify = IntPtr.Zero;  // 检测句柄，用于主线程，比对时人脸检测
        private IntPtr m_pRecog = IntPtr.Zero;      // 识别句柄，用于主线程，人脸比对

        private IntPtr m_pAgeAttri = IntPtr.Zero;      // 年龄属性句柄，用于主线程
        private IntPtr m_pGenderAttri = IntPtr.Zero;   // 性别属性句柄，用于主线程
        private IntPtr m_pRaceAttri = IntPtr.Zero;     // 人种属性句柄，用于主线程

        private IntPtr m_pNirLive = IntPtr.Zero;     // 红外活体句柄，用于主线程

        private IntPtr m_buffVerify = IntPtr.Zero;  // 人脸比对时保存结果的指针，保证内存只分配一次


        private string exePath = "";
        public bool isInit = false; // 是否已经初始化

        public FaceValid()
        {
            exePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
        }
        #region 析构函数
        ~FaceValid()
        {
            if (IntPtr.Zero != m_pDet)
            {
                NativeCWFaceDetection.cwReleaseDetHandle(m_pDet);
                m_pDet = IntPtr.Zero;
            }

            if (IntPtr.Zero != m_pDetVerify)
            {
                NativeCWFaceDetection.cwReleaseDetHandle(m_pDetVerify);
                m_pDetVerify = IntPtr.Zero;
            }

            if (IntPtr.Zero != m_pRecog)
            {
                NativeCWFaceRecognition.cwReleaseRecogHandle(m_pRecog);
                m_pRecog = IntPtr.Zero;
            }

            DestroyAttributeHandle();
        }
        #endregion

        #region 设置人脸比对阈值(SetShrelhod)
        /// <summary>
        /// 设置人脸比对阈值
        /// </summary>
        public MessageModel<bool> SetShrelhod(string num)
        {
            m_iShrelhod = num.ToInt();
            return new MessageModel<bool>()
            {
                success = true,
                msg = "设置人脸比对阈值成功"
            };

        }
        #endregion

        #region 初始化(Init)
        /// <summary>
        /// 初始化
        /// </summary>
        public MessageModel<bool> Init()
        {
            var result = new MessageModel<bool>() { msg = "初始化人脸识别模块失败" };
            // 获取配置文件中的人脸配置
            //m_iShrelhod = Appsettings.app(new string[] { "Face","Shrelhod" }).ObjToInt();

            //if (!File.Exists("libCWFaceSDK.dll"))
            //{
            //    throw new Exception("不存在人脸识别dll");
            //}
            if (isInit || IntPtr.Zero != m_pDet && IntPtr.Zero != m_pRecog)
            {
                Console.WriteLine("已初始化过，不再初始化");
                result.msg = "已初始化过，不再初始化";
                result.success = true;
                return result;
            }

            if (m_iShrelhod <= 0)
            {
                m_iShrelhod = 85;
            }
            Console.WriteLine("进入到Init");

            //初始化模块
            StringBuilder sbVersion = new StringBuilder(100);
            NativeCWFaceVersion.cwGetSDKVersion(sbVersion, 100);


            // 32位和64位检测模型不能共用，64位用_configs_frontend.xml，非64位用_configs_frontend_x86_arm.xml
            string sModelXmlPath;
            if (Environment.Is64BitProcess)
            {
                //sModelXmlPath = @"CWModels\_configs_frontend.xml";
                sModelXmlPath = Path.Combine(exePath, @"CWModels\_configs_frontend.xml");
            }
            else
            {
                sModelXmlPath = Path.Combine(exePath, @"CWModels\_configs_frontend_x86.xml");
                //sModelXmlPath = @"CWModels\_configs_frontend_x86.xml";
            }

            cw_errcode_t errCode = cw_errcode_t.CW_OK;
            m_pDet = NativeCWFaceDetection.cwCreateDetHandle(out errCode, sModelXmlPath, m_sLicence);
            if (IntPtr.Zero == m_pDet || errCode != cw_errcode_t.CW_OK)
            {
                Console.WriteLine("检测句柄1创建失败，错误码：" + errCode.ToString());
                result.devMsg = "检测句柄1创建失败，错误码：" + errCode.ToString();
                return result;
            }
            cw_det_param_t param;
            NativeCWFaceDetection.cwGetFaceParam(m_pDet, out param);
            param.minSize = 48;
            param.maxSize = 600;
            param.pConfigFile = sModelXmlPath;    // 设置接口功能参数
            NativeCWFaceDetection.cwSetFaceParam(m_pDet, ref param);

            m_pDetVerify = NativeCWFaceDetection.cwCreateDetHandle(out errCode, sModelXmlPath, m_sLicence);
            if (IntPtr.Zero == m_pDetVerify || errCode != cw_errcode_t.CW_OK)
            {
                Console.WriteLine("检测句柄2创建失败，错误码：" + errCode.ToString());
                result.devMsg = "检测句柄2创建失败，错误码：" + errCode.ToString();
                return result;
            }
            NativeCWFaceDetection.cwGetFaceParam(m_pDetVerify, out param);
            param.minSize = 30;
            param.maxSize = 600;
            param.pConfigFile = sModelXmlPath;    // 设置接口功能参数
            NativeCWFaceDetection.cwSetFaceParam(m_pDetVerify, ref param);

            //     NativeCWFaceDetection.cwSetFaceBufOrder(m_pDetVerify, 1);

            //提取特征
            m_pRecog = NativeCWFaceRecognition.cwCreateRecogHandle(out errCode, Path.Combine(exePath, @"CWModels\CWR_Config3.0_1_1.xml"), m_sLicence, cw_recog_pattern_t.CW_FEATURE_EXTRACT);

            if (IntPtr.Zero == m_pRecog || errCode != cw_errcode_t.CW_OK)
            {
                Console.WriteLine("识别句柄创建失败，错误码：" + errCode.ToString());
                result.devMsg = "识别句柄创建失败，错误码：" + errCode.ToString();
                return result;
            }

            errCode = CreateAttributeHandle();
            if (errCode != cw_errcode_t.CW_OK)
            {
                result.devMsg = "识别句柄创建失败，错误码：" + errCode.ToString();
                return result;
            }

            cw_nirliveness_err_t errCodeNir = CreateNirLivenessHandle();
            if (errCodeNir == cw_nirliveness_err_t.CW_NIRLIV_OK)
            {
                Console.WriteLine("初始化成功");
                isInit = true;
                result.success = true;
                result.msg = "初始化成功";
            }
            return result;
        }
        #endregion

        #region 获取脸部特征 字符串(GetFeatureStr)
        /// <summary>
        /// 获取脸部特征 字符串
        /// </summary>
        /// <param name="picBytes">图片bytes  File.ReadAllBytes </param>
        /// <returns></returns>
        public MessageModel<string> GetFeatureStr(List<byte> picBytes)
        {
            var feature = GetFeatureByte(picBytes);
            var result = new MessageModel<string>(feature);

            if (result.success)
                result.response = System.Convert.ToBase64String(feature.response);

            return result;
        }

        #endregion

        #region 获取脸部特征(GetFeatureByte)
        /// <summary>
        /// 获取脸部特征 bytes
        /// </summary>
        /// <param name="picBytes">图片bytes  File.ReadAllBytes </param>
        /// <returns></returns>
        public MessageModel<byte[]> GetFeatureByte(List<byte> picBytes)
        {
            var result = new MessageModel<byte[]>() { msg = "获取人脸特征失败" };
            if (!isInit)
            {
                var res = Init();
                if (!res.success)
                {
                    return new MessageModel<byte[]>(res);
                }
            }


            // 获取人脸bytes长度
            int iFeaLen = NativeCWFaceRecognition.cwGetFeatureLength(m_pRecog);
            // 分配内存空间
            byte[] pFea1 = new byte[iFeaLen];

            // 给图像结构体赋值
            cw_img_t srcImg = new cw_img_t();
            srcImg.data = Marshal.UnsafeAddrOfPinnedArrayElement(picBytes.ToArray(), 0);
            srcImg.dataLen = picBytes.Count;
            srcImg.height = 0;
            srcImg.width = 0;
            srcImg.format = cw_img_form_t.CW_IMAGE_BINARY;
            srcImg.angle = cw_img_angle_t.CW_IMAGE_ANGLE_0;
            srcImg.mirror = cw_img_mirror_t.CW_IMAGE_MIRROR_NONE;

            int iSize = Marshal.SizeOf(typeof(cw_face_res_t));
            if (IntPtr.Zero == m_buffVerify)
            {
                m_buffVerify = Marshal.AllocHGlobal(2 * iSize);
                if (IntPtr.Zero == m_buffVerify)
                {
                    result.devMsg = "IntPtr.Zero == m_buffVerify";
                    return result;
                }
            }

            int iFaceNum = 0;
            // 人脸检测，获取对齐人脸
            cw_errcode_t errCode = NativeCWFaceDetection.cwFaceDetection(m_pDetVerify, ref srcImg, m_buffVerify, 2, ref iFaceNum,
                (int)(DetectTrackOperationType.CW_OP_DET | DetectTrackOperationType.CW_OP_ALIGN));

            if (errCode != cw_errcode_t.CW_OK)
            {
                result.devMsg = "Face detect Error, Code: " + errCode.ToString();
                return result;
            }
            if (iFaceNum < 1)
            {
                result.msg = "图片中不包含人脸!";
                return result;
            }

            // 取第一张人脸的数据
            cw_face_res_t faceRect = new cw_face_res_t();
            faceRect = (cw_face_res_t)Marshal.PtrToStructure(m_buffVerify, typeof(cw_face_res_t));
            errCode = NativeCWFaceRecognition.cwGetFaceFeature(m_pRecog, ref faceRect.faceAligned, pFea1);

            if (cw_errcode_t.CW_OK != errCode)
            {
                result.devMsg = "Get Feature Error: " + errCode.ToString();
                return result;
            }

            // 人脸byte[]转字符串

            result.msg = "获取人脸特征成功";
            result.success = true;
            result.response = pFea1;

            return result;
        }
        #endregion

        #region 在特征List中查找相似度高于一定值的特征(FeatureMatch)
        /// <summary>
        /// 在特征List中查找相似度高于一定值的特征
        /// 遍历特征List取最高，若某个特征值高于95则直接返回
        /// </summary>
        /// <param name="featureFaces">特征List string</param>
        /// <param name="facePic">比对的照片byte File.ReadAllBytes</param>
        /// <returns>返回最符合特征的index(featureFaces中的index)</returns>
        public MessageModel<string> FeatureMatch(List<string> featureFaces, List<byte> facePic)
        {
            var data = new MessageModel<string>();
            if (!isInit)
            {
                isInit = Init().success;
            }

            // 获取特征byte长度
            var len = NativeCWFaceRecognition.cwGetFeatureLength(m_pRecog);

            // featureFaces的byte合集
            byte[] faces = new byte[0];

            if (featureFaces == null || featureFaces.Count == 0 || facePic.Count == 0)
            {
                data.msg = "比对特征失败，传入数据为空";
                return data;
            }

            // 分配空间
            byte[] face = new byte[len];

            // 获取需要比对的脸部特征
            face = GetFeatureByte(facePic).response;

            float[] pScores = new float[featureFaces.Count];

            //循环对比
            for (int i = 0; i < featureFaces.Count; i++)
            {
                if (featureFaces[i] == null || featureFaces[i].Length <= 1000)
                {
                    continue;
                }

                var bytes = System.Convert.FromBase64String(featureFaces[i]);

                float[] soc = new float[1];

                cw_errcode_t errCode = NativeCWFaceRecognition.cwComputeMatchScore(m_pRecog, bytes, face, 1, soc);
                pScores[i] = soc[0];

                // 若当前分数高于95则直接返回
                if (soc[0] * 100 >= 95)
                {
                    data.response = i.ToString();
                    data.msg = @$"比对分数:{soc[0] * 100}";
                    data.success = true;
                    return data;
                }


            }

            float score = pScores.Max() * 100;

            if (score <= m_iShrelhod)
            {
                data.msg = "未包含匹配项";
                return data;
            }

            var maxIndex = MaxIndex(pScores);
            data.response = maxIndex.ToString();
            data.success = true;
            data.devMsg = "FeatureMatch-完全遍历";
            data.msg = @$"比对分数:{score}";
            return data;
        }
        #endregion



        /// <summary>
        /// 传入一个数组,求出一个数组的最大值的位置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr"></param>
        /// <returns></returns>
        private static int MaxIndex<T>(T[] arr) where T : IComparable<T>
        {
            var i_Pos = 0;
            var value = arr[0];
            for (var i = 1; i < arr.Length; ++i)
            {
                var _value = arr[i];
                if (_value.CompareTo(value) > 0)
                {
                    value = _value;
                    i_Pos = i;
                }
            }
            return i_Pos;
        }

        /// <summary>
        /// 销毁人种属性句柄
        /// </summary>
        private void DestroyAttributeHandle()
        {
            if (IntPtr.Zero != m_pRaceAttri)
            {
                NativeCWFaceAttribute.cwReleaseAttributeHandle(m_pRaceAttri);
                m_pRaceAttri = IntPtr.Zero;
            }

            if (IntPtr.Zero != m_pAgeAttri)
            {
                NativeCWFaceAttribute.cwReleaseAttributeHandle(m_pAgeAttri);
                m_pAgeAttri = IntPtr.Zero;
            }

            if (IntPtr.Zero != m_pGenderAttri)
            {
                NativeCWFaceAttribute.cwReleaseAttributeHandle(m_pGenderAttri);
                m_pGenderAttri = IntPtr.Zero;
            }
        }


        /****************************************************人脸属性************************************************************/
        private cw_errcode_t CreateAttributeHandle()
        {
            //创建属性句柄
            cw_errcode_t errCode = cw_errcode_t.CW_OK;
            m_pRaceAttri = NativeCWFaceAttribute.cwCreateAttributeHandle(out errCode, Path.Combine(exePath, @"CWModels\attribute\faceRace\cw_race_config.xml"), m_sLicence);
            if (IntPtr.Zero == m_pRaceAttri || errCode != cw_errcode_t.CW_OK)
            {
                Console.WriteLine("人种句柄创建失败，错误码：" + errCode.ToString());
                return errCode;
            }

            m_pAgeAttri = NativeCWFaceAttribute.cwCreateAttributeHandle(out errCode, Path.Combine(exePath, @"CWModels\attribute\ageGroup\cw_age_group_config.xml"), m_sLicence);
            if (IntPtr.Zero == m_pAgeAttri || errCode != cw_errcode_t.CW_OK)
            {
                Console.WriteLine("年龄段句柄创建失败，错误码：" + errCode.ToString());
                return errCode;
            }

            m_pGenderAttri = NativeCWFaceAttribute.cwCreateAttributeHandle(out errCode, Path.Combine(exePath, @"CWModels\attribute\faceGender\cw_gender_config.xml"), m_sLicence);
            if (IntPtr.Zero == m_pGenderAttri || errCode != cw_errcode_t.CW_OK)
            {
                Console.WriteLine("性别句柄创建失败，错误码：" + errCode.ToString());
                return errCode;
            }

            return errCode;
        }

        /*****************************************************红外活体************************************************************/
        private cw_nirliveness_err_t CreateNirLivenessHandle()
        {
            //创建红外活体句柄
            cw_nirliveness_err_t errCode = cw_nirliveness_err_t.CW_NIRLIV_OK;

            string pNirModelPath = Path.Combine(exePath, @"CWModels\nirLiveness_model_20181102_pc.bin");
            string pRecogModelPath = Path.Combine(exePath, @"CWModels\hd171019.bin");
            string pPairFilePath = Path.Combine(exePath, @"CWModels\matrix_para640x480.xml");
            float fskinThread = 0.35f;
            m_pNirLive = CWFaceNisLiveness.cwCreateNirLivenessHandle(out errCode, pNirModelPath, pRecogModelPath, pPairFilePath, Path.Combine(exePath, @"faceLog"), fskinThread, m_sLicence);
            if (IntPtr.Zero == m_pNirLive || errCode != cw_nirliveness_err_t.CW_NIRLIV_OK)
            {
                Console.WriteLine("红外活体句柄创建失败，错误码：" + errCode.ToString());
                return errCode;
            }
            return errCode;
        }





    }
}
