using Mijin.Library.App.Model;
using Mijin.Library.App.Model.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Util;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// 万特身份证/高频卡 读卡器
    /// </summary>
    public class WonteReader : IdentityReader
    {
        private int port = 0;

        public const int cbDataSize = 128;

        public ISystemFunc _systemFunc { get; set; }

        // 读高频卡的com口
        private static int hfCom = 0;
        public WonteReader(ISystemFunc systemFunc)
        {
            _systemFunc = systemFunc;
        }

        #region identity DllImport
        [DllImport("termb.dll")]
        static extern int InitComm(int port);//连接身份证阅读器 

        [DllImport("termb.dll")]
        static extern int InitCommExt();//自动搜索身份证阅读器并连接身份证阅读器 

        [DllImport("termb.dll")]
        static extern int CloseComm();//断开与身份证阅读器连接 

        [DllImport("termb.dll")]
        static extern int Authenticate();//判断是否有放卡，且是否身份证 

        [DllImport("termb.dll")]
        public static extern int Read_Content(int index);//读卡操作,信息文件存储在dll所在下

        [DllImport("termb.dll")]
        public static extern int ReadContent(int index);//读卡操作,信息文件存储在dll所在下

        [DllImport("termb.dll")]
        static extern int GetSAMID(StringBuilder SAMID);//获取SAM模块编号

        [DllImport("termb.dll")]
        static extern int GetSAMIDEx(StringBuilder SAMID);//获取SAM模块编号（10位编号）

        [DllImport("termb.dll")]
        static extern int GetBmpPhoto(string PhotoPath);//解析身份证照片

        [DllImport("termb.dll")]
        static extern int GetBmpPhotoToMem(byte[] imageData, int cbImageData);//解析身份证照片

        [DllImport("termb.dll")]
        static extern int GetBmpPhotoExt();//解析身份证照片

        [DllImport("termb.dll")]
        static extern int Reset_SAM();//重置Sam模块

        [DllImport("termb.dll")]
        static extern int GetSAMStatus();//获取SAM模块状态 

        [DllImport("termb.dll")]
        static extern int GetCardInfo(int index, StringBuilder value);//解析身份证信息 

        [DllImport("termb.dll")]
        static extern int ExportCardImageV();//生成竖版身份证正反两面图片(输出目录：dll所在目录的cardv.jpg和SetCardJPGPathNameV指定路径)

        [DllImport("termb.dll")]
        static extern int ExportCardImageH();//生成横版身份证正反两面图片(输出目录：dll所在目录的cardh.jpg和SetCardJPGPathNameH指定路径) 

        [DllImport("termb.dll")]
        static extern int SetTempDir(string DirPath);//设置生成文件临时目录

        [DllImport("termb.dll")]
        static extern int GetTempDir(StringBuilder path, int cbPath);//获取文件生成临时目录

        [DllImport("termb.dll")]
        static extern void GetPhotoJPGPathName(StringBuilder path, int cbPath);//获取jpg头像全路径名 


        [DllImport("termb.dll")]
        static extern int SetPhotoJPGPathName(string path);//设置jpg头像全路径名

        [DllImport("termb.dll")]
        static extern int SetCardJPGPathNameV(string path);//设置竖版身份证正反两面图片全路径

        [DllImport("termb.dll")]
        static extern int GetCardJPGPathNameV(StringBuilder path, int cbPath);//获取竖版身份证正反两面图片全路径

        [DllImport("termb.dll")]
        static extern int SetCardJPGPathNameH(string path);//设置横版身份证正反两面图片全路径

        [DllImport("termb.dll")]
        static extern int GetCardJPGPathNameH(StringBuilder path, int cbPath);//获取横版身份证正反两面图片全路径

        [DllImport("termb.dll")]
        static extern int getName(StringBuilder data, int cbData);//获取姓名

        [DllImport("termb.dll")]
        static extern int getSex(StringBuilder data, int cbData);//获取性别

        [DllImport("termb.dll")]
        static extern int getNation(StringBuilder data, int cbData);//获取民族

        [DllImport("termb.dll")]
        static extern int getBirthdate(StringBuilder data, int cbData);//获取生日(YYYYMMDD)

        [DllImport("termb.dll")]
        static extern int getAddress(StringBuilder data, int cbData);//获取地址

        [DllImport("termb.dll")]
        static extern int getIDNum(StringBuilder data, int cbData);//获取身份证号

        [DllImport("termb.dll")]
        static extern int getIssue(StringBuilder data, int cbData);//获取签发机关

        [DllImport("termb.dll")]
        static extern int getEffectedDate(StringBuilder data, int cbData);//获取有效期起始日期(YYYYMMDD)

        [DllImport("termb.dll")]
        static extern int getExpiredDate(StringBuilder data, int cbData);//获取有效期截止日期(YYYYMMDD) 

        [DllImport("termb.dll")]
        static extern int getBMPPhotoBase64(StringBuilder data, int cbData);//获取BMP头像Base64编码 

        [DllImport("termb.dll")]
        static extern int getJPGPhotoBase64(StringBuilder data, int cbData);//获取JPG头像Base64编码

        [DllImport("termb.dll")]
        static extern int getJPGCardBase64V(StringBuilder data, int cbData);//获取竖版身份证正反两面JPG图像base64编码字符串

        [DllImport("termb.dll")]
        static extern int getJPGCardBase64H(StringBuilder data, int cbData);//获取横版身份证正反两面JPG图像base64编码字符串

        [DllImport("termb.dll")]
        static extern int HIDVoice(int nVoice);//语音提示。。仅适用于与带HID语音设备的身份证阅读器（如ID200）

        [DllImport("termb.dll")]
        static extern int IC_SetDevNum(int iPort, StringBuilder data, int cbdata);//设置发卡器序列号

        [DllImport("termb.dll")]
        static extern int IC_GetDevNum(int iPort, StringBuilder data, int cbdata);//获取发卡器序列号

        [DllImport("termb.dll")]
        static extern int IC_GetDevVersion(int iPort, StringBuilder data, int cbdata);//设置发卡器序列号 

        [DllImport("termb.dll")]
        static extern int IC_WriteData(int iPort, int keyMode, int sector, int idx, StringBuilder key, StringBuilder data, int cbdata, ref uint snr);//写数据

        [DllImport("termb.dll")]
        static extern int IC_ReadData(int iPort, int keyMode, int sector, int idx, StringBuilder key, StringBuilder data, int cbdata, ref uint snr);//du数据

        [DllImport("termb.dll")]
        static extern int IC_GetICSnr(int iPort, ref uint snr);//读IC卡物理卡号 

        [DllImport("termb.dll")]
        static extern int IC_GetIDSnr(int iPort, StringBuilder data, int cbdata);//读身份证物理卡号 

        [DllImport("termb.dll")]
        static extern int getEnName(StringBuilder data, int cbdata);//获取英文名

        [DllImport("termb.dll")]
        static extern int getCnName(StringBuilder data, int cbdata);//获取中文名 

        [DllImport("termb.dll")]
        static extern int getPassNum(StringBuilder data, int cbdata);//获取港澳台居通行证号码

        [DllImport("termb.dll")]
        static extern int getVisaTimes();//获取签发次数

        [DllImport("termb.dll")]
        static extern int IC_ChangeSectorKey(int iPort, int keyMode, int nSector, StringBuilder oldKey, StringBuilder newKey);
        #endregion

        /// <summary>
        /// 读高频卡卡号
        /// </summary>
        /// <param name="com">com口，不填则自动寻找，读卡一次成功则直接保存com口号直到关闭软件</param>
        /// <returns></returns>
        public MessageModel<string> ReadHFCardNo(Int64? com = null)
        {
            var result = new MessageModel<string>();
            uint icNumber = 0;

            
            List<string> coms = null; // 当前存在的com口

            if (hfCom == 0) // 还没有一次读成功
            {
                if (!com.IsNull())
                {
                    if (IC_GetICSnr(com.ToInt(), ref icNumber) == 1)
                    {
                        hfCom = com.ToInt();
                    }
                }
                else
                {
                    coms = _systemFunc.GetComs().response?.ToList();
                    foreach (var c in coms)
                    {
                        int comNum = c.Replace("COM", "").ToInt();
                        if (IC_GetICSnr(comNum, ref icNumber) == 1)
                        {
                            hfCom = comNum;
                            break;
                        }
                    }
                }
                // 如果在连接时读卡并未成功，则直接返回读卡失败
                if (icNumber <= 0)
                {
                    result.msg = "未连接到读卡器";
                    result.devMsg = @$"已扫描的com口：{string.Join(",",coms)}";
                    return result;
                }

            }
            else
            {
                if (IC_GetICSnr(hfCom, ref icNumber) != 1)
                {
                    result.msg = "读IC卡物理卡号失败";
                    result.devMsg = "IC_GetICSnr Faild！";
                    return result;
                }
            }

            // 龙腾单独编译
            //{
            //    var card = Convert.ToString(icNumber, 16);
            //    var str = "";
            //    for (int i = 0; i < card.Length; i += 2)
            //    {
            //        string dt = card[i].ToString() + card[i + 1].ToString();
            //        str = str.Insert(0, dt);
            //    }
            //    result.response = str.ToUpper();
            //}

            result.response = IcSettings.DataHandle(icNumber.ToString(), _systemFunc.LibrarySettings?.IcSettings);

            result.success = true;
            result.msg = "读IC卡物理卡号成功";

            return result;

        }

        /// <summary>
        /// 读身份证信息
        /// </summary>
        public MessageModel<IdentityInfo> ReadIdentity()
        {
            var result = new MessageModel<IdentityInfo>();
            //未连接读卡器
            if (port == 0)
            {
                int AutoSearchReader = InitCommExt();
                if (AutoSearchReader <= 0)
                {
                    result.msg = "自动搜索连接读卡器失败";
                    return result;
                }
                port = AutoSearchReader;
            }
            int FindCard = Authenticate();

            if (FindCard != 1) //无卡或不是身份证
            {
                result.msg = "无卡或不是身份证 或 移开卡再重试";
                return result;
            }

            //读卡
            int rs = Read_Content(1);
            if (rs != 1 && rs != 2 && rs != 3)
            {
                result.msg = "读卡失败";
                return result;
            }

            IdentityInfo identityInfo = new IdentityInfo();

            StringBuilder sb = new StringBuilder(cbDataSize);

            //姓名
            getName(sb, cbDataSize);
            identityInfo.Name = sb.ToString();

            //民族/国家
            getNation(sb, cbDataSize);
            identityInfo.Country = sb.ToString();

            //性别
            getSex(sb, cbDataSize);
            identityInfo.Sex = sb.ToString();

            //出生
            getBirthdate(sb, cbDataSize);
            identityInfo.Birth = sb.ToString();

            //地址
            getAddress(sb, cbDataSize);
            identityInfo.Addr = sb.ToString();

            //身份证号
            getIDNum(sb, cbDataSize);
            identityInfo.Identity = sb.ToString();

            //显示头像
            GetBmpPhotoExt();
            int cbPhoto = 256 * 1024;
            StringBuilder sbPhoto = new StringBuilder(cbPhoto);
            int nRet = getBMPPhotoBase64(sbPhoto, cbPhoto);
            if (nRet == 1)
            {
                identityInfo.FacePicBase64 = sbPhoto.ToString();
            }

            result.response = identityInfo;
            result.success = true;
            result.msg = "读身份证成功";
            return result;

        }

    }
}
