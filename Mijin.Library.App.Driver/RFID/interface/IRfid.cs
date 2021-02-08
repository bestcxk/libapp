using Mijin.Library.App.Driver.RFID.Model;
using Mijin.Library.App.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver.RFID
{
    interface IRfid
    {
        event Action<LabelInfo> OnTagEpcLog;

        MessageModel<bool> Auto232Connect();
        MessageModel<bool> Connect(string mode, string conStr);
        MessageModel<FreqRange> GetFreqRange();
        MessageModel<Dictionary<byte, byte>> GetPower();
        MessageModel<bool> Read();
        MessageModel<bool> ReadByAntId(List<uint> antIds);
        MessageModel<LabelInfo> ReadOnce();
        MessageModel<LabelInfo> ReadOnceByAntId(List<uint> antIds);
        MessageModel<bool> SetFreqRange(byte index);
        MessageModel<bool> SetGpo(Dictionary<string, byte> dic);
        MessageModel<Dictionary<byte, byte>> SetPower(Dictionary<byte, byte> dt);
        MessageModel<bool> StartInventory(GpiAction gpiAction = GpiAction.Default);
        MessageModel<bool> Stop();
        MessageModel<bool> StopInventory();
        MessageModel<bool> WriteLabel(byte area, byte startAddr, string data, string baseTid = null, string password = "00000000", int timeOut = 3);
    }
}
