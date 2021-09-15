using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PublicAPI.CKC001.Others
{
    public class RfidReport
    {
        public int all_tag_num { get; set; } = 0;
        public int add_tag_num { get; set; } = 0;
        public List<EpcInfo> add_tag_list { get; set; } = new List<EpcInfo>();
        public int delete_tag_num { get; set; } = 0;
        public List<EpcInfo> delete_tag_list { get; set; } = new List<EpcInfo>();
        public List<EpcInfo> tag_list { get; set; } = new List<EpcInfo>();
    }




    public class EpcInfo
    {
        public string epc { get; set; }
    }

}
