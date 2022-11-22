using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using Mijin.Library.App.Driver.Interface;
using Mijin.Library.App.Model;
using Mijin.Library.App.Model.Setting;

namespace Mijin.Library.App.Driver.Drivers.Reader;

public class MiniBlackHFReader : IHFReader
{
    public ISystemFunc _systemFunc { get; }

    private bool isConnect { get; set; }

    public MiniBlackHFReader(ISystemFunc systemFunc)
    {
        _systemFunc = systemFunc;
    }

    #region DllImport

    [DllImport("Device.dll", EntryPoint = "JA_Init")]
    public static extern int JA_Init(int port, int band);

    [DllImport("Device.dll", EntryPoint = "JA_Exit")]
    public static extern void JA_Exit();

    [DllImport("Device.dll", EntryPoint = "JA_Version")]
    public static extern int JA_Version(byte[] DataBuf, byte[] DataLen);

    [DllImport("Device.dll", EntryPoint = "JA_ActiveCard")]
    public static extern int JA_ActiveCard(byte ReqCode, byte[] TagType, byte[] Sak, byte[] SnrLen, byte[] Snr);

    [DllImport("Device.dll", EntryPoint = "JA_Auth")]
    public static extern int JA_Auth(byte Sector, byte KeyAB, byte[] Key);

    [DllImport("Device.dll", EntryPoint = "JA_Read")]
    public static extern int JA_Read(byte Block, byte[] Data);

    [DllImport("Device.dll", EntryPoint = "JA_Write")]
    public static extern int JA_Write(byte Block, byte[] Data);

    [DllImport("Device.dll", EntryPoint = "JA_Transfer")]
    public static extern int JA_Transfer(byte Block);

    [DllImport("Device.dll", EntryPoint = "JA_Halt")]
    public static extern int JA_Halt();

    [DllImport("Device.dll", EntryPoint = "JA_Auth2")]
    public static extern int JA_Auth2(byte Key_Sector, byte KeyAB);

    [DllImport("Device.dll", EntryPoint = "JA_LoadKey")]
    public static extern int JA_LoadKey(byte Key_Sector, byte KeyAB, byte[] Key);

    [DllImport("Device.dll", EntryPoint = "JA_WriteUserData")]
    public static extern int JA_WriteUserData(byte OffsetAddr, byte DataLen, byte[] DataBuf);

    [DllImport("Device.dll", EntryPoint = "JA_ReadUserData")]
    public static extern int JA_ReadUserData(byte OffsetAddr, byte DataLen, byte[] DataBuf);

    [DllImport("Device.dll", EntryPoint = "JA_SetAddress")]
    public static extern int JA_SetAddress(byte Address);

    [DllImport("Device.dll", EntryPoint = "JA_GetAddress")]
    public static extern int JA_GetAddress(byte[] Address);

    [DllImport("Device.dll", EntryPoint = "JA_SetBand")]
    public static extern int JA_SetBand(byte Band);

    [DllImport("Device.dll", EntryPoint = "JA_SetBuzzer")]
    public static extern int JA_SetBuzzer(byte Worktime_10ms, byte Intervaltime_10ms, byte Count);

    [DllImport("Device.dll", EntryPoint = "JA_SetIO")]
    public static extern int JA_SetIO(byte number, byte flag);

    [DllImport("Device.dll", EntryPoint = "JA_SetReminder")]
    public static extern int JA_SetReminder(byte flag, byte sw);

    [DllImport("Device.dll", EntryPoint = "JA_SetWorkMode")]
    public static extern int JA_SetWorkMode(byte[] Mode);

    [DllImport("Device.dll", EntryPoint = "JA_GetWorkMode")]
    public static extern int JA_GetWorkMode(byte[] Mode);

    [DllImport("Device.dll", EntryPoint = "JA_CPU_Rats")]
    public static extern int JA_CPU_Rats(byte[] DataLen, byte[] Data);

    [DllImport("Device.dll", EntryPoint = "JA_CPU_Command")]
    public static extern int JA_CPU_Command(byte[] SendBuf, byte SendLen, byte[] RecvBuf, byte[] RecvLen);

    [DllImport("Device.dll", EntryPoint = "JA_PSAM_Rats")]
    public static extern int JA_PSAM_Rats(byte[] DataLen, byte[] Data);

    [DllImport("Device.dll", EntryPoint = "JA_PSAM_Command")]
    public static extern int JA_PSAM_Command(byte[] SendBuf, byte SendLen, byte[] RecvBuf, byte[] RecvLen);

    [DllImport("Device.dll", EntryPoint = "JA_ReadPage")]
    public static extern int JA_ReadPage(byte Page, byte[] Data);

    [DllImport("Device.dll", EntryPoint = "JA_WritePage")]
    public static extern int JA_WritePage(byte Page, byte[] Data);

    [DllImport("Device.dll", EntryPoint = "JA_ResetReader")]
    public static extern int JA_ResetReader();

    [DllImport("Device.dll", EntryPoint = "JA_DllVersion")]
    public static extern int JA_DllVersion(byte[] VerBuf);

    [DllImport("Device.dll", EntryPoint = "JA_CmdDebug")]
    public static extern int JA_CmdDebug(byte[] SendBuf, byte SendLen, byte[] RecvBuf, byte[] RecvLen);

    [DllImport("Device.dll", EntryPoint = "JA_Update_Start")]
    public static extern int JA_Update_Start(byte[] SendBuf, int len);

    [DllImport("Device.dll", EntryPoint = "JA_Update_Data")]
    public static extern int JA_Update_Data(byte[] SendBuf, int len);

    #endregion

    #region Init(初始化)

    /// <summary>
    /// 初始化, 自动找串口
    /// </summary>
    /// <returns></returns>
    public MessageModel<bool> Init()
    {
        if (isConnect)
        {
            return new MessageModel<bool>()
            {
                msg = "连接成功",
                response = true,
                success = true
            };
        }
            
        //获取所有串口号
        var coms = SerialPort.GetPortNames().Select(s => int.Parse(s[3..])).ToList();

        foreach (var com in coms)
        {
            var res = Inited(com);
            if (!res.success) continue;

            isConnect = true;
            return res;
        }

        return new MessageModel<bool>
        {
            msg = "串口无效或者已被占用"
        };

    }

    /// <summary>
    /// 初始化, 需要指定串口和波特率
    /// </summary>
    /// <param name="com">串口号</param>
    /// <param name="baud">波特率</param>
    /// <returns></returns>
    private MessageModel<bool> Inited(int com, int baud = 115200)
    {
        var res = new MessageModel<bool>();

        var isConnect = JA_Init(com, baud);

        if (isConnect != 0)
        {
            res.msg = "串口无效或者已被占用";
            return res;
        }

        var version = JA_Version(new byte[1], new byte[1]);

        if (version != 0)
        {
            res.msg = "串口无效或者已被占用";
            return res;
        }

        res.msg = "连接成功";
        res.response = true;
        res.success = true;

        return res;
    }

    #endregion

    /// <summary>
    /// 读块
    /// </summary>
    /// <param name="sector">扇区</param>
    /// <param name="block">块</param>
    /// <param name="HexKey">密钥</param>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public MessageModel<string> ReadBlock(long sector, long block, string HexKey = "FFFFFFFFFFFF")
    {
        var res = new MessageModel<string>();

        var isConnect = Init();

        if (!isConnect.success)
        {
            res.msg = "设备未连接";
            return res;
        }

        //先验证密码再读块
        byte[] buf = new byte[6];
        byte[] bytetmp = new byte[255];

        bytetmp = Encoding.Default.GetBytes(HexKey);
        for (int i = 0; i < HexKey.Length; i++)
        {
            if ((bytetmp[i] >= '0') && (bytetmp[i] <= '9'))
            {
                bytetmp[i] = (byte)(bytetmp[i] - '0');
            }
            else if ((bytetmp[i] >= 'A') && (bytetmp[i] <= 'F'))
            {
                bytetmp[i] = (byte)(bytetmp[i] - 'A' + 10);
            }
            else
            {
                res.msg = "输入的数据格式有误";
                return res;
            }
        }
        for (int i = 0; i < 6; i++)
        {
            buf[i] = (byte)((bytetmp[2 * i] << 4) + bytetmp[2 * i + 1]);
        }

        var isAuth = JA_Auth((byte)sector, 0, buf);
        if (isAuth != 0)
        {
            res.msg = "验证密钥失败";
            return res;
        }

        //读块
        var data = new byte[255];
        var readBlock = JA_Read((byte)block, data);
        if (readBlock != 0)
        {
            res.msg = "读数据失败";
            return res;
        }

        var str = string.Empty;
        byte[] rdata = new byte[16];
        Array.Copy(data, 0, rdata, 0, 16);
        for (int i = 0; i < 16; i++)
        {
            str += rdata[i].ToString("X2");
        }

        if (string.IsNullOrWhiteSpace(str))
        {
            res.msg = "读数据失败";
            return res;
        }

        res.response = str;
        res.msg = "读数据成功";
        res.success = true;

        return res;
    }


    /// <summary>
    /// 读卡号
    /// </summary>
    /// <returns></returns>
    public MessageModel<string> ReadCardNo()
    {
        var res = new MessageModel<string>();

        var isConnect = Init();

        if (!isConnect.success)
        {
            res.msg = "设备未连接";
            return res;
        }

        var tagType = new byte[2];
        var sak = new byte[1];
        var snrLen = new byte[1];
        var snr = new byte[7];

        var isRead = JA_ActiveCard(0, tagType, sak, snrLen, snr);

        if (isRead != 0)
        {
            res.msg = "读取卡号失败";
            return res;
        }

        var uid = string.Empty;
        var uidBytes = new byte[snrLen[0]];
        for (var i = 0; i < snrLen[0]; i++)
        {
            uidBytes[i] = snr[i];
            uid += uidBytes[i].ToString("X2");
        }

        if (!string.IsNullOrWhiteSpace(uid))
        {
            res.response = _systemFunc.ClientSettings.HFOriginalCard ? uid : IcSettings.DataHandle(Convert.ToInt64(uid, 16).ToString(), _systemFunc.LibrarySettings?.IcSettings);
            res.msg = "读取卡号成功";
            res.success = true;
            return res;
        }

        res.msg = "读取卡号失败";
        return res;
    }


}