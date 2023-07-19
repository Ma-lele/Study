using Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.TokenManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticateService _authService;
        public AuthenticationController(IAuthenticateService authService)
        {
            this._authService = authService;
        }

        [AllowAnonymous]
        [HttpPost, Route("requestToken")]
        public ActionResult RequestToken(LoginRequestDTO lrd)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Request");
            }

            var claims = new[]
             {
                        new Claim(ClaimTypes.Name,lrd.Username)
                    };
            var token = _authService.Create(claims);
            if (!string.IsNullOrEmpty(token))
            {

                var zz = _authService.Decode(token);
                return Ok(token);
            }
            else
            {
                return BadRequest("Invalid Request");
            };
        }
    }
}
