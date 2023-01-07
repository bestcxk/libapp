using GDotnet.Reader.Api.DAL;
using Mijin.Library.App.Model;
using System;

namespace Mijin.Library.App.Driver
{
    public class GRfidDoor : GRfid, IRfidDoor
    {
        public int gpiInIndex { get; set; } = 0; // 通道门入口的gpi 索引值

        public int gpiOutIndex { get; set; } = 1; // 通道门出口的gpi 索引值

        public int inCount { get; set; } = 0;
        public int outCount { get; set; } = 0;
        protected System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        protected int intervalTime = 3000; // 触发器之间的间隔
        protected int firstTrigger = -1; // 首先触发GPI索引

        // 是否已经开启了人员进出判断
        protected bool isStartWatch = false;

        ~GRfidDoor()
        {
            Dispose();
        }

        public override event Action<WebViewSendModel<LabelInfo>> OnReadUHFLabel;

        public override event Action<WebViewSendModel<GpiEvent>> OnGpiEvent;

        public event Action<WebViewSendModel<GpiEvent>> OnStartGpiEvent;


        public GRfidDoor() : base(null)
        {
            _gpiAction = GpiAction.WatchPeopleInOut;
        }

        // 人员进出事件(需要先设置 _gpiAction 为 InventoryGun 模式)
        public event Action<WebViewSendModel<PeopleInOut>> OnPeopleInOut;


        protected override void OnEncapedTagEpcLog(EncapedLogBaseEpcInfo msg)
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
                    OnReadUHFLabel.Invoke(new WebViewSendModel<LabelInfo>()
                    {
                        msg = "获取成功",
                        success = true,
                        response = new LabelInfo(msg.logBaseEpcInfo),
                        method = nameof(OnReadUHFLabel),
                        status = 1001,
                    });
                }
            }
        }

        #region GPI触发事件(OnEncapedGpiStart)

        /// <param name="msg"></param>
        protected override void OnEncapedGpiStart(EncapedLogBaseGpiStart msg)
        {
            if (msg is null || msg.logBaseGpiStart.Level != 1)
            {
                return;
            }


            OnGpiEvent?.Invoke(new WebViewSendModel<GpiEvent>()
            {
                success = true,
                msg = "获取成功",
                method = nameof(OnGpiEvent),
                response = new GpiEvent()
                {
                    Gpi = msg.logBaseGpiStart.GpiPort,
                    Level = msg.logBaseGpiStart.Level
                }
            });

            if (!isStartWatch)
            {
                return;
            }


            // 进馆/出馆 处理
            if (_gpiAction == GpiAction.WatchPeopleInOut)
            {
                if (msg.logBaseGpiStart.GpiPort != gpiInIndex && msg.logBaseGpiStart.GpiPort != gpiOutIndex)
                {
                    return;
                }

                //进先高，出低
                if (firstTrigger == -1)
                {
                    firstTrigger = msg.logBaseGpiStart.GpiPort;
                    OnStartGpiEvent?.Invoke(new WebViewSendModel<GpiEvent>()
                    {
                        success = true,
                        status = 1001,
                        msg = "获取成功",
                        method = nameof(OnStartGpiEvent),
                        response = new GpiEvent()
                        {
                            Gpi = msg.logBaseGpiStart.GpiPort,
                            Level = msg.logBaseGpiStart.Level
                        }
                    });
                    stopwatch.Reset();
                    stopwatch.Start();
                }
                else if (firstTrigger != -1)
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
                                OnPeopleInOut?.Invoke(new WebViewSendModel<PeopleInOut>()
                                {
                                    msg = "获取成功",
                                    success = true,
                                    response = new PeopleInOut(InOut.In, inCount, outCount),
                                    method = "OnPeopleInOut",
                                    status = 1001,
                                });
                            }
                            else if (firstTrigger == gpiOutIndex)
                            {
                                outCount++;
                                OnPeopleInOut?.Invoke(new WebViewSendModel<PeopleInOut>()
                                {
                                    msg = "获取成功",
                                    success = true,
                                    response = new PeopleInOut(InOut.Out, inCount, outCount),
                                    method = "OnPeopleInOut",
                                    status = 1001,
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
            if (!isStartWatch)
            {
                if (clear)
                    inCount = outCount = 0;
            }
            else
            {
                result.success = true;
            }

            _gpiAction = GpiAction.WatchPeopleInOut;
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
            result.success = true;
            result.msg = "停止出入馆进出判断" + (result.success ? "成功" : "失败");
            //result.devMsg = msg2.RtMsg;
            isStartWatch = false;

            return result;
        }

        #endregion
    }
}