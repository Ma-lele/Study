using Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.User
{
    public class UserService : IUserService
    {
        public int isfff()
        {
            return 0;
        }

        //模拟测试，默认都是人为验证有效
        public bool IsValid(LoginRequestDTO lrd)
        {
            return true;
        }
    }
}
