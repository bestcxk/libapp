using System.Collections.Generic;
using System.Threading.Tasks;
using Mijin.Library.App.Common.Domain;
using Mijin.Library.App.Model;

namespace Mijin.Library.App.Driver.Services.Network;

public interface INetWorkTranspondService
{
    Task StartOrUpdateListen(ClientSettings settings);
    string GetVisitUrl(string url);
    Task ClearAllListen();
}