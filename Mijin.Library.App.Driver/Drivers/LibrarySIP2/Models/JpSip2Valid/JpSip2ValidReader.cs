using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bing.Extensions;
using SIP2Client.Entities;
using SIP2Client.Entities.Sip2Response;

namespace Mijin.Library.App.Driver.Drivers.LibrarySIP2.Models.JpSip2Valid
{
    /// <summary>
    /// 查询读者信息判断
    /// </summary>
    public class JpSip2ValidReader : BaseSip2Response
    {
        public override ErrorCode Valid(Sip2Transaction sip2Transaction)
        {
            //验证操作是否成功
            if (sip2Transaction.Field.ContainsKey("BL"))
            {
                if (!sip2Transaction.Field.GetValueOrDefault("BL").IsEmpty())
                {
                    if(sip2Transaction.Field.GetValueOrDefault("BL") == "Y") return ErrorCode.Success;
                }
            }

            return ErrorCode.Failed;
        }
    }
}
