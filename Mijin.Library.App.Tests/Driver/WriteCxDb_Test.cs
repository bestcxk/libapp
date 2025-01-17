﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mijin.Library.App.Driver;
using Newtonsoft.Json;
using Xunit;

namespace Mijin.Library.App.Tests.Driver
{
    public class WriteCxDb_Test
    {
        public WriteCxDb writeCxDb { get; set; } = new WriteCxDb();


        [Fact]
        public void Write_Test()
        {
            // 设置数据库连接字符串
            writeCxDb.ConnectDbStr = "Server=192.168.0.81;Database=mj;User=thirdlib;Password=Thirdlib@123;MultipleActiveResultSets=True;";

            // 设置实体
            var entity = new CxEntity();
            entity.sn_code = "test";

            // 写数据库
            var res = writeCxDb.WriteDb(entity);
            Assert.True(res.success);
        }

        [Fact]
        public void Read_Test()
        {
            writeCxDb.ConnectDbStr = "Server=192.168.0.81;Database=mj;User=thirdlib;Password=Thirdlib@123;MultipleActiveResultSets=True;";
            var res = writeCxDb.ReadData("2022-03-09 11:31:21");
            Assert.NotNull(res);
        }

    }
}
