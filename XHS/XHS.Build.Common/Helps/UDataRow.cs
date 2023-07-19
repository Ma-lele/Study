using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace XHS.Build.Common.Helps
{
    /// <summary>
    /// DataRow的工具辅助类
    /// </summary>
    public class UDataRow
    {
        /// <summary>
        /// 整形默认值
        /// </summary>
        private const int INT_DEFAULT = 0;
        /// <summary>
        /// decimal默认值
        /// </summary>
        private const int DECIMAL_DEFAULT = 0;
        /// <summary>
        /// DateTime型默认值
        /// </summary>
        private static DateTime DATETIME_DEFAULT = DateTime.Parse("1999-01-01");

        /// <summary>
        /// 转int
        /// </summary>
        /// <param name="dr">DataRow</param>
        /// <param name="colname">字段名</param>
        /// <returns></returns>
        public static int ToInt(DataRow dr, string colname)
        {
            return ToInt(dr, colname, INT_DEFAULT);
        }

        /// <summary>
        /// 转int
        /// </summary>
        /// <param name="dr">DataRow</param>
        /// <param name="colname">字段名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static int ToInt(DataRow dr, string colname, int defaultValue)
        {
            if (!dr.Table.Columns.Contains(colname) || dr.IsNull(colname))
                return defaultValue;
            else
                return Convert.ToInt32(dr[colname]);
        }

        /// <summary>
        /// 转decimal
        /// </summary>
        /// <param name="dr">DataRow</param>
        /// <param name="colname">字段名</param>
        /// <returns></returns>
        public static decimal ToDecimal(DataRow dr, string colname)
        {
            return ToDecimal(dr, colname, DECIMAL_DEFAULT);
        }

        /// <summary>
        /// 转decimal
        /// </summary>
        /// <param name="dr">DataRow</param>
        /// <param name="colname">字段名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static decimal ToDecimal(DataRow dr, string colname, decimal defaultValue)
        {
            if (!dr.Table.Columns.Contains(colname) || dr.IsNull(colname))
                return defaultValue;
            else
                return Convert.ToDecimal(dr[colname]);
        }

        /// <summary>
        /// 转DateTime
        /// </summary>
        /// <param name="dr">DataRow</param>
        /// <param name="colname">字段名</param>
        /// <returns></returns>
        public static DateTime ToDateTime(DataRow dr, string colname)
        {
            return ToDateTime(dr, colname, DATETIME_DEFAULT);
        }

        /// <summary>
        /// 转DateTime
        /// </summary>
        /// <param name="dr">DataRow</param>
        /// <param name="colname">字段名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static DateTime ToDateTime(DataRow dr, string colname, DateTime defaultValue)
        {
            if (!dr.Table.Columns.Contains(colname) || dr.IsNull(colname))
                return defaultValue;
            else
                return Convert.ToDateTime(dr[colname]);
        }

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <param name="dr">DataRow</param>
        /// <param name="colname">字段名</param>
        /// <returns></returns>
        public static string ToString(DataRow dr, string colname)
        {
            return ToString(dr, colname, string.Empty);
        }

        /// <summary>
        /// 转字符串
        /// </summary>
        /// <param name="dr">DataRow</param>
        /// <param name="colname">字段名</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string ToString(DataRow dr, string colname, string defaultValue)
        {
            if (!dr.Table.Columns.Contains(colname) || dr.IsNull(colname))
                return defaultValue;
            else
                return Convert.ToString(dr[colname]);
        }
    }
}
