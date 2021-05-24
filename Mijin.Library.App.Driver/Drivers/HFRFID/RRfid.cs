using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IsUtil;
using IsUtil.Helpers;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// 荣睿高频RFID读写器
    /// </summary>
    public class RRfid : IRRfid
    {
        #region Dll Import

        private const string DLLNAME = @"RR9000MUL.dll";

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int OpenNetPort(int Port,
                                             string IPaddr,
                                             ref byte ComAddr,
                                             ref int PortHandle);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int CloseNetPort(int PortHandle);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int OpenComPort(int port,
                                             ref byte comAddr,
                                             byte baud,
                                             ref int frmComPortIndex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int CloseComPort();

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int CloseSpecComPort(int frmComPortIndex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int AutoOpenComPort(ref int port,
                                                 ref byte comAddr,
                                                 byte baud,
                                                 ref int frmComPortIndex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int Inventory(ref byte comAddr,
                                           ref byte State,
                                           ref byte Afi,
                                           byte[] DsfidAndUID,
                                           ref byte CardNum,
                                           int frmComPortIndex);

        /*[DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int AdvInventory(ref byte comAddr,
                                           ref byte Afi,
                                           byte[] DsfidAndUID,
                                           ref int CardNum,
                                           int frmComPortIndex);*/

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int StayQuiet(ref byte comAddr,
                                           byte[] UID,
                                           ref byte errorCode,
                                           int frmComPortIndex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadSingleBlock(ref byte comAddr,
                                                 ref byte State,
                                                 byte[] UID,
                                                 byte BlockNum,
                                                 ref byte BlockSecStatus,
                                                 byte[] Data,
                                                 ref byte errorCode,
                                                 int frmComPortIndex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetReaderInformation(ref byte comAddr,
                                                      byte[] versionInfo,
                                                      ref byte readerType,
                                                      byte[] trType,
                                                      ref byte InventoryScanTime,
                                                      int frmComPortIndex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadMultipleBlock(ref byte comAddr,
                                                    ref byte state,
                                                    byte[] UID,
                                                    byte blocknum,
                                                    byte blockCount,
                                                    byte[] blockSecStatus,
                                                    byte[] data,
                                                    ref byte errorCode,
                                                    int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteSingleBlock(ref byte comAddr,
                                                   ref byte state,
                                                   byte[] UID,
                                                   byte blocknum,
                                                   byte[] data,
                                                   ref byte errorCode,
                                                   int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int LockBlock(ref byte comAddr,
                                            ref byte state,
                                            byte[] UID,
                                            byte blocknum,
                                            ref byte errorCode,
                                            int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int Select(ref byte comAddr,
                                         byte[] UID,
                                         ref byte errorCode,
                                         int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int ResetToReady(ref byte comAddr,
                                               ref byte state,
                                               byte[] UID,
                                               ref byte errorCode,
                                               int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteAFI(ref byte comAddr,
                                           ref byte state,
                                           byte[] UID,
                                           byte AFI,
                                           ref byte errorCode,
                                           int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int LockAFI(ref byte comAddr,
                                          ref byte state,
                                          byte[] UID,
                                          ref byte errorCode,
                                          int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteDSFID(ref byte comAddr,
                                             ref byte state,
                                             byte[] UID,
                                             byte DSFID,
                                             ref byte errorCode,
                                             int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int LockDSFID(ref byte comAddr,
                                            ref byte state,
                                            byte[] UID,
                                            ref byte ErrorCode,
                                            int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetSystemInformation(ref byte comAddr,
                                                       ref byte state,
                                                       byte[] UIDI,
                                                       ref byte InformationFlag,
                                                       byte[] UIDO,
                                                       ref byte DSFID,
                                                       ref byte AFI,
                                                       byte[] MemorySize,
                                                       ref byte ICReference,
                                                       ref byte ErrorCode,
                                                       int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int OpenRf(ref byte comAddr,
                                         int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int CloseRf(ref byte comAddr,
                                          int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteComAdr(ref byte currentComAddr,
                                              ref byte newComAddr,
                                              int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteInventoryScanTime(ref byte comAddr,
                                                         ref byte InventoryScanTime,
                                                         int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetGeneralOutput(ref byte comAddr,
                                                   ref byte _Output,
                                                   int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetGeneralInput(ref byte comAddr,
                                                  ref byte _Input,
                                                  int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetRelay(ref byte comAddr,
                                           ref byte _Relay,
                                           int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetActiveANT(ref byte comAddr,
                                               ref byte _ANT_Status,
                                               int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetANTStatus(ref byte comAddr,
                                               ref byte Get_ANT_Status,
                                               int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetActiveANT_2(ref byte comAddr,
                                                 ref byte _ANT_Status,
                                                 int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetANTStatus_2(ref byte comAddr,
                                                 ref byte Get_ANT_Status,
                                                 int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetUserDefinedBlockLength(ref byte comAddr,
                                                            ref byte _Block_len,
                                                            int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetScanMode(ref byte comAddr,
                                              byte[] _Scan_Mode_Data,
                                              int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int ReadScanModeData(byte[] ScanModeData,
                                                   ref int ValidDatalength,
                                                   int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetUserDefinedBlocklength(ref byte comAddr,
                                                            ref byte _Block_len,
                                                            int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetScanModeStatus(ref byte comAddr,
                                                    byte[] _Scan_Mode_Status,
                                                    int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int CustomizedReadSingleBlock(ref byte comAddr,
                                                            ref byte state,
                                                            byte[] UID,
                                                            byte blocknum,
                                                            byte[] DataBuffer,
                                                            ref byte errorCode,
                                                            int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int CustomizedWriteSingleBlock(ref byte comAddr,
                                                             ref byte state,
                                                             byte[] UID,
                                                             byte blocknum,
                                                             byte[] DataBuffer,
                                                             byte byteCount,
                                                             ref byte ErrorCode,
                                                             int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int CustomizedReadMultipleBlock(ref byte comAddr,
                                                              ref byte state,
                                                              byte[] UID,
                                                              byte blocknum,
                                                              byte blockCount,
                                                              byte[] DataBuffer,
                                                              ref byte errorCode,
                                                              int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetAccessTime(ref byte comAddr,
                                                ref byte AccessTime,
                                                int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetAccessTime(ref byte comAddr,
                                                ref byte AccessTimeRet,
                                                int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetReceiveChannel(ref byte comAddr,
                                                    ref byte ReceiveChannel,
                                                    int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetReceiveChannelStatus(ref byte comAddr,
                                                          ref byte ReceiveChannelStatus,
                                                          int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int TransparentRead(ref byte comAddr,
                                                  byte RspLength,
                                                  byte CustomDatalength,
                                                  byte[] CustomData,
                                                  ref byte FeedbackDataLength,
                                                  byte[] FeedbackData,
                                                  ref byte errorCode,
                                                  int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int TransparentWrite(ref byte comAddr,
                                                   byte[] option,
                                                   byte RspLength,
                                                   byte CustomDatalength,
                                                   byte[] CustomData,
                                                   ref byte FeedbackDataLength,
                                                   byte[] FeedbackData,
                                                   ref byte errorCode,
                                                   int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int TransparentCustomizedCmd(ref byte comAddr,
                                                           byte[] RspTime,
                                                           byte RspLength,
                                                           byte CustomDataLength,
                                                           byte[] CustomData,
                                                           ref byte FeedbackDataLength,
                                                           byte[] FeedbackData,
                                                           ref byte errorCode,
                                                           int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetParseMode(ref byte comAddr,
                                               ref byte _ParseMode,
                                               int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetParseMode(ref byte comAddr,
                                               ref byte Get_ParseMode,
                                               int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetPwr(ref byte comAddr,
                                               ref byte _Pwr,
                                               int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetPwrByValue(ref byte comAddr,
                                               ref byte _PwrVal,
                                               int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetPwr(ref byte comAddr,
                                        ref byte _Pwr,
                                        ref byte _PwrVal,
                                        int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int CheckAntenna(ref byte comAddr,
                                        ref byte _AntValid,
                                        int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int AdjustPwr(ref byte comAddr,
                                        ref byte _DirStep,
                                        int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int CalibratePwr(ref byte comAddr,
                                        ref byte _VRMS,
                                        int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int ProgramPwrTable(ref byte comAddr,
                                                 byte _Pwr,
                                                 byte _PwrVal,
                                                 byte _VRMS,
                                                 int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int SyncScan(ref byte comAddr,
                                        byte _Sync,
                                        int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetInventoryTime(ref int m_Time,
                                                  int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int WriteMultipleBlock(ref byte comAddr,
                                                   ref byte state,
                                                   byte[] UID,
                                                   byte StartBlock,
                                                   byte blocknum,
                                                   byte[] data,
                                                   ref byte errorCode,
                                                   int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int LockMultipleBlock(ref byte comAddr,
                                            ref byte state,
                                            byte[] UID,
                                            byte StartBlock,
                                            byte blocknum,
                                            ref byte errorCode,
                                            int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int ISO14443AInventory(ref byte comAddr,
                                            byte[] SN,
                                            int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetAFIScanParameters(ref byte comAddr,
                                            byte AFIEnable,
                                            byte AFIValue,
                                            int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int GetAFIScanParameters(ref byte comAddr,
                                            ref byte AFIEnable,
                                            ref byte AFIValue,
                                            int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int SetEASAlarm(ref byte comAddr,
                                            byte State,
                                            byte[] UID,
                                            int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int ReSetEAS(ref byte comAddr,
                                            byte State,
                                            byte[] UID,
                                            int frmportindex);

        [DllImport(DLLNAME, CallingConvention = CallingConvention.StdCall)]
        public static extern int DetectEASAlarm(ref byte comAddr,
                                            byte State,
                                            byte[] UID,
                                            int frmportindex);
        #endregion
        /// <summary>
        /// com口号
        /// </summary>
        public int portIndex = 0;
        /// <summary>
        /// RFID读写器地址
        /// </summary>
        public byte readerAddr = 0xff;

        public RRfid()
        {
        }

        ~RRfid()
        {
            if (!portIndex.IsZeroOrMinus())
            {
                // 关闭com口
                CloseSpecComPort(portIndex);
            }
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
            fCmdRet = AutoOpenComPort(ref portNum, ref readerAddr, fbaud, ref portIndex);
            if (fCmdRet != 0)
            {
                res.msg = "连接失败";
                res.devMsg = @$"错误码{fCmdRet}";
                return res;
            }
            res.msg = "自动连接设备成功";
            res.devMsg = @$"设备位于 COM{portIndex}";
            res.success = true;
            return res;

        }

        /// <summary>
        /// 新的扫描
        /// NewScan(3,2) 即为 从第三块区开始读，读2个区的数据，即为读3 4 区的数据
        /// NewScan(3,5) 即为 从第三块区开始读，读5个区的数据，即为读3 4 5 6 7 区的数据
        /// </summary>
        /// <param name="startBlockNum">起始块地址</param>
        /// <param name="blockCount">连续读取的数据块数</param>
        /// <returns></returns>
        public MessageModel<List<ScanDataModel>> NewScan(Int64 startBlockNum, Int64 blockCount)
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
            fCmdRet = Inventory(ref readerAddr, ref state, ref AFI, DSFIDAndUID, ref cardNumber, portIndex);
            if (fCmdRet == 0x0e)
            {
                res.devMsg = "0x0e 无标签需要解析或全部标签都解析完成";
            }
            else if (fCmdRet == 0x0a)
            {
                res.devMsg = "0x0a 未成功解析出一张标签，但由于InventoryScanTime规定的解析时间已到，命令执行结束";
            }
            else if (fCmdRet == 0x0b)
            {
                res.devMsg = "0x0b 已经解析出部分标签而未解析出全部标签，但由于InventoryScanTime规定的解析时间已到，命令执行结束。";
            }

            // 获取Uids
            Array.Resize(ref DSFIDAndUID, cardNumber * 9);
            for (int i = 0; i < DSFIDAndUID.Length / 9; i++)
            {
                uids.Add(DSFIDAndUID.Skip(i * 9 + 1).Take(8).ToArray());
            }
            foreach (var uid in uids)
            {
                byte[] data = new byte[4 * blockCount];
                byte[] blockSecStatus = new byte[blockCount];
                byte errorCode = 0;
                state = 0;
                fCmdRet = ReadMultipleBlock(ref readerAddr, ref state, uid, (byte)startBlockNum, (byte)blockCount,
                                                       blockSecStatus, data, ref errorCode, portIndex);
                if (fCmdRet == 0)
                {
                    ScanDataModel scanData = new ScanDataModel();
                    scanData.Uid = uid;
                    scanData.UidHexStr = SerialPortHelper.ByteArrayToHexString(uid).Replace(" ", "");

                    scanData.BlockData = SerialPortHelper.ByteArrayToHexString(data).Replace(" ", "");
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
                fCmdRet = SetEASAlarm(ref readerAddr, 1, UID, portIndex);
            else
                fCmdRet = ReSetEAS(ref readerAddr, 1, UID, portIndex);

            if (fCmdRet != 0)
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
                fCmdRet = SetEASAlarm(ref readerAddr, 1, UID, portIndex);
            else
                fCmdRet = ReSetEAS(ref readerAddr, 1, UID, portIndex);

            if (fCmdRet != 0)
            {
                res.msg = "设置失败";
                return res;
            }
            res.msg = "设置成功";
            res.success = true;
            return res;
        }
    }

    public class ScanDataModel
    {
        public byte[] Uid { get; set; }
        public string UidHexStr { get; set; }
        public string BlockData { get; set; }
    }
}
