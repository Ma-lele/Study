using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Extensions;
using XHS.Build.Common.Response;

namespace xhs.build.app.Filters
{
    /// <summary>
    /// Admin异常错误过滤
    /// </summary>
    public class ExceptionFilter : IExceptionFilter, IAsyncExceptionFilter
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ExceptionFilter> _logger;

        /// <summary>
        /// 异常
        /// </summary>
        /// <param name="env"></param>
        /// <param name="logger"></param>
        public ExceptionFilter(IWebHostEnvironment env, ILogger<ExceptionFilter> logger)
        {
            _env = env;
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {
            string message;
            if (_env.IsProduction())
            {
                message = Enums.StatusCodes.Status500InternalServerError.ToDescription();
            }
            else
            {
                message = context.Exception.Message;
            }

            _logger.LogError(context.Exception, "");
            var data = ResponseOutput.NotOk(message);
            context.Result = new InternalServerErrorResult(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task OnExceptionAsync(ExceptionContext context)
        {
            OnException(context);
            return Task.CompletedTask;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class InternalServerErrorResult : ObjectResult
    {
        public InternalServerErrorResult(object value) : base(value)
        {
            StatusCode = Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError;
        }
    }
}
