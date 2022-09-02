using Microsoft.Extensions.DependencyInjection;
using Mijin.Library.App.Driver.Services.Network;
using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mijin.Library.App.Driver.Drivers.DataConvert;
using Mijin.Library.App.Driver.Drivers.DhCamera;
using Mijin.Library.App.Driver.Drivers.LibrarySIP2;
using Mijin.Library.App.Driver.Interface;
using Mijin.Library.App.Driver.Drivers.Lock;
using Mijin.Library.App.Driver.Drivers.RFID;

namespace Mijin.Library.App.Driver.Extentions
{
    public static partial class Extentions
    {
        public static void AddDriver(this IServiceCollection services)
        {
            ClientSettings clientSettings = new ClientSettings();

            services.AddSingleton<IDriverHandle, DriverHandle>();
            services.AddSingleton<IWenhuaSIP2Client, WenhuaSIP2Client>();
            services.AddSingleton<ICabinetLock, CabinetLock>();
            services.AddSingleton<IPosPrint, MyPosPrint>();

            if (clientSettings.IsM513IdentityReader)
                services.AddSingleton<IdentityReader, M513Reader>();
            else
                services.AddSingleton<IdentityReader, WonteReader>();

            if (clientSettings.QrcodeDriver == QrcodeDriver.qrcode)
                services.AddSingleton<IQrCode, QrCode>();
            else
                services.AddSingleton<IQrCode, VbarQrCode>();

            services.AddSingleton<IRRfid, RRfid>();
            services.AddSingleton<IRfid, GRfid>();
            services.AddSingleton<IGRfidDoorController, GRfidDoorController>();
            services.AddSingleton<IKeyboard, Keyboard>();
            services.AddSingleton<ISystemFunc, SystemFunc>();
            services.AddSingleton<ICamera, Camera>();
            services.AddSingleton<ICardSender, CardSender>();
            services.AddSingleton<IDoorController, ZktDoorController>();
            services.AddSingleton<ITuChuangSIP2Client, TuChuangSIP2Client>();
            services.AddSingleton<ICkDoorController, CkDoorController>();
            services.AddSingleton<ITrack, Track>();
            services.AddSingleton<IMultiGrfid, MultiGrfid>();
            services.AddSingleton<ISudo, Sudo>();
            services.AddSingleton<IWjSIP2Client, WjSIP2Client>();
            services.AddSingleton<IDataConvert, DataConvert>();
            services.AddSingleton<IWriteCxDb, WriteCxDb>();
            services.AddSingleton<INetWorkTranspondService, TestTranspondService>();
            services.AddSingleton<IJpSip2Client, JpSip2Client>();

            services.AddSingleton<IDhCamera, DhCamera>();
            services.AddSingleton<IDhPeopleInoutCamera, DhCamera>();
            services.AddSingleton<IRfidDoor, GRfidDoor>();


            #region HFReader IOC

            if (clientSettings.HFReader == HFReaderEnum.BlackReader)
                services.AddSingleton<IHFReader, BlackHFReader>();
            else
                services.AddSingleton<IHFReader, RRHFReader>();

            services.AddSingleton<ICkLock, CkLock>();
            services.AddSingleton<IGrfidKeyboard, GrfidKeyboard>();
            services.AddSingleton<IGrfidKeyboard1, GrfidKeyboard>();


            #endregion
        }
    }
}