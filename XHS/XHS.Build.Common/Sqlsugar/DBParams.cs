using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Common.Helps;

namespace XHS.Build.Common.Sqlsugar
{
    public class DBParams : SugarParameter
    {
        public DBParams(string name, object value) : base(name, value)
        {
            if (value != null)
            {
                base.Value = HtmlHelper.htmlFilter(value);
            }
            base.ParameterName = name;
        }
    }
}
