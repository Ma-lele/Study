using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Services.OperateLogS;

namespace XHS.Build.Analyst.Web.Logs
{
    /// <summary>
    /// 操作日志处理
    /// </summary>
    public class LogHandler : ILogHandler
    {
        private readonly IOperateLogService _operateLogService;

        public LogHandler(IOperateLogService operateLogService)
        {
            _operateLogService = operateLogService;
        }

        public async Task LogAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var sw = new Stopwatch();
            sw.Start();
            dynamic actionResult = (await next()).Result;
            sw.Stop();

            //请求api响应内容
            var res = actionResult == null ? null : actionResult?.Value as IResponseOutput;
            string bodyStr = string.Empty;
            using (var reader = new StreamReader(context.HttpContext.Request.Body, System.Text.Encoding.UTF8))
            {
                reader.BaseStream.Seek(0, SeekOrigin.Begin);  //大概是== Request.Body.Position = 0;的意思
                bodyStr = await reader.ReadToEndAsync();

            }
            var LogEntity = new MongoOperateLog
            {
                //Id=Guid.NewGuid().ToString(),
                ApiMethod = context.HttpContext.Request.Method.ToLower(),
                ApiPath = context.ActionDescriptor.AttributeRouteInfo.Template.ToLower(),
                ElapsedMilliseconds = sw.ElapsedMilliseconds,
                NoteMsg = res?.msg,
                Status = res?.success,
                UrlQueryString = context.HttpContext.Request.QueryString.Value,
                Body = bodyStr,
                CreateTime = DateTime.Now
            };
            await _operateLogService.AddAsync(LogEntity);
        }
    }
}
