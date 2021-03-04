using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Util.Cache;

namespace Mijin.Library.Core.Common.Cache.MemoryCache
{
    public static partial class Extensions
    {
        /// <summary>
        /// 注册 MemoryCaching
        /// </summary>
        /// <param name="services">服务集合</param>
        public static void AddMemoryCache(this IServiceCollection services)
        {
            services.AddScoped<ICaching, MemoryCaching>();
        }
    }
}
