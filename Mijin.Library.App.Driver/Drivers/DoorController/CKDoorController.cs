using System;
using Exceptionless.Submission;
using Mijin.Library.App.Model;
using PublicAPI.CKC001.Connected;

namespace Mijin.Library.App.Driver
{
    public class CkDoorController : ICkDoorController
    {
        private CykeoCtrlServer Server;
        public event Action<WebViewSendModel<string>> OnCkDoorControllerConnected;
        public event Action<WebViewSendModel<string>> OnCkDoorControllerDisconnected;
        public event Action<WebViewSendModel<object>> OnNotityLock;

        private bool _connected = false;

        public CkDoorController()
        {
            Server = new PublicAPI.CKC001.Connected.CykeoCtrlServer(5460);
            if (Server.OnStart())
            {
                Server.ClientConnected += ip =>
                {
                    _connected = true;
                    OnCkDoorControllerConnected?.Invoke(new WebViewSendModel<string>()
                    {
                        method = nameof(OnCkDoorControllerConnected),
                        response = ip,
                        success = true
                    });
                };
                Server.ClientDisconnected += ip =>
                {
                    _connected = false;
                    OnCkDoorControllerDisconnected?.Invoke(new WebViewSendModel<string>()
                    {
                        method = nameof(OnCkDoorControllerDisconnected),
                        response = ip,
                        success = true
                    });
                };
                Server.dNotifyLock += notiy =>
                {
                    OnNotityLock?.Invoke(new WebViewSendModel<object>()
                    {
                        method = nameof(OnNotityLock),
                        response = notiy,
                        success = true
                    });
                };
            }
        }

        public MessageModel<bool> OpenLock()
        {
            Server?.SendAsync(new PublicAPI.CKC001.MessageObj.MsgObj.MsgObj_Lock_OpenLock());

            return new MessageModel<bool>()
            {
                response = true,
                success = true,
                msg = "开锁成功"
            };
        }

        public MessageModel<bool> IsConnected()
        {
            return new MessageModel<bool>()
            {
                success = _connected,
                response = _connected,
                msg = _connected ? "连接成功" : "连接失败"
            };
        }
    }
}