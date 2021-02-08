using Mijin.Library.App.Model.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Util.Helpers;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime before = DateTime.Now;
            Task.Delay(1000).GetAwaiter().GetResult();

            var dt = (DateTime.Now - before).TotalMilliseconds;
        }
    }

    public enum e
    {
        OK = 0,
        Timeout = 1,
        NoResponse = 2
    }

    public class Test
    {
        public int Id { get; set; } 
        public string Name { get; set; } 

        public int Age { get; set; } 

        public string Addr { get; set; }

        public string exist { get; set; } 

        public int isBool { get; set; }
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
