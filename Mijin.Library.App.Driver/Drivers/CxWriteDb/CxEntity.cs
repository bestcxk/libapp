using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Dependency;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// 超鑫写门禁实体
    /// </summary>
    public class CxEntity
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string user_id { get; set; }
        /// <summary>
        /// 认证记录日期及时间
        /// </summary>
        public string visit_date_time { get; set; }
        /// <summary>
        /// 认证记录日期
        /// </summary>
        public string visit_date { get; set; }
        /// <summary>
        /// 认证记录时间
        /// </summary>
        public string visit_time { get; set; }
        /// <summary>
        /// 方向
        /// </summary>
        public string direction { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string sn_name { get; set; }
        /// <summary>
        /// 设备序列号
        /// </summary>
        public string sn_code { get; set; }
        /// <summary>
        /// 人员名称
        /// </summary>
        public string user_name { get; set; }
        /// <summary>
        /// 卡号（军官证号）
        /// </summary>
        public string card_id { get; set; }

        /// <summary>
        /// 写数据库，失败报异常
        /// </summary>
        public void WriteToDb()
        {
            CxVisitHelper.Write(this);
        }
    }
}
