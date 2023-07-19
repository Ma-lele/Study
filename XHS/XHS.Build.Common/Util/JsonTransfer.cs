using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace XHS.Build.Common.Util
{
    /// <summary>
    /// JSON生成器
    /// </summary>
    public class JsonTransfer
    {
        /// <summary>
        /// 空JSON字符串
        /// </summary>
        private const string STR_EMPTY = "{}";

        /// <summary>
        /// 将一个数据表集合转换成一个JSON字符串，在客户端可以直接转换成二维数组。
        /// </summary>
        /// <param name="ds">需要转换的表集合.</param>
        /// <returns></returns>
        public static string dataSet2Json(DataSet ds)
        {
            if (ds == null || ds.Tables.Count == 0)
                return STR_EMPTY;

            int len = ds.Tables.Count;
            string[] tableName = new string[len];
            for (int i = 0; i < len; i++)
            {
                tableName[i] = i.ToString();
            }

            return dataSet2Json(ds, tableName);
        }

        /// <summary>
        /// 将一个数据表集合转换成一个JSON字符串，在客户端可以直接转换成二维数组。
        /// </summary>
        /// <param name="ds">需要转换的表集合。</param>
        /// <param name="tableNames">按顺序的表名。</param>
        /// <returns></returns>
        public static string dataSet2Json(DataSet ds, string[] tableNames)
        {
            if (ds == null || ds.Tables.Count == 0)
                return STR_EMPTY;

            StringBuilder sb = new StringBuilder("{");
            for (int i = 0; i < tableNames.Length; i++)
            {
                sb.Append("\"" + tableNames[i] + "\":" + dataTable2Json(ds.Tables[i]) + ",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// 将多个JSON数据集合并转换成一个JSON字符串。
        /// </summary>
        /// <param name="jsonTables">需要合并的JSON数据集合。</param>
        /// <returns></returns>
        public static string JsonUnion(params string[] jsonTables)
        {
            if (jsonTables == null || jsonTables.Length == 0)
                return STR_EMPTY;

            StringBuilder sb = new StringBuilder("{");
            for (int i = 0; i < jsonTables.Length; i++)
            {
                if (string.IsNullOrEmpty(jsonTables[i]))
                    jsonTables[i] = STR_EMPTY;

                sb.Append("\"" + i + "\":" + jsonTables[i] + ",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// 将多个DataTable数据集合并转换成一个JSON字符串。
        /// </summary>
        /// <param name="dts">DataTable集合</param>
        /// <returns></returns>
        public static string JsonUnion(params DataTable[] dts)
        {
            if (dts == null || dts.Length == 0)
                return STR_EMPTY;

            string[] jsons = new string[dts.Length];
            for (int i = 0; i < dts.Length; i++)
            {
                jsons[i] = dataTable2Json(dts[i]);
            }
            return JsonUnion(jsons);
        }

        /// <summary>
        /// 将一个数据表转换成一个JSON字符串，在客户端可以直接转换成二维数组。
        /// </summary>
        /// <param name="dt">需要转换的表。</param>
        /// <returns></returns>
        public static string dataTable2Json(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return "[]";

            StringBuilder sb = new StringBuilder("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sb.Append("{ ");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    if (j < dt.Columns.Count - 1)
                    {
                        sb.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" + filter(dt.Rows[i][j].ToString()) + "\",");
                    }
                    else if (j == dt.Columns.Count - 1)
                    {
                        sb.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + "\"" + filter(dt.Rows[i][j].ToString()) + "\"");
                    }
                }
                if (i == dt.Rows.Count - 1)
                {
                    sb.Append("} ");
                }
                else
                {
                    sb.Append("}, ");
                }
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// 将一个数据表集合转换成一个JSON字符串，在客户端可以直接转换成二维数组。
        /// </summary>
        /// <param name="ds">需要转换的表集合。</param>
        /// <param name="tableNames">按顺序的表名。</param>
        /// <returns></returns>
        public static JArray dataSet2JArray(DataSet ds)
        {
            JArray result = new JArray();
            if (ds == null || ds.Tables.Count == 0)
                return result;
            for (int i = 0; i < ds.Tables.Count; i++)
            {
                result.Add(dataTable2JArray(ds.Tables[i]));
            }
            return result;
        }


        /// <summary>
        /// 将一个数据表转换成一个JArray对象
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <returns></returns>
        public static JArray dataTable2JArray(DataTable dt)
        {
            JArray ja = new JArray();
            if (dt == null || dt.Rows.Count == 0)
                return ja;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                JObject jo = new JObject();
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jo.Add(dt.Columns[j].ColumnName.ToString(), Convert.ToString(dt.Rows[i][j]));
                }
                ja.Add(jo);
            }
            return ja;
        }

        /// <summary>
        /// 将DataRow转JObject对象
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static JObject dataRow2JObject(DataRow row)
        {
            JObject result = new JObject();

            if (row != null)
            {
                foreach (DataColumn col in row.Table.Columns)
                {
                    string colName = col.ColumnName;
                    if (!col.DataType.IsClass && !col.DataType.IsInterface && col.DataType.GetInterfaces().Any(q => q == typeof(IFormattable)) && col.DataType.Name != "DateTime")
                    {
                        result.Add(colName, double.Parse(row[colName].ToString()));
                    }
                    else
                    {
                        result.Add(colName, filter(Convert.ToString(row[colName])));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 将数据字典转表单
        /// </summary>
        /// <param name="dic">数据字典</param>
        /// <returns></returns>
        public static string dic2Form(Dictionary<string, object> dic)
        {
            if (dic == null || dic.Count <= 0)
                return null;

            StringBuilder sb = new StringBuilder(string.Empty);
            foreach (string key in dic.Keys)
            {
                sb.AppendFormat("{0}={1}&", key, dic[key]);
            }
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

        /// <summary>
        /// 将一个数据表行Row转换成一个JSON字符串。
        /// </summary>
        /// <param name="dt">需要转换的表行Row。</param>
        /// <returns></returns>
        public static string dataRow2Json(DataRow row)
        {
            if (row == null)
                return STR_EMPTY;

            StringBuilder sb = new StringBuilder("{");


            foreach (DataColumn col in row.Table.Columns)
            {
                string colName = col.ColumnName;
                if (!col.DataType.IsClass && !col.DataType.IsInterface && col.DataType.GetInterfaces().Any(q => q == typeof(IFormattable)))
                {
                    sb.Append("\"" + colName + "\":" + filter(row[colName].ToString()) + ",");
                }
                else
                {
                    sb.Append("\"" + colName + "\":" + "\"" + filter(row[colName].ToString()) + "\",");
                }
            }

            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// 将Master转JSON数据
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string master2Json(DataTable dt)
        {
            if (dt == null || dt.Rows.Count == 0)
                return STR_EMPTY;

            StringBuilder sb = new StringBuilder("{");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sb.Append("\"" + filter(dt.Rows[i][0].ToString()) + "\":");
                sb.Append("\"" + filter(dt.Rows[i][1].ToString()) + "\",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// 指定键值对应字段名将Master转JSON数据
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="keyCol">键字段名</param>
        /// <param name="valueCol">值字段名</param>
        /// <returns></returns>
        public static string master2Json(DataTable dt, string keyCol, string valueCol)
        {
            if (dt == null || dt.Rows.Count == 0)
                return STR_EMPTY;

            StringBuilder sb = new StringBuilder("{");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sb.Append("\"" + filter(dt.Rows[i][keyCol].ToString()) + "\":");
                sb.Append("\"" + filter(dt.Rows[i][valueCol].ToString()) + "\",");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// 将我的菜单转成JSON数组
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public static string menu2Json(Dictionary<string, Dictionary<string, string>> menu)
        {
            if (menu == null || menu.Count == 0)
                return STR_EMPTY;

            StringBuilder sb = new StringBuilder("{");
            foreach (string menuid in menu.Keys)
            {
                sb.Append("\"" + menuid + "\":{");
                int count = menu[menuid].Keys.Count;
                int i = 0;
                foreach (string key in menu[menuid].Keys)
                {
                    if (i < count - 1)
                    {
                        sb.Append("\"" + key + "\":" + "\'" + menu[menuid][key].ToString() + "\',");
                    }
                    else if (i >= count - 1)
                    {
                        sb.Append("\"" + key + "\":" + "\'" + menu[menuid][key].ToString() + "\'}");
                    }
                    i++;
                }
                sb.Append(",\n");
            }

            sb.Remove(sb.Length - 2, 2);
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// json转换成键值对
        /// </summary>
        /// <param name="json">json</param>
        /// <returns>结果集</returns>
        public static NameValueCollection strToNvc(String json)
        {
            json = Uri.UnescapeDataString(json);
            NameValueCollection formdataList = new NameValueCollection();
            string[] aryStrings = json.Split(new char[] { '&' });
            string[] nameAndValue;
            foreach (string s in aryStrings)
            {
                nameAndValue = s.Split(new char[] { '=' });
                formdataList.Add(nameAndValue[0], nameAndValue[1]);
            }

            return formdataList;
        }

        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="value">值</param>
        /// <returns></returns>
        private static string filter(string value)
        {
            value = HttpUtility.HtmlDecode(value);//decode
            value = value.Replace("\"", "“");//过滤双引号
            value = value.Replace("\\", "/");//过滤斜杠
            value = value.Replace("/r/n", @"\r\n");//还原换行符

            return value;
        }

        /// <summary>
        /// 将DataTable转JS数组
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="menu">菜单列表</param>
        /// <returns></returns>
        public static string dataTable2JsArray(DataTable dt, Dictionary<string, Dictionary<string, string>> menu)
        {
            if (dt == null || dt.Rows.Count == 0)
                return "[]";

            StringBuilder sb = new StringBuilder("[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string menuid = dt.Rows[i][0].ToString();
                //有权限,且是菜单的才算数
                if (menu.ContainsKey(menuid) && Convert.ToBoolean(menu[menuid]["bmenu"]))
                    sb.AppendFormat("\"{0}\",", dt.Rows[i][0].ToString());
            }
            if (sb.Length > 1)
                sb.Remove(sb.Length - 1, 1);
            sb.Append("]");
            return sb.ToString();
        }
    }
}
