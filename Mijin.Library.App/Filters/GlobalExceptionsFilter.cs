using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using Util.Logs;
using Util.Logs.Extensions;

namespace Mijin.Library.App.Filters
{
    /// <summary>
    /// 全局异常处理
    /// </summary>
    public class GlobalExceptionsFilter
    {
        /// <summary>
        /// 触发异常时，写日志 (捕获不到 非 await 的子线程异常)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            e.Exception.Log(Log.GetLog().Caption("全局异常处理"));
        }
    }
}
