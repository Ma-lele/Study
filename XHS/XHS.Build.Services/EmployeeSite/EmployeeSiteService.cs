using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.EmployeeSite
{
    public class EmployeeSiteService:BaseServices<GCEmployeeSiteEntity>,IEmployeeSiteService
    {
        private readonly IBaseRepository<GCEmployeeSiteEntity>  _baseRepository;
        public EmployeeSiteService(IBaseRepository<GCEmployeeSiteEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
        }

        /// <summary>
        /// 更新员工监测点映射关系数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public int doSiteRegist(GCEmployeeSiteEntity entity)
        {
            SgParams sp = new SgParams();
            sp.SetParams(entity,true);
            _baseRepository.Db.Ado.UseStoredProcedure().ExecuteCommand("spEmployeeSiteRegist", sp.Params);
            return sp.ReturnValue;

            //var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            //var ps = param.ToList();
            //ps.Add(output);
            //_baseRepository.Db.Ado.UseStoredProcedure().ExecuteCommand("spEmployeeSiteRegist", ps);
            //return output.Value.ObjToInt()>0?output.Value.ObjToInt():0;
        }

        public async Task<DataTable> GetAttendPerson(int siteid,DateTime billdate)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spAttendPerson", new { SITEID = siteid, billdate = billdate });
        }
    }
}
