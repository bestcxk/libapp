using System;
using Exceptionless.Submission;
using IsUtil.Helpers;
using Mijin.Library.App.Model;
using Mijin.Library.App.Model.Setting;
using PublicAPI.CKC001.Connected;

namespace Mijin.Library.App.Driver
{
    public class CkDoorController : ICkDoorController
    {
        private CykeoCtrlServer Server;
        public event Action<WebViewSendModel<string>> OnCkDoorControllerConnected;
        public event Action<WebViewSendModel<string>> OnCkDoorControllerDisconnected;
        public event Action<WebViewSendModel<object>> OnNotityLock;
        public event Action<WebViewSendModel<string>> OnNotityHFCard;


        private bool _connected = false;
        private readonly ISystemFunc _systemFunc;

        public CkDoorController(ISystemFunc systemFunc)
        {
            this._systemFunc = systemFunc;

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

                Server.dNotifyHF += notiy =>
                {
                    var bytes = notiy.getIDNumByte;
                    // 转换卡号
                    string m_cardNo = string.Empty;
                    var cardStr = "";


                    for (int q = 0; q < bytes.Length; q++)
                    {
                        m_cardNo += SerialPortHelper.byteHEX(bytes[q]);
                    }
                    string str = "";
                    for (int i = 0; i < m_cardNo.Length; i += 2)
                    {
                        string dt = m_cardNo[i].ToString() + m_cardNo[i + 1].ToString();
                        str = str.Insert(0, dt);
                    }

                    if (_systemFunc.ClientSettings.HFOriginalCard)
                        cardStr = m_cardNo.ToUpper();
                    else
                        cardStr = IcSettings.DataHandle(Convert.ToInt64(str, 16).ToString(), _systemFunc.LibrarySettings?.IcSettings);


                    OnNotityHFCard?.Invoke(new WebViewSendModel<string>()
                    {
                        method = nameof(OnNotityHFCard),
                        response = cardStr,
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