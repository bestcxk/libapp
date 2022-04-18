using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Model
{
    public class baseMessageModel
    {
        public baseMessageModel()
        {
        }

        public baseMessageModel(baseMessageModel @base)
        {
            status = @base.status;
            success = @base.success;
            msg = @base.msg;
            devMsg = @base.devMsg;
        }

        /// <summary>
        /// 状态码
        /// </summary>
        public int status { get; set; } = 200;
        /// <summary>
        /// 操作是否成功
        /// </summary>
        public bool success { get; set; } = false;
        /// <summary>
        /// 返回信息
        /// </summary>
        public string msg { get; set; } = "服务器异常";

        /// <summary>
        /// 调试信息
        /// </summary>
        public string devMsg { get; set; }


        public void SetMsg()
        {
            if (success)
                this.msg = "操作成功";
            else this.msg = "操作失败";
        }
    }
}
