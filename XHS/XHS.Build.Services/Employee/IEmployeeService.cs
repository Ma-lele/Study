using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Employee
{
    public interface IEmployeeService:IBaseServices<GCEmployeeEntity>
    {
        int doRegist(params SugarParameter[] param);
        Task<PageOutput<EmployeeListOutput>> GetEmployeePageList( string keyword, int page, int size, string order, string ordertype);
        Task<int> InsertOrUpdate(GCEmployeeEntity entity);
    }
}
