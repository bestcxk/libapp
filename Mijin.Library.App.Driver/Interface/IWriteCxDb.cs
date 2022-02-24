using Mijin.Library.App.Model;

namespace Mijin.Library.App.Driver
{
    public interface IWriteCxDb
    {
        string ConnectDbStr { get; set; }

        MessageModel<string> WriteDb(CxEntity entity);
    }
}