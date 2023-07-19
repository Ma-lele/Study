using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Log
{
    public interface ILogService : IBaseServices<CCOperationLog>
    {
        Task<List<LogCountDto>> GetTypeCount(Expression<Func<CCOperationLog, bool>> whereExpression);

        Task<PageOutput<CCOperationLog>> GetPageList(Expression<Func<CCOperationLog, bool>> whereExpression, 
            string sort, int page, int size);

        Task<PageOutput<GCFogLog>> GetFogLogPageList(Expression<Func<GCFogLog, bool>> whereExpression,
            string sort, int page, int size);
    }
}
