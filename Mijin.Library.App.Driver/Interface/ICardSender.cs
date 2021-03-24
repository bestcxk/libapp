using Mijin.Library.App.Model;
using System;

namespace Mijin.Library.App.Driver
{
    public interface ICardSender
    {

        /// <summary>
        /// 发卡器状态 仅初始化以后才会发送
        /// </summary>
        event Action<MessageModel<CardSenderStatus>> OnCardSenderStatus;

        /// <summary>
        /// 获取 发卡/卡槽 数量
        /// </summary>
        /// <returns></returns>
        MessageModel<CardSenderInOut> GetInOutNum();
        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        MessageModel<string> Init();
        /// <summary>
        /// 重置 发卡/卡槽 数量
        /// </summary>
        /// <returns></returns>
        MessageModel<string> RestInOutNum();
        /// <summary>
        /// 发卡
        /// </summary>
        /// <returns>response:[卡号]</returns>
        MessageModel<string> SpitCard();
    }
}