using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Model
{
    public class ReqMessageModel<T> : baseMessageModel
    {
        public ReqMessageModel()
        {
        }

        public ReqMessageModel(baseMessageModel @base) : base(@base)
        {
        }

        /// <summary>
        /// 执行方法 sample : ISIP2Client.Connect
        /// </summary>
        public string method { get; set; }

        /// <summary>
        /// 请求参数
        /// </summary>
        public object[] @params { get; set; }

        /// <summary>
        /// guid，确保唯一性
        /// </summary>
        public string guid { get; set; }
    }
}