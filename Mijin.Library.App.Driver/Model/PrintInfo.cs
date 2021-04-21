using System;
using System.Collections.Generic;
using System.Text;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// 小票打印类
    /// </summary>
    public class PrintInfo
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public ActionType Action { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 书籍信息列表
        /// </summary>
        public List<BookInfo> BookInfos { get; set; } = new List<BookInfo>();
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户卡号
        /// </summary>
        public string UserCardNo { get; set; }
        /// <summary>
        /// 用户身份证
        /// </summary>
        public string UserIdentity { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal Pay { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderNo { get; set; }
    }

    /// <summary>
    /// 书籍信息
    /// </summary>
    public class BookInfo
    {
        /// <summary>
        /// 书名
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 图书一维码值
        /// </summary>
        public string Serial { get; set; }
        /// <summary>
        /// 图书ISBN
        /// </summary>
        public string Isbn { get; set; }
        /// <summary>
        /// 应还时间
        /// </summary>
        public DateTime ShouldBackTime { get; set; }

    }

    /// <summary>
    /// 类型枚举
    /// </summary>
    public enum ActionType
    {
        借阅 = 1,
        续借 = 2,
        归还 = 3,
        办证缴费 = 4,
        逾期缴费 = 5,
    }
}
