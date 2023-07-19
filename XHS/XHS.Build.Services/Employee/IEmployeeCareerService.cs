using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Employee
{
    public interface IEmployeeCareerService : IBaseServices<GCEmployeeCareerEntity>
    {
        Task<PageOutput<GCEmployeeCareerEntity>> GetCareerPageList(string id, string keyword, int page, int size);
    }
}
