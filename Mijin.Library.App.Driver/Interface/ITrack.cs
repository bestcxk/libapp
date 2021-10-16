using Mijin.Library.App.Model;
using System;

namespace Mijin.Library.App.Driver
{
    public interface ITrack
    {
        event Action<WebViewSendModel<TrackModel>> OnTrack;

        MessageModel<bool> Connect(long comPort);
        MessageModel<bool> Output(long no, bool on = false);
    }
}