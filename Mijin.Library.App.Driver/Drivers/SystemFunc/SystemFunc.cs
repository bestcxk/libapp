﻿using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver
{
    public class SystemFunc : ISystemFunc
    {
        public SystemFunc()
        {
        }

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


        public MessageModel<string> Test()
        {
            return new MessageModel<string>() {
                msg = "调用成功"
            };
        }

    }
}
