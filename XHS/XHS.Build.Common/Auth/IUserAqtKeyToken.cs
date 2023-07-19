using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace XHS.Build.Common.Auth
{
    public interface IUserAqtKeyToken
    {
        string Create(Claim[] claims);

        Claim[] Decode(string jwtToken);
    }
}
