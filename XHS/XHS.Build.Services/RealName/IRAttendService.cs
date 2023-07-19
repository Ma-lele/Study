using System;
using System.Threading.Tasks;
using XHS.Build.Common.Response;
using XHS.Build.Model.Base;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.RealName
{
    public interface IRAttendService : IBaseServices<BaseEntity>
    {
        Task<IResponseOutput> EmployeeRegist(EmployeeInput entity);

        Task<IResponseOutput> EmployeePassHisInsert(EmployeePassHisInsertInput entity);
    }
}
