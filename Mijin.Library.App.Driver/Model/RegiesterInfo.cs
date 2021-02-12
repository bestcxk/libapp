using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mijin.Library.App.Driver
{
    /// <summary>
    /// SIP 用户自助办证信息类
    /// </summary>
    public class RegiesterInfo
    {
        /// <summary>
        /// 身份证号
        /// </summary>
        public string Identity { get; set; } = "";
        /// <summary>
        /// 密码
        /// </summary>
        public string Pw { get; set; } = "";
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; } = "";
        /// <summary>
        /// 创建用户的馆
        /// </summary>
        public string CreateReaderLibrary { get; set; } = "";
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; } = "";
        /// <summary>
        /// 住址
        /// </summary>
        public string Addr { get; set; } = "";
        /// <summary>
        /// 用户创建类型
        /// </summary>
        public string Type { get; set; } = "";
        /// <summary>
        /// 用户押金
        /// </summary>
        public decimal Moeny { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public bool Sex { get; set; } = true;
    }
}
