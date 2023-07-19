using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XHS.Build.Center.Logs
{
    public interface ILogHandler
    {
        Task LogAsync(ActionExecutingContext context, ActionExecutionDelegate next);
    }
}
