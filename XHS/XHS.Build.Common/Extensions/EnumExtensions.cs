using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace XHS.Build.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDescription(this Enum item)
        {
            string name = item.ToString();
            var desc = item.GetType().GetField(name)?.GetCustomAttribute<DescriptionAttribute>();
            return desc?.Description ?? name;
        }
    }
}
