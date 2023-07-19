using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Log
{
    public class LogService : BaseServices<CCOperationLog>, ILogService
    {
        private readonly IBaseRepository<CCOperationLog> _repository;
        public LogService(IBaseRepository<CCOperationLog> repository)
        {
            _repository = repository;
            BaseDal = repository;
        }

        public async Task<PageOutput<GCFogLog>> GetFogLogPageList(Expression<Func<GCFogLog, bool>> whereExpression, string sort, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _repository.Db.Queryable<GCFogLog>()
                .WhereIF(whereExpression != null, whereExpression)
                .OrderByIF(!string.IsNullOrWhiteSpace(sort), sort)
                .ToPageListAsync(page, size, totalCount);

            var data = new PageOutput<GCFogLog>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }

        public async Task<PageOutput<CCOperationLog>> GetPageList(Expression<Func<CCOperationLog, bool>> whereExpression,
            string sort,int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _repository.Db.Queryable<CCOperationLog>()
                .WhereIF(whereExpression != null, whereExpression)
                .OrderByIF(!string.IsNullOrWhiteSpace(sort),sort)
                .ToPageListAsync(page, size, totalCount);

            var data = new PageOutput<CCOperationLog>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }

        public async Task<List<LogCountDto>> GetTypeCount(Expression<Func<CCOperationLog, bool>> whereExpression)
        {
            return await _repository.Db.Queryable<CCOperationLog>()
                .WhereIF(whereExpression != null, whereExpression)
                .GroupBy(ii => ii.type)
                .Select(ii => new LogCountDto
                {
                    type = ii.type,
                    Count = SqlFunc.AggregateCount(ii)
                }).ToListAsync();
        }
    }
}
