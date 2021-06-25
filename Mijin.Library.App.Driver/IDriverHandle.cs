using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;

namespace Mijin.Library.App.Driver
{
    public interface IDriverHandle
    {

        string[] BlackListLogMethod { get; }
        /// <summary>
        /// 所有Driver模块的事件
        /// </summary>
        event Action<object> OnDriverEvent;

        /// <summary>
        /// 调用Driver方法
        /// </summary>
        /// <param name="cls">接口名</param>
        /// <param name="mthod">方法名</param>
        /// <param name="parameters">传入参数</param>
        /// <returns>调用结果</returns>
        MessageModel<object> Invoke(string cls, string mthod, object[]? parameters);
    }
}