using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace Mijin.Library.App.Model.Setting
{
    /// <summary>
    /// 图书馆设置
    /// </summary>
    public class LibrarySettings
    {
        /// <summary>
        /// 如果存在设置文件，则读设置文件
        /// </summary>
        public LibrarySettings()
        {

        }

        /// <summary>
        /// 自助借阅支持类型
        /// </summary>
        /// <value></value>
        public List<AutoLendSupport> AutoLendSupports { get; set; } = null;

        /// <summary>
        /// 通道的Ips
        /// </summary>
        /// <value></value>
        public List<string> DoorIps { get; set; } = null;

        /// <summary>
        /// 自助借阅机是否支持小票打印
        /// </summary>
        /// <value></value>
        public bool PosPrintEnabled { get; set; } = false;
        /// <summary>
        /// IC卡设置
        /// </summary>
        /// <value></value>
        public IcSettings IcSettings { get; set; }

        /// <summary>
        /// 是否开启人脸注册页面
        /// </summary>
        /// <value></value>
        public bool FaceRegiestEnabled { get; set; } = false;

        /// <summary>
        /// 人脸办证是否必须包含人脸
        /// </summary>
        /// <value></value>
        public bool FaceRegiestNeedFace { get; set; } = false;

        /// <summary>
        /// 人脸比对阈值(最大99)
        /// </summary>
        /// <value></value>
        public int FaceMinValid { get; set; } = 70;

    }

    /// <summary>
    /// 高频卡设置
    /// </summary>
    public class IcSettings
    {
        /// <summary>
        /// 高频卡读取长度
        /// </summary>
        public int ICLength { get; set; } = 0;

        /// <summary>
        /// IC卡号不足ICLength补字符的位置
        /// </summary>
        public DirectionEnum ICAddDirection { get; set; }

        /// <summary>
        /// IC卡号不足ICLength补的字符
        /// </summary>
        public string ICAddStr { get; set; }

        /// <summary>
        /// 添加字符
        /// </summary>
        /// <param name="icSettings">高频卡参数设置</param>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string DataHandle(string val, IcSettings icSettings)
        {
            if (val.IsEmpty() || icSettings.IsNull() || icSettings.ICAddStr.IsEmpty() || icSettings.ICLength <= val.Length)
            {
                return val;
            }
            return @$"{(icSettings.ICAddDirection == DirectionEnum.left ? icSettings.ICAddStr : "")}{val}{(icSettings.ICAddDirection == DirectionEnum.right ? icSettings.ICAddStr : "")}";
        }
    }

    /// <summary>
    /// 自助借阅支持
    /// </summary>
    public enum AutoLendSupport
    {
        // 高频卡
        HFCard = 1,
        // 身份证
        IdentityCard = 2,
        // 人脸识别
        Face = 3
    }

    /// <summary>
    /// IC卡号不足ICLength补字符位置枚举
    /// </summary>
    public enum DirectionEnum
    {
        /// <summary>
        /// 左边
        /// </summary>
        left = 1,
        /// <summary>
        /// 右边
        /// </summary>
        right = 2
    }
}
