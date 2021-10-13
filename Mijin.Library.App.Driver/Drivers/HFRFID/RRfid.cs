using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IsUtil;
using IsUtil.Helpers;
using ReaderA;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// 荣睿高频RFID读写器
    /// </summary>
    public class RRfid : IRRfid
    {
        public event Action<WebViewSendModel<List<ScanDataModel>>> OnReadHFLabels;
        //#region Dll Import

        //private const string DLLNAME = @"RR9000MUL.dll";

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int OpenNetPort(int Port,
        //                                     string IPaddr,
        //                                     ref byte ComAddr,
        //                                     ref int PortHandle);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int CloseNetPort(int PortHandle);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int OpenComPort(int port,
        //                                     ref byte comAddr,
        //                                     byte baud,
        //                                     ref int frmComPortIndex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int CloseComPort();

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int CloseSpecComPort(int frmComPortIndex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int AutoOpenComPort(ref int port,
        //                                         ref byte comAddr,
        //                                         byte baud,
        //                                         ref int frmComPortIndex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int Inventory(ref byte comAddr,
        //                                   ref byte State,
        //                                   ref byte Afi,
        //                                   byte[] DsfidAndUID,
        //                                   ref byte CardNum,
        //                                   int frmComPortIndex);

        ///*[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int AdvInventory(ref byte comAddr,
        //                                   ref byte Afi,
        //                                   byte[] DsfidAndUID,
        //                                   ref int CardNum,
        //                                   int frmComPortIndex);*/

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int StayQuiet(ref byte comAddr,
        //                                   byte[] UID,
        //                                   ref byte errorCode,
        //                                   int frmComPortIndex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int ReadSingleBlock(ref byte comAddr,
        //                                         ref byte State,
        //                                         byte[] UID,
        //                                         byte BlockNum,
        //                                         ref byte BlockSecStatus,
        //                                         byte[] Data,
        //                                         ref byte errorCode,
        //                                         int frmComPortIndex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int GetReaderInformation(ref byte comAddr,
        //                                              byte[] versionInfo,
        //                                              ref byte readerType,
        //                                              byte[] trType,
        //                                              ref byte InventoryScanTime,
        //                                              int frmComPortIndex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int ReadMultipleBlock(ref byte comAddr,
        //                                            ref byte state,
        //                                            byte[] UID,
        //                                            byte blocknum,
        //                                            byte ReadBlockCount,
        //                                            byte[] blockSecStatus,
        //                                            byte[] data,
        //                                            ref byte errorCode,
        //                                            int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int WriteSingleBlock(ref byte comAddr,
        //                                           ref byte state,
        //                                           byte[] UID,
        //                                           byte blocknum,
        //                                           byte[] data,
        //                                           ref byte errorCode,
        //                                           int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int LockBlock(ref byte comAddr,
        //                                    ref byte state,
        //                                    byte[] UID,
        //                                    byte blocknum,
        //                                    ref byte errorCode,
        //                                    int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int Select(ref byte comAddr,
        //                                 byte[] UID,
        //                                 ref byte errorCode,
        //                                 int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int ResetToReady(ref byte comAddr,
        //                                       ref byte state,
        //                                       byte[] UID,
        //                                       ref byte errorCode,
        //                                       int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int WriteAFI(ref byte comAddr,
        //                                   ref byte state,
        //                                   byte[] UID,
        //                                   byte AFI,
        //                                   ref byte errorCode,
        //                                   int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int LockAFI(ref byte comAddr,
        //                                  ref byte state,
        //                                  byte[] UID,
        //                                  ref byte errorCode,
        //                                  int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int WriteDSFID(ref byte comAddr,
        //                                     ref byte state,
        //                                     byte[] UID,
        //                                     byte DSFID,
        //                                     ref byte errorCode,
        //                                     int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int LockDSFID(ref byte comAddr,
        //                                    ref byte state,
        //                                    byte[] UID,
        //                                    ref byte ErrorCode,
        //                                    int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int GetSystemInformation(ref byte comAddr,
        //                                               ref byte state,
        //                                               byte[] UIDI,
        //                                               ref byte InformationFlag,
        //                                               byte[] UIDO,
        //                                               ref byte DSFID,
        //                                               ref byte AFI,
        //                                               byte[] MemorySize,
        //                                               ref byte ICReference,
        //                                               ref byte ErrorCode,
        //                                               int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int OpenRf(ref byte comAddr,
        //                                 int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int CloseRf(ref byte comAddr,
        //                                  int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int WriteComAdr(ref byte currentComAddr,
        //                                      ref byte newComAddr,
        //                                      int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int WriteInventoryScanTime(ref byte comAddr,
        //                                                 ref byte InventoryScanTime,
        //                                                 int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int SetGeneralOutput(ref byte comAddr,
        //                                           ref byte _Output,
        //                                           int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int GetGeneralInput(ref byte comAddr,
        //                                          ref byte _Input,
        //                                          int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int SetRelay(ref byte comAddr,
        //                                   ref byte _Relay,
        //                                   int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int SetActiveANT(ref byte comAddr,
        //                                       ref byte _ANT_Status,
        //                                       int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int GetANTStatus(ref byte comAddr,
        //                                       ref byte Get_ANT_Status,
        //                                       int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int SetActiveANT_2(ref byte comAddr,
        //                                         ref byte _ANT_Status,
        //                                         int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int GetANTStatus_2(ref byte comAddr,
        //                                         ref byte Get_ANT_Status,
        //                                         int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int SetUserDefinedBlockLength(ref byte comAddr,
        //                                                    ref byte _Block_len,
        //                                                    int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int SetScanMode(ref byte comAddr,
        //                                      byte[] _Scan_Mode_Data,
        //                                      int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int ReadScanModeData(byte[] ScanModeData,
        //                                           ref int ValidDatalength,
        //                                           int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int GetUserDefinedBlocklength(ref byte comAddr,
        //                                                    ref byte _Block_len,
        //                                                    int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int GetScanModeStatus(ref byte comAddr,
        //                                            byte[] _Scan_Mode_Status,
        //                                            int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int CustomizedReadSingleBlock(ref byte comAddr,
        //                                                    ref byte state,
        //                                                    byte[] UID,
        //                                                    byte blocknum,
        //                                                    byte[] DataBuffer,
        //                                                    ref byte errorCode,
        //                                                    int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int CustomizedWriteSingleBlock(ref byte comAddr,
        //                                                     ref byte state,
        //                                                     byte[] UID,
        //                                                     byte blocknum,
        //                                                     byte[] DataBuffer,
        //                                                     byte byteCount,
        //                                                     ref byte ErrorCode,
        //                                                     int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int CustomizedReadMultipleBlock(ref byte comAddr,
        //                                                      ref byte state,
        //                                                      byte[] UID,
        //                                                      byte blocknum,
        //                                                      byte ReadBlockCount,
        //                                                      byte[] DataBuffer,
        //                                                      ref byte errorCode,
        //                                                      int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int SetAccessTime(ref byte comAddr,
        //                                        ref byte AccessTime,
        //                                        int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int GetAccessTime(ref byte comAddr,
        //                                        ref byte AccessTimeRet,
        //                                        int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int SetReceiveChannel(ref byte comAddr,
        //                                            ref byte ReceiveChannel,
        //                                            int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int GetReceiveChannelStatus(ref byte comAddr,
        //                                                  ref byte ReceiveChannelStatus,
        //                                                  int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int TransparentRead(ref byte comAddr,
        //                                          byte RspLength,
        //                                          byte CustomDatalength,
        //                                          byte[] CustomData,
        //                                          ref byte FeedbackDataLength,
        //                                          byte[] FeedbackData,
        //                                          ref byte errorCode,
        //                                          int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int TransparentWrite(ref byte comAddr,
        //                                           byte[] option,
        //                                           byte RspLength,
        //                                           byte CustomDatalength,
        //                                           byte[] CustomData,
        //                                           ref byte FeedbackDataLength,
        //                                           byte[] FeedbackData,
        //                                           ref byte errorCode,
        //                                           int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int TransparentCustomizedCmd(ref byte comAddr,
        //                                                   byte[] RspTime,
        //                                                   byte RspLength,
        //                                                   byte CustomDataLength,
        //                                                   byte[] CustomData,
        //                                                   ref byte FeedbackDataLength,
        //                                                   byte[] FeedbackData,
        //                                                   ref byte errorCode,
        //                                                   int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int SetParseMode(ref byte comAddr,
        //                                       ref byte _ParseMode,
        //                                       int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int GetParseMode(ref byte comAddr,
        //                                       ref byte Get_ParseMode,
        //                                       int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int SetPwr(ref byte comAddr,
        //                                       ref byte _Pwr,
        //                                       int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int SetPwrByValue(ref byte comAddr,
        //                                       ref byte _PwrVal,
        //                                       int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int GetPwr(ref byte comAddr,
        //                                ref byte _Pwr,
        //                                ref byte _PwrVal,
        //                                int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int CheckAntenna(ref byte comAddr,
        //                                ref byte _AntValid,
        //                                int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int AdjustPwr(ref byte comAddr,
        //                                ref byte _DirStep,
        //                                int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int CalibratePwr(ref byte comAddr,
        //                                ref byte _VRMS,
        //                                int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int ProgramPwrTable(ref byte comAddr,
        //                                         byte _Pwr,
        //                                         byte _PwrVal,
        //                                         byte _VRMS,
        //                                         int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int SyncScan(ref byte comAddr,
        //                                byte _Sync,
        //                                int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int GetInventoryTime(ref int m_Time,
        //                                          int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int WriteMultipleBlock(ref byte comAddr,
        //                                           ref byte state,
        //                                           byte[] UID,
        //                                           byte StartBlock,
        //                                           byte blocknum,
        //                                           byte[] data,
        //                                           ref byte errorCode,
        //                                           int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int LockMultipleBlock(ref byte comAddr,
        //                                    ref byte state,
        //                                    byte[] UID,
        //                                    byte StartBlock,
        //                                    byte blocknum,
        //                                    ref byte errorCode,
        //                                    int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int ISO14443AInventory(ref byte comAddr,
        //                                    byte[] SN,
        //                                    int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int SetAFIScanParameters(ref byte comAddr,
        //                                    byte AFIEnable,
        //                                    byte AFIValue,
        //                                    int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int GetAFIScanParameters(ref byte comAddr,
        //                                    ref byte AFIEnable,
        //                                    ref byte AFIValue,
        //                                    int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int SetEASAlarm(ref byte comAddr,
        //                                    byte State,
        //                                    byte[] UID,
        //                                    int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int ReSetEAS(ref byte comAddr,
        //                                    byte State,
        //                                    byte[] UID,
        //                                    int frmportindex);

        //[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        //public static extern int DetectEASAlarm(ref byte comAddr,
        //                                    byte State,
        //                                    byte[] UID,
        //                                    int frmportindex);
        //#endregion

        private const byte lengthError = 0x01;
        private const byte operationNotSupport = 0x02;
        private const byte dataRangError = 0x03;
        private const byte cmdNotOperation = 0x04;
        private const byte rfClosed = 0x05;
        private const byte EEPROM = 0x06;
        private const byte timeOut = 0x0a;
        private const byte moreUID = 0x0b;
        private const byte ISOError = 0x0c;
        private const byte noElectronicTag = 0x0e;
        private const byte operationError = 0x0f;
        private const byte cmdNotSupport = 0x01;
        private const byte cmdNotIdentify = 0x02;
        private const byte errOperationNotSupport = 0x03;
        private const byte unknownError = 0x0f;
        private const byte blockError = 0x10;
        private const byte blockLockedCntLock = 0x11;
        private const byte blockLockedCntWrite = 0x12;
        private const byte blockCntOperate = 0x13;
        private const byte blockCntLock = 0x14;
        private const byte communicationErr = 0x30;
        private const byte retCRCErr = 0x31;
        private const byte retDataErr = 0x32;
        private const byte communicationBusy = 0x33;
        private const byte executeCmdBusy = 0x34;
        private const byte comPortOpened = 0x35;
        private const byte comPortClose = 0x36;
        private const byte invalidHandle = 0x37;
        private const byte invalidPort = 0x38;
        private const byte OK = 0x00;


        /// <summary>
        /// com口号
        /// </summary>
        public int portIndex = 0;
        /// <summary>
        /// RFID读写器地址
        /// </summary>
        public byte readerAddr = 0xff;

        private bool startRead = false;

        private bool reading = false;

        /// <summary>
        /// 读标签起始块
        /// </summary>
        private int startReadBlock = 0;
        /// <summary>
        /// 读标签块数量
        /// </summary>
        private int readBlockCount = 0;
        /// <summary>
        /// 读/写 标签时操作块大小
        /// </summary>
        private int actionBlockSize = 4;

        /// <summary>
        /// 写标签类型
        /// A标签： 块4 = 0x00  块8 = 0x04
        /// B标签： 块4 = 0x08  块8 = 0x0C
        /// </summary>
        private int writeState = 0;


        Task readTask = null;

        /// <summary>
        /// 多线程读标签事件间隔毫秒
        /// </summary>
        public int ScanSpaceMs { get; set; } = 1500;

        public RRfid()
        {

        }

        ~RRfid()
        {
            if (!portIndex.IsZeroOrMinus())
            {
                // 关闭com口
                StaticClassReaderA.CloseSpecComPort(portIndex);
            }
        }

        private string GetReturnCodeDesc(int cmdRet)
        {
            switch (cmdRet)
            {
                case lengthError:
                    return "命令操作数长度错误";
                case operationNotSupport:
                    return "操作命令不支持";
                case dataRangError:
                    return "操作数范围不符";
                case rfClosed:
                    return "感应场处于关闭状态";
                case EEPROM:
                    return "EEPROM操作出错";
                case timeOut:
                    return "指定的Inventory-Scan-Time溢出";
                case moreUID:
                    return "在Inventory-Scan-Time时间内无得到所有电子标签的UID";
                case ISOError:
                    return "ISO 错误";
                case noElectronicTag:
                    return "无电子标签可操作";
                case operationError:
                    return "操作出错";
                case communicationErr:
                    return "";
                case retCRCErr:
                    return "CRC校验错误";
                case communicationBusy:
                    return "通讯繁忙，设备正在执行其他指令";
                case cmdNotOperation:
                    return "操作命令当前无法执行";
                case comPortOpened:
                    return "端口已打开";
                case comPortClose:
                    return "端口已关闭";
                case invalidHandle:
                    return "无效的句柄";
                case invalidPort:
                    return "无效的端口 ";
                case OK:
                    return "操作成功";
                default:
                    return "";
            }
        }

        private string GetErrorCodeDesc(byte errorCode)
        {
            switch (errorCode)
            {
                case cmdNotSupport:
                    return "命令不被支持 ";
                case cmdNotIdentify:
                    return "命令不被识别 ";
                case errOperationNotSupport:
                    return "该操作不被支持";
                case unknownError:
                    return "未知的错误类型 ";
                case blockError:
                    return "所指定的操作块不能被使用或不存在 ";
                case blockLockedCntLock:
                    return "所指定的操作块已经被锁定，不能再次被锁定";
                case blockLockedCntWrite:
                    return "所指定的操作块已经被锁定，不能对其内容进行改写";
                case blockCntOperate:
                    return "所指定的操作块不能被正常操作";
                case blockCntLock:
                    return "所指定的操作块不能被正常锁定";

                default:
                    return "";
            }
        }

        private string GetReturnCodeAndErrorCodeDesc(int cmdRet = 0x99, byte errorCode = 0x99)
        {
            return @$"ReturnDesc：{GetReturnCodeDesc(cmdRet)}" + "\r\n" + @$"ErrorCodeDesc：{GetErrorCodeDesc(errorCode)}";
        }

        /// <summary>
        /// 设置AFI
        /// </summary>
        /// <param name="uidHex"></param>
        /// <param name="afi"></param>
        /// <returns></returns>
        public MessageModel<bool> SetAFI(string uidHex, Int64 afi)
        {
            var res = new MessageModel<bool>();

            byte state = 6;
            byte errorCode = 0;
            // 获取byte[] uid
            var uid = SerialPortHelper.HexStringToByteArray(uidHex);

            var fCmdRet = StaticClassReaderA.WriteAFI(ref readerAddr, ref state, uid, (byte)afi, ref errorCode, portIndex);

            res.msg = fCmdRet != OK ?"设置失败":"设置成功";
            res.success = fCmdRet == OK;
            return res;
        }

        /// <summary>
        /// 自动连接设备
        /// </summary>
        /// <returns></returns>
        public MessageModel<string> AutoOpenComPort()
        {
            var res = new MessageModel<string>();
            if (portIndex > 0)
            {
                res.msg = "已连接设备，无需再次连接";
                res.success = true;
                return res;
            }
            int portNum = -1;
            int fCmdRet = 0x30;
            byte fbaud = 0;
            fCmdRet = StaticClassReaderA.AutoOpenComPort(ref portNum, ref readerAddr, fbaud, ref portIndex);
            if (fCmdRet != 0)
            {
                res.msg = "连接失败";
                res.devMsg = @$"错误码{fCmdRet}";
                return res;
            }
            if (readTask.IsNull())
                readTask = Task.Run(ReadLabelHandle);

            res.msg = "自动连接设备成功";
            res.devMsg = @$"设备位于 COM{portIndex}";
            res.success = true;
            return res;

        }

        /// <summary>
        /// 设置读写标签参数
        /// </summary>
        /// <param name="startReadBlock">起始块</param>
        /// <param name="ReadBlockCount"></param>
        /// <param name="actionBlockSize"></param>
        /// <param name="writeState"></param>
        /// <returns></returns>
        public MessageModel<string> SetActionLabelPara(Int64 startReadBlock = 0, Int64 ReadBlockCount = 0, Int64 actionBlockSize = 0, Int64 writeState = 0)
        {
            if (!startReadBlock.IsZeroOrMinus())
                this.startReadBlock = (int)startReadBlock;
            if (!ReadBlockCount.IsZeroOrMinus())
                this.readBlockCount = (int)ReadBlockCount;
            if (!actionBlockSize.IsZeroOrMinus())
                this.actionBlockSize = (int)actionBlockSize;
            if (!writeState.IsZeroOrMinus())
                this.writeState = (int)writeState;

            return new()
            {
                msg = "设置 读/写 标签参数成功",
                success = true
            };
        }

        /// <summary>
        /// 新的扫描
        /// NewScan(3,2) 即为 从第三块区开始读，读2个区的数据，即为读3 4 区的数据
        /// NewScan(3,5) 即为 从第三块区开始读，读5个区的数据，即为读3 4 5 6 7 区的数据
        /// </summary>
        /// <param name="startBlockNum">起始块地址</param>
        /// <param name="readBlockCount">连续读取的数据块数</param>
        /// <returns></returns>
        public MessageModel<List<ScanDataModel>> NewScan(Int64 startBlockNum = -1, Int64 readBlockCount = -1)
        {
            var res = new MessageModel<List<ScanDataModel>>() { response = new List<ScanDataModel>() };
            int fCmdRet = 0x30;
            byte state = 6, AFI = 0, cardNumber = 0;
            byte[] DSFIDAndUID = new byte[2300];
            var uids = new List<byte[]>();
            if (portIndex.IsZeroOrMinus())
            {
                res.msg = "未连接设备";
                return res;
            }

            //不传参则使用全局参数
            if (startBlockNum.IsZeroOrMinus())
                startBlockNum = this.startReadBlock;
            if (readBlockCount.IsZeroOrMinus())
                readBlockCount = this.readBlockCount;

            // 开始扫描
            fCmdRet = StaticClassReaderA.Inventory(ref readerAddr, ref state, ref AFI, DSFIDAndUID, ref cardNumber, portIndex);
            // 获取错误数据
            //res.devMsg = GetReturnCodeAndErrorCodeDesc(fCmdRet);

            // 获取Uids
            Array.Resize(ref DSFIDAndUID, cardNumber * 9);
            for (int i = 0; i < DSFIDAndUID.Length / 9; i++)
            {
                uids.Add(DSFIDAndUID.Skip(i * 9 + 1).Take(8).ToArray());
            }
            foreach (var uid in uids)
            {
                byte[] data = new byte[actionBlockSize * readBlockCount];
                byte[] blockSecStatus = new byte[readBlockCount];
                byte errorCode = 0;
                state = 0;
                fCmdRet = StaticClassReaderA.ReadMultipleBlock(ref readerAddr, ref state, uid, (byte)startBlockNum, (byte)readBlockCount,
                                                       blockSecStatus, data, ref errorCode, portIndex);
                if (fCmdRet == 0)
                {
                    ScanDataModel scanData = new ScanDataModel();
                    //scanData.Uid = uid;
                    scanData.UidHexStr = SerialPortHelper.ByteArrayToHexString(uid).Replace(" ", "");
                    scanData.BlockHexData = SerialPortHelper.ByteArrayToHexString(data).Replace(" ", "");
                    //scanData.BlockAsciiData = Encoding.ASCII.GetString(data);
                    //scanData.BlockData = data;
                    res.response.Add(scanData);
                }
            }
            if (res.response.IsEmpty())
                res.msg = "未扫描到标签";
            else
            {
                res.msg = "扫描完成";
                res.success = true;
            }
            return res;
        }

        /// <summary>
        /// 开始读
        /// </summary>
        /// <returns></returns>
        public MessageModel<string> Read()
        {
            var res = new MessageModel<string>();
            if (portIndex.IsZeroOrMinus())
            {
                res.msg = "未连接设备";
                return res;
            }
            startRead = true;
            res.success = true;
            res.msg = "开始读取高频标签";
            return res;
        }

        /// <summary>
        /// 开始读
        /// </summary>
        /// <returns></returns>
        public MessageModel<string> Stop()
        {
            var res = new MessageModel<string>();
            if (portIndex.IsZeroOrMinus())
            {
                res.msg = "未连接设备";
                return res;
            }
            startRead = false;
            res.success = true;
            res.msg = "停止读高频标签";
            return res;
        }

        /// <summary>
        /// 只读一个
        /// </summary>
        /// <returns></returns>
        public MessageModel<ScanDataModel> ReadOnce()
        {
            var res = new MessageModel<string>();
            if (portIndex.IsZeroOrMinus())
            {
                return new()
                {
                    msg = "未连接设备"
                };
            }
            var data = NewScan();
            return new()
            {
                msg = data.msg,
                success = data.success,
                devMsg = data.devMsg,
                response = data.response?.FirstOrDefault()
            };
        }

        /// <summary>
        /// 写标签
        /// </summary>
        /// <param name="uidHex"></param>
        /// <param name="data"></param>
        /// <param name="actionBlockSize">标签单个块大小，4 or 8</param>
        /// <param name="writeState">写入类型，参考 writeState 注释</param>
        /// <returns></returns>
        public MessageModel<string> WriteLabel(string uidHex, byte[] data, Int64 actionBlockSize = -1, Int64 writeState = -1)
        {
            var res = new MessageModel<string>();

            // 不传参数则使用全局参数
            if (actionBlockSize.IsZeroOrMinus())
                actionBlockSize = this.actionBlockSize;
            if (writeState.IsZeroOrMinus())
                writeState = this.writeState;

            byte state = (byte)writeState;
            byte errorCode = 0;
            int fCmdRet = 0x30;

            if (portIndex.IsZeroOrMinus())
            {
                return new()
                {
                    msg = "未连接设备"
                };
            }
            // 获取byte[] uid
            var uid = SerialPortHelper.HexStringToByteArray(uidHex);

            // 获取需要写入的块数量
            var writeBlockCountDouble = (double)data.Length / actionBlockSize;
            // 包含小数则进1
            if (((writeBlockCountDouble - (int)writeBlockCountDouble)) * 100 > 0)
            {
                writeBlockCountDouble += 1;
            }
            // 最终需要写入的块数量
            var writeBlockCount = (int)writeBlockCountDouble;

            for (byte i = 0; i < writeBlockCount; i++)
            {
                int startIndex = (int)(i * actionBlockSize);
                int endIndex = (int)(i * actionBlockSize + actionBlockSize);
                // 防止越界
                if (endIndex > data.Length)
                    endIndex = data.Length;

                fCmdRet = StaticClassReaderA.WriteSingleBlock(ref readerAddr, ref state, uid, i, data[startIndex..endIndex], ref errorCode, portIndex);
                if (fCmdRet != OK)
                {
                    res.msg = "写入失败";
                    return res;
                }
            }
            res.msg = "写入成功";
            res.success = true;
            return res;
        }

        public MessageModel<string> WriteLabel(string uidHex, string hexData, Int64 actionBlockSize = -1, Int64 writeState = -1)
        {
            return WriteLabel(uidHex, SerialPortHelper.HexStringToByteArray(hexData), actionBlockSize, writeState);
        }

        public MessageModel<string> WriteLabelByAscii(string uidHex, string asciiData, Int64 actionBlockSize = -1, Int64 writeState = -1)
        {
            var asciiBytes = System.Text.Encoding.ASCII.GetBytes(asciiData);
            return WriteLabel(uidHex, asciiBytes, actionBlockSize, writeState);
        }

        /// <summary>
        /// 设置高频标签EAS
        /// </summary>
        /// <param name="uid">标签Uid</param>
        /// <param name="enabled">使能EAS报警</param>
        /// <returns></returns>
        public MessageModel<string> SetEAS(List<Int64> uid, bool enabled)
        {
            var res = new MessageModel<string>();
            var fCmdRet = -1;
            var UID = uid.ConvertAll(new Converter<Int64, byte>(int64 => (byte)int64)).ToArray();
            if (enabled)
                fCmdRet = StaticClassReaderA.SetEASAlarm(ref readerAddr, 1, UID, portIndex);
            else
                fCmdRet = StaticClassReaderA.ReSetEAS(ref readerAddr, 1, UID, portIndex);

            res.devMsg = GetReturnCodeAndErrorCodeDesc(fCmdRet);

            if (fCmdRet != OK)
            {
                res.msg = "设置失败";
                return res;
            }
            res.msg = "设置成功";
            res.success = true;
            return res;
        }

        public MessageModel<string> SetEAS(string uidHexStr, bool enabled)
        {
            var res = new MessageModel<string>();
            var fCmdRet = -1;
            var UID = SerialPortHelper.HexStringToByteArray(uidHexStr);
            if (enabled)
                fCmdRet = StaticClassReaderA.SetEASAlarm(ref readerAddr, 1, UID, portIndex);
            else
                fCmdRet = StaticClassReaderA.ReSetEAS(ref readerAddr, 1, UID, portIndex);

            res.devMsg = GetReturnCodeAndErrorCodeDesc(fCmdRet);

            if (fCmdRet != OK)
            {
                res.msg = "设置失败";
                return res;
            }
            res.msg = "设置成功";
            res.success = true;
            return res;
        }

        public MessageModel<string> SetScanSpaceMs(Int64 ms)
        {
            ScanSpaceMs = (int)ms;
            return new()
            {
                success = true,
                msg = "设置成功"
            };
        }
        private async void ReadLabelHandle()
        {
            while (true)
            {
                // 未开启读 或 未初始化，直接返回
                if (!startRead || portIndex.IsZeroOrMinus())
                {
                    await Task.Delay(50);
                    continue;
                }

                // 300毫秒读一次
                await Task.Delay(ScanSpaceMs);

                // 如果别的线程正在读，则直接返回
                if (reading) continue;

                reading = true;

                var data = NewScan();
                reading = false;
                if (data.success)
                {
                    OnReadHFLabels?.Invoke(new()
                    {
                        method = nameof(OnReadHFLabels),
                        response = data.response,
                        success = true,
                        msg = data.msg,
                        devMsg = data.devMsg
                    });
                }
            }
        }
    }

    public class ScanDataModel
    {
        //public byte[] Uid { get; set; }
        public string UidHexStr { get; set; }
        //public string BlockAsciiData { get; set; }
        //public byte[] BlockData { get; set; }
        public string BlockHexData { get; set; }
    }
}
