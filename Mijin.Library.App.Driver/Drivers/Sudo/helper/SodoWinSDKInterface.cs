using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver.Drivers.Sudo
{
    /// <summary>
    /// 返回值状态枚举
    /// </summary>
    public enum STATUS_CODE
    {
        //成功
        BASE_SUCCESS = 0x00010000,
        //失败
        BASE_FAIL = 0x00010001,

        //交易被拒绝
        TRADE_REJECT = 0x00010100,
        //交易被中止
        TRADE_ABORT = 0x00010101,
        //前笔交易冲正失败
        TRADE_REVERSAL_FAIL = 0x00010102,

        //服务器连接失败
        SERVER_CONNECT_FAIL = 0x00010200,
        //服务器超时
        SERVER_TIMEOUT,
        //发送服务数据失败
        SERVER_SEND_FAIL,
        //服务未注册初始化
        SERVER_UNREGISTER,

        //设备未注册初始化
        DEVICE_UNREGISTER = 0x00010300,
        //设备串口未打开
        DEVICE_NOTOPENED,
        //设备写失败
        DEVICE_WRITE_FAIL,
        //设备读失败
        DEVICE_READ_FAIL,
        //设备打开失败
        DEVICE_OPEN_ERR,
        //设备协议适配失败
        DEVICE_PROTOCOLADAPTER_FAIL,
        //指令超时
        COMMAND_TIMEOUT,

        //参数错误
        PARAMETER_ERR = 0x00010400,
        //金额参数错误
        PARAMETER_ERR_AMOUNT,
        //交易超时参数错误
        PARAMETER_ERR_FLASH_TIME,
    }

    /// <summary>
    /// 交易信息结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct TRADE_RECORD
    {
        //终端号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string termID;
        //商户号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string merchID;
        //卡号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string cardNumber;
        //交易日期
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string date;
        //交易流水号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string sn;
        //交易应答码
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string rcCode;
        //批次号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string batchNO;

        //检索参考号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 15)]
        public string refNO;

        //交易类型，00-脱机电子现金，01-小额免密联机消费
        [MarshalAs(UnmanagedType.I4)]
        public int tradeType;
        //交易金额
        [MarshalAs(UnmanagedType.I4)]
        public int amount;
        //折扣后金额
        [MarshalAs(UnmanagedType.I4)]
        public int discountAmount;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct TRADE_SFZ_OP_PARAM
    {
        public byte cmdop1;
        public byte cmdop2;

        //发送数据
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1024)]
        public char[] datasend;
        public int lensend;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4096)]
        public byte[] recvBuf;
        public int lenrecv;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct STR_TRANS_OP_PARAM
    {
        public byte transType;//交易类型，0x60-寻卡，40-消费
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] transDate;//交易日期，BCD编码
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] transTime;//交易时间，BCD编码
        public int transAmt; //交易金额

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] transNo;//交易流水号

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4096)]
        public byte[] transOutBuf;//交易返回的数据
        public int lenTransOutbuf;//交易返回的数据长度
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct STR_SB_INFO
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4096)]
        public byte[] recvBuf;//交易返回的数据SB信息
        public int lenrecv;//交易返回的数据长度
    }


    /// <summary>
    /// 卡信息结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CARD_INFO
    {
        //卡号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string cardNumber;
        //电子现金余额
        [MarshalAs(UnmanagedType.I4)]
        public int eBalance;
    }

    /// <summary>
    /// 撤销交易信息结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct TRANS_BACK_OUT_INFO
    {
        //卡号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string cardNumber;
        //流水号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string transNO;
        //消费金额
        [MarshalAs(UnmanagedType.I4)]
        public int eTransAmt;

        //服务器返回码
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string serCode;
    }

    class SodoWinSDKHandle
    {

        /// <summary>
        /// 初始化串口参数
        /// </summary>
        /// <param name="nPort">串口号，比如串口1，则nPort值为1</param>
        /// <param name="nBaud">波特率，例如115200、9600</param>  
        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_InitSerialPort", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern void Sodo_InitSerialPort(int nPort, int nBaud);


        /// <summary>
        /// 初始化中间件服务参数
        /// </summary>
        /// <param name="serverIP">服务器IP，例如"127.0.0.1"</param>
        /// <param name="serverPort">服务器端口，例如8443</param> 
        /// <param name="nConnectTimeout">连接超时时间，单位秒；传入0时，使用动态库的默认值</param>
        /// <param name="readTimeout">接收超时时间，单位秒；传入0时，使用动态库的默认值</param>
        /// <param name="writeTimeout">发送超时时间，单位秒；传入0时，使用动态库的默认值</param>
        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_InitServer", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern void Sodo_InitServer([MarshalAs(UnmanagedType.LPStr)] string serverIP, UInt16 serverPort,
        int nConnectTimeout, UInt32 readTimeout, UInt32 writeTimeout);


        /// <summary>
        /// 初始化https服务
        /// </summary>
        /// <param name="location"> location表示南方还是北方 南方为0  北方是1</param>
        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_InitHttps", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern void Sodo_InitHttps(UInt16 location);

        //初始化https服务器url参数
        /// <param name="location"> location表示南方还是北方 南方为0  北方是1</param>
        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_SetHttpsServer", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern void Sodo_SetHttpsServer(IntPtr url);

        /// <summary>
        /// 尝试打开串口连接，并检测非接模块是否可以通讯
        /// </summary>
        /// <returns>参见STATUS_CODE枚举值</returns>
        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_Start", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int Sodo_Start();


        /// <summary>
        /// 关闭串口连接，并释放动态库占用的资源
        /// </summary>
        /// <returns>参见STATUS_CODE枚举值</returns>
        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_Stop", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int Sodo_Stop();

        /// <summary>
        /// 检测模块串口通讯
        /// </summary>
        /// <param name="nTimes">检测次数</param>
        /// <returns>参见STATUS_CODE枚举值</returns>
        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_TerminalMatch", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int Sodo_TerminalMatch(int nTimes);

        /// <summary>
        /// 检测中间件服务通讯
        /// </summary>
        /// <param name="nTimes">检测次数</param>
        /// <returns>参见STATUS_CODE枚举值</returns>
        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_ServerMatch", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int Sodo_ServerMatch(int nTimes);

        /// <summary>
        /// 消费指令
        /// </summary>
        /// <param name="nAmount">消费金额，单位分，例如1、100分别一分钱、一块钱</param>
        /// <param name="nTimeout">等待卡片超时时间，单位秒</param>
        /// <param name="tradeRecord">TRADE_RECORD交易信息结构体，消费成功后返回给上位机</param>
        /// <returns>参见STATUS_CODE枚举值</returns>
        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_Trans", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int Sodo_Trans(int nAmount, int nTimeout, IntPtr ptrTradeRecord);

        /// <summary>
        /// 取消交易
        /// </summary>
        /// <returns>参见STATUS_CODE枚举值</returns>
        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_TransCancelOnce", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int Sodo_TransCancelOnce();

        /// <summary>
        /// 预消费寻卡指令，消费前可通过此指令先检测卡是否入场，并读取卡信息
        /// </summary>
        /// <param name="nAmount">消费金额，单位分，例如1、100分别一分钱、一块钱</param>
        /// <param name="nTimeout">等待卡片超时时间，单位秒</param>
        /// <param name="cardInfo">CARD_INFO卡信息结构体，检测到卡片后返回卡信息给上位机</param>
        /// <returns>参见STATUS_CODE枚举值</returns>
        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_DetectCard", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int Sodo_DetectCard(int nAmount, int nTimeout, IntPtr ptrCardInfo);

        /// <summary>
        /// 当有指令执行失败时，可通过此接口获取错误信息
        /// </summary>
        /// <param name="errMsg">错误信息，模块会返回上一笔失败指令的错误内容：错误码+错误说明，可直接显示的字符串</param>
        /// <returns>参见STATUS_CODE枚举值</returns>
        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_QueryErrorMsg", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int Sodo_QueryErrorMsg(ref IntPtr errMsg, int msgMaxLen);

        /// <summary>
        /// 检测模块状态
        /// 可作为空闲时的心跳指令，发送此指令后，模块会自动去检测状态，当状态不对时，会自行联网进行状态修复。
        /// 上层程序调用此指令后，可不予理会返回值，直接进行其他业务处理。
        /// </summary>
        /// <returns>参见STATUS_CODE枚举值</returns>
        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_CheckTermState", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int Sodo_CheckTermState();

        /// <summary>
        /// 设置模块Debug日志级别
        /// LVERBOSE     0,
        // 	        LDEBUG, 1
        // 	        LINFO,2
        // 	        LWARN,3
        // 	        LERROR,4
        /// 上层程序调用此指令后，可不予理会返回值，直接进行其他业务处理。
        /// </summary>
        /// <returns>参见STATUS_CODE枚举值</returns>
        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_SetLogLevel", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int Sodo_SetLogLevel(int logLevel);

        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_TransBackOut", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int Sodo_TransBackOut(int nTypeBackout, IntPtr transno, IntPtr transinfo);

        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_TransAmtCallBackOut", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int Sodo_TransAmtBack(int nTypeBackout, IntPtr transno, int backAmt, IntPtr transinfo);

        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_SetLogFilePath", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int Sodo_SetLogFilePath([MarshalAs(UnmanagedType.LPStr)] string filePath);

        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_ClearLogFile", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int Sodo_ClearLogFile([MarshalAs(UnmanagedType.LPStr)] string filePath, int days);

        /// <summary>
        /// 消费指令，带附加信息
        /// </summary>
        /// <param name="nAmount">消费金额，单位分，例如1、100分别一分钱、一块钱</param>
        /// <param name="nTimeout">等待卡片超时时间，单位秒</param>
        /// <param name="tradeRecord">TRADE_RECORD交易信息结构体，消费成功后返回给上位机</param>
        /// <returns>参见STATUS_CODE枚举值</returns>
        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_Trans_AddOtherInfo", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int Sodo_Trans_AddOtherInfo(int nAmount, int nTimeout,
                                                  IntPtr ptrTradeRecord, [MarshalAs(UnmanagedType.LPStr)] string infoOther);

        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_GetFirmAppVersion", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int Sodo_GetFirmAppVersion(ref IntPtr appVersion, int ntimeout_sec);

        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_UpdateFirm", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int Sodo_UpdateFirm(IntPtr binFirmBuff, int lenbinfile);


        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_GetCardAndQrcode", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int Sodo_GetCardAndQrcode(int transAmt, int ntimeout_sec, ref IntPtr appVersion, int ifUsb, int bInit, int bStop);

        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_GetQrcode", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int Sodo_GetQrcode(int ntimeout_sec, ref IntPtr info, int ifUsb, int bInit, int bStop);



        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_Sfz_Process", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int Sodo_Sfz_Process(ref TRADE_SFZ_OP_PARAM sfzOpParam);

        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_RequestCardAndQrcode", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int Sodo_RequestCardAndQrcode(ref STR_TRANS_OP_PARAM op);

        [DllImport("SodoXP100.dll", EntryPoint = "Sodo_SB_Process", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern int Sodo_SB_Process(ref STR_SB_INFO op);
    }
}
