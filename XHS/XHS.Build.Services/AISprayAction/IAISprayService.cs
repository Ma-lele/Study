using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Models;
using XHS.Build.Services.AISprayAction.Dtos;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.AISprayAction
{
    public interface IAISprayService : IBaseServices<AISprayActionEntity>
    {
        /// <summary>
        /// 存储过程 新增围挡喷淋
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> InsertSparyPROC(AISparyInputDto input);

        /// <summary>
        /// 取得数据
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="date">日期</param>
        /// <returns>结果集</returns>
        Task<DataTable> GetSprayList(int SITEID, DateTime date);

        Task<DataTable> GetAiSprayDataCountAsync(int type,int siteid);

        Task<DataTable> GetAiSprayDuringAnalysisAsync(int type, int siteid);
    }
}
