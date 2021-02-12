using System;
using System.Collections.Generic;
using System.Text;

namespace Mijin.Library.App.Driver
{
    public class baseSIP2BookInfo
    {
        /// <summary>
        /// 书籍条码
        /// </summary>
        public string Serial { get; set; }
    }
    /// <summary>
    /// 文华图书信息
    /// </summary>
    public class WenhuaBookInfo : baseSIP2BookInfo
    {
        /// <summary>
        /// 在馆/借出
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 图书流通状态
        /// </summary>
        public string CirculationStatus { get; set; }
        /// <summary>
        /// 当前借了该条码图书的读者证号
        /// </summary>
        public string ItemHolder { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 作者
        /// </summary>
        public string Author { get; set; }
        /// <summary>
        /// Isbn
        /// </summary>
        public string Isbn { get; set; }
        /// <summary>
        /// 图书类型
        /// </summary>
        public string MediaType { get; set; }
        /// <summary>
        /// 索引号
        /// </summary>
        public string Callno { get; set; }
        /// <summary>
        /// 所属馆藏地
        /// </summary>
        public string PermanentLocation { get; set; }
        /// <summary>
        /// 当前馆藏地
        /// </summary>
        public string CurrentLocation { get; set; }
        /// <summary>
        /// 预约分配了该条码图书的读者证号
        /// </summary>
        public string ReservationRdid { get; set; }
        /// <summary>
        /// 出版社
        /// </summary>
        public string Publisher { get; set; }
        /// <summary>
        /// 主题词
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public string Page { get; set; }
        /// <summary>
        /// 图书单价
        /// </summary>
        public string ItemProperties { get; set; }
        /// <summary>
        /// 架位号
        /// </summary>
        public string ShelfNo { get; set; }
        /// <summary>
        /// 续借次数
        /// </summary>

        public string ContinueNum { get; set; }


        /// <summary>
        /// 验证码
        /// </summary>
        public string SequenceNumber { get; set; }
        /// <summary>
        /// 应还日期
        /// </summary>
        public string ShuldBackDate { get; set; }
        /// <summary>
        /// 借出日期
        /// </summary>
        public string LendDate { get; set; }

        /// <summary>
        /// 提示信息
        /// </summary>
        public string ScreenMsg { get; set; }
        /// <summary>
        /// 打印信息
        /// </summary>
        public string PrintLine { get; set; }


    }
}
