using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Helmet
{
    public interface IHelmetBeaconService:IBaseServices<GCHelmetBeaconEntity>
    {
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PageOutput<HelmetBeaconOutputList>> GetHelmetBuildPage(int groupid, string keyword, int page, int size, string order = "", string ordertype = "");

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<GroupHelmetBeaconCount>> GetGroupHelmetBuildCount();
    }
}
