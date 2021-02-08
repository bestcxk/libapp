using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver.RFID.Model
{
    /// <summary>
    /// RFID 频段
    /// </summary>
    public class FreqRange
    {
        [JsonIgnore]
        public static readonly string[] freqNames = { "国标 920~925MHz", "国标 840~845MH", "国标 840~845MHz 和 920~925MHz", "FCC， 902~928MHz", "ETSI， 866~868MHz" };
        /// <summary>
        /// FreqRange索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 当前频段名称
        /// </summary>
        public string Name { get; set; }
    }
}
