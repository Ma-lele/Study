using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model
{
    public class PasswordInput
    {
        public string pwd { get; set; }
        public string pwdNew { get; set; }

        /// <summary>
        /// 登录类别
        /// </summary>
        public string LoginType { get; set; } = "site";

        /// <summary>
        /// 租户ID
        /// </summary>
        public string teid { get; set; } = "0";
    }
}
