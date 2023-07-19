using Common;
using Common.Configs;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Services.TokenManagement;
using Services.User;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace JWT.Auth
{
    public class AuthenticateService : IAuthenticateService
    {
        private readonly IUserService _userService;
        private readonly TokenManagement _tokenManagement;
        public AuthenticateService(IUserService userService, IOptions<TokenManagement> tokenManagement)
        {
            _userService = userService;
            _tokenManagement = tokenManagement.Value;
        }


        //public bool IsAuthenticated(LoginRequestDTO lrd, out string token)
        //{
        //    token = string.Empty;
        //    if (!_userService.IsValid(lrd))
        //        return false;
        //    var claims = new[]
        //    {
        //            new Claim(ClaimTypes.Name,lrd.Username)
        //        };
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenManagement.Secret));
        //    var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //    var jwtToken = new JwtSecurityToken(_tokenManagement.Issuer, _tokenManagement.Audience, claims,
        //        expires: DateTime.Now.AddMinutes(_tokenManagement.AccessExpiration),
        //        signingCredentials: credentials);
        //    token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
        //    return true;
        //}
        public string Create(Claim[] claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenManagement.Secret));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var refreshExpires = DateTime.Now.AddMinutes(_tokenManagement.RefreshExpiration).ToString();
            claims = claims.Append(new Claim(_tokenManagement.RefreshExpiration.ToString(), refreshExpires)).ToArray();

            var token = new JwtSecurityToken(
                issuer: _tokenManagement.Issuer,
                audience: _tokenManagement.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddMinutes(_tokenManagement.AccessExpiration),
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
