using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;

namespace Mijin.Library.App.Driver
{
    public interface ICabinetLock
    {
        /// <summary>
        /// 串口是否已打开
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// 获取锁控板锁状态
        /// </summary>
        /// <returns></returns>
        MessageModel<List<bool>> GetLockStatus();
        /// <summary>
        /// 开指定柜号
        /// </summary>
        /// <param name="boxIndex">索引从1开始</param>
        /// <returns></returns>
        MessageModel<string> OpenBox(Int64 boxIndex);
        /// <summary>
        /// 打开串口
        /// </summary>
        /// <param name="com"></param>
        /// <param name="baud"></param>
        /// <returns></returns>
        MessageModel<bool> OpenSerialPort(string com, Int64 baud);
    }
}