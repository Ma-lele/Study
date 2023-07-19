using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Screen
{
    public interface IScreenService:IBaseServices<GCScreenEntity>
    {
        Task<DataRow> getNoticeBycode(string screencode);

        Task<DataRow> syncNotice(SgParams sp);

        Task<PageOutput<VSiteScreen>> GetSiteScreenPageList(int groupid, string keyword, int page, int size);

        Task<List<GroupHelmetBeaconCount>> GetGroupCount();
    }
}
