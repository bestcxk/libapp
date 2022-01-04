using Mijin.Library.App.Model;

namespace Mijin.Library.App.Driver
{
    public interface IWjSIP2Client
    {
        MessageModel<object> BackBook(string bookserial, string libCode);
        MessageModel<bool> Connect(string host, string port);
        MessageModel<object> ContinueBook(string bookserial, string readerNo, string libCode);
        MessageModel<object> GerReaderInfo(RegiesterInfo readerInfo);
        MessageModel<object> GetBookInfo(string bookserial,string libCode);
        MessageModel<object> GetReaderInfo(string readerNo = "", string identity = "", string libCode = "", string readerPw = "");
        MessageModel<object> LendBook(string bookserial, string readerNo, string libCode);
        MessageModel<object> Login(string account, string pw, long cp);
        MessageModel<object> RegiesterReader(RegiesterInfo readerInfo);
    }
}