using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace XHS.Build.Net.Auth
{
    public class ApiResponseHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public ApiResponseHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock
        ) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            throw new NotImplementedException();
        }

        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized;
            await Response.WriteAsync(JsonConvert.SerializeObject(
                new ResponseStatusData
                {
                    Code = StatusCodes.Status401Unauthorized,
                    Msg = "未登录"
                },
                new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            ));
        }

        protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            Response.ContentType = "application/json";
            Response.StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status403Forbidden;
            await Response.WriteAsync(JsonConvert.SerializeObject(
                new ResponseStatusData
                {
                    Code = StatusCodes.Status403Forbidden,
                    Msg = "权限不足"
                },
                new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                }
            ));
        }
    }

    public class ResponseStatusData
    {
        public int Code { get; set; }
        public string Msg { get; set; }
    }
}
