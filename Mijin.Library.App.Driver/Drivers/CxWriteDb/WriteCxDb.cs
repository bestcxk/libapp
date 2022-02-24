using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bing.Extensions;
using IsUtil.Maps;
using Mijin.Library.App.Model;
using Util.Dependency;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// 超鑫写数据库
    /// </summary>
    public class WriteCxDb : IWriteCxDb
    {
        private string connectDbStr;

        public string ConnectDbStr
        {
            get { return connectDbStr; }
            set
            {
                connectDbStr = value;
                SQLHelper.ConnStrs.Remove("cx");
                SQLHelper.ConnStrs.Add("cx", value);
            }
        }


        public MessageModel<string> WriteDb(CxEntity entity)
        {
            var res = new MessageModel<string>();
            try
            {
                entity.WriteToDb();
                res.success = true;
                res.msg = "写入成功";
            }
            catch (Exception e)
            {
                res.msg = @$"写入失败：{e}";
            }
            return res;
        }

        public MessageModel<string> WriteDb(string entity)
        {
            return WriteDb(entity.JsonMapTo<CxEntity>());
        }



    }
}
