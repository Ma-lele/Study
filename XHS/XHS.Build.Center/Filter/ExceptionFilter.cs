using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Response;

namespace XHS.Build.Center.Filter
{
    public class ExceptionFilter : IExceptionFilter, IAsyncExceptionFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(IWebHostEnvironment env, ILogger<ExceptionFilter> logger)
        {
            _env = env;
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            string message;
            if (_env.IsProduction())
            {
                message = "服务器发生错误";
            }
            else
            {
                message = context.Exception.Message;
            }

            _logger.LogError(context.Exception, "");
            var data = ResponseOutput.NotOk(message);
            context.Result = new InternalServerErrorResult(data);
        }

        public Task OnExceptionAsync(ExceptionContext context)
        {
            OnException(context);
            return Task.CompletedTask;
        }
    }

    public class InternalServerErrorResult : ObjectResult
    {
        public InternalServerErrorResult(object value) : base(value)
        {
            StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError;
        }
    }
}
