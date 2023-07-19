using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;

namespace XHS.Build.Repository.Unload
{
    public interface IUnloadRepository :IBaseRepository<GCUnloadEntity>
    {
        /// <summary>
        /// 获取卸料平台列表
        /// </summary>
        /// <returns>数据集</returns>
        Task<PageOutput<GCUnloadPageListOutput>> GetSiteUnloadPageList(int groupid, string keyword, int page, int size);


        Task<List<GroupHelmetBeaconCount>> GetGroupCount();


        Task<int> UpdateRtdData(string unloadid , string paramjson);
    }
}
