using Mijin.Library.App.Driver.RFID.Model;
using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;

namespace Mijin.Library.App.Driver
{
    public interface IDriverHandle
    {

        /// <summary>
        /// 开/关 锁事件 false:未打开  true:打开
        /// </summary>
        event Action<List<bool>> lockStatusEvent;

        /// <summary>
        /// 读到标签事件
        /// </summary>
        event Action<LabelInfo> OnTagEpcLog;

        /// <summary>
        /// 人进出事件
        /// </summary>
        event Action<PeopleInOut> OnPeopleInOut;

        /// <summary>
        /// 调用Driver方法
        /// </summary>
        /// <param name="cls">接口名</param>
        /// <param name="mthod">方法名</param>
        /// <param name="parameters">传入参数</param>
        /// <returns>调用结果</returns>
        MessageModel<object> Invoke(string cls, string mthod, object[] parameters);
    }
}