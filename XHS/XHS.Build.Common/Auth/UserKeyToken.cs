using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SqlSugar;
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
    public class UserKeyToken : IUserKeyToken
    {
        private readonly IConfiguration _configuration;

        public UserKeyToken(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Create(Claim[] claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JWTConfig:securityKey").Value));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var refreshExpires = DateTime.Now.AddMinutes(_configuration.GetSection("JWTConfig:expires").Value.ObjToInt()).ToString();
            claims = claims.Append(new Claim(KeyClaimAttributes.RefreshExpires, refreshExpires)).ToArray();

            var token = new JwtSecurityToken(
                issuer: _configuration.GetSection("JWTConfig:issuer").Value,
                audience: _configuration.GetSection("JWTConfig:audience").Value,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(_configuration.GetSection("JWTConfig:expires").Value.ObjToInt()),
                signingCredentials: signingCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public Claim[] Decode(string jwtToken)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = jwtSecurityTokenHandler.ReadJwtToken(jwtToken);
            return jwtSecurityToken?.Claims?.ToArray();
        }
    }
}
