using System;
using System.Collections.Generic;
using System.Text;
using Util.Helpers;

namespace Mijin.Library.App.Model.Model
{
    /// <summary>
    /// 通用返回信息类
    /// </summary>
    public class MessageModel<T>
    {
        public MessageModel()
        {

        }
        /// <summary>
        /// 该构造函数最好还是重新赋值response
        /// </summary>
        /// <param name="messageModel"></param>
        public MessageModel(dynamic messageModel)
        {
            status = messageModel.status;
            success = messageModel.success;
            msg = messageModel.msg;
            devMsg = messageModel.devMsg;

            try
            {
                //response = (T)messageModel.response;
                response = Json.ToObject<T>(Json.ToJson(messageModel.response));
            }
            catch (Exception)
            {
            }
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
        /// <summary>
        /// 返回数据集合
        /// </summary>
        public T response { get; set; }
    }
}
