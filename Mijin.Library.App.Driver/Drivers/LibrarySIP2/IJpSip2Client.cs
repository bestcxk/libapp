using Mijin.Library.App.Model;
using SIP2Client.Entities;
using SIP2Client.Entities.Sip2Request;

namespace Mijin.Library.App.Driver.Drivers.LibrarySIP2;

public interface IJpSip2Client
{
    MessageModel<string> Init(Sip2Model sip2Info);

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="sip2Info">初始化信息</param>
    /// <returns></returns>
    MessageModel<string> Init(object sip2Info);

    /// <summary>
    /// 获取书籍信息
    /// </summary>
    /// <param name="bookIdentifier">图书条码</param>
    /// <param name="institutionId">图书馆名称</param>
    /// <returns></returns>
    MessageModel<object> GetBookInfo(string bookIdentifier, string institutionId);

    /// <summary>
    /// 获取读者信息
    /// </summary>
    /// <param name="readerIdentifier">读者证号</param>
    /// <param name="institutionId">图书馆名称</param>
    /// <returns></returns>
    MessageModel<object> GetReaderInfo(string readerIdentifier, string institutionId);

    /// <summary>
    /// 借书
    /// </summary>
    /// <param name="bookIdentifier">图书条码</param>
    /// <param name="readerIdentifier">读者证号</param>
    /// <param name="institutionId">图书馆名称</param>
    /// <returns></returns>
    MessageModel<object> LendBook(string bookIdentifier, string readerIdentifier, string institutionId);

    /// <summary>
    /// 还书
    /// </summary>
    /// <param name="bookIdentifier">图书条码</param>
    /// <param name="institutionId">图书馆名称</param>
    /// <returns></returns>
    MessageModel<object> BackBook(string bookIdentifier, string institutionId);

    /// <summary>
    /// 续借
    /// </summary>
    /// <param name="bookIdentifier">图书条码</param>
    /// <param name="readerIdentifier">读者证号</param>
    /// <param name="institutionId">图书馆名称</param>
    /// <returns></returns>
    MessageModel<object> ReNewBook(string bookIdentifier, string readerIdentifier, string institutionId);

    /// <summary>
    /// 自助办证
    /// </summary>
    /// <param name="readerRegister">自助办证</param>
    /// <returns></returns>
    MessageModel<object> RegisterReader(Sip2ReaderRegisterRequest readerRegister);

    /// <summary>
    /// 自助办证(用于web进行反射调用)
    /// </summary>
    /// <param name="sip2Info">自助办证</param>
    /// <returns></returns>
    MessageModel<object> RegisterReader(object sip2Info);
}