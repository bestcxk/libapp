﻿using Mijin.Library.App.Model;
using Mijin.Library.App.Model.Setting;
using System;
using System.Collections.Generic;

namespace Mijin.Library.App.Driver
{
    public interface IGRfidDoorController
    {
        event Action<WebViewSendModel<PeopleInOut>> OnDoorPeopleInOut;
        event Action<WebViewSendModel<LabelInfo>> OnDoorReadUHFLabel;

        MessageModel<string> ConnectDoors(Dictionary<int, DoorSetting> connectDic);
        MessageModel<List<object>> GetConnectDoorInfos();
        MessageModel<Dictionary<int, Dictionary<byte, byte>>> GetDoorsPower();
        MessageModel<string> SetDoorsPower(Dictionary<int, Dictionary<byte, byte>> powers);
        MessageModel<string> StartAllDoorWatch(bool clear = false);
        MessageModel<string> StopAllDoorWatch();
    }
}