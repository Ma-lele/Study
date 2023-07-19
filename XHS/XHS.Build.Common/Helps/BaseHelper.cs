using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Common.Configs;

namespace XHS.Build.Common.Helps
{
    /// <summary>
    /// 助手基类
    /// </summary>
    public class BaseHelper
    {
        public static string APPNAME = string.IsNullOrEmpty(Webconfig.AppName) ? Properties.Settings.Default.AppName : Webconfig.AppName;
    }
}
