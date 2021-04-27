using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Util
{
    /// <summary>
    /// 系统扩展 - 验证
    /// </summary>
    public static partial class Extensions
    {
        #region CheckNull(检查对象是否为null)

        /// <summary>
        /// 检查对象是否为null，为null则抛出<see cref="ArgumentNullException"/>异常
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="parameterName">参数名</param>
        public static void CheckNull(this object obj, string parameterName)
        {
            if (obj == null)
                throw new ArgumentNullException(parameterName);
        }

        #endregion

        #region IsEmpty(是否为空)

        /// <summary>
        /// 判断 字符串 是否为空、null或空白字符串
        /// </summary>
        /// <param name="value">数据</param>
        public static bool IsEmpty(this string value) => string.IsNullOrWhiteSpace(value);

        /// <summary>
        /// 判断 Guid 是否为空、null或Guid.Empty
        /// </summary>
        /// <param name="value">数据</param>
        public static bool IsEmpty(this Guid value) => value == Guid.Empty;

        /// <summary>
        /// 判断 Guid 是否为空、null或Guid.Empty
        /// </summary>
        /// <param name="value">数据</param>
        public static bool IsEmpty(this Guid? value) => value == null || IsEmpty(value.Value);

        /// <summary>
        /// 判断 StringBuilder 是否为空
        /// </summary>
        /// <param name="sb">数据</param>
        public static bool IsEmpty(this StringBuilder sb) => sb == null || sb.Length == 0 || sb.ToString().IsEmpty();

        /// <summary>
        /// 判断 迭代集合 是否为空
        /// </summary>
        /// <typeparam name="T">泛型对象</typeparam>
        /// <param name="list">数据</param>
        public static bool IsEmpty<T>(this IEnumerable<T> list) => null == list || !list.Any();

        /// <summary>
        /// 判断 字典 是否为空
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="dictionary">数据</param>
        public static bool IsEmpty<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) => null == dictionary || dictionary.Count == 0;

        ///// <summary>
        ///// 判断 字典 是否为空
        ///// </summary>
        ///// <param name="dictionary">数据</param>
        //public static bool IsEmpty(this IDictionary dictionary) => null == dictionary || dictionary.Count == 0;

        #endregion

        #region IsNull(是否为空)

        /// <summary>
        /// 判断目标对象是否为空
        /// </summary>
        /// <param name="target">目标对象</param>
        public static bool IsNull(this object target) => target.IsNull<object>();

        /// <summary>
        /// 判断目标对象是否为空
        /// </summary>
        /// <typeparam name="T">目标对象类型</typeparam>
        /// <param name="target">目标对象</param>
        public static bool IsNull<T>(this T target) => ReferenceEquals(target, null);

        #endregion

        #region NotEmpty(是否非空)

        /// <summary>
        /// 判断 字符串 是否非空
        /// </summary>
        /// <param name="value">数据</param>
        public static bool NotEmpty(this string value) => !string.IsNullOrWhiteSpace(value);

        /// <summary>
        /// 判断 Guid 是否非空
        /// </summary>
        /// <param name="value">数据</param>
        public static bool NotEmpty(this Guid value) => value != Guid.Empty;

        /// <summary>
        /// 判断 Guid? 是否非空
        /// </summary>
        /// <param name="value">数据</param>
        public static bool NotEmpty(this Guid? value) => value != null && value != Guid.Empty;

        /// <summary>
        /// 判断 StringBuilder 是否为空
        /// </summary>
        /// <param name="sb">数据</param>
        public static bool NotEmpty(this StringBuilder sb) => sb != null && sb.Length != 0 && sb.ToString().NotEmpty();

        /// <summary>
        /// 判断 迭代集合 是否非空
        /// </summary>
        /// <typeparam name="T">泛型对象</typeparam>
        /// <param name="enumerable">数据</param>
        public static bool NotEmpty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                return false;
            if (enumerable.Any())
                return true;
            return false;
        }

        #endregion

        #region IsZeroOrMinus(是否为0或负数)

        /// <summary>
        /// 判断 short 是否为0或负数
        /// </summary>
        /// <param name="value">数据</param>
        public static bool IsZeroOrMinus(this short value) => value <= 0;

        /// <summary>
        /// 判断 int 是否为0或负数
        /// </summary>
        /// <param name="value">数据</param>
        public static bool IsZeroOrMinus(this int value) => value <= 0;

        /// <summary>
        /// 判断 long 是否为0或负数
        /// </summary>
        /// <param name="value">数据</param>
        public static bool IsZeroOrMinus(this long value) => value <= 0;

        /// <summary>
        /// 判断 float 是否为0或负数
        /// </summary>
        /// <param name="value">数据</param>
        public static bool IsZeroOrMinus(this float value) => value <= 0;

        /// <summary>
        /// 判断 double 是否为0或负数
        /// </summary>
        /// <param name="value">数据</param>
        public static bool IsZeroOrMinus(this double value) => value <= 0;

        /// <summary>
        /// 判断 decimal 是否为0或负数
        /// </summary>
        /// <param name="value">数据</param>
        public static bool IsZeroOrMinus(this decimal value) => value <= 0;

        #endregion

        #region IsPercentage(是否为百分数)

        /// <summary>
        /// 判断 float 是否为百分数
        /// </summary>
        /// <param name="value">数据</param>
        public static bool IsPercentage(this float value) => value > 0 && value <= 1;

        /// <summary>
        /// 判断 double 是否为百分数
        /// </summary>
        /// <param name="value">数据</param>
        public static bool IsPercentage(this double value) => value > 0 && value <= 1;

        /// <summary>
        /// 判断 decimal 是否为百分数
        /// </summary>
        /// <param name="value">数据</param>
        public static bool IsPercentage(this decimal value) => value > 0 && value <= 1;

        #endregion

        #region IsZeroOrPercentage(是否为0或百分数)

        /// <summary>
        /// 判断 float 是否为0或百分数
        /// </summary>
        /// <param name="value">数据</param>
        public static bool IsZeroOrPercentage(this float value) => value.IsPercentage() || value.Equals(0f);

        /// <summary>
        /// 判断 double 是否为0或百分数
        /// </summary>
        /// <param name="value">数据</param>
        public static bool IsZeroOrPercentage(this double value) => value.IsPercentage() || value.Equals(0d);

        /// <summary>
        /// 判断 decimal 是否为0或百分数
        /// </summary>
        /// <param name="value">数据</param>
        public static bool IsZeroOrPercentage(this decimal value) => value.IsPercentage() || value.Equals(0m);

        #endregion

        #region IsMatch(是否匹配正则表达式)

        /// <summary>
        /// 确定所指定的正则表达式在指定的输入字符串中是否找到了匹配项
        /// </summary>
        /// <param name="value">要搜索匹配项的字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式</param>
        /// <returns>如果正则表达式找到匹配项，则为 true；否则，为 false</returns>
        public static bool IsMatch(this string value, string pattern)
        {
            if (value == null)
            {
                return false;
            }

            return Regex.IsMatch(value, pattern);
        }

        /// <summary>
        /// 确定所指定的正则表达式在指定的输入字符串中找到匹配项
        /// </summary>
        /// <param name="value">要搜索匹配项的字符串</param>
        /// <param name="pattern">要匹配的正则表达式模式</param>
        /// <param name="options">规则</param>
        /// <returns>如果正则表达式找到匹配项，则为 true；否则，为 false</returns>
        public static bool IsMatch(this string value, string pattern, RegexOptions options)
        {
            if (value == null)
            {
                return false;
            }

            return Regex.IsMatch(value, pattern, options);
        }

        #endregion
    }
}
