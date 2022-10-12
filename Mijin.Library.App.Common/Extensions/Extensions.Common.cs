using Bing.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IsUtil {
    /// <summary>
    /// 系统扩展 - 公共
    /// </summary>
    public static partial class Extensions {
        /// <summary>
        /// 安全获取值，当值为null时，不会抛出异常
        /// </summary>
        /// <param name="value">可空值</param>
        public static T SafeValue<T>( this T? value ) where T : struct {
            return value ?? default( T );
        }

        /// <summary>
        /// 获取枚举值
        /// </summary>
        /// <param name="instance">枚举实例</param>
        public static int Value( this System.Enum instance ) {
            if( instance == null )
                return 0;
            return IsUtil.Helpers.Enum.GetValue( instance.GetType(), instance );
        }

        /// <summary>
        /// 获取枚举值
        /// </summary>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="instance">枚举实例</param>
        public static TResult Value<TResult>( this System.Enum instance ) {
            if( instance == null )
                return default( TResult );
            return IsUtil.Helpers.Convert.To<TResult>( Value( instance ) );
        }

        /// <summary>
        /// 获取枚举描述,使用System.ComponentModel.Description特性设置描述
        /// </summary>
        /// <param name="instance">枚举实例</param>
        public static string Description( this System.Enum instance ) {
            if ( instance == null )
                return string.Empty;
            return IsUtil.Helpers.Enum.GetDescription( instance.GetType(), instance );
        }

        /// <summary>
        /// 转换为用分隔符连接的字符串
        /// </summary>
        /// <typeparam name="T">集合元素类型</typeparam>
        /// <param name="list">集合</param>
        /// <param name="quotes">引号，默认不带引号，范例：单引号 "'"</param>
        /// <param name="separator">分隔符，默认使用逗号分隔</param>
        public static string Join<T>( this IEnumerable<T> list, string quotes = "", string separator = "," ) {
            return IsUtil.Helpers.String.Join( list, quotes, separator );
        }

        /// <summary>
        /// 给现有对象属性赋值，覆盖相同 属性名和属性类型 的字段
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="resource"></param>
        /// <param name="override">覆盖字段</param>
        public static void SetPropValue(this object obj, object resource, bool @override = false)
        {
            PropertyInfo[] objProps = obj.GetType().GetProperties();
            foreach (PropertyInfo reProp in resource.GetType().GetProperties())
            {
                if (!@override)
                {
                    Type type = reProp.GetType();
                    object defaultValue = type.IsValueType ? Activator.CreateInstance(type) : null;
                    if (reProp.GetValue(obj) != defaultValue)
                        continue;
                }
                PropertyInfo setProp = objProps.FirstOrDefault(o => o.Name == reProp.Name && o.PropertyType == reProp.PropertyType);
                if (!setProp.IsNull())
                {
                    setProp.SetValue(obj, reProp.GetValue(resource));
                }
            }
        }
    }
}
