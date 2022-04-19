using System.Collections.Generic;
using Bing.Extensions;
using SIP2Client.Entities;
using SIP2Client.Entities.Sip2Response;

namespace Mijin.Library.App.Driver.Drivers.LibrarySIP2.Models.JpSip2Valid
{
    /// <summary>
    /// 查询书籍信息判断
    /// </summary>
    public class JpSip2ValidBook : BaseSip2Response
    {
        public override ErrorCode Valid(Sip2Transaction sip2Transaction)
        {
            //验证操作是否成功
            if (sip2Transaction.Field.ContainsKey("AJ") || sip2Transaction.Field.ContainsKey("AQ"))
            {
                if (!sip2Transaction.Field.GetValueOrDefault("AJ").IsEmpty() || !sip2Transaction.Field.GetValueOrDefault("AQ").IsEmpty())
                {
                    return ErrorCode.Success;
                }
            }

            return ErrorCode.Failed;
        }
    }
}
