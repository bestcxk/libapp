using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Model
{
    /// <summary>
    /// web 前端发送类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WebViewSendModel<T> : MessageModel<T>
    {
        public WebViewSendModel()
        {
        }

        public WebViewSendModel(baseMessageModel @base) : base(@base)
        {
        }
        /// <summary>
        /// 执行方法 sample : ISIP2Client.Connect
        /// </summary>
        public string method { get; set; }
        /// <summary>
        /// guid，确保唯一性
        /// </summary>
        public string guid { get; set; }
    }
}
