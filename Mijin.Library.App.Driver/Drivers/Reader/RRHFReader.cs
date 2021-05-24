using Mijin.Library.App.Model;
using ReaderB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver.Drivers.Reader
{
    public class RRHFReader : IHFReader
    {
        private byte readerAddr = 0xff;
        private int portFrmIndex = -1;
        private int fopencomport = -1;

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
        public MessageModel<bool> Init()
        {
            int fCmdRet = 0x30;
            int port = 1;
            string s;
            fCmdRet = StaticClassReaderB.AutoOpenComPort(ref port, ref readerAddr, ref portFrmIndex);
            if (fCmdRet == 0)
            {
                fopencomport = portFrmIndex;
            }
            else
            {
                
            }
            return null;
        }

        public MessageModel<string> ReadBlock(string block)
        {
            return null;
        }

        public MessageModel<string> ReadCardNo()
        {
            return null;
        }
    }
}
