using Mijin.Library.App.Model;

namespace Mijin.Library.App.Driver
{
    public interface IDataConvert
    {
        MessageModel<string> RFID_Decode96bit(string epc);
    }
}