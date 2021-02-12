using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// 锁孔板接口
    /// </summary>
    public interface ICabinetLock
    {
        /// <summary>
        /// 是否已经打开串口
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// 开/关 锁事件 false:未打开  true:打开
        /// </summary>
        event Action<List<bool>> lockStatusEvent;


        /// <summary>
        /// 打开指定柜锁
        /// </summary>
        /// <param name="lockIndex">柜号 1开始</param>
        /// <returns></returns>
        MessageModel<bool> OpenBox(string lockIndex);

        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="com"></param>
        /// <param name="baud"></param>
        /// <returns></returns>
        MessageModel<bool> OpenSerialPort(string com, string baud = "115200");
    }
}