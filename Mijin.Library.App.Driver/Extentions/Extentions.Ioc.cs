using Microsoft.Extensions.DependencyInjection;
using Mijin.Library.App.Driver.LibrarySIP2;
using Mijin.Library.App.Driver.LibrarySIP2.Interface;
using Mijin.Library.App.Driver.Lock;
using Mijin.Library.App.Driver.Lock.Interface;
using Mijin.Library.App.Driver.PosPrint;
using Mijin.Library.App.Driver.PosPrint.Interface;
using Mijin.Library.App.Driver.Reader;
using Mijin.Library.App.Driver.RFID;
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
            services.AddSingleton<ISIP2Client, WenhuaSIP2Client>();
            services.AddSingleton<ICabinetLock, CabinetLock>();
            services.AddSingleton<IPosPrint, MyPosPrint>();
            services.AddSingleton<IdentityReader, WonteReader>();
            services.AddSingleton<IHFReader, BlackHFReader>();
            services.AddSingleton<IRfid, GRfid>();
            services.AddSingleton<IRfidDoor, GRfidDoor>();

            services.AddSingleton<IDriverHandle, DriverHandle>();
        }
    }
}
