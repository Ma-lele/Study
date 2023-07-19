using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Services.Log;
using XHS.Build.Common.Helps;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 日志
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class LogController  : ControllerBase
    {
        private readonly ILogService _logService;

        /// <summary>
        /// ctor
        /// </summary>
        public LogController(ILogService logService)
        {
            _logService = logService;
        }


        /// <summary>
        /// 获取操作日志类型Count
        /// </summary>
        /// <param name="startDate"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetLogTypeCount(DateTime startDate)
        {
            DateTime endDate = startDate.AddDays(7); //多加一天
            Expression<Func<CCOperationLog, bool>> whereExpression = (ii) => true;
            whereExpression = whereExpression.And(ii => ii.operatedate >= startDate && ii.operatedate < endDate);

            var data = await _logService.GetTypeCount(whereExpression);
      
            List<LogCountDto> allList = new List<LogCountDto> {
                new LogCountDto{
                    type = TypeName.登录,
                    Count = 0
                },
                new LogCountDto{
                    type = TypeName.添加,
                    Count = 0
                },
                new LogCountDto{ 
                    type = TypeName.修改,
                    Count = 0
                },
                new LogCountDto{
                    type = TypeName.删除,
                    Count = 0
                },
                new LogCountDto{
                    type = TypeName.批处理,
                    Count = 0
                },
                new LogCountDto{
                    type = TypeName.移动端,
                    Count = 0
                },
                new LogCountDto{
                    type = TypeName.系统异常,
                    Count = 0
                },
                new LogCountDto{
                    type = TypeName.数据接收服务端,
                    Count = 0
                },
                new LogCountDto{
                    type = TypeName.其他,
                    Count = 0
                }
            };
            data.ForEach(ii =>
            {
                if (allList.Find(jj => jj.type == ii.type) != null)
                {
                    allList.Find(jj => jj.type == ii.type).Count += ii.Count;
                } 
               
            }); 
             
            return ResponseOutput.Ok(allList);
        }


        /// <summary>
        /// 获取操作日志列表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="startDate"></param>
        /// <param name="orderField"></param>
        /// <param name="sort"></param>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetOpLogPageList(TypeName type, DateTime startDate, string orderField, string sort
            , string keyword = "", int page = 1, int size = 20)
        {
            if (startDate == null || !Enum.IsDefined(typeof(TypeName), type))
            {
                return ResponseOutput.NotOk("请求参数错误");
            }
            DateTime endDate = startDate.AddDays(7); //多加一天
            Expression<Func<CCOperationLog, bool>> whereExpression = ii => ii.operatedate >= startDate && ii.operatedate < endDate;
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                whereExpression = whereExpression.And(ii => ii.url.Contains(keyword) || ii.@params.Contains(keyword)
                || ii.ip.Contains(keyword) || ii.operation.Contains(keyword));
            }
            if (type != TypeName.全部)
            {
                whereExpression = whereExpression.And(ii => ii.type == type);
            }

            string orderby = " operatedate desc";
            if (!string.IsNullOrWhiteSpace(orderField) && !string.IsNullOrWhiteSpace(sort))
            {
                orderby = $"{orderField} {sort}";
            }


            var data = await _logService.GetPageList(whereExpression, orderby, page, size);


            return ResponseOutput.Ok(data);
        }


        /// <summary>
        /// 获取雾炮日志列表
        /// </summary>
        /// <param name="direct"></param>
        /// <param name="cmd"></param>
        /// <param name="startDate"></param>
        /// <param name="orderField"></param>
        /// <param name="sort"></param>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetFogLogPageList(string direct,string cmd, DateTime startDate, string orderField, string sort
            , string keyword = "", int page = 1, int size = 20)
        {

            DateTime endDate = startDate.AddDays(7); //多加一天
            Expression<Func<GCFogLog, bool>> whereExpression = ii => ii.operatedate >= startDate && ii.operatedate < endDate;
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                whereExpression = whereExpression.And(ii => ii.fogcode.Contains(keyword) || ii.msg.Contains(keyword)
                || ii.cmd.Contains(keyword) || ii.command.Contains(keyword) || ii.@operator.Contains(keyword));
            }
            if (!string.IsNullOrWhiteSpace(direct))
            {
                whereExpression = whereExpression.And(ii => ii.direct == direct);
            }
            if (!string.IsNullOrWhiteSpace(cmd))
            {
                whereExpression = whereExpression.And(ii => ii.cmd == cmd);
            }

            string orderby = " operatedate desc";
            if (!string.IsNullOrWhiteSpace(orderField) && !string.IsNullOrWhiteSpace(sort))
            {
                orderby = $"{orderField} {sort}";
            }


            var data = await _logService.GetFogLogPageList(whereExpression, orderby, page, size);


            return ResponseOutput.Ok(data);
        }
    }
}
