using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.EmployeeSite
{
    public interface IEmployeeSiteService:IBaseServices<GCEmployeeSiteEntity>
    {
        int doSiteRegist(GCEmployeeSiteEntity entity);
        Task<DataTable> GetAttendPerson(int siteid, DateTime billdate);
    }
}
