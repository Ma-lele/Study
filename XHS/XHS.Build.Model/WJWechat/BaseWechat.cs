using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.WJWechat
{
    public class BaseWechat
    {
        /// <summary>
        /// 微信凭证
        /// </summary>
        public string AccessToken { get; set; }
        protected static string AppName = "";
    }
}
