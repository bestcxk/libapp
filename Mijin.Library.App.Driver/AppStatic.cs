using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Threading.Tasks;
using IsUtil.Helper;
using Microsoft.Extensions.DependencyInjection;
using Mijin.Library.App.Driver.Services.Network;

namespace Mijin.Library.App.Driver;

public class AppStatic
{
    public static IServiceProvider Services { get; set; }

    public static void CloseRfid()
    {

        Process current = Process.GetCurrentProcess();
        var list = new List<Process>();

        var network = AppStatic.Services?.GetService<INetWorkTranspondService>();

        network.ClearAllListen();



        var sudo = AppStatic.Services?.GetService<ISudo>();
        sudo?.Close();

        var rfids = AppStatic.Services?.GetService<IMultiGrfid>();
        rfids?.Stop();
        rfids?.Close();

        var rfid = AppStatic.Services?.GetService<IRfid>();

        rfid?.Stop();
        rfid?.Close();

        var rfidDoor = AppStatic.Services?.GetService<IRfidDoor>();
        rfidDoor?.Stop();
        rfidDoor?.Close();

        var rfidDoorController = AppStatic.Services?.GetService<IGRfidDoorController>();
        rfidDoorController?.StopAllDoorWatch();
        rfidDoorController?.CloseAll();
    }

}