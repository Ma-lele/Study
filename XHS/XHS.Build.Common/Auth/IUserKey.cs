using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Common.Auth
{
    public interface IUserKey
    {
        public string Key { get;  }

        //public string Secret { get; }

        public int GroupId { get; }
        public string UserId { get; }

        public string Name { get;}
    }
}
