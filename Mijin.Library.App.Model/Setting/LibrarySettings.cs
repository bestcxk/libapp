using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bing.Extensions;
using IsUtil;
using IsUtil.Helpers;

namespace Mijin.Library.App.Model.Setting
{
    /// <summary>
    /// 图书馆设置
    /// </summary>
    public class LibrarySettings
    {
        /// <summary>
        /// 每个客户端参数设置
        /// </summary>
        /// <value></value>
        public List<ClientSetting> Clients { get; set; }



        /// <summary>
        /// 通道的Ips
        /// </summary>
        /// <value></value>
        public List<DoorSetting> DoorSettings { get; set; } = null;

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


    public class ClientSetting
    {
        /// <summary>
        /// 设备Id
        /// </summary>
        /// <value></value>
        public string Id { get; set; }
        /// <summary>
        /// 自助借阅支持类型
        /// </summary>
        /// <value></value>
        public List<DriverSupport> Supports { get; set; } = null;

        /// <summary>
        /// RFID设备com口
        /// </summary>
        /// <value></value>
        public string RfidCom { get; set; }

        /// <summary>
        /// 身份证设备com口
        /// </summary>
        /// <value></value>
        public string IdentityReaderCom { get; set; }

        /// <summary>
        /// 读高频卡使用身份证？ 
        /// </summary>
        /// <value></value>
        public bool ReadCardUseByIdentity { get; set; } = false;

        

    }

    public class DoorSetting
    {
        public int InGpi { get; set; }
        public int OutGpi { get; set; }
        /// <summary>
        /// ip:port
        /// </summary>
        public string Address { get; set; }

        public string Description { get; set; }
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
    public enum DriverSupport
    {
        //高频卡
        HFCard = 1,
        // 身份证
        IdentityCard = 2,
        // 人脸识别
        Face = 3,
        // 小票打印机
        PosPrint = 4
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
