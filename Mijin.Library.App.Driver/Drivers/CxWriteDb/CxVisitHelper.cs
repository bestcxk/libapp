using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog.Internal;
using Util.Dependency;

namespace Mijin.Library.App.Driver
{
    public class CxVisitHelper
    {
        static CxVisitHelper()
        {
        }

        public static void Write(CxEntity cx)
        {
            var str = $@"INSERT INTO visit_data_log (user_id,visit_date_time,visit_date,visit_time,direction,sn_name,sn_code,user_name,card_id)
VALUES('{cx.user_id}','{cx.visit_date_time}','{cx.visit_date}','{cx.visit_time}','{cx.direction}','{cx.sn_name}','{cx.sn_code}','{cx.user_name}','{cx.card_id}')";

            //str = Encoding.UTF8.GetString();
            try
            {
                var count = SQLHelper.Execute("cx", str, null, CommandType.Text);
                Console.WriteLine("写入数据库成功");

            }
            catch (Exception e)
            {
                Console.WriteLine($@"写数据库失败：{e.ToString()}" + "\r\n" + e.StackTrace);
                throw;
            }
        }
    }
}
