using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// 小票打印机接口
    /// </summary>
    public interface IPosPrint
    {
        /// <summary>
        /// 初始化小票打印机
        /// </summary>
        /// <returns></returns>
        MessageModel<bool> Init(string com = null);

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="print"></param>
        /// <returns></returns>

        MessageModel<bool> Print(PrintInfo print);
    }
}
