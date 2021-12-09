using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;

namespace Mijin.Library.App.Driver
{
    public interface IMultiGrfid
    {
        event Action<WebViewSendModel<LabelInfo>> OnReadUHFLabel;

        MessageModel<string> Close();
        MessageModel<string> Connected();
        MessageModel<bool> ConnectRfids(List<MultiGrfidProp> props);
        MessageModel<bool> ReadByAntIdNoTid(List<string> antIdStrs);
        MessageModel<bool> ReadByNoTid();
        MessageModel<string> SetAllAntPower(long power);
        MessageModel<string> Stop();
    }
}