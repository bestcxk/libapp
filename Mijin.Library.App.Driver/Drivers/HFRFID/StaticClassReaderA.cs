﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver
{
    public static class StaticClassReaderA
    {
        private const string DLLNAME = @"HFReader9.dll";

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
    }
}
