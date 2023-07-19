using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class LoginUserOutput
    {
        public string Id { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 姓名、昵称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        public string Gender { get; set; }

        public int GroupId { get; set; }

        public List<string> RoleIds { get; set; }

        public string userregion { get; set; }

        //2.0工地端默认监测点信息
        public short usersitetype { get; set; }
        public int siteid { get; set; }
        public string sitename { get; set; }
    }
}
