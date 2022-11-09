using IsUtil;
using IsUtil.Helpers;
using Mijin.Library.App.Model;
using Mijin.Library.App.Model.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Bing.Extensions;
using Mijin.Library.App.Driver.Interface;
using Util.Logs;
using Util.Logs.Extensions;
namespace Mijin.Library.App.Driver
{
    public class RRHFReader : IRRHFReader
    {
        private byte readerAddr = 0xff;
        private int portFrmIndex = -1;
        private int fopencomport = -1;
        private byte[] SNR = new byte[4];

        /// <summary>
        /// 14443A : 0
        /// 14443B : 1
        /// </summary>
        private byte beforeKeyStyle = 0;

		public ISystemFunc _systemFunc { get; }
		public RRHFReader(ISystemFunc systemFunc)
		{
			_systemFunc = systemFunc;
		}

		#region ErrorCode
		private const byte OK = 0x00;
        private const byte lengthError = 0x01;
        private const byte operationNotSupport = 0x02;
        private const byte dataRangError = 0x03;
        private const byte cmdNotOperation = 0x04;
        private const byte RfClosed = 0x05;
        private const byte EEPROM = 0x06;
        private const byte timeOut = 0x0a;
        private const byte moreUID = 0x0b;
        private const byte ISOError = 0x0c;
        private const byte noElectronicTag = 0x0e;
        private const byte operationError = 0x0f;
        private const byte cmdNotSupport = 0x01;
        private const byte cmdNotIdentify = 0x02;
        private const byte unknownError = 0x0f;
        private const byte blockError = 0x10;
        private const byte blockLockedAndCntLock = 0x11;
        private const byte blockLockedAndCntWrite = 0x12;
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
        #endregion

        private string GetErrorCodeDesc(byte errorCode)
        {
            switch (errorCode)
            {
                case cmdNotSupport:
                    return "命令不被支持";
                case cmdNotIdentify:
                    return "命令不被识别";
                case unknownError:
                    return "未知的错误类型";
                case blockError:
                    return "所指定的操作块不能被使用或不存在";
                case blockLockedAndCntLock:
                    return "所指定的操作块已经被锁定";
                case blockLockedAndCntWrite:
                    return "所指定的操作块已经被锁定";
                case blockCntOperate:
                    return "所指定的操作块不能被正常操作";
                case blockCntLock:
                    return "所指定的操作块不能被正常锁定";
                case cmdNotOperation:
                    return "操作命令当前无法执行";
                case 0x33:
                    return "Utralight电子标签防冲突失败";
                case 0x32:
                    return "MF1与Utralight电子标签冲突";
                case 0x31:
                    return "不允许多张标签进入";
                case 0x30:
                    return "防冲突失败";
                case 0x2E:
                    return "Utralight电子标签写失败";
                case 0x2D:
                    return "值操作失败";
                case 0x2C:
                    return "检查写数据不符";
                case 0x2B:
                    return "检查写失败";
                case 0x2A:
                    return "存储密钥失败";
                case 0x29:
                    return "读写E2PROM失败";
                case 0x28:
                    return "数据存储传送失败";
                case 0x27:
                    return "钱包增减失败";
                case 0x26:
                    return "读钱包数据失败";
                case 0x25:
                    return "钱包初始化失败";
                case 0x24:
                    return "写数据失败";
                case 0x23:
                    return "读数据失败";
                case 0x22:
                    return "验证失败";
                case 0x21:
                    return "选择电子标签失败";
                case 0x20:
                    return "无ISO14443A协议标签可操作";
                case 0x34:
                    return "无ISO14443B协议标签可操作";
                case 0x35:
                    return "选择电子标签失败";
                case 0x36:
                    return "Halt失败";
                case 0x37:
                    return "执行透明命令出错";
                case 0x38:
                    return "防冲突失败";
                default:
                    return "";
            }
        }
        private string HFReaderGetReturnCodeDesc(int retCode)
        {
            switch (retCode)
            {
                case OK: return "操作成功";
                case lengthError: return "命令操作数长度错误";
                case operationNotSupport: return "操作命令不支持";
                case RfClosed: return "感应场处于关闭状态";
                case EEPROM: return "EEPROM操作出错";
                case timeOut: return "最大询查时间溢出,但没有得到一张电子标签的UID";
                case moreUID: return "在最大询查时间内没有得到所有电子标签的UID";
                case ISOError: return "ISO error";
                case noElectronicTag: return "无电子标签可操作";
                case operationError: return "操作出错";
                case communicationErr: return "通讯错误";
                case retCRCErr: return "CRC校验错误";
                case retDataErr: return "返回的数据不正确";
                case communicationBusy: return "通讯繁忙，设备正在执行其他指令";
                case cmdNotOperation: return "操作命令当前无法执行";
                case dataRangError: return "操作数范围不符";
                case invalidHandle: return "无效句柄";
                case 0x1F: return "工作模式不正确";
                default:
                    return "";
            }
        }

		/// <summary>
		/// 初始化(连接设备)
		/// </summary>
		/// <returns></returns>
        public MessageModel<bool> Init()
        {
            var res = new MessageModel<bool>();
            int fCmdRet = 0x30;
            int port = 1;
            string s;

            if (!fopencomport.IsZeroOrMinus())
            {
                res.success = true;
                res.msg = "已连接设备，无需再次连接";
                return res;
            }

            fCmdRet = StaticClassReaderB.AutoOpenComPort(ref port, ref readerAddr, ref portFrmIndex);
            if (fCmdRet == 0)
            {
                fopencomport = portFrmIndex;
                res.success = true;
            }
            else
            {
                res.success = false;
                var rtCodeMsg = HFReaderGetReturnCodeDesc(fCmdRet);
                Log.GetLog().SetClassAndMethod().Caption(rtCodeMsg).Debug();
                res.devMsg = rtCodeMsg;
            }
            res.msg = res.success ? "连接设备成功" : "连接设备失败";
            return res;
        }

		/// <summary>
		/// 转换成ISO14443A模式，可不调用
		/// </summary>
		/// <returns></returns>
		public MessageModel<string> ChangeToISO14443A()
        {
            var res = new MessageModel<string>();
            int fCmdRet = 0x30;

            fCmdRet = StaticClassReaderB.ChangeTo14443A(ref readerAddr, portFrmIndex);
            if (fCmdRet != 0)
            {
                var rtCodeMsg = HFReaderGetReturnCodeDesc(fCmdRet);
                var logStr = @$"rtCodeMsg: {rtCodeMsg}";
                Log.GetLog().SetClassAndMethod().Caption(logStr).Debug();
                res.msg = rtCodeMsg;
                res.devMsg = logStr;
                return res;
            }
            res.msg = "切换到14443A成功";
            res.success = true;
            return res;
        }

        /// <summary>
        /// 读指定扇区内的指定块内容
        /// </summary>
        /// <param name="sector">扇区号</param>
        /// <param name="block">块号</param>
        /// <param name="HexKey">密钥</param>
        /// <returns></returns>
        public MessageModel<string> ReadBlock(Int64 sector, Int64 block, string HexKey = "FFFFFFFFFFFF")
        {
            var res = new MessageModel<string>();
            int fCmdRet = 0x30;
            byte[] Data = new byte[2];
            byte errorCode = 0;
            byte reserved = 0;
            byte size = 0;

            fCmdRet = StaticClassReaderB.ISO14443ARequest(ref readerAddr, 1, Data, ref errorCode, portFrmIndex);
            if (fCmdRet != 0)
            {
                var rtCodeMsg = HFReaderGetReturnCodeDesc(fCmdRet);
                var errCodeMsg = GetErrorCodeDesc(errorCode);
                var logStr = @$"rtCodeMsg: {rtCodeMsg} | errCodeMsg: {errCodeMsg}";
                Log.GetLog().SetClassAndMethod().Caption(logStr).Debug();
                res.msg = rtCodeMsg;
                res.devMsg = logStr;
                return res;
            }

            fCmdRet = StaticClassReaderB.ISO14443AAnticoll(ref readerAddr, reserved, SNR, ref errorCode, portFrmIndex);
            if (fCmdRet != 0)
            {
                var rtCodeMsg = HFReaderGetReturnCodeDesc(fCmdRet);
                var errCodeMsg = GetErrorCodeDesc(errorCode);
                var logStr = @$"rtCodeMsg: {rtCodeMsg} | errCodeMsg: {errCodeMsg}";
                Log.GetLog().SetClassAndMethod().Caption(logStr).Debug();
                res.msg = rtCodeMsg;
                res.devMsg = logStr;
                return res;
            }

            fCmdRet = StaticClassReaderB.ISO14443ASelect(ref readerAddr, SNR, ref size, ref errorCode, portFrmIndex);
            if (fCmdRet != 0)
            {
                var rtCodeMsg = HFReaderGetReturnCodeDesc(fCmdRet);
                var errCodeMsg = GetErrorCodeDesc(errorCode);
                var logStr = @$"rtCodeMsg: {rtCodeMsg} | errCodeMsg: {errCodeMsg}";
                Log.GetLog().SetClassAndMethod().Caption(logStr).Debug();
                res.msg = rtCodeMsg;
                res.devMsg = logStr;
                return res;
            }

            var key = SerialPortHelper.HexStringToByteArray(HexKey);
            fCmdRet = StaticClassReaderB.ISO14443AAuthKey(ref readerAddr, beforeKeyStyle, (byte)sector, key, ref errorCode, portFrmIndex);
            if (fCmdRet != 0)
            {
                var rtCodeMsg = HFReaderGetReturnCodeDesc(fCmdRet);
                var errCodeMsg = GetErrorCodeDesc(errorCode);
                var logStr = @$"rtCodeMsg: {rtCodeMsg} | errCodeMsg: {errCodeMsg}";
                Log.GetLog().SetClassAndMethod().Caption(logStr).Debug();
                res.msg = rtCodeMsg;
                res.devMsg = logStr;
                return res;
            }

            byte[] data = new byte[16];
            fCmdRet = StaticClassReaderB.ISO14443ARead(ref readerAddr, (byte)block, data, ref errorCode, portFrmIndex);
            if (fCmdRet != 0)
            {
                var rtCodeMsg = HFReaderGetReturnCodeDesc(fCmdRet);
                var errCodeMsg = GetErrorCodeDesc(errorCode);
                var logStr = @$"rtCodeMsg: {rtCodeMsg} | errCodeMsg: {errCodeMsg}";
                Log.GetLog().SetClassAndMethod().Caption(logStr).Debug();
                res.msg = rtCodeMsg;
                res.devMsg = logStr;
                return res;
            }

            res.response = SerialPortHelper.ByteArrayToHexString(data).Replace(" ", "");
            res.success = true;
            res.msg = "读取成功";
            return res;
        }

		/// <summary>
		/// 读卡号
		/// </summary>
		/// <returns></returns>
        public MessageModel<string> ReadCardNo()
        {
            var res = new MessageModel<string>();
            int fCmdRet = 0x30;
            byte[] Data = new byte[2];
            byte errorCode = 0;
            byte reserved = 0;

            fCmdRet = StaticClassReaderB.ISO14443ARequest(ref readerAddr, 1, Data, ref errorCode, portFrmIndex);
            if (fCmdRet != 0)
            {
                var rtCodeMsg = HFReaderGetReturnCodeDesc(fCmdRet);
                var errCodeMsg = GetErrorCodeDesc(errorCode);
                var logStr = @$"rtCodeMsg: {rtCodeMsg} | errCodeMsg: {errCodeMsg}";
                Log.GetLog().SetClassAndMethod().Caption(logStr).Debug();
                res.msg = rtCodeMsg;
                res.devMsg = logStr;
                return res;
            }

            fCmdRet = StaticClassReaderB.ISO14443AAnticoll(ref readerAddr, reserved, SNR, ref errorCode, portFrmIndex);
            if (fCmdRet != 0)
            {
                var rtCodeMsg = HFReaderGetReturnCodeDesc(fCmdRet);
                var errCodeMsg = GetErrorCodeDesc(errorCode);
                var logStr = @$"rtCodeMsg: {rtCodeMsg} | errCodeMsg: {errCodeMsg}";
                Log.GetLog().SetClassAndMethod().Caption(logStr).Debug();
                res.msg = rtCodeMsg;
                res.devMsg = logStr;
                return res;
            }
			res.response = IcSettings.DataHandle(SerialPortHelper.ByteArrayToHexString(SNR).Replace(" ", ""), _systemFunc.LibrarySettings?.IcSettings);
			res.success = true;
            res.msg = "读卡号成功";
            return res;
        }
    }

	public class StaticClassReaderB
	{
		private const string DLLNAME = @"RR7036.dll";

		[DllImport(DLLNAME)]
		public static extern int OpenNetPort(int Port,
			string IPaddr,
			ref byte ComAddr,
			ref int PortHandle);

		[DllImport(DLLNAME)]
		public static extern int CloseNetPort(int PortHandle);

		[DllImport(DLLNAME)]
		public static extern int OpenComPort(int Port,
			ref byte ComAddr,
			ref int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int CloseComPort();

		[DllImport(DLLNAME)]
		public static extern int AutoOpenComPort(ref int Port,
			ref byte ComAddr,
			ref int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int CloseSpecComPort(int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Inventory(ref byte ComAddr,
			ref byte State,
			ref byte AFI,
			byte[] DSFIDAndUID,
			ref byte CardNum,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int StayQuiet(ref byte ComAddr,
			byte[] UID,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ReadSingleBlock(ref byte ComAddr,
			ref byte State,
			byte[] UID,
			byte BlockNum,
			ref byte BlockSecStatus,
			byte[] Data,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ReadMultipleBlock(ref byte ComAddr,
			ref byte State,
			byte[] UID,
			byte BlockNum,
			byte BlockCount,
			byte[] BlockSecStatus,
			byte[] Data,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int WriteSingleBlock(ref byte ComAddr,
			ref byte State,
			byte[] UID,
			byte BlockNum,
			byte[] Data,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int LockBlock(ref byte ComAddr,
			ref byte State,
			byte[] UID,
			byte BlockNum,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Select(ref byte ComAddr,
			byte[] UID,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ResetToReady(ref byte ComAddr,
			ref byte State,
			byte[] UID,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int WriteAFI(ref byte ComAddr,
			ref byte State,
			byte[] UID,
			byte AFI,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int LockAFI(ref byte ComAddr,
			ref byte State,
			byte[] UID,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int WriteDSFID(ref byte ComAddr,
			ref byte State,
			byte[] UID,
			byte DSFID,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int LockDSFID(ref byte ComAddr,
			ref byte State,
			byte[] UID,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int GetSystemInformation(ref byte ComAddr,
			ref byte State,
			byte[] UIDI,
			ref byte InformationFlag,
			byte[] UIDO,
			ref byte DSFID,
			ref byte AFI,
			byte[] MemorySize,
			ref byte ICReference,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int GetReaderInformation(ref byte ComAddr,
			byte[] VersionInfo,
			ref byte ReaderType,
			byte[] TrType,
			ref byte InventoryScanTime,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ChangeTo14443A(ref byte ComAddr,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ChangeTo14443B(ref byte ComAddr,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ChangeTo15693(ref byte ComAddr,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int SetLED(ref byte ComAddr,
			byte OpenTime,
			byte CloseTime,
			byte RepeatCount,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int SetBeep(ref byte ComAddr,
			byte OpenTime,
			byte CloseTime,
			byte RepeatCount,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int OpenRf(ref byte ComAddr,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int CloseRf(ref byte ComAddr,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int WriteComAdr(ref byte ComAddr,
			ref byte NewComAddr,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int WriteInventoryScanTime(ref byte ComAddr,
			ref byte InventoryScanTime,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443ARequest(ref byte ComAddr,
			byte Mode,
			byte[] Tagtype,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443AAnticoll(ref byte ComAddr,
			byte Reserved,
			byte[] UID,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443AAnticoll2(ref byte ComAddr,
			byte EnColl,
			byte Reserved,
			byte[] UID,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443AULAnticoll(ref byte ComAddr,
			byte Reserved,
			byte[] UID,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443AAnticollLevel1(ref byte ComAdr,
			byte Reserved,
			byte[] SN,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443ASelectLevel1(ref byte ComAdr,
			byte[] SN,
			ref byte size,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443AAnticollLevel2(ref byte ComAdr,
			byte Reserved,
			byte[] SN,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443ASelectLevel2(ref byte ComAdr,
			byte[] SN,
			ref byte size,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443AAnticollLevel3(ref byte ComAdr,
			byte Reserved,
			byte[] SN,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443ASelectLevel3(ref byte ComAdr,
			byte[] SN,
			ref byte size,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443ASelect(ref byte ComAddr,
			byte[] UID,
			ref byte Size,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443AAuthentication(ref byte ComAddr,
			byte Mode,
			byte SecNum,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443AAuthentication2(ref byte ComAddr,
			byte Mode,
			byte AccessSector,
			byte KeySector,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443AAuthKey(ref byte ComAddr,
			byte Mode,
			byte AuthSector,
			byte[] Key,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443AHalt(ref byte ComAddr,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443ARead(ref byte ComAddr,
			byte BlockNum,
			byte[] ReadData,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443AWrite(ref byte ComAddr,
			byte BlockNum,
			byte[] WrittenData,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443AULWrite(ref byte ComAddr,
			byte ULPage,
			byte[] Data,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443AInitValue(ref byte ComAddr,
			byte BlockNum,
			byte[] InitValueData,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443AReadValue(ref byte ComAddr,
			byte BlockNum,
			byte[] Value,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443AIncrement(ref byte ComAddr,
			byte BlockNum,
			byte[] IncrementData,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443ADecrement(ref byte ComAddr,
			byte BlockNum,
			byte[] DecrementData,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ISO14443ARestore(ref byte ComAddr,
			byte BlockNum,
			ref byte ErrorCode,
			int FrmHandle);


		[DllImport(DLLNAME)]
		public static extern int ISO14443ATransfer(ref byte ComAddr,
			byte BlockNum,
			ref byte ErrorCode,
			int FrmHandle);


		[DllImport(DLLNAME)]
		public static extern int ISO14443ALoadKey(ref byte ComAddr,
			byte Mode,
			byte SecNum,
			byte[] Key,
			ref byte ErrorCode,
			int FrmHandle);


		[DllImport(DLLNAME)]
		public static extern int ISO14443ACheckWrite(ref byte ComAddr,
			byte[] UID,
			byte Mode,
			byte BlockNum,
			byte[] Data,
			ref byte ErrorCode,
			int FrmHandle);


		[DllImport(DLLNAME)]
		public static extern int ISO14443AReadE2(ref byte ComAddr,
			byte StartAddr,
			byte DataLength,
			byte[] ReadE2Data,
			ref byte ErrorCode,
			int FrmHandle);


		[DllImport(DLLNAME)]
		public static extern int ISO14443AWriteE2(ref byte ComAddr,
			byte E2Addr,
			byte DataLength,
			byte[] Data,
			ref byte ErrorCode,
			int FrmHandle);


		[DllImport(DLLNAME)]
		public static extern int ISO14443AValue(ref byte ComAddr,
			byte Mode,
			byte SourceAddr,
			byte[] ValueData,
			byte TargetAddr,
			ref byte ErrorCode,
			int FrmHandle);


		[DllImport(DLLNAME)]
		public static extern int ISO14443BRequest(ref byte ComAddr,
			byte Mode,
			byte AFI,
			byte[] PUPI,
			byte[] ApplicationData,
			byte[] ProtocolInfo,
			ref byte ErrorCode,
			int FrmHandle);


		[DllImport(DLLNAME)]
		public static extern int ISO14443BAnticoll(ref byte ComAddr,
			byte Mode,
			byte AFI,
			byte[] PUPI,
			byte[] ApplicationData,
			byte[] ProtocolInfo,
			ref byte ErrorCode,
			int FrmHandle);


		[DllImport(DLLNAME)]
		public static extern int ISO14443BSelect(ref byte ComAddr,
			byte[] PUPI,
			byte CID,
			byte param1,
			byte param2,
			byte param3,
			byte param4,
			ref byte CIDO,
			ref byte MBLI,
			ref byte ErrorCode,
			int FrmHandle);


		[DllImport(DLLNAME)]
		public static extern int ISO14443BHalt(ref byte ComAddr,
			byte[] PUPI,
			ref byte ErrorCode,
			int FrmHandle);


		[DllImport(DLLNAME)]
		public static extern int ISO14443BTransparentCmd(ref byte ComAddr,
			byte Time_M,
			byte Time_N,
			byte CMD_Length,
			byte RSP_Length,
			byte[] CMD_Data,
			byte[] RSP_Data,
			ref byte ErrorCode,
			int FrmHandle);


		[DllImport(DLLNAME)]
		public static extern int ISO15693TransparentCmd(ref byte ComAddr,
			byte Time_M,
			byte Time_N,
			byte CMD_Length,
			byte RSP_Length,
			byte[] CMD_Data,
			byte[] RSP_Data,
			int FrmHandle);


		[DllImport(DLLNAME)]
		public static extern int ISO14443ATransparentCmd(ref byte ComAddr,
			byte Time_M,
			byte Time_N,
			byte CMD_Length,
			ref byte RSP_Length,
			byte[] CMD_Data,
			byte[] RSP_Data,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int GetUIDofHID(ref byte ComAddr,
			byte[] UID,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int SRI512_4K_Initiate(ref byte ComAddr,
			byte[] chipID,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int SRI512_4K_Select(ref byte ComAddr,
			byte[] chipID,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int SRI512_4K_PCall16(ref byte ComAddr,
			byte[] chipID,
			ref byte ErrorCode,
			int FrmHandle);


		[DllImport(DLLNAME)]
		public static extern int SRI512_4K_ResetToInventory(ref byte ComAddr,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int SRI512_4K_GetUID(ref byte ComAddr,
			byte[] chipID,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int SRI512_4K_SlotMarker(ref byte ComAddr,
			byte SlotNumber,
			byte[] chipID,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int SRI512_4K_Completion(ref byte ComAddr,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int SRI512_4K_ReadBlock(ref byte ComAddr,
			byte blockaddr,
			byte[] blockdata,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int SRI512_4K_WriteBlock(ref byte ComAddr,
			byte blockaddr,
			byte[] blockdata,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ICODE_UID_Destroy(ref byte ComAddr,
			byte[] IDD,
			byte[] DestoryCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ICODE_UID_Write(ref byte ComAddr,
			byte blockStartAddr,
			byte len,
			byte[] blockdata,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ICODE_UID_Inventory(ref byte ComAddr,
			byte[] IDDData,
			ref byte cardNum,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ChangeToStandby(ref byte ComAddr,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ChangeToICODEUID(ref byte ComAddr,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Des(ref byte ComAddr,
			byte nLen,
			byte[] inputData,
			byte CMD_Length,
			byte[] outputData,
			int mode,
			int encrypt);

		[DllImport(DLLNAME)]
		public static extern int GetChallenge28(ref byte ComAddr,
			byte[] CMD_Data,
			byte CMD_Length,
			byte[] RSP_Data,
			ref byte RSP_Length,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int SetRATS28(ref byte ComAddr,
			byte[] CMD_Data,
			byte CMD_Length,
			byte[] RSP_Data,
			ref byte RSP_Length,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int SetPPS28(ref byte ComAddr,
			byte[] CMD_Data,
			byte CMD_Length,
			byte[] RSP_Data,
			ref byte RSP_Length,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ExternalAuthenticate28(ref byte ComAddr,
			byte[] CMD_Data,
			byte CMD_Length,
			byte[] RSP_Data,
			ref byte RSP_Length,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int InternalAuthenticate28(ref byte ComAddr,
			byte[] CMD_Data,
			byte CMD_Length,
			byte[] RSP_Data,
			ref byte RSP_Length,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Select28(ref byte ComAddr,
			byte[] CMD_Data,
			byte CMD_Length,
			byte[] RSP_Data,
			ref byte RSP_Length,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ReadRecord28(ref byte ComAddr,
			byte[] CMD_Data,
			byte CMD_Length,
			byte[] RSP_Data,
			ref byte RSP_Length,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Erase28(ref byte ComAddr,
			byte[] CMD_Data,
			byte CMD_Length,
			byte[] RSP_Data,
			ref byte RSP_Length,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int CreateFile28(ref byte ComAddr,
			byte[] CMD_Data,
			byte CMD_Length,
			byte[] RSP_Data,
			ref byte RSP_Length,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Writekey28(ref byte ComAddr,
			byte[] CMD_Data,
			byte CMD_Length,
			byte[] RSP_Data,
			ref byte RSP_Length,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int UpdateBinary28(ref byte ComAddr,
			byte[] CMD_Data,
			byte CMD_Length,
			byte[] RSP_Data,
			ref byte RSP_Length,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int UpdateRecord28(ref byte ComAddr,
			byte[] CMD_Data,
			byte CMD_Length,
			byte[] RSP_Data,
			ref byte RSP_Length,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int AppendRecord28(ref byte ComAddr,
			byte[] CMD_Data,
			byte CMD_Length,
			byte[] RSP_Data,
			ref byte RSP_Length,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Verifypin28(ref byte ComAddr,
			byte[] CMD_Data,
			byte CMD_Length,
			byte[] RSP_Data,
			ref byte RSP_Length,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int ReadBinaryFile28(ref byte ComAddr,
			byte[] CMD_Data,
			byte CMD_Length,
			byte[] RSP_Data,
			ref byte RSP_Length,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int SendWTX28(ref byte ComAddr,
			byte[] CMD_Data,
			byte CMD_Length,
			byte[] RSP_Data,
			ref byte RSP_Length,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern bool CheckWTX28(byte[] CMD_Data,
			byte CMD_Length);

		[DllImport(DLLNAME)]
		public static extern int Desfire_Rats(ref byte ComAddr,
			byte CID,
			byte[] RSP_Data,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_Pps(ref byte ComAddr,
			byte CID,
			byte Divisor,
			byte[] RSP_Data,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_Authenticate(ref byte ComAddr,
			byte CID,
			byte KeyNo,
			byte[] keys,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_ChangeKeySettings(ref byte ComAddr,
			byte CID,
			byte ChangeKeySettings,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_GetKeySettings(ref byte ComAddr,
			byte CID,
			ref byte KeySettings,
			ref byte MaxNoOfKeys,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_ChangeKey(ref byte ComAddr,
			byte CID,
			byte KeyNo,
			byte[] OldKey,
			byte[] NewKey,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_GetKeyVersion(ref byte ComAddr,
			byte CID,
			byte KeyNo,
			ref byte KeyVer,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_CreateApplication(ref byte ComAddr,
			byte CID,
			byte[] AID,
			byte KeySettings,
			byte NumberOfKeys,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_DeleteApplication(ref byte ComAddr,
			byte CID,
			byte[] AID,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_GetApplicationIDs(ref byte ComAddr,
			byte CID,
			byte[] AID,
			ref byte nNum,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_SelectApplication(ref byte ComAddr,
			byte CID,
			byte[] AID,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_FormatPICC(ref byte ComAddr,
			byte CID,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_GetVersion(ref byte ComAddr,
			byte CID,
			byte[] VerInfo,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_GetFileIDs(ref byte ComAddr,
			byte CID,
			byte[] FileIDs,
			ref byte NumberOfFileIDs,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_GetFileSettings(ref byte ComAddr,
			byte CID,
			byte FileNo,
			byte[] FileSettings,
			ref byte nLen,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_ChangeFileSettings(ref byte ComAddr,
			byte CID,
			byte FileNo,
			byte CommSet,
			byte[] AccessRights,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_CreateStdDataFile(ref byte ComAddr,
			byte CID,
			byte FileNo,
			byte CommSet,
			byte[] AccessRights,
			byte[] FileSize,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_CreateBackupDataFile(ref byte ComAddr,
			byte CID,
			byte FileNo,
			byte CommSet,
			byte[] AccessRights,
			byte[] FileSize,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_CreateValueFile(ref byte ComAddr,
			byte CID,
			byte FileNo,
			byte CommSet,
			byte[] AccessRights,
			byte[] LowerLimit,
			byte[] UpperLimit,
			byte[] Value,
			byte LimitedCreditEnabled,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_CreateLinearRecordDataFile(ref byte ComAddr,
			byte CID,
			byte FileNo,
			byte CommSet,
			byte[] AccessRights,
			byte[] RecordSize,
			byte[] MaxNumOfRecords,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_CreateCyclicRecordDataFile(ref byte ComAddr,
			byte CID,
			byte FileNo,
			byte CommSet,
			byte[] AccessRights,
			byte[] RecordSize,
			byte[] MaxNumOfRecords,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_DeleteFile(ref byte ComAddr,
			byte CID,
			byte FileNo,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_ReadData(ref byte ComAddr,
			byte CID,
			byte FileNo,
			byte[] Offset,
			byte[] Length,
			byte[] Data,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_WriteData(ref byte ComAddr,
			byte CID,
			byte FileNo,
			byte[] Offset,
			byte[] Length,
			byte[] Data,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_GetValue(ref byte ComAddr,
			byte CID,
			byte FileNo,
			byte[] Data,
			ref byte DataLen,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_Credit(ref byte ComAddr,
			byte CID,
			byte FileNo,
			byte[] Data,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_Debit(ref byte ComAddr,
			byte CID,
			byte FileNo,
			byte[] Data,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_LitmitedCredit(ref byte ComAddr,
			byte CID,
			byte FileNo,
			byte[] Data,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_ReadRecord(ref byte ComAddr,
			byte CID,
			byte FileNo,
			byte[] Offset,
			byte[] Length,
			byte[] Data,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_WriteRecord(ref byte ComAddr,
			byte CID,
			byte FileNo,
			byte[] Offset,
			byte[] Length,
			byte[] Data,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_ClearRecordFile(ref byte ComAddr,
			byte CID,
			byte FileNo,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_CommitTransaction(ref byte ComAddr,
			byte CID,
			ref byte ErrorCode,
			int FrmHandle);

		[DllImport(DLLNAME)]
		public static extern int Desfire_AbortTransaction(ref byte ComAddr,
			byte CID,
			ref byte ErrorCode,
			int FrmHandle);
	}
}
