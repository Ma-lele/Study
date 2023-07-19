using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace XHS.Build.Common.Helps
{
    public class HtmlHelper
    {
        const string AT = "@";
        const string SLASH = @"\";
        const string NEW_LINE = @"\r\n";
        const string NULL = @"NULL";

        /// <summary>
        /// 过滤HTML字符串
        /// </summary>
        /// <param name="strHtml">html字符串</param>
        /// <returns></returns>
        public static string htmlFilter(object strHtml)
        {
            string result = null;

            if (strHtml == null || strHtml.ToString().ToUpper() == NULL)
                return result;
            else
            {
                result = strHtml.ToString();
                //string[] aryReg = { @"'", "\"", @"\", @"?", @"<", @">", @"%", @">=", @"=<", @"_", @";", @"||", @"[", @"]", @"&", @"/", @"-", @"|" };
                string[] aryReg = { @";", @"=" };
                for (int i = 0; i < aryReg.Length; i++)
                {
                    result = result.Replace(aryReg[i], UFilter.ToSBC(aryReg[i]));
                }
                result = HttpUtility.HtmlEncode(result);
            }

            //替换换行符
            result = result.Replace(Environment.NewLine, NEW_LINE);
            result = result.Replace("\n", NEW_LINE);
            //替换Tab符号
            result = result.Replace("\t", string.Empty);

            return result;
        }

        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static string filter(string value)
        {
            value = HttpUtility.HtmlDecode(value);//decode
            value = value.Replace("\"", "“");//过滤双引号
            value = value.Replace("\\", "/");//过滤斜杠
            value = value.Replace("/r/n", @"\r\n");//还原换行符

            return value;
        }
    }
}
