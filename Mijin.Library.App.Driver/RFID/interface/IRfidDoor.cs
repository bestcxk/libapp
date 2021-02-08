using Mijin.Library.App.Driver.RFID.Model;
using Mijin.Library.App.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver.RFID
{
    interface IRfidDoor : IRfid
    {
        event Action<PeopleInOut> OnPeopleInOut;

        MessageModel<bool> StartWatchPeopleInOut(bool clear = false);
        MessageModel<bool> StopWatchPeopleInOut();
    }
}
