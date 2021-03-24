using Mijin.Library.App.Model;
using Mijin.Library.App.Model.Setting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Util;
using Util.Helpers;

namespace Mijin.Library.App.Driver
{
    public class CardSender : ICardSender
    {
        #region dll
        [DllImport("K720_Dll.dll")]
        public static extern int K720_CommOpen(string strport);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_CommOpenWithBaud(string strport, int Baudate);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_CommClose(int ComHandle);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_SetCommBaud(int ComHandle, int Baudate, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_Reset(int ComHandle, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_GetHardVersion(int ComHandle, byte[] _Version, char[] recordInfo);

        /*************************************S50***********************************************/
        [DllImport("K720_Dll.dll")]
        public static extern int K720_S50DetectCard(int ComHandle, byte MacAddr, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_S50GetCardID(int ComHandle, byte MacAddr, byte[] _CardID, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_S50LoadSecKey(int ComHandle, byte MacAddr, byte SectorAddr, byte _KEYType, byte[] _KEY, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_S50ReadBlock(int ComHandle, byte MacAddr, byte SectorAddr, byte BlockAddr, byte[] _BlockData, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_S50WriteBlock(int ComHandle, byte MacAddr, byte SectorAddr, byte BlockAddr, byte[] _BlockData, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_S50InitValue(int ComHandle, byte MacAddr, byte SectorAddr, byte BlockAddr, byte[] _BlockData, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_S50Increment(int ComHandle, byte MacAddr, byte SectorAddr, byte BlockAddr, byte[] _BlockData, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_S50Decrement(int ComHandle, byte MacAddr, byte SectorAddr, byte BlockAddr, byte[] _BlockData, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_S50Halt(int ComHandle, byte MacAddr, char[] recordInfo);


        /*************************************S70***********************************************/
        [DllImport("K720_Dll.dll")]
        public static extern int K720_S70DetectCard(int ComHandle, byte MacAddr, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_S70DetectCard(int ComHandle, byte MacAddr, byte[] _CardID, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_S70LoadSecKey(int ComHandle, byte MacAddr, byte SectorAddr, byte _KEYType, byte[] _KEY, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_S70ReadBlock(int ComHandle, byte MacAddr, byte SectorAddr, byte BlockAddr, byte[] _BlockData, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_S70WriteBlock(int ComHandle, byte MacAddr, byte SectorAddr, byte BlockAddr, byte[] _BlockData, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_S70InitValue(int ComHandle, byte MacAddr, byte SectorAddr, byte BlockAddr, byte[] _BlockData, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_S70Increment(int ComHandle, byte MacAddr, byte SectorAddr, byte BlockAddr, byte[] _BlockData, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_S70Decrement(int ComHandle, byte MacAddr, byte SectorAddr, byte BlockAddr, byte[] _BlockData, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_S70Halt(int ComHandle, byte MacAddr, char[] recordInfo);


        /*************************************UL***********************************************/
        [DllImport("K720_Dll.dll")]
        public static extern int K720_ULDetectCard(int ComHandle, byte MacAddr, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_ULGetCardID(int ComHandle, byte MacAddr, byte[] _CardID, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_ULReadBlock(int ComHandle, byte MacAddr, byte SectorAddr, byte BlockAddr, byte[] _BlockData, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_ULWriteBlock(int ComHandle, byte MacAddr, byte SectorAddr, byte BlockAddr, byte[] _BlockData, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_ULHalt(int ComHandle, byte MacAddr, char[] recordInfo);


        /*************************************CPU TypeA***********************************************/
        [DllImport("K720_Dll.dll")]
        public static extern int K720_CPUCardPowerOn(int ComHandle, byte MacAddr, byte[] AtrData, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_CPUAPDU(int ComHandle, byte MacAddr, byte SCH, int datalen, byte[] _APDUData, byte[] RCH, byte[] _exData, int[] exdatalen, char[] recordInfo);


        /*************************************D1801***********************************************/
        [DllImport("K720_Dll.dll")]
        public static extern int K720_GetVersion(int ComHandle, byte MacAddr, byte[] _Version, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_SendCmd(int ComHandle, byte MacAddr, byte[] p_Cmd, int length, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_Query(int ComHandle, byte MacAddr, byte[] StateInfo, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_SensorQuery(int ComHandle, byte MacAddr, byte[] StateInfo, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_GetCountSum(int ComHandle, byte MacAddr, byte[] StateInfo, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_ClearSendCount(int ComHandle, byte MacAddr, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_ClearRecycleCount(int ComHandle, byte MacAddr, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_AutoTestMac(int ComHandle, byte MacAddr, char[] recordInfo);


        /*************************************15693***********************************************/
        [DllImport("K720_Dll.dll")]
        public static extern int K720_15693LockDSFID(int ComHandle, byte MacAddr, bool Uid, byte[] UID, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_15693LockAFI(int ComHandle, byte MacAddr, bool Uid, byte[] UID, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_15693LockBlock(int ComHandle, byte MacAddr, bool Uid, byte[] UID, byte LockAddress, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_15693WriteAFI(int ComHandle, byte MacAddr, bool Uid, byte[] UID, byte WriteBit, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_15693WriteDSFID(int ComHandle, byte MacAddr, bool Uid, byte[] UID, byte WriteBit, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_15693ReadSafeBit(int ComHandle, byte MacAddr, bool Uid, byte[] UID, byte BlockAddr, byte BlockLen, byte[] ReadBlockLen, byte[] BlockLockStatus, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_15693GetMessage(int ComHandle, byte MacAddr, bool Uid, byte[] UID, byte[] Message, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_15693ChooseCard(int ComHandle, byte MacAddr, bool Uid, byte[] UID, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_15693WriteData(int ComHandle, byte MacAddr, bool Uid, byte[] UID, byte BlockAddr, byte BlockLen, byte[] _BlockData, byte[] WriteBlockLen, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_15693ReadData(int ComHandle, byte MacAddr, bool Uid, byte[] UID, byte BlockAddr, byte BlockLen, byte[] _BlockData, byte[] ReadBlockLen, char[] recordInfo);

        [DllImport("K720_Dll.dll")]
        public static extern int K720_15693GetUid(int ComHandle, byte MacAddr, byte[] UID, char[] recordInfo);
        #endregion

        CardSenderInOut cardInOut;
        int ComHandle = 0;
        byte MacAddr = 0;
        char[] recordInfo = new char[1000];

        public ISystemFunc _systemFunc { get; }

        public event Action<MessageModel<CardSenderStatus>> OnCardSenderStatus;

        private readonly object lockObj = new object();

        private MessageModel<CardSenderStatus> nowStatusRes;

        private bool inited = false;

        public CardSender(ISystemFunc systemFunc)
        {
            if (!Directory.Exists("CacheFiles"))
            {
                Directory.CreateDirectory("CacheFiles");
            }
            var path = "CacheFiles/CardSender-CardInOut.json";
            if (File.Exists(path))
            {
                cardInOut = Json.ToObject<CardSenderInOut>(FileHelper.ReadFile(path, Encoding.UTF8));
            }
            else
            {
                cardInOut = new();
            }
            _systemFunc = systemFunc;

            Task.Run(GetStatusTask);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public MessageModel<string> Init()
        {
            var res = new MessageModel<string>();
            var coms = System.IO.Ports.SerialPort.GetPortNames().OrderBy(c => c);
            foreach (var item in coms)
            {
                try
                {
                    int nRet = 0, i;
                    ComHandle = K720_CommOpenWithBaud(item, 9600);
                    if (ComHandle > 0)
                    {
                        for (i = 0; i < 16; i++)
                        {
                            nRet = K720_AutoTestMac(ComHandle, (byte)i, recordInfo);
                            if (nRet == 0)
                            {
                                MacAddr = (byte)i;
                                break;
                            }
                        }
                        if (nRet == 0)
                        {
                            res.msg = "连接成功";
                            res.success = true;
                            inited = true;
                            Task.Delay(1000).GetAwaiter().GetResult();
                            return res;

                        }
                        else
                        {
                            K720_CommClose(ComHandle);
                        }
                    }
                }
                catch (Exception e)
                {

                }
            }

            ComHandle = 0;
            res.msg = "初始化失败";
            return res;
        }
        /// <summary>
        /// 发卡
        /// </summary>
        /// <returns></returns>
        public MessageModel<string> SpitCard()
        {
            var res = new MessageModel<string>();
            int i = 0;

            // 若未初始化，则进行初始化
            if (!inited)
            {
                var resInit = Init();
                if (!resInit.success) return resInit;
            }

            int nRet = 0;
            byte[] cmd = new byte[10];

            if (nowStatusRes == null)
            {
                nowStatusRes = GetStatus();
            }

            if (nowStatusRes.response.Box.Contains("卡箱预空"))
            {
                res.msg = "卡箱预空，请联系管理员";
                return res;
            }

            #region 移动卡到读卡位置
            cmd = new byte[10];
            cmd[0] = 0x46;
            cmd[1] = 0x43;
            cmd[2] = 0x37;
            nRet = K720_SendCmd(ComHandle, MacAddr, cmd, 3, recordInfo);
            if (nRet != 0)
            {
                res.msg = "移动卡到读卡位置失败";
                return res;
            }

            for (i = 0; i < 30; i++)
            {
                nowStatusRes = GetStatus();
                if (nowStatusRes.response.CardLocation.Contains("读卡位置"))
                {
                    break;
                }
            }
            if (!nowStatusRes.response.CardLocation.Contains("读卡位置"))
            {
                res.msg = "卡未到达指定位置";
                return res;
            }

            #endregion
            #region 读卡
            byte[] bytes = new byte[10];
            for (i = 0; i < 3; i++)
            {
                nRet = K720_S50GetCardID(ComHandle, MacAddr, bytes, recordInfo);
                if (nRet == 0) break;
                Task.Delay(50).GetAwaiter().GetResult();
            }
            if (nRet != 0) // 读卡失败
            {
                res.msg = "读卡失败";
                cmd[0] = 0x43;
                cmd[1] = 0x50;

                nRet = K720_SendCmd(ComHandle, MacAddr, cmd, 2, recordInfo); // 读卡失败则收卡至卡槽
                if (nRet == 0)
                {
                    cardInOut.SlotNum++;
                    cardInOut.Save();
                    return SpitCard(); // 收卡成功后再一次发卡
                }
                else
                {
                    res.msg = "收卡至卡槽失败，请联系管理员！";
                }
                return res;
            }

            #region 卡号转换
            int len = bytes.Where(b => b != 0).Count();
            string m_cardNo = string.Empty;
            for (int q = 0; q < len; q++)
            {
                m_cardNo += byteHEX(bytes[q]);
            }
            string str = "";
            for (i = 0; i < m_cardNo.Length; i += 2)
            {
                string dt = m_cardNo[i].ToString() + m_cardNo[i + 1].ToString();
                str = str.Insert(0, dt);
            }
            res.response = IcSettings.DataHandle(Convert.ToInt64(str, 16).ToString(), _systemFunc.LibrarySettings?.IcSettings);
            #endregion

            #endregion
            #region 发卡到出卡口
            cmd = new byte[10];
            cmd[0] = 0x46;
            cmd[1] = 0x43;
            cmd[2] = 0x30;

            nRet = K720_SendCmd(ComHandle, MacAddr, cmd, 3, recordInfo);
            if (nRet != 0)
            {
                res.msg = "送卡到出卡口失败";

                return res;
            }
            #endregion

            cardInOut.OutPutNum++;
            cardInOut.Save();
            res.msg = "发卡成功";
            res.success = true;
            return res;
        }

        /// <summary>
        /// 获取发卡 收卡的数量
        /// </summary>
        /// <returns></returns>
        public MessageModel<CardSenderInOut> GetInOutNum()
        {
            var res = new MessageModel<CardSenderInOut>();
            res.response = cardInOut;
            res.success = true;
            res.msg = "获取成功";
            return res;
        }

        /// <summary>
        /// 重置数量
        /// </summary>
        /// <returns></returns>
        public MessageModel<string> RestInOutNum()
        {
            cardInOut.OutPutNum = 0;
            cardInOut.SlotNum = 0;
            cardInOut.Save();
            return new MessageModel<string>()
            {
                success = true,
                msg = "重置成功"
            };
        }

        /// <summary>
        /// 重置设备
        /// </summary>
        /// <returns></returns>
        private bool RestMachine()
        {
            int nRet;
            byte[] cmd = new byte[10];
            cmd[0] = 0x52;
            cmd[1] = 0x53;

            nRet = K720_SendCmd(ComHandle, MacAddr, cmd, 2, recordInfo);

            return nRet == 0;
        }

        /// <summary>
        /// 获取设备状态
        /// </summary>
        private MessageModel<CardSenderStatus> GetStatus()
        {
            lock (lockObj)
            {
                var res = new MessageModel<CardSenderStatus>()
                {
                    response = new()
                };
                int nRet = 0;
                byte[] stateInfo = new byte[10];
                nRet = K720_SensorQuery(ComHandle, MacAddr, stateInfo, recordInfo);
                if (nRet != 0)
                {
                    res.msg = "查询失败";
                    return res;
                }
                switch (stateInfo[0])
                {
                    case 0x39:
                        res.response.Machine = "回收箱卡满/卡箱预满";
                        break;
                    case 0x38:
                        res.response.Machine = "回收箱卡满";
                        break;
                    case 0x34:
                        res.response.Machine = "命令不能执行，请点击“复位”";
                        break;
                    case 0x32:
                        res.response.Machine = "准备卡失败，请点击“复位”";
                        break;
                    case 0x31:
                        res.response.Machine = "正在准备卡";
                        break;
                    case 0x30:
                        res.response.Machine = "空闲";
                        break;
                }

                switch (stateInfo[1])
                {
                    case 0x38:
                        res.response.Action = "正在发卡";
                        break;
                    case 0x34:
                        res.response.Action = "正在收卡";
                        break;
                    case 0x32:
                        res.response.Action = "发卡出错，请点击“复位”";
                        break;
                    case 0x31:
                        res.response.Action = "收卡出错，请点击“复位”";
                        break;
                    case 0x30:
                        res.response.Action = "空闲";
                        break;
                }
                switch (stateInfo[2])
                {
                    case 0x39:
                        res.response.Box = "发卡箱已满，无法再回收到发卡箱";
                        break;
                    case 0x38:
                        res.response.Box = "发卡箱已满，无法再回收到发卡箱/卡箱预空";
                        break;
                    case 0x34:
                        res.response.Box = "重叠卡";
                        break;
                    case 0x32:
                        res.response.Box = "卡堵塞";
                        break;
                    case 0x31:
                        res.response.Box = "卡箱预空";
                        break;
                    case 0x30:
                        res.response.Box = "卡箱为非预空状态";
                        break;
                }

                switch (stateInfo[3])
                {
                    case 0x3E:
                        res.response.CardLocation = "只有一张卡在传感器2-3位置";
                        break;
                    case 0x3B:
                        res.response.CardLocation = "只有一张卡在传感器1-2位置";
                        break;
                    case 0x39:
                        res.response.CardLocation = "只有一张卡在传感器1位置";
                        break;
                    case 0x38:
                        res.response.CardLocation = "卡箱已空";
                        break;
                    case 0x37:
                        res.response.CardLocation = "卡在传感器1-2-3的位置";
                        break;
                    case 0x36:
                        res.response.CardLocation = "卡在传感器2-3的位置";
                        break;
                    case 0x35:
                        res.response.CardLocation = "卡在传感器取卡位置";
                        break;
                    case 0x34:
                        res.response.CardLocation = "卡在传感器3位置";
                        break;
                    case 0x33:
                        res.response.CardLocation = "卡在传感器1-2位置(读卡位置)";
                        break;
                    case 0x32:
                        res.response.CardLocation = "卡在传感器2位置";
                        break;
                    case 0x31:
                        res.response.CardLocation = "卡在传感器1位置(取卡位置)";
                        break;
                    case 0x30:
                        res.response.CardLocation = "空闲";
                        break;
                }
                res.response.MachineCode = stateInfo[0];
                res.response.ActionCode = stateInfo[1];
                res.response.BoxCode = stateInfo[2];
                res.response.CardLocationCode = stateInfo[3];
                res.devMsg = BitConverter.ToString(stateInfo.Where(b => b != 0).ToArray(), 0).ToLower();
                res.success = true;
                res.msg = "查询成功";
                return res;
            }
        }

        private async void GetStatusTask()
        {
            while (true)
            {
                if (inited)
                {
                    try
                    {
                        nowStatusRes = GetStatus();
                        OnCardSenderStatus?.Invoke(nowStatusRes);
                        await Task.Delay(100);
                    }
                    catch (Exception)
                    {

                    }
                }


            }
        }

        #region byteHEX
        /// <summary>
        /// 单个字节转字字符.
        /// </summary>
        /// <param name="ib">字节.</param>
        /// <returns>转换好的字符.</returns>
        private string byteHEX(Byte ib)
        {
            string _str = string.Empty;
            try
            {
                char[] Digit = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A',
                'B', 'C', 'D', 'E', 'F' };
                char[] ob = new char[2];
                ob[0] = Digit[(ib >> 4) & 0X0F];
                ob[1] = Digit[ib & 0X0F];
                _str = new string(ob);
            }
            catch (Exception)
            {
                new Exception("对不起有错。");
            }
            return _str;

        }
        #endregion

    }

    /// <summary>
    /// 发卡器状态
    /// </summary>
    public class CardSenderStatus
    {
        public string Machine { get; set; }
        public string Action { get; set; }
        public string Box { get; set; }
        public string CardLocation { get; set; }

        public byte MachineCode { get; set; }
        public byte ActionCode { get; set; }
        public byte BoxCode { get; set; }
        public byte CardLocationCode { get; set; }
    }

    /// <summary>
    /// 发卡器数量
    /// </summary>
    public class CardSenderInOut
    {
        /// <summary>
        /// 出卡数
        /// </summary>
        public int OutPutNum { get; set; }
        /// <summary>
        /// 卡槽数量
        /// </summary>
        public int SlotNum { get; set; }

        public void Save()
        {
            var path = "CacheFiles/CardSender-CardInOut.json";
            FileHelper.WriteFile(path, Json.ToJson(this), Encoding.UTF8);
        }

    }
}
