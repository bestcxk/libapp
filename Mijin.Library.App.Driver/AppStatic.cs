using System;
using Microsoft.Extensions.DependencyInjection;

namespace Mijin.Library.App.Driver;

public class AppStatic
{
    public static IServiceProvider Services { get; set; }

    public static void CloseRfid()
    {
        var rfids = AppStatic.Services?.GetService<IMultiGrfid>();
        rfids?.Stop();
        rfids?.Close();
        
        var rfid =  AppStatic.Services?.GetService<IRfid>();

        rfid?.Stop();
        rfid?.Close();

        var rfidDoor = AppStatic.Services?.GetService<IRfidDoor>();
        rfidDoor?.Stop();
        rfidDoor?.Close();
    }
    
}