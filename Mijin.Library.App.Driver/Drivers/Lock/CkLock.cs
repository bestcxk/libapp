using Mijin.Library.App.Driver.Interface;
using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver.Drivers.Lock;
public class CkLock : ICkLock
{
    public bool IsOpen => throw new NotImplementedException();

    public event Action<WebViewSendModel<List<bool>>> OnLockEvent;

    public MessageModel<List<bool>> GetLockStatus()
    {
        throw new NotImplementedException();
    }

    public MessageModel<string> OpenBox(long boxIndex)
    {
        throw new NotImplementedException();
    }

    public MessageModel<bool> OpenSerialPort(string com, long baud)
    {
        throw new NotImplementedException();
    }
}
