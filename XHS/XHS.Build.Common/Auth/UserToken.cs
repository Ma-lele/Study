using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using XHS.Build.Common.Attributes;
using XHS.Build.Common.Configs;

namespace XHS.Build.Common.Auth
{
    [SingleInstance]
    public class UserToken : IUserToken
    {
        private readonly JwtConfig _jwtConfig;
        private readonly ILogger<UserToken> _logger;

        public UserToken(JwtConfig jwtConfig, ILogger<UserToken> logger)
        {
            _jwtConfig = jwtConfig;
            _logger = logger;
        }

        public string Create(Claim[] claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.SecurityKey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var refreshExpires = DateTime.Now.AddMinutes(_jwtConfig.RefreshExpires).ToString();
            claims = claims.Append(new Claim(ClaimAttributes.RefreshExpires, refreshExpires)).ToArray();

            var token = new JwtSecurityToken(
                issuer: _jwtConfig.Issuer,
                audience: _jwtConfig.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(_jwtConfig.Expires),
                signingCredentials: signingCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Claim[] Decode(string jwtToken)
        {
            //忽略保活程序的调用
            if (string.IsNullOrEmpty(jwtToken) || "quartz".Equals(jwtToken, StringComparison.OrdinalIgnoreCase))
                return null;

            try
            {
                var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
                var jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(jwtToken);
                return jwtSecurityToken?.Claims?.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message + Environment.NewLine + jwtToken);
            }
            return null;
        }
    }
}
