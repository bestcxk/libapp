using System;
using System.Collections.Generic;
using System.Text;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// 用户信息基类
    /// </summary>
    public class baseSIP2ReaderInfo
    {
        /// <summary>
        /// 读者卡号
        /// </summary>
        public string CardNo { get; set; }
    }

    /// <summary>
    /// 文华用户信息
    /// </summary>
    public class SIP2ReaderInfo : baseSIP2ReaderInfo
    {
        /// <summary>
        /// 读者姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 欠费
        /// </summary>
        public string Owe { get; set; }
        /// <summary>
        /// 押金
        /// </summary>
        public string Moeny { get; set; }
        /// <summary>
        /// 用户预付款
        /// </summary>
        public string Prepay { get; set; }
        /// <summary>
        /// 押金保证系数
        /// </summary>
        public string Depositrate { get; set; }
        /// <summary>
        /// 所借图书总额
        /// </summary>
        public string Loanedvalue { get; set; }
        /// <summary>
        /// 允许最大借书数
        /// </summary>
        public string HoldItemsLimit { get; set; }

        /// <summary>
        /// 读者类型
        /// </summary>
        public string ReaderType { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public string Enddate { get; set; }
        /// <summary>
        /// 当前借阅，未超期的图书条码
        /// </summary>
        public string HoldItems { get; set; }
        /// <summary>
        /// 当前借阅，已超期的图书条码
        /// </summary>
        public string OverdueItems { get; set; }
        /// <summary>
        /// 当前借阅，全部的图书条码
        /// </summary>
        public string AllItems { get; set; }

        /// <summary>
        /// 读者证状态 代码
        /// </summary>
        public string ReaderCode { get; set; }
        /// <summary>
        /// 读者证状态 中文
        /// </summary>
        public string ReaderCodeForChs { get; set; }

        public string ScreenMsg { get; set; }

        public string PrintLine { get; set; }

    }
}
