using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{

    /// <summary>
    /// 登录输入
    /// </summary>
    public class LoginUserInput
    {
        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string RegverifyCode { get; set; }
    }
}
