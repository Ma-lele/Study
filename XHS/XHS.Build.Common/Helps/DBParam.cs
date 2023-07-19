using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace XHS.Build.Common.Helps
{
    /// <summary>
    /// 数据库参数基类
    /// </summary>
    public class DBParam : Dictionary<string, object>
    {
        const string AT = "@";
        const string SLASH = @"\";
        const string NEW_LINE = @"\r\n";
        const string NULL = @"NULL";

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">参数值</param>
        public new void Add(string key, object value)
        {
            string val = null;
            val = htmlFilter(value);

            base.Add(AT + key, val);
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">参数值</param>
        /// <param name="value">默认值</param>
        public void Add(string key, object value, object defaultValue)
        {
            string val = null;
            val = htmlFilter(value);

            base.Add(AT + key, val);
            this.SetDefault(key, defaultValue);
        }

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="key">键</param>
        public object Get(string key)
        {
            return this[AT + key];
        }

        /// <summary>
        /// 重置参数
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public void Set(string key, object value)
        {
            string val = null;
            val = htmlFilter(value);

            this[AT + key] = htmlFilter(val);
        }

        /// <summary>
        /// 重置参数
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">参数值</param>
        /// <param name="defaultValue">默认值</param>
        public void Set(string key, object value, object defaultValue)
        {
            this.Set(key, value);
            this.SetDefault(key, defaultValue);
        }

        /// <summary>
        /// 设置参数的默认值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">默认值</param>
        public void SetDefault(string key, object value)
        {
            if (!this.ContainsKey(key))
            {
                this.Add(key, value);
                return;
            }

            if (this.Get(key) == null || string.IsNullOrEmpty(this.Get(key).ToString()))
            {
                this.Set(key, value);
            }
        }

        /// <summary>
        /// 设置参数的默认值
        /// </summary>
        /// <param name="keys">键数组</param>
        /// <param name="value">默认值</param>
        public void SetDefault(string[] keys, object value)
        {
            foreach (string key in keys)
            {
                this.SetDefault(key, value);
            }
        }

        /// <summary>
        /// 将键列表中值为空的设为null
        /// </summary>
        /// <param name="keys">键数组</param>
        public void SetEmptyToNull(string[] keys)
        {
            foreach (string key in keys)
            {
                this.SetDefault(key, null);
            }
        }

        /// <summary>
        /// 是否包含键
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public new bool ContainsKey(string key)
        {
            return base.ContainsKey(AT + key);
        }

        /// <summary>
        /// 将表单转为数据库参数
        /// </summary>
        /// <param name="form">表单</param>
        /// <returns></returns>
        public void form2Param(NameValueCollection form)
        {
            foreach (string key in form)
            {
                this.Add(key, form[key]);
            }
        }

        /// <summary>
        /// 过滤HTML字符串
        /// </summary>
        /// <param name="strHtml">html字符串</param>
        /// <returns></returns>
        private string htmlFilter(object strHtml)
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
        /// 删除参数
        /// </summary>
        /// <param name="key">键</param>
        public new void Remove(string key)
        {
            if (ContainsKey(key))
            {
                base.Remove(AT + key);
            }

        }

        /// <summary>
        /// json转换成DB参数
        /// </summary>
        /// <param name="json">json</param>
        /// <returns>结果集</returns>
        public static List<DBParam> strToParam(String json)
        {
            List<Dictionary<string, string>> sdList = new List<Dictionary<string, string>>();
            List<DBParam> dBParamList = new List<DBParam>();
            DBParam dBParam = new DBParam();

            sdList = json2Dic(json);
            foreach (Dictionary<string, string> sd in sdList)
            {
                dBParam = new DBParam();
                foreach (string key in sd.Keys)
                {
                    dBParam.Add(key, sd[key]);
                }
                dBParamList.Add(dBParam);
            }

            return dBParamList;
        }

        /// <summary>
        /// json转换成字典列表（2维）
        /// </summary>
        /// <param name="jsonText">json</param>
        /// <returns>结果集</returns>
        public static List<Dictionary<string, string>> json2Dic(string jsonText)
        {
            List<Dictionary<string, string>> sdList = new List<Dictionary<string, string>>();

            Dictionary<string, string> sd = new Dictionary<string, string>();
            JArray mJObj = JArray.Parse(jsonText);

            foreach (JObject jo in mJObj)
            {
                sd = new Dictionary<string, string>();
                sd = jo.ToObject<Dictionary<string, string>>();
                sdList.Add(sd);
            }

            return sdList;
        }
    }
}
