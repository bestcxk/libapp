using GDotnet.Reader.Api.DAL;
using GDotnet.Reader.Api.Protocol.Gx;
using Mijin.Library.App.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IsUtil;
using IsUtil.Maps;

namespace Mijin.Library.App.Driver
{
    public partial class GRfid : IRfid
    {
        protected GClient _gClient;

        // 当前GClient实例模块的 功率 数据
        protected Dictionary<byte, byte> _powerDic;

        //是否只读一次模式
        protected bool _readOnce = false;
        // ReadOnce 存储缓存标签
        protected LabelInfo _tempLabel = null;

        // gpi触发Action ,默认不处理
        protected GpiAction _gpiAction = GpiAction.Default;

        // 标签触发事件
        public event Action<WebViewSendModel<LabelInfo>> OnReadLabel;
        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public GRfid()
        {
            _gClient = new GClient();
            // 默认4天线模式
            _powerDic = new Dictionary<byte, byte>()
            {
                { 1 , 1},
                { 2 , 1},
                { 3 , 1},
                { 4 , 1},
            };
        }
        #endregion

        #region 标签读取事件(OnEncapedTagEpcLog)
        private void OnEncapedTagEpcLog(EncapedLogBaseEpcInfo msg)
        {
            // 回调内部如有阻塞，会影响API正常使用
            // 标签回调数量较多，请将标签数据先缓存起来再作业务处理
            if (null != msg && 0 == msg.logBaseEpcInfo.Result)
            {
                // 只读一次的缓存label
                if (_readOnce)
                {
                    _tempLabel = new LabelInfo(msg.logBaseEpcInfo);
                    this.Stop(); // 达到次数后停止读标签
                }
                else
                { 
                    OnReadLabel.Invoke(new WebViewSendModel<LabelInfo>()
                    {
                        msg = "获取成功",
                        success = true,
                        response = new LabelInfo(msg.logBaseEpcInfo),
                        method = "OnReadLabel"
                    });
                }

            }
        }
        #endregion

        #region GPI触发事件(OnEncapedGpiStart)
        protected virtual void OnEncapedGpiStart(EncapedLogBaseGpiStart msg)
        {
            if (null == msg) { return; }

            // 扫码枪 处理
            if (_gpiAction == GpiAction.InventoryGun && msg.logBaseGpiStart.GpiPort == 0) //扫码枪
            {
                Dictionary<int, int> keys = new Dictionary<int, int>();
                keys[msg.logBaseGpiStart.GpiPort] = msg.logBaseGpiStart.Level;
                if (msg.logBaseGpiStart.Level == 1)
                {
                    Task.Run(() =>
                    {
                        ReadByAntId(new List<string> { "2" });
                    });
                }
                else
                {
                    Task.Run(() =>
                    {
                        Stop();
                    });
                }
            }
        }
        #endregion

        #region 连接设备(Connect)
        /// <summary>
        /// 连接设备
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="conStr"></param>
        /// <param name="timeOutMs">超时毫秒</param>
        /// <returns></returns>
        public MessageModel<bool> Connect(string mode, string conStr,Int64 timeOutMs = 1500)
        {
            var result = new MessageModel<bool>();
            eConnectionAttemptEventStatusType status = eConnectionAttemptEventStatusType.NoResponse;
            _gClient.Close();
            if ("tcp".Equals(mode))
            {
                try
                {
                    if (_gClient.OpenTcp(conStr, (int)timeOutMs, out status))
                    {
                        result.success = GetPower().success;
                    }
                }
                catch (Exception)
                {

                }
                
            }
            else
            {
                try
                {
                    if (_gClient.OpenSerial(conStr, (int)timeOutMs, out status))
                    {
                        result.success = GetPower().success;
                    }
                }
                catch (Exception)
                {

                }
                
            }
            if (result.success)
            {
                // 清除绑定的委托
                ClearEvent("all");
                _gClient.OnEncapedTagEpcLog += OnEncapedTagEpcLog;  //标签读取时间
                _gClient.OnEncapedGpiStart += new delegateEncapedGpiStart(OnEncapedGpiStart);
            }
            else
            { 
                _gClient.Close();
            }

            result.msg = "连接" + (result.success ? "成功" : "失败");
            result.devMsg = status.ToString();
            return result;
        }
        #endregion

        #region 232自动连接(Auto232Connect)
        /// <summary>
        /// 232自动连接
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="conStr"></param>
        /// <returns></returns>
        public MessageModel<bool> Auto232Connect()
        {
            var result = new MessageModel<bool>();


            var coms = System.IO.Ports.SerialPort.GetPortNames().OrderBy(c => int.Parse(c.Split('M')[1])).ToArray(); // rfid串口最好修改到com1
            for (int i = 0; i < coms.Length; i++)
            {
                try
                {
                    result = Connect("232", $"{coms[i]}:115200");
                    if (result.success)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {

                }
            }
            return result;
        }
        #endregion

        #region 清除绑定的委托(ClearEvent)
        /// <summary>
        /// 清除绑定的委托
        /// </summary>
        /// <param name="model"></param>
        private void ClearEvent(string model)
        {
            try
            {
                if ("all".Equals(model) || "label".Equals(model))
                    _gClient.OnEncapedTagEpcLog?.GetInvocationList().Select(p => _gClient.OnEncapedTagEpcLog -= p as delegateEncapedTagEpcLog).ToList();
            }
            catch (Exception e) { }
            try
            {
                if ("all".Equals(model) || "gpi".Equals(model))
                {
                    _gClient.OnEncapedGpiStart?.GetInvocationList().Select(p => _gClient.OnEncapedGpiStart -= p as delegateEncapedGpiStart).ToList();

                    //_gClient.OnEncapedGpiOver?.GetInvocationList().Select(p => _gClient.OnEncapedGpiOver -= p as delegateEncapedGpiOver).ToList();
                }
            }
            catch (Exception e) { }
        }

        #endregion

        #region 停止读标签(Stop)
        /// <summary>
        /// 停止读标签
        /// </summary>
        /// <returns></returns>
        public MessageModel<bool> Stop()
        {
            var result = new MessageModel<bool>();
            
            try
            {
                // 停止指令，空闲态
                MsgBaseStop msgBaseStop = new MsgBaseStop();
                _gClient.SendSynMsg(msgBaseStop, 500);
                result.success = msgBaseStop.RtCode == 0;
                result.devMsg = msgBaseStop.RtMsg;
            }
            catch (Exception e)
            {
                result.success = false;
            }
            result.msg = "停止" + (result.success ? "成功" : "失败");
            return result;
        }

        #endregion

        #region 获取功率信息(GetPower)
        /// <summary>
        /// 获取功率信息
        /// </summary>
        /// <returns></returns>
        public MessageModel<Dictionary<byte, byte>> GetPower()
        {
            var result = new MessageModel<Dictionary<byte, byte>>();
            MsgBaseGetPower msgBaseSetPower = new MsgBaseGetPower();

            // 因Auto232连接判定是该方法，故加 try/catch 
            try
            {
                _gClient.SendSynMsg(msgBaseSetPower);
                result.success = msgBaseSetPower.RtCode == 0;
                if (result.success)
                {
                    result.response = msgBaseSetPower.DicPower;
                    _powerDic = msgBaseSetPower.DicPower;
                }
            }
            catch (Exception)
            {

            }
            result.msg = "获取" + (result.success ? "成功" : "失败");
            result.devMsg = msgBaseSetPower.RtMsg;
            return result;
        }
        #endregion

        #region 设置天线功率(SetPower)
        /// <summary>
        /// 设置天线功率
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public MessageModel<Dictionary<byte, byte>> SetPower(Dictionary<byte, byte> dt)
        {
            var result = new MessageModel<Dictionary<byte, byte>>();
            MsgBaseSetPower msgBaseSetPower = new MsgBaseSetPower();
            msgBaseSetPower.DicPower = dt;
            _gClient.SendSynMsg(msgBaseSetPower);
            result.success = msgBaseSetPower.RtCode == 0;
            if (result.success)
            {
                result.response = msgBaseSetPower.DicPower;
                _powerDic = msgBaseSetPower.DicPower;
            }
            result.msg = "设置" + (result.success ? "成功" : "失败");
            result.devMsg = msgBaseSetPower.RtMsg;
            return result;
        }
        #endregion

        #region 获取频段信息(GetFreqRange)
        /// <summary>
        /// 获取频段信息
        /// </summary>
        /// <returns></returns>
        public MessageModel<FreqRange> GetFreqRange()
        {
            var result = new MessageModel<FreqRange>();
            MsgBaseGetFreqRange msgBaseGetFreqRange = new MsgBaseGetFreqRange();
            _gClient.SendSynMsg(msgBaseGetFreqRange);
            result.response = new FreqRange()
            {
                Index = msgBaseGetFreqRange.FreqRangeIndex,
                Name = FreqRange.freqNames[msgBaseGetFreqRange.FreqRangeIndex]
            };
            result.success = msgBaseGetFreqRange.RtCode == 0;
            result.msg = "获取" + (result.success ? "成功" : "失败");
            result.devMsg = msgBaseGetFreqRange.RtMsg;
            return result;
        }
        #endregion

        #region 设置频段(SetFreqRange)
        public MessageModel<bool> SetFreqRange(string index)
        {
            var result = new MessageModel<bool>();
            MsgBaseSetFreqRange msgBaseSetFreqRange = new MsgBaseSetFreqRange();
            msgBaseSetFreqRange.FreqRangeIndex = byte.Parse(index);
            _gClient.SendSynMsg(msgBaseSetFreqRange);
            result.success = msgBaseSetFreqRange.RtCode == 0;
            result.msg = "设置" + (result.success ? "成功" : "失败");
            result.devMsg = msgBaseSetFreqRange.RtMsg;
            return result;
        }
        #endregion

        #region 开启指定天线读标签(ReadByAntId)
        /// <summary>
        /// 开启指定天线读标签
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public MessageModel<bool> ReadByAntId(List<string> antIdStrs)
        {
            var result = new MessageModel<bool>();
            MsgBaseInventoryEpc msgBaseInventoryEpc = new MsgBaseInventoryEpc();

            List<uint> antIds = antIdStrs.JsonMapToList<uint>();


            //天线排序
            antIds = antIds.OrderBy(p => p).ToList();

            //设置天线
            for (int i = 0; i < antIds.Count; i++)
            {
                double doub = Math.Pow(2.0, Convert.ToDouble(antIds[i] - 1));
                antIds[i] = (uint)doub;
                msgBaseInventoryEpc.AntennaEnable |= antIds[i];
            }

            msgBaseInventoryEpc.InventoryMode = (byte)eInventoryMode.Inventory;
            msgBaseInventoryEpc.ReadTid = new ParamEpcReadTid();                // TID和EPC
            msgBaseInventoryEpc.ReadTid.Mode = (byte)eParamTidMode.Auto;
            msgBaseInventoryEpc.ReadTid.Len = 6;

            _gClient.SendSynMsg(msgBaseInventoryEpc);

            result.success = msgBaseInventoryEpc.RtCode == 0;
            result.msg = "设置" + (result.success ? "成功" : "失败");
            result.devMsg = msgBaseInventoryEpc.RtMsg;
            return result;
        }

        /// <summary>
        /// 开启指定天线读标签
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public MessageModel<bool> ReadByAntId(string jsonStr)
        {
            //var list = jArray.Select(j => j.ToString()).ToList();
            return ReadByAntId(Json.ToObject<List<string>>(jsonStr));
        }
        #endregion

        #region 开启所有天线读标签(Read)
        /// <summary>
        /// 开启所有天线读标签
        /// </summary>
        /// <returns></returns>
        public MessageModel<bool> Read()
        {
            return ReadByAntId(_powerDic.Select(p => p.Key.ToString()).ToList());
        }
        #endregion

        #region 只读一个标签(ReadOnceByAntId)
        /// <summary>
        /// 只读一个标签
        /// </summary>
        /// <param name="antIds"></param>
        /// <returns></returns>
        public MessageModel<LabelInfo> ReadOnceByAntId(List<string> antIdStrs)
        {
            var result = new MessageModel<LabelInfo>();
            _tempLabel = null;
            _readOnce = true; // 为true时且读到标签时，_tempLabel 才获取值
            var res = ReadByAntId(antIdStrs);
            if (!res.success)
            {
                _readOnce = false;
                return new MessageModel<LabelInfo>(res);
            }

            DateTime beforeDate = DateTime.Now;

            while (true)
            {
                if (_tempLabel != null)
                {
                    break;
                }
                Task.Delay(5).GetAwaiter().GetResult(); // 没有读到，则延时 5 ms后继续查看

                // 超过 1500 毫秒，直接退出循环 并 返回
                if ((DateTime.Now - beforeDate).TotalMilliseconds >= 1500)
                {
                    break;
                }

            }
            _readOnce = false;
            result.response = _tempLabel;
            result.success = result.response != null;
            result.msg = "获取标签" + (result.success ? "成功" : "失败");

            return result;
        }

        
        #endregion

        #region 只读一个标签(ReadOnce)
        public MessageModel<LabelInfo> ReadOnce()
        {
            return ReadOnceByAntId(_powerDic.Select(p => p.Key.ToString()).ToList());
        }
        #endregion

        #region 设置GPO输出(SetGpo)
        /// <summary>
        /// 设置GPO输出
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public MessageModel<bool> SetGpo(Dictionary<string, byte> dic)
        {
            var result = new MessageModel<bool>();
            MsgAppSetGpo msgAppSetGpo = new MsgAppSetGpo()
            {
                Gpo1 = (byte)(dic.ContainsKey("Gpo1") ? dic["Gpo1"] : 0),
                Gpo2 = (byte)(dic.ContainsKey("Gpo2") ? dic["Gpo2"] : 0),
                Gpo3 = (byte)(dic.ContainsKey("Gpo3") ? dic["Gpo3"] : 0),
                Gpo4 = (byte)(dic.ContainsKey("Gpo4") ? dic["Gpo4"] : 0),
            };
            _gClient.SendSynMsg(msgAppSetGpo);
            result.success = msgAppSetGpo.RtCode == 0;
            result.msg = "设置" + (result.success ? "成功" : "失败");
            result.devMsg = msgAppSetGpo.RtMsg;
            return result;
        }
        #endregion

        #region 写标签(WriteLabel)
        /// <summary>
        /// 写标签
        /// </summary>
        /// <param name="area">0， 保留区； 1， EPC 区； 2， TID 区； 3， 用户数据区</param>
        /// <param name="startAddr"></param>
        /// <param name="data"></param>
        /// <param name="baseTid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public MessageModel<bool> WriteLabel(Int64 area, Int64 startAddr, string data, string baseTid = null, string password = "00000000", Int64 timeOut = 3)
        {
            var result = new MessageModel<bool>();
            MsgBaseWriteEpc msgBaseWriteEpc = new MsgBaseWriteEpc();
            msgBaseWriteEpc.AntennaEnable = (ushort)eAntennaNo._1;
            msgBaseWriteEpc.Area = (byte)area;
            msgBaseWriteEpc.Start = (byte)startAddr;
            int iWordLen = data.Length / 4;                            // 1 word = 2 byte     
            ushort iPc = (ushort)(iWordLen << 11);                              // PC值为EPC区域的长度标识（前5个bit标记长度），参考文档说明 
            String sPc = Convert.ToString(iPc, 16).PadLeft(4, '0');
            data = sPc + data;                                // 
            msgBaseWriteEpc.HexWriteData = data.Trim().PadRight(iWordLen * 4, '0');
            msgBaseWriteEpc.HexPassword = password;

            if (!string.IsNullOrEmpty(baseTid))
            {
                msgBaseWriteEpc.Filter = new ParamEpcFilter();
                // 匹配TID写标签示例，用于多标签环境写单个标签
                msgBaseWriteEpc.Filter.Area = (byte)eParamFilterArea.TID;
                msgBaseWriteEpc.Filter.BitStart = 0;
                msgBaseWriteEpc.Filter.HexData = baseTid.Trim();
                msgBaseWriteEpc.Filter.BData = GDotnet.Reader.Api.Utils.Util.ConvertHexStringToByteArray(msgBaseWriteEpc.Filter.HexData);
                msgBaseWriteEpc.Filter.BitLength = (byte)(msgBaseWriteEpc.Filter.BData.Length * 8);
            }

            while (timeOut-- >= 0)
            {
                Thread.Sleep(5);
                //Task.Delay(5).GetAwaiter().GetResult();
                _gClient.SendSynMsg(msgBaseWriteEpc);

                result.success = msgBaseWriteEpc.RtCode == 0;
                if (result.success)
                {
                    break;
                }
            }
            result.msg = "写入" + (result.success ? "成功" : "失败");
            result.devMsg = msgBaseWriteEpc.RtMsg;

            return result;
        }
        #endregion

        # region 开始盘点(StartInventory)
        /// <summary>
        /// 开始盘点
        /// </summary>
        /// <param name="gpiAction"></param>
        /// <returns></returns>
        public MessageModel<bool> StartInventory(GpiAction gpiAction = GpiAction.Default)
        {
            MessageModel<bool> result = new MessageModel<bool>();
            MsgAppSetGpiTrigger msg = new MsgAppSetGpiTrigger()
            {
                GpiPort = 0,
                TriggerStart = 5,
            };
            _gClient.SendSynMsg(msg);
            _gpiAction = gpiAction;

            // 不是 扫码枪，则直接开启读标签
            if (_gpiAction != GpiAction.InventoryGun)
            {
                result = this.Read();
            }
            else
            {
                result.success = msg.RtCode == 0;
                result.devMsg = msg.RtMsg;
            }
            result.msg = "开启盘点" + (result.success ? "成功" : "失败");
            return result;

        }

        /// <summary>
        /// 调用版本
        /// </summary>
        /// <param name="gpiAction"></param>
        /// <returns></returns>
        public MessageModel<bool> StartInventory(Int64 gpiAction = 0)
        {
            MessageModel<bool> result = new MessageModel<bool>();
            MsgAppSetGpiTrigger msg = new MsgAppSetGpiTrigger()
            {
                GpiPort = 0,
                TriggerStart = 5,
            };
            _gClient.SendSynMsg(msg);
            _gpiAction = (GpiAction)gpiAction;

            // 不是 扫码枪，则直接开启读标签
            if (_gpiAction != GpiAction.InventoryGun)
            {
                result = this.Read();
            }
            else
            {
                result.success = msg.RtCode == 0;
                result.devMsg = msg.RtMsg;
            }
            result.msg = "开启盘点" + (result.success ? "成功" : "失败");
            return result;

        }
        #endregion

        #region 停止盘点(StopInventory)
        /// <summary>
        /// 停止盘点
        /// </summary>
        /// <param name="gpiAction"></param>
        /// <returns></returns>
        public MessageModel<bool> StopInventory()
        {
            MessageModel<bool> result = new MessageModel<bool>();
            MsgAppSetGpiTrigger msg = new MsgAppSetGpiTrigger()
            {
                GpiPort = 0,
                TriggerStart = 0,
            };
            _gClient.SendSynMsg(msg);

            _gpiAction = GpiAction.Default;

            result = this.Stop();
            result.msg = "停止盘点" + (result.success ? "成功" : "失败");
            return result;

        }
        #endregion


        #region 设置天线功率(SetPower(用于web进行反射调用))
        /// <summary>
        /// 设置天线功率(用于web进行反射调用)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public MessageModel<Dictionary<byte, byte>> SetPower(object dt)
        {
            return SetPower(dt.JsonMapTo<Dictionary<byte, byte>>());
        }
        #endregion

        #region 设置GPO输出(SetGpo(用于web进行反射调用))
        /// <summary>
        /// 设置GPO输出
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public MessageModel<bool> SetGpo(object dic)
        {
            return SetGpo(dic.JsonMapTo<Dictionary<string, byte>>());
        }
        #endregion

    }
}
