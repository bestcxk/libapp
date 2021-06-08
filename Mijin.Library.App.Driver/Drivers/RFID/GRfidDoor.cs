using GDotnet.Reader.Api.DAL;
using GDotnet.Reader.Api.Protocol.Gx;
using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IsUtil;

namespace Mijin.Library.App.Driver
{
    public class GRfidDoor : GRfid, IRfidDoor
    {
        public int gpiInIndex { get; set; } = 0; // 通道门入口的gpi 索引值

        public int gpiOutIndex { get; set; } = 1; // 通道门出口的gpi 索引值

        public int inCount { get; set; } = 0;
        public int outCount { get; set; } = 0;
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        private int intervalTime = 3000;          // 触发器之间的间隔
        private int firstTrigger = -1;            // 首先触发GPI索引

        // 是否已经开启了人员进出判断
        private bool isStartWatch = false;

        public GRfidDoor() : base()
        {
            _gpiAction = GpiAction.WatchPeopleInOut;
        }

        // 人员进出事件(需要先设置 _gpiAction 为 InventoryGun 模式)
        public event Action<WebViewSendModel<PeopleInOut>> OnPeopleInOut;

        #region GPI触发事件(OnEncapedGpiStart)

        /// <param name="msg"></param>
        protected override void OnEncapedGpiStart(EncapedLogBaseGpiStart msg)
        {
            if (null == msg || !isStartWatch ) { return; }
            // 进馆/出馆 处理
            if (_gpiAction == GpiAction.WatchPeopleInOut)
            {
                if (msg.logBaseGpiStart.GpiPort != gpiInIndex && msg.logBaseGpiStart.GpiPort != gpiOutIndex) { return; }
                //进先高，出低
                if (firstTrigger == -1 && msg.logBaseGpiStart.Level == 1)
                {
                    firstTrigger = msg.logBaseGpiStart.GpiPort;
                    stopwatch.Reset();
                    stopwatch.Start();
                }
                else if (firstTrigger != -1 && msg.logBaseGpiStart.Level == 0)
                {
                    if (firstTrigger != msg.logBaseGpiStart.GpiPort)
                    {
                        stopwatch.Stop();
                        TimeSpan timespan = stopwatch.Elapsed;
                        stopwatch.Reset();
                        stopwatch.Start();
                        if (timespan.TotalMilliseconds < intervalTime)
                        {
                            var eventObj = new WebViewSendModel<PeopleInOut>();
                            if (firstTrigger == gpiInIndex)
                            {
                                inCount++;
                                OnPeopleInOut.Invoke(new WebViewSendModel<PeopleInOut>()
                                {
                                    msg = "获取成功",
                                    success = true,
                                    response = new PeopleInOut(InOut.In, inCount, outCount),
                                    method = "OnPeopleInOut"
                                });
                            }
                            else if (firstTrigger == gpiOutIndex)
                            {
                                outCount++;
                                OnPeopleInOut.Invoke(new WebViewSendModel<PeopleInOut>()
                                {
                                    msg = "获取成功",
                                    success = true,
                                    response = new PeopleInOut(InOut.Out, inCount, outCount),
                                    method = "OnPeopleInOut"
                                });
                            }
                            firstTrigger = -1;

                        }
                        else
                        {
                            firstTrigger = msg.logBaseGpiStart.GpiPort;
                        }
                    }
                    else
                    {
                        stopwatch.Reset();
                        stopwatch.Start();
                    }
                }

            }
        }
        #endregion

        #region 开启出入馆进出判断(StartWatchPeopleInOut)
        /// <summary>
        /// 开启出入馆进出判断
        /// </summary>
        /// <param name="clear"></param>
        /// <param name="ms"></param>
        public MessageModel<bool> StartWatchPeopleInOut(bool clear = false)
        {
            var result = new MessageModel<bool>();
            //TriggerStart: 触发开始（0 触发关闭， 1 低电平触发， 2 高电平触发， 3 上升沿触发， 4 下降沿触发， 5 任意边沿触发）
            //MsgAppSetGpiTrigger msg1 = new MsgAppSetGpiTrigger()
            //{
            //    GpiPort = 0,
            //    TriggerStart = 5,
            //    TriggerCommand = "0001021000080000000101020006",
            //    TriggerOver = 6,
            //    OverDelayTime = 200  // 单位 10ms

            //};
            //MsgAppSetGpiTrigger msg2 = new MsgAppSetGpiTrigger()
            //{
            //    GpiPort = 1,
            //    TriggerStart = 5,
            //    TriggerCommand = "0001021000080000000101020006",
            //    TriggerOver = 6,
            //    OverDelayTime = 200
            //};
            if (!isStartWatch)
            {
                //base._gClient.SendSynMsg(msg1);
                //base._gClient.SendSynMsg(msg2);

                if (clear)
                    inCount = outCount = 0;

                //result.success = msg2.RtCode == 0;
            }
            else
            {
                result.success = true;
            }
            result.success = true;
            result.msg = "开启出入馆进出判断" + (result.success ? "成功" : "失败");
            //result.devMsg = msg2.RtMsg;

            isStartWatch = true;

            return result;
        }
        #endregion

        #region 停止出入馆进出判断(StopWatchPeopleInOut)
        /// <summary>
        /// 停止出入馆进出判断
        /// </summary>
        /// <returns></returns>
        public MessageModel<bool> StopWatchPeopleInOut()
        {
            var result = new MessageModel<bool>();

            //TriggerStart: 触发开始（0 触发关闭， 1 低电平触发， 2 高电平触发， 3 上升沿触发， 4 下降沿触发， 5 任意边沿触发）
            //MsgAppSetGpiTrigger msg1 = new MsgAppSetGpiTrigger()
            //{
            //    GpiPort = 0,
            //    TriggerStart = 0,
            //};
            //MsgAppSetGpiTrigger msg2 = new MsgAppSetGpiTrigger()
            //{
            //    GpiPort = 1,
            //    TriggerStart = 0,
            //};

            //base._gClient.SendSynMsg(msg1);
            //base._gClient.SendSynMsg(msg2);

            //result.success = msg2.RtCode == 0;
            result.success = true;
            result.msg = "停止出入馆进出判断" + (result.success ? "成功" : "失败");
            //result.devMsg = msg2.RtMsg;
            isStartWatch = false;

            return result;
        }
        #endregion


    }
}
