﻿using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// RFID通道门接口
    /// </summary>
    public interface IRfidDoor : IRfid
    {
        /// <summary>
        /// 通道门入口的gpi 索引值
        /// </summary>
        public int gpiInIndex { get; set; }

        /// <summary>
        /// 通道门出口的gpi 索引值
        /// </summary>
        public int gpiOutIndex { get; set; } 
        /// <summary>
        /// 进人数
        /// </summary>
        int inCount { get; set; }
        /// <summary>
        /// 出人数
        /// </summary>
        int outCount { get; set; }
        /// <summary>
        /// 人进出事件
        /// </summary>
        event Action<WebViewSendModel<PeopleInOut>> OnPeopleInOut;
        event Action<WebViewSendModel<GpiEvent>> OnStartGpiEvent;

        /// <summary>
        /// 开始监听人员进出
        /// </summary>
        /// <param name="clear">清空人数缓存统计</param>
        /// <returns></returns>
        MessageModel<bool> StartWatchPeopleInOut(bool clear = false);

        /// <summary>
        /// 停止监听人员进出
        /// </summary>
        /// <returns></returns>
        MessageModel<bool> StopWatchPeopleInOut();
    }
}
