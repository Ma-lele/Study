using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Security
{
    public interface ISecurityHisService : IBaseServices<GCSecureHisEntity>
    {


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<PageOutput<GCSecureHisEntity>> GetHisPageList(int siteid, int securityid, DateTime date, int page, int size);

        Task<dynamic> GetMonthCount(int siteid, DateTime date);

        Task<Dictionary<string, List<SecureHisListOutput>>> GetHisALlList(int siteid, int securityid, DateTime date);
    }
}
