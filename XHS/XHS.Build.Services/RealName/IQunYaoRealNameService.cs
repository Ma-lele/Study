using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.RealName
{
    public interface IQunYaoRealNameService : IBaseServices<BaseEntity>
    {
        /// <summary>
        /// 工地当日人员实时考勤信息
        /// </summary>
        /// <returns>结果集</returns>
        Task<string> GetInAndOutByPeopleAndDay(string stTm, string itemId);
        Task<string> GetProjList();
    }
}
