using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Employee
{
    public class EmployeeService : BaseServices<GCEmployeeEntity>, IEmployeeService
    {
        private readonly IBaseRepository<GCEmployeeEntity> _baseRepository;
        public EmployeeService(IBaseRepository<GCEmployeeEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="param">注册信息</param>
        /// <returns>工地人员ID</returns>
        public int doRegist(params SugarParameter[] param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            _baseRepository.Db.Ado.UseStoredProcedure().ExecuteCommand("spEmployeeRegist", ps);
            return output.Value.ObjToInt();
        }

        public async Task<PageOutput<EmployeeListOutput>> GetEmployeePageList(string keyword, int page, int size, string order, string ordertype)
        {
            RefAsync<int> totalCount = 0;
            var list = await _baseRepository.Db.Queryable<EmployeeListOutput>()
                .WhereIF(!string.IsNullOrEmpty(keyword), e => e.RealName.Contains(keyword) || e.ID.Contains(keyword))
                .OrderByIF(!string.IsNullOrEmpty(order), order + " " + ordertype)
                .ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<EmployeeListOutput>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }

        /// <summary>
        /// 新增或修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> InsertOrUpdate(GCEmployeeEntity entity)
        {
            //return await _baseRepository.Db.Saveable<GCEmployeeEntity>(entity).ExecuteCommandAsync();
            var db = await _baseRepository.Db.Queryable<GCEmployeeEntity>().SingleAsync(e => e.ID == entity.ID);
            if (db == null)
            {
                return await _baseRepository.Db.Insertable(entity).ExecuteCommandAsync();
            }
            else
            {
                return await _baseRepository.Db.Updateable(entity).ExecuteCommandAsync();
            }
        }
    }
}
