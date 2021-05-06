using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver.Extentions
{
    public static partial class Extentions
    {
        public static void AddDriver(this IServiceCollection services)
        {
            services.AddSingleton<IDriverHandle, DriverHandle>();
            services.AddSingleton<IWenhuaSIP2Client, WenhuaSIP2Client>();
            services.AddSingleton<ICabinetLock, CabinetLock>();
            services.AddSingleton<IPosPrint, MyPosPrint>();
            services.AddSingleton<IdentityReader, WonteReader>();
            services.AddSingleton<IHFReader, BlackHFReader>();
            services.AddSingleton<IRfid, GRfid>();
            services.AddSingleton<IGRfidDoorController, GRfidDoorController>();
            services.AddSingleton<IKeyboard, Keyboard>();
            services.AddSingleton<ISystemFunc, SystemFunc>();
            services.AddSingleton<ICamera, Camera>();
            services.AddSingleton<ICardSender, CardSender>();
            services.AddSingleton<IDoorController, ZktDoorController>();
            services.AddSingleton<ITuChuangSIP2Client, TuChuangSIP2Client>();
        }
    }
}
