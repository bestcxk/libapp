using Mijin.Library.App.Model;
using Mijin.Library.App.Model.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util;
using Util.Maps;

namespace Mijin.Library.App.Driver
{
    public class SystemFunc : ISystemFunc
    {
        public SystemFunc()
        {
            ClientSettings = new ClientSettings();
        }

        public LibrarySettings LibrarySettings { get; set ; }
        public ClientSettings ClientSettings { get; set; }

        /// <summary>
        /// 获取系统中所有能使用的com口
        /// </summary>
        /// <returns></returns>
        public MessageModel<string[]> GetComs() =>
            new MessageModel<string[]>()
            {
                msg = "获取成功",
                response = System.IO.Ports.SerialPort.GetPortNames(),
                success = true
            };

        /// <summary>
        /// 配置图书馆设置
        /// </summary>
        /// <param name="librarySettings"></param>
        /// <returns></returns>
        public MessageModel<int> SetLibrarySettings(LibrarySettings librarySettings)
        {
            var result = new MessageModel<int>();
            if (librarySettings.IsNull())
            {
                result.msg = "参数不可为空";
                return result;
            }
            LibrarySettings = librarySettings;
            result.success = true;
            result.response = ClientSettings.Id;
            result.msg = "设置成功";
            return result;
        }

        public MessageModel<int> SetLibrarySettings(object librarySettings)
        {
            return SetLibrarySettings(librarySettings.JsonMapTo<LibrarySettings>());
        }

    }
}
