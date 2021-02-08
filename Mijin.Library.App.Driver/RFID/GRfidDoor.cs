using GDotnet.Reader.Api.DAL;
using GDotnet.Reader.Api.Protocol.Gx;
using Mijin.Library.App.Driver.RFID.Model;
using Mijin.Library.App.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver.RFID
{
    public class GRfidDoor : GRfid, IRfidDoor
    {

        private int gpiInIndex = 0;              // 通道门入口的gpi 索引值
        private int gpiOutIndex = 1;             // 通道门出口的gpi 索引值

        private int inCount = 0;
        private int outCount = 0;
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        private int intervalTime = 3000;          // 触发器之间的间隔
        private int firstTrigger = -1;            // 首先触发GPI索引

        // 是否已经开启了人员进出判断
        private bool isStartWatch = false;

        public GRfidDoor() : base()
        {

        }

        // 人员进出事件(需要先设置 _gpiAction 为 InventoryGun 模式)
        public event Action<PeopleInOut> OnPeopleInOut;

        #region GPI触发事件(OnEncapedGpiStart)

        /// <param name="msg"></param>
        protected override void OnEncapedGpiStart(EncapedLogBaseGpiStart msg)
        {
            if (null == msg) { return; }
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
                            if (firstTrigger == gpiInIndex)
                            {
                                inCount++;
                                OnPeopleInOut.Invoke(new PeopleInOut(InOut.In, inCount, outCount));
                            }
                            else if (firstTrigger == gpiOutIndex)
                            {
                                outCount++;
                                OnPeopleInOut.Invoke(new PeopleInOut(InOut.Out, inCount, outCount));
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
        /// <returns></returns>
        public MessageModel<bool> StartWatchPeopleInOut(bool clear = false)
        {
            var result = new MessageModel<bool>();

            //TriggerStart: 触发开始（0 触发关闭， 1 低电平触发， 2 高电平触发， 3 上升沿触发， 4 下降沿触发， 5 任意边沿触发）
            MsgAppSetGpiTrigger msg1 = new MsgAppSetGpiTrigger()
            {
                GpiPort = 0,
                TriggerStart = 5,
            };
            MsgAppSetGpiTrigger msg2 = new MsgAppSetGpiTrigger()
            {
                GpiPort = 1,
                TriggerStart = 5,
            };
            if (!isStartWatch)
            {
                base._gClient.SendSynMsg(msg1);
                base._gClient.SendSynMsg(msg2);

                if (clear)
                    inCount = outCount = 0;

                result.success = msg2.RtCode == 0;
            }
            else
            {
                result.success = true;
            }
            result.msg = "开启出入馆进出判断" + (result.success ? "成功" : "失败");
            result.devMsg = msg2.RtMsg;

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
            MsgAppSetGpiTrigger msg1 = new MsgAppSetGpiTrigger()
            {
                GpiPort = 0,
                TriggerStart = 0,
            };
            MsgAppSetGpiTrigger msg2 = new MsgAppSetGpiTrigger()
            {
                GpiPort = 1,
                TriggerStart = 0,
            };

            base._gClient.SendSynMsg(msg1);
            base._gClient.SendSynMsg(msg2);

            result.success = msg2.RtCode == 0;
            result.msg = "停止出入馆进出判断" + (result.success ? "成功" : "失败");
            result.devMsg = msg2.RtMsg;

            isStartWatch = true;

            return result;
        }
        #endregion
    }
}
