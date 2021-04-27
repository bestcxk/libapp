using Mijin.Library.App.Model;
using System;

namespace Mijin.Library.App.Driver
{
    public interface IDoorController
    {
        /// <summary>
        /// 连接门控
        /// </summary>
        /// <param name="ip">ip地址</param>
        /// <param name="port">端口号</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        MessageModel<bool> Connect(string ip, Int64 port, Int64 timeout);
        /// <summary>
        /// 开门
        /// </summary>
        /// <param name="openTime">(0:, 255:Hormal-open, value range : 1~60 seconds)</param>
        /// <returns></returns>
        MessageModel<bool> OpenDoor(Int64 openTime = 5);
    }
}