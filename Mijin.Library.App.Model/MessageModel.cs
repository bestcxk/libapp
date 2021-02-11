using System;
using System.Collections.Generic;
using System.Text;
using Util;

namespace Mijin.Library.App.Model
{
    /// <summary>
    /// 通用返回信息类
    /// </summary>
    public class MessageModel<T> : baseMessageModel
    {
        public MessageModel(baseMessageModel @base) : base(@base)
        {

        }
        public MessageModel()
        {

        }
        ///// <summary>
        ///// 该构造函数最好还是重新赋值response
        ///// </summary>
        ///// <param name="messageModel"></param>
        //public MessageModel(dynamic messageModel)
        //{
        //    status = messageModel.status;
        //    success = messageModel.success;
        //    msg = messageModel.msg;
        //    devMsg = messageModel.devMsg;

        //    try
        //    {
        //        //response = (T)messageModel.response;
        //        response = Json.ToObject<T>(Json.ToJson(messageModel.response));
        //    }
        //    catch (Exception)
        //    {
        //    }
        //}

        /// 返回数据集合
        /// </summary>
        public T response { get; set; }
    }
}
