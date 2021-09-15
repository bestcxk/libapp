using Mijin.Library.App.Model;
using System;

namespace Mijin.Library.App.Driver
{
    public interface ICkDoorController
    {
        event Action<WebViewSendModel<string>> OnCkDoorControllerConnected;
        event Action<WebViewSendModel<string>> OnCkDoorControllerDisconnected;
        event Action<WebViewSendModel<object>> OnNotityLock;

        MessageModel<bool> OpenLock();
    }
}