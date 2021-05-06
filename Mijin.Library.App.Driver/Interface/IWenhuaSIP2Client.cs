using Mijin.Library.App.Model;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// 其他图书馆ISP2接口
    /// </summary>
    public interface IWenhuaSIP2Client
    {
        /// <summary>
        /// 归还书籍
        /// </summary>
        /// <param name="bookserial">书籍条码</param>
        /// <returns></returns>
        MessageModel<object> BackBook(string bookserial);
        /// <summary>
        /// 连接Socket
        /// </summary>
        /// <param name="host"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        MessageModel<bool> Connect(string host, string port);

        /// <summary>
        /// 续借书籍
        /// </summary>
        /// <param name="bookserial"></param>
        /// <param name="readerNo"></param>
        /// <returns></returns>
        MessageModel<object> ContinueBook(string bookserial, string readerNo);

        /// <summary>
        /// 获取书籍信息
        /// </summary>
        /// <param name="bookserial"></param>
        /// <returns></returns>
        MessageModel<object> GetBookInfo(string bookserial);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="readerNo"></param>
        /// <param name="readerPw"></param>
        /// <returns></returns>
        MessageModel<object> GetReaderInfo(string readerNo, string readerPw = null);

        /// <summary>
        /// 借阅书籍
        /// </summary>
        /// <param name="bookserial"></param>
        /// <param name="readerNo"></param>
        /// <returns></returns>
        MessageModel<object> LendBook(string bookserial, string readerNo);

        /// <summary>
        /// 登录SIP2
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pw"></param>
        /// <returns></returns>
        MessageModel<object> Login(string account, string pw);
        /// <summary>
        /// 注册读者
        /// </summary>
        /// <param name="readerInfo"></param>
        /// <returns></returns>
        MessageModel<object> RegiesterReader(RegiesterInfo readerInfo);
    }
}