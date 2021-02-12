using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// GPI执行枚举
    /// </summary>
    public enum GpiAction
    { 
        /// <summary>
        /// 无内容执行
        /// </summary>
        Default,
        /// <summary>
        /// 进出馆
        /// </summary>
        WatchPeopleInOut,
        /// <summary>
        /// 盘点枪
        /// </summary>
        InventoryGun
    }
}
