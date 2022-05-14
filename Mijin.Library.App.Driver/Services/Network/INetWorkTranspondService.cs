﻿using System.Collections.Generic;
using Mijin.Library.App.Common.Domain;
using Mijin.Library.App.Model;

namespace Mijin.Library.App.Driver.Services.Network;

public interface INetWorkTranspondService
{
    List<NetWorkTranspond> Transponds { get; set; }
    void StartOrUpdateListen(ClientSettings settings);
    string GetVisitUrl(string url);
    void ClearAllListen();
}