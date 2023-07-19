using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Employee
{
    public class EmployeeCareerService : BaseServices<GCEmployeeCareerEntity>, IEmployeeCareerService
    {
        private readonly IBaseRepository<GCEmployeeCareerEntity> _baseRepository;
        public EmployeeCareerService(IBaseRepository<GCEmployeeCareerEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
        }
        public async Task<PageOutput<GCEmployeeCareerEntity>> GetCareerPageList(string id, string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _baseRepository.Db.Queryable<GCEmployeeCareerEntity>()
                .Where(e => e.ID == id)
                .WhereIF(!string.IsNullOrEmpty(keyword), (e) => e.Papername.Contains(keyword) || e.Papercode.Contains(keyword))
                .OrderBy((e) => e.Papercode)
                .ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<GCEmployeeCareerEntity>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }
    }
}
