using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 登录请求
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// 账号
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空！")]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 登录类别
        /// </summary>
        public string LoginType { get; set; } = "site";
    }
}
