using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using IsUtil.Helpers;
using Mijin.Library.App.Driver.Interface;
using Mijin.Library.App.Model;
using Mijin.Library.App.Model.Setting;
using String = System.String;

namespace Mijin.Library.App.Driver.Drivers.Reader
{
    /// <summary>
    /// 黑色高频读卡器
    /// 可直接调用读卡方法，读卡方法会进行初始化
    /// </summary>
    public class BlackHFReader : IBlackHFReader
    {
        public ISystemFunc _systemFunc { get; }


        public BlackHFReader(ISystemFunc systemFunc)
        {
            _systemFunc = systemFunc;
        }

        #region DllImport
        [DllImport("kernel32.dll")]
        static extern void Sleep(int dwMilliseconds);
        //=========================== System Function =============================
        [DllImport("hfrdapi.dll")]
        static extern int Sys_GetDeviceNum(UInt16 vid, UInt16 pid, ref UInt32 pNum);

        [DllImport("hfrdapi.dll")]
        static extern int Sys_GetHidSerialNumberStr(UInt32 deviceIndex,
                                                    UInt16 vid,
                                                    UInt16 pid,
                                                    [Out] StringBuilder deviceString,
                                                    UInt32 deviceStringLength);

        [DllImport("hfrdapi.dll")]
        static extern int Sys_Open(ref IntPtr device,
                                   UInt32 index,
                                   UInt16 vid,
                                   UInt16 pid);

        [DllImport("hfrdapi.dll")]
        static extern bool Sys_IsOpen(IntPtr device);

        [DllImport("hfrdapi.dll")]
        static extern int Sys_Close(ref IntPtr device);

        [DllImport("hfrdapi.dll")]
        static extern int Sys_SetLight(IntPtr device, byte color);

        [DllImport("hfrdapi.dll")]
        static extern int Sys_SetBuzzer(IntPtr device, byte msec);

        [DllImport("hfrdapi.dll")]
        static extern int Sys_SetAntenna(IntPtr device, byte mode);

        [DllImport("hfrdapi.dll")]
        static extern int Sys_InitType(IntPtr device, byte type);

        //========================= ISO15693 Card Function =========================
        [DllImport("hfrdapi.dll")]
        static extern int I15693_Inventory(IntPtr device, byte[] pData, ref byte pLen);

        [DllImport("hfrdapi.dll")]
        static extern int I15693_Inventorys(IntPtr device, byte[] pData, ref byte pLen);

        [DllImport("hfrdapi.dll")]
        static extern int I15693_GetSystemInformation(IntPtr device,
                                                      byte mode,
                                                      byte[] pUID,
                                                      byte[] pData,
                                                      ref byte pLen);

        [DllImport("hfrdapi.dll")]
        static extern int I15693_Select(IntPtr device, byte[] pUID);

        [DllImport("hfrdapi.dll")]
        static extern int I15693_ResetToReady(IntPtr device, byte mode, byte[] pUID);

        [DllImport("hfrdapi.dll")]
        static extern int I15693_StayQuiet(IntPtr device, byte[] pUID);

        [DllImport("hfrdapi.dll")]
        static extern int I15693_GetBlockSecurity(IntPtr device,
                                                  byte mode,
                                                  byte[] pUID,
                                                  byte block,
                                                  byte number,
                                                  byte[] pData,
                                                  ref byte pLen);

        [DllImport("hfrdapi.dll")]
        static extern int I15693_Read(IntPtr device,
                                      byte mode,
                                      byte[] pUID,
                                      byte block,
                                      byte number,
                                      byte[] pData,
                                      ref byte pLen);

        [DllImport("hfrdapi.dll")]
        static extern int I15693_Write(IntPtr device,
                                       byte mode,
                                       byte[] pUID,
                                       byte block,
                                       byte[] pData);

        [DllImport("hfrdapi.dll")]
        static extern int I15693_LockBlock(IntPtr device,
                                           byte mode,
                                           byte[] pUID,
                                           byte block);

        [DllImport("hfrdapi.dll")]
        static extern int I15693_WriteAFI(IntPtr device,
                                          byte mode,
                                          byte[] pUID,
                                          byte AFI);

        [DllImport("hfrdapi.dll")]
        static extern int I15693_LockAFI(IntPtr device, byte mode, byte[] pUID);

        [DllImport("hfrdapi.dll")]
        static extern int I15693_WriteDSFID(IntPtr device,
                                            byte mode,
                                            byte[] pUID,
                                            byte DSFID);

        [DllImport("hfrdapi.dll")]
        static extern int I15693_LockDSFID(IntPtr device, byte mode, byte[] pUID);

        [DllImport("hfrdapi.dll")]
        static extern int I15693_DirectTransmit(IntPtr device,
                                                byte[] pCommand,
                                                byte cmdLen,
                                                byte[] pData,
                                                ref byte pMsgLg);


        //=========================== M1 Card Function =============================
        [DllImport("hfrdapi.dll")]
        static extern int TyA_Request(IntPtr device, byte mode, ref UInt16 pTagType);

        [DllImport("hfrdapi.dll")]
        static extern int TyA_Anticollision(IntPtr device,
                                            byte bcnt,
                                            byte[] pSnr,
                                            ref byte pLen);

        [DllImport("hfrdapi.dll")]
        static extern int TyA_Select(IntPtr device,
                                     byte[] pSnr,
                                     byte snrLen,
                                     ref byte pSak);

        [DllImport("hfrdapi.dll")]
        static extern int TyA_Halt(IntPtr device);

        [DllImport("hfrdapi.dll")]
        static extern int TyA_CS_Authentication2(IntPtr device,
                                                 byte mode,
                                                 byte block,
                                                 byte[] pKey);

        [DllImport("hfrdapi.dll")]
        static extern int TyA_CS_Read(IntPtr device,
                                      byte block,
                                      byte[] pData,
                                      ref byte pLen);

        [DllImport("hfrdapi.dll")]
        static extern int TyA_CS_Write(IntPtr device, byte block, byte[] pData);

        [DllImport("hfrdapi.dll")]
        static extern int TyA_CS_InitValue(IntPtr device, byte block, Int32 value);

        [DllImport("hfrdapi.dll")]
        static extern int TyA_CS_ReadValue(IntPtr device, byte block, ref Int32 pValue);

        [DllImport("hfrdapi.dll")]
        static extern int TyA_CS_Decrement(IntPtr device, byte block, Int32 value);

        [DllImport("hfrdapi.dll")]
        static extern int TyA_CS_Increment(IntPtr device, byte block, Int32 value);

        [DllImport("hfrdapi.dll")]
        static extern int TyA_CS_Restore(IntPtr device, byte block);

        [DllImport("hfrdapi.dll")]
        static extern int TyA_CS_Transfer(IntPtr device, byte block);

        IntPtr g_hDevice = (IntPtr)(-1); //g_hDevice must init as -1
        #endregion


        //private bool 

        #region 扩展方法

        
        private static String byteHEX(Byte ib)
        {
            String _str = String.Empty;
            try
            {
                char[] Digit = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A',
                'B', 'C', 'D', 'E', 'F' };
                char[] ob = new char[2];
                ob[0] = Digit[(ib >> 4) & 0X0F];
                ob[1] = Digit[ib & 0X0F];
                _str = new String(ob);
            }
            catch (Exception)
            {
                new Exception("byteHEX error !");
            }
            return _str;

        }

        private static byte GetHexBitsValue(byte ch)
        {
            byte sz = 0;
            if (ch <= '9' && ch >= '0')
                sz = (byte)(ch - 0x30);
            if (ch <= 'F' && ch >= 'A')
                sz = (byte)(ch - 0x37);
            if (ch <= 'f' && ch >= 'a')
                sz = (byte)(ch - 0x57);

            return sz;
        }

        private static byte[] ToDigitsBytes(string theHex)
        {
            byte[] bytes = new byte[theHex.Length / 2 + (((theHex.Length % 2) > 0) ? 1 : 0)];
            for (int i = 0; i < bytes.Length; i++)
            {
                char lowbits = theHex[i * 2];
                char highbits;

                if ((i * 2 + 1) < theHex.Length)
                    highbits = theHex[i * 2 + 1];
                else
                    highbits = '0';

                int a = (int)GetHexBitsValue((byte)lowbits);
                int b = (int)GetHexBitsValue((byte)highbits);
                bytes[i] = (byte)((a << 4) + b);
            }

            return bytes;
        }

        private static string ToHexString(byte[] bytes, byte len)
        {
            String hexString = String.Empty;
            for (int i = 0; i < len; i++)
                hexString += byteHEX(bytes[i]);

            return hexString;
        }
        #endregion


        public MessageModel<string> ReadBlockForTC()
        {
            MessageModel<string> result = new MessageModel<string>();
            result.msg = "读卡失败";

            int status;
            byte bcnt = 0;
            byte[] dataBuffer = new byte[64];
            byte lenByUid = 0;
            byte block;
            //IntPtr pSnr;
            byte len1 = 0;
            byte readLen2 = 0;
            byte readLen3 = 0;
            byte readLen4 = 0;
            byte readLen5 = 0;
            byte readLen6 = 0;

            //检查读卡器是否连接
            if (true != Sys_IsOpen(g_hDevice))
            {
                // 未连接则进行初始化
                var initResult = this.InitBy15693();

                // 初始化失败则直接返回结果
                if (!initResult.success)
                    return new MessageModel<string>(initResult);
                //return new MessageModel<string>(initResult);
            }

            status = I15693_Inventory(g_hDevice, dataBuffer, ref len1);//获取UID
            if (status != 0)
            {
                result.devMsg = "I15693_Inventory failed !";
                return result;
            }

            byte[] bytesData = new byte[64];
            for (int i = 0; i < 8; i++)
                bytesData[i] = Marshal.ReadByte(dataBuffer, i + 1);
            string Uid = ToHexString(bytesData, 8);

            //=================== Read block data =====================
            byte[] bytesUID = ToDigitsBytes(Uid);
            block = Convert.ToByte("0");
            status = I15693_Read(g_hDevice, 0x02, bytesUID, block, 1, dataBuffer, ref readLen2);//读取区块0数据
            if (status != 0)
            {
                result.devMsg = "I15693_Read failed !";
                return result;
            }

            byte[] bytesData1 = new byte[64];
            for (int i = 0; i < readLen2; i++)
                bytesData1[i] = Marshal.ReadByte(dataBuffer, i);

            block = Convert.ToByte("1");
            status = I15693_Read(g_hDevice, 0x02, bytesUID, block, 1, dataBuffer, ref readLen3);//读取区块1数据
            if (status != 0)
            {
                result.devMsg = "I15693_Read1 failed !";
                return result;
            }

            byte[] bytesData2 = new byte[64];
            for (int i = 0; i < readLen3; i++)
                bytesData2[i] = Marshal.ReadByte(dataBuffer, i);

            block = Convert.ToByte("2");
            status = I15693_Read(g_hDevice, 0x02, bytesUID, block, 1, dataBuffer, ref readLen4); //读取区块2数据
            if (status != 0)
            {
                result.devMsg = "I15693_Read1 failed !";
                return result;
            }
            byte[] bytesData3 = new byte[64];
            for (int i = 0; i < readLen4; i++)
                bytesData3[i] = Marshal.ReadByte(dataBuffer, i);

            block = Convert.ToByte("3");
            status = I15693_Read(g_hDevice, 0x02, bytesUID, block, 1, dataBuffer, ref readLen5); //读取区块2数据
            if (status != 0)
            {
                result.devMsg = "I15693_Read1 failed !";
                return result;
            }
            byte[] bytesData4 = new byte[64];
            for (int i = 0; i < readLen5; i++)
                bytesData4[i] = Marshal.ReadByte(dataBuffer, i);

            block = Convert.ToByte("4");
            status = I15693_Read(g_hDevice, 0x02, bytesUID, block, 1, dataBuffer, ref readLen6); //读取区块2数据
            if (status != 0)
            {
                result.devMsg = "I15693_Read1 failed !";
                return result;
            }
            byte[] bytesData5 = new byte[64];
            for (int i = 0; i < readLen6; i++)
                bytesData5[i] = Marshal.ReadByte(dataBuffer, i);

            result.response = ToHexString(bytesData1, readLen2) + ToHexString(bytesData2, readLen3)+ ToHexString(bytesData3, readLen4) + ToHexString(bytesData4, readLen5) + ToHexString(bytesData5, readLen6); //  Encoding.ASCII.GetString(bts, 0, bts.Length);

            result.success = true;
            result.msg = "读卡成功";

            return result;
        }

        public MessageModel<bool> InitBy15693()
        {
            MessageModel<bool> result = new MessageModel<bool>();
            result.msg = "初始化失败";
            int status;
            //=========================== Connect reader =========================
            //Check whether the reader is connected or not
            if (true == Sys_IsOpen(g_hDevice))
            {
                //If the reader is already open , close it firstly
                status = Sys_Close(ref g_hDevice);
                if (0 != status)
                {
                    result.devMsg = "Sys_Close failed !";
                    result.msg = "连接读卡器失败";
                    return result;
                }
            }

            //Connect
            status = Sys_Open(ref g_hDevice, 0, 0x0416, 0x8020);
            if (0 != status)
            {
                result.devMsg = "Sys_Open failed !";
                result.msg = "连接读卡器失败";
                return result;
            }


            //============= Init the reader before operating the card ============
            //Close antenna of the reader
            status = Sys_SetAntenna(g_hDevice, 0);
            if (0 != status)
            {
                result.devMsg = "Sys_SetAntenna failed !";
                return result;
            }
            Sleep(5); //Appropriate delay after Sys_SetAntenna operating 

            //Set the reader's working mode
            status = Sys_InitType(g_hDevice, (byte)'1');
            if (0 != status)
            {
                result.devMsg = "Sys_InitType failed !";
                return result;
            }
            Sleep(5); //Appropriate delay after Sys_InitType operating

            //Open antenna of the reader
            status = Sys_SetAntenna(g_hDevice, 1);
            if (0 != status)
            {
                result.devMsg = "Sys_SetAntenna failed !";
                return result;
            }
            Sleep(5); //Appropriate delay after Sys_SetAntenna operating


            //============================ Success Tips ==========================
            //Beep 200 ms
            status = Sys_SetBuzzer(g_hDevice, 20);
            if (0 != status)
            {
                result.devMsg = "Sys_SetBuzzer failed !";
                return result;
            }
            result.success = true;
            result.msg = "初始化成功";
            return result;
        }

        /// <summary>
        /// 初始化设备
        /// </summary>
        /// <returns></returns>
        public MessageModel<bool> Init()
        {
            MessageModel<bool> result = new MessageModel<bool>();
            result.msg = "初始化失败";
            int status;
            //=========================== Connect reader =========================
            //Check whether the reader is connected or not
            if (true == Sys_IsOpen(g_hDevice))
            {
                //If the reader is already open , close it firstly
                status = Sys_Close(ref g_hDevice);
                if (0 != status)
                {
                    result.devMsg = "Sys_Close failed !";
                    result.msg = "连接读卡器失败";
                    return result;
                }
            }

            //Connect
            status = Sys_Open(ref g_hDevice, 0, 0x0416, 0x8020);
            if (0 != status)
            {
                result.devMsg = "Sys_Open failed !";
                result.msg = "连接读卡器失败";
                return result;
            }


            //============= Init the reader before operating the card ============
            //Close antenna of the reader
            status = Sys_SetAntenna(g_hDevice, 0);
            if (0 != status)
            {
                result.devMsg = "Sys_SetAntenna failed !";
                return result;
            }
            Sleep(5); //Appropriate delay after Sys_SetAntenna operating 

            //Set the reader's working mode
            status = Sys_InitType(g_hDevice, (byte)'A');
            if (0 != status)
            {
                result.devMsg = "Sys_InitType failed !";
                return result;
            }
            Sleep(5); //Appropriate delay after Sys_InitType operating

            //Open antenna of the reader
            status = Sys_SetAntenna(g_hDevice, 1);
            if (0 != status)
            {
                result.devMsg = "Sys_SetAntenna failed !";
                return result;
            }
            Sleep(5); //Appropriate delay after Sys_SetAntenna operating


            //============================ Success Tips ==========================
            //Beep 200 ms
            status = Sys_SetBuzzer(g_hDevice, 20);
            if (0 != status)
            {
                result.devMsg = "Sys_SetBuzzer failed !";
                return result;
            }
            result.success = true;
            result.msg = "初始化成功";
            return result;
        }

        /// <summary>
        /// 读卡，返回卡号
        /// </summary>
        /// <returns></returns>
        public MessageModel<string> ReadCardNo()
        {
            MessageModel<string> result = new MessageModel<string>();
            result.msg = "读卡失败";
            byte mode = 0x26;
            ushort TagType = 0;
            int status;
            byte bcnt = 0;
            byte[] dataBuffer = new byte[256];
            //IntPtr pSnr;
            byte len = 255;
            byte sak = 0;

            //检查读卡器是否连接
            if (true != Sys_IsOpen(g_hDevice))
            {
                // 未连接则进行初始化
                var initResult = this.Init();

                // 初始化失败则直接返回结果
                if (!initResult.success)
                    return new MessageModel<string>(initResult);
            }

            status = TyA_Request(g_hDevice, mode, ref TagType);//搜寻所有的卡
            if (status != 0)
            {
                result.devMsg = "TyA_Request failed !";
                return result;
            }

            status = TyA_Anticollision(g_hDevice, bcnt, dataBuffer, ref len);//返回卡的序列号
            if (status != 0)
            {
                result.devMsg = "TyA_Anticollision failed !";
                return result;
            }

            status = TyA_Select(g_hDevice, dataBuffer, len, ref sak);//锁定一张ISO14443-3 TYPE_A 卡
            if (status != 0)
            {
                result.devMsg = "TyA_Select failed !";
                return result;
            }

            // 转换卡号
            string m_cardNo = string.Empty;
            for (int q = 0; q < len; q++)
            {
                m_cardNo += SerialPortHelper.byteHEX(dataBuffer[q]);
            }
            string str = "";
            for (int i = 0; i < m_cardNo.Length; i += 2)
            {
                string dt = m_cardNo[i].ToString() + m_cardNo[i + 1].ToString();
                str = str.Insert(0, dt);
            }

            if (_systemFunc.ClientSettings.HFOriginalCard)
                result.response = m_cardNo.ToUpper();
            else
                result.response = IcSettings.DataHandle(Convert.ToInt64(str, 16).ToString(), _systemFunc.LibrarySettings?.IcSettings);

            result.success = true;
            result.msg = "读卡成功";
            return result;
        }

        /// <summary>
        /// 读指定块数据
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public MessageModel<string> ReadBlock(string block)
        {
            MessageModel<string> result = new MessageModel<string>();
            result.msg = "读卡失败";

            byte mode = 0x26;
            ushort TagType = 0;
            int status;
            byte bcnt = 0;
            byte[] dataBuffer = new byte[256];
            //IntPtr pSnr;
            byte len = 255;
            byte sak = 0;

            //检查读卡器是否连接
            if (true != Sys_IsOpen(g_hDevice))
            {
                // 未连接则进行初始化
                var initResult = this.Init();

                // 初始化失败则直接返回结果
                if (!initResult.success)
                    return new MessageModel<string>(initResult);
                //return new MessageModel<string>(initResult);
            }

            status = TyA_Request(g_hDevice, mode, ref TagType);//搜寻所有的卡
            if (status != 0)
            {
                result.devMsg = "TyA_Request failed !";
                return result;
            }

            status = TyA_Anticollision(g_hDevice, bcnt, dataBuffer, ref len);//返回卡的序列号
            if (status != 0)
            {
                result.devMsg = "TyA_Anticollision failed !";
                return result;
            }

            status = TyA_Select(g_hDevice, dataBuffer, len, ref sak);//锁定一张ISO14443-3 TYPE_A 卡
            if (status != 0)
            {
                result.devMsg = "TyA_Select failed !";
                return result;
            }

            status = TyA_CS_Authentication2(g_hDevice, 0x60, 0, new byte[] { 0xff, 0xff, 0xff, 0xff, 0xff, 0xff });
            if (status != 0)
            {
                result.devMsg = "TyA_CS_Authentication2 failed !";
                return result;
            }
            var bts = new byte[256];
            byte rtbtsLen = 0;
            status = TyA_CS_Read(g_hDevice, byte.Parse(block), bts, ref rtbtsLen);
            if (status != 0)
            {
                result.devMsg = "TyA_CS_Read failed !";
                return result;
            }
            bts = bts.Take(rtbtsLen).ToArray();

            result.response = SerialPortHelper.ByteArrayToHexString(bts).Replace(" ", ""); //  Encoding.ASCII.GetString(bts, 0, bts.Length);
            result.success = true;
            result.msg = "读卡成功";

            return result;
        }

        public MessageModel<string> ReadBlock(long sector, long block, string HexKey = "FFFFFFFFFFFF")
        {
            return ReadBlock(block.ToString());
        }

    }
}
