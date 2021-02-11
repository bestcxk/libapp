﻿using Mijin.Library.App.Driver.PosPrint.Model;
using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver.PosPrint.Interface
{
    public interface IPosPrint
    {
        /// <summary>
        /// 初始化小票打印机
        /// </summary>
        /// <returns></returns>
        MessageModel<bool> Init();

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="print"></param>
        /// <returns></returns>

        MessageModel<bool> Print(PrintInfo print);
    }
}
