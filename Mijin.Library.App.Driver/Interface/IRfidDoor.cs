using Mijin.Library.App.Model;
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
        /// 人进出事件
        /// </summary>
        event Action<PeopleInOut> OnPeopleInOut;

        /// <summary>
        /// 开始监听人员进出
        /// </summary>
        /// <param name="clear">清空人数缓存统计</param>
        /// <returns></returns>
        MessageModel<bool> StartWatchPeopleInOut(string clear = "false");

        /// <summary>
        /// 停止监听人员进出
        /// </summary>
        /// <returns></returns>
        MessageModel<bool> StopWatchPeopleInOut();
    }
}
