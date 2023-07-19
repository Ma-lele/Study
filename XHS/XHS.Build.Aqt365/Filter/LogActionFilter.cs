using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Aqt365.Logs;
using XHS.Build.Common.Attributes;

namespace XHS.Build.Aqt365.Filter
{
    public class LogActionFilter : IAsyncActionFilter
    {
        private readonly ILogHandler _logHandler;

        public LogActionFilter(ILogHandler logHandler)
        {
            _logHandler = logHandler;
        }

        public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ActionDescriptor.EndpointMetadata.Any(m => m.GetType() == typeof(NoOprationLogAttribute)))
            {
                return next();
            }
            return _logHandler.LogAsync(context, next);
        }
    }
}
