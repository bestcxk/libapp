using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Util.Logs;
using Util.Logs.Extensions;
namespace IsUtil
{
    public static partial class Extensions
    {
        /// <summary>
        /// 设置 调用的类名 和 方法名
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        public static ILog SetClassAndMethod(this ILog log)
        {
            MethodBase method = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod();
            string className = method.ReflectedType.FullName;
            string methodName = method.Name;
            //return log.Caption(@$"Class: [{className}] method: [{methodName}]");
            return log.Class(className).Method(methodName);
        }
    }
}
