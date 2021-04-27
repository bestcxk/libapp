using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver
{
    public class PeopleInOut
    {
        public PeopleInOut()
        {
        }

        public PeopleInOut(InOut inOut, int inCount, int outCount)
        {
            InOut = inOut;
            InCount = inCount;
            OutCount = outCount;
        }

        public InOut InOut { get; set; }
        public int InCount { get; set; }
        public int OutCount { get; set; }
        public int DoorKey { get; set; }
    }
    public enum InOut
    {
        /// <summary>
        /// 出馆
        /// </summary>
        Out,
        /// <summary>
        /// 进馆
        /// </summary>
        In
    }
}
