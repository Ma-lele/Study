using Common;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services.TokenManagement
{
  

    public interface IAuthenticateService
    {

        string Create(Claim[] claims);

        Claim[] Decode(string jwtToken);
    }
}
