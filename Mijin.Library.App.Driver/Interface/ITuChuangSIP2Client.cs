using Mijin.Library.App.Model;

namespace Mijin.Library.App.Driver
{
    public interface ITuChuangSIP2Client
    {
        MessageModel<object> BackBook(string bookserial);
        MessageModel<object> ContinueBook(string bookserial, string readerNo);
        MessageModel<object> GetBookInfo(string bookserial);
        MessageModel<object> GetReaderInfo(string readerNo, string readerPw = null);
        MessageModel<object> LendBook(string bookserial, string readerNo);
    }
}