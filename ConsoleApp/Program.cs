using Microsoft.Win32;
using Mijin.Library.App.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;


namespace ConsoleApp
{
    class Program
    {
        public static ISystemFunc systemFunc;
        public static CardSender cardSender;
        static void Main(string[] args)
        {
            systemFunc = new SystemFunc();
            cardSender = new CardSender(systemFunc);

            var initRes = cardSender.Init();
            var res = cardSender.SpitCard();
            Console.WriteLine(res.response);
            //Thread.Sleep(2000);
            //var dt = cardSender.GetStatus();
            //cardSender.Test();
            //Console.WriteLine(dt.msg);

        }



    }
}
