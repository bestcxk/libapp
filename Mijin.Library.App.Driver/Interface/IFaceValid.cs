using Mijin.Library.App.Model;
using System.Collections.Generic;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// 人脸识别接口
    /// </summary>
    public interface IFaceValid
    {
        /// <summary>
        /// 在特征List中查找相似度高于一定值的特征
        /// 遍历特征List取最高，若某个特征值高于95则直接返回
        /// </summary>
        /// <param name="featureFaces">特征List string</param>
        /// <param name="facePic">比对的照片byte File.ReadAllBytes</param>
        /// <returns>返回最符合特征的index(featureFaces中的index)</returns>
        MessageModel<string> FeatureMatch(List<string> featureFaces, List<byte> facePic);
        /// <summary>
        /// 获取脸部特征 bytes
        /// </summary>
        /// <param name="picBytes">图片bytes</param>
        /// <returns></returns>
        MessageModel<byte[]> GetFeatureByte(List<byte> picBytes);
        /// <summary>
        /// 获取脸部特征 字符串
        /// </summary>
        /// <param name="picBytes">图片bytes  File.ReadAllBytes </param>
        /// <returns></returns>
        MessageModel<string> GetFeatureStr(List<byte> picBytes);
        /// <summary>
        /// 初始化
        /// </summary>
        MessageModel<bool> Init();
        /// <summary>
        /// 设置人脸比对阈值
        /// </summary>
        MessageModel<bool> SetShrelhod(string num);
    }
}