using Microsoft.Win32;
using Mijin.Library.App.Driver;
using Mijin.Library.App.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Util;
using Util.Helpers;
using Util.Maps;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            {
                string result = null;
                RegistryKey key;
                try
                {
                    key = Registry.LocalMachine.OpenSubKey(@"F3017226-FE2A-4295-8BDF-00C3A9A7E4C5");
                    //if (key != null)
                    //{
                    //    object o = key.GetValue(Define.VersionSign);
                    //    return o.ToString();
                    //}
                }
                catch (Exception e)
                {
                    //Helper.Write(e.ToString());
                }
            }
            {
                IRfid rfid = new GRfid();
                EventInfo[] eventInfos = typeof(GRfid).GetEvents();

                foreach (var eventInfo in eventInfos)
                {
                    //eventInfo.AddEventHandler(rfid,);
                }

                //IDriverHandle driverHandle = new DriverHandle(new GRfid());

                //var result = driverHandle.Invoke("IRfid", "Test",null);


            }
            ClientSettings settings = new ClientSettings();
            settings.Write();

            {
                IRfid rfid = new GRfid();

                Type type = rfid.GetType();
                const BindingFlags InstanceBindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

                //PropertyInfo propertyInfo = rfid.GetType().GetProperties(InstanceBindFlags).Where(p => p.PropertyType.Name == "Test").FirstOrDefault();

                object[] parameters = null;

                // 获取执行参数的所有type
                Type[] parametersTypes = parameters == null ? new Type[] { } : parameters.Select(p => p.GetType()).ToArray();

                var obj = type.GetMethod("Test", parametersTypes).Invoke(rfid, parameters);

                //var obj = propertyInfo.PropertyType.GetMethod("Print", parametersTypes).Invoke(propertyInfo, parameters);

                //var obj = type.InvokeMember("_posPrint", InstanceBindFlags, null, driverHandle, parametersTypes);
                //type.FindInterfaces();


            }

            Test test = new Test();

            string reqStr = "{\"method\": \"TestInvoke\",\"guid\": \"sadwqeewqesad\",\"parameters\": [\"test\", [4, 5, 6],{Name:\"大王\",Age:2}]}";

            ReqMessageModel reqModel = Json.ToObject<ReqMessageModel>(reqStr);

            List<string> strings = new List<string>()
            {
                "1","2"
            };

            for (int i = 0; i < 1000; i++)
            {
                strings.Add((i+5).ToString());
            }

            var strsb = strings.JsonMapToList<uint>();

            var bl = "false".ToBool();
            //reqModel.parameters[1] = Json.ToObject<List<dynamic>>(reqModel.parameters[1].ToString());

            //reqModel.parameters[1] = Json.ToObject<List<object>>(reqModel.parameters[1].ToString());
            reqModel.parameters[1] = reqModel.parameters[1].JsonMapTo<List<object>>();

            reqModel.parameters[2] = reqModel.parameters[2].JsonMapTo<Dictionary<object, object>>();

            reqModel.parameters[2] = reqModel.parameters[2].JsonMapTo<People>();
            //reqModel.parameters[2] = Json.ToObject<People>(reqModel.parameters[2].ToString());

            var types = reqModel.parameters.Select(p => p.GetType()).ToArray();

            var data = test.GetType().GetMethod(reqModel.method, types).Invoke(test, reqModel.parameters);
            //var convertData = new WebMessageModel<object>(data);
            var before = DateTime.Now;
            //var convertData = new WebMessageModel<int>(data);

            var dts = new MessageModel<object>()
            {
                status = 100,
                success = true,
                msg = "fasad",
                devMsg = "sad",
                response = new List<int>() {1,2,3 }
            };

            Console.WriteLine((DateTime.Now- before).TotalMilliseconds + "ms");
            Console.ReadKey();
        }

        

        private class ReqMessageModel
        {
            public string method { get; set; }

            public string guid { get; set; }

            public object[] parameters { get; set; }
        }

        private class WebMessageModel<T> : MessageModel<T>
        {
            public WebMessageModel()
            {
            }

            public WebMessageModel(baseMessageModel @base) : base(@base)
            {

            }

            public string method { get; set; }

            public string guid { get; set; }
        }
    }
    public class People
    {
        public string Name { get; set; }
        public string Age { get; set; }
    }
    public enum e
    {
        OK = 0,
        Timeout = 1,
        NoResponse = 2
    }
    
    interface ITest
    {
        MessageModel<Dictionary<string, string>> GetString();
    }
    public class Test : ITest
    {
        public Test()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public int Age { get; set; }

        public string Addr { get; set; }

        public string exist { get; set; }

        public int isBool { get; set; }

        public MessageModel<Dictionary<string, string>> GetString()
        {
            return new MessageModel<Dictionary<string, string>>()
            {
                msg = "成功",
                response = new Dictionary<string, string>() {
                    { "1","1"}
                }
            };
        }

        public MessageModel<Dictionary<string, string>> TestInvoke(string dt, IEnumerable<object> dat, People people)
        {
            return new MessageModel<Dictionary<string, string>>()
            {
                msg = "成功People",
                response = new Dictionary<string, string>() {
                    { "dat",Json.ToJson(dat)}
                }
            };
        }

        public MessageModel<Dictionary<string, string>> TestInvoke(string dt, IEnumerable<object> dat, object people)
        {
            return TestInvoke(dt,dat,Json.ToObject<People>(people.ToString()));
        }
    }

    public class Test2
    {
        public int Id { get; set; }
        public string Name { get; set; }


        public int Age { get; set; }

        public string Addr { get; set; }


        public string NoExist { get; set; }

        public bool isBool { get; set; }


        public string Exist { get; set; }

    }
}
