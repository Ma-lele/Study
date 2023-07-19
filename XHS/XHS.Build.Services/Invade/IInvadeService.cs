using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Invade
{
    public interface IInvadeService : IBaseServices<GCInvadeEntity>
    {
        Task<PageOutput<GCInvadeListOutput>> GetInvadePageList(int groupid, string keyword, int page, int size);

        Task<List<GroupHelmetBeaconCount>> GetGroupCount();

        Task<int> SPWarnInsertForInvade(InvadeWarnInsertInput input);

        Task<List<GCInvadeEntity>> GetDistinctInvadeList(DateTime updatetime);
    }
}
