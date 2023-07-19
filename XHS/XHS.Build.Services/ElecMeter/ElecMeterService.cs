using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Common.Sqlsugar;

namespace XHS.Build.Services.ElecMeter
{
    public class ElecMeterService:BaseServices<GCElecMeterEntity>,IElecMeterService
    {
        private readonly IBaseRepository<GCElecMeterEntity> _baseRepository;
        public ElecMeterService(IBaseRepository<GCElecMeterEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
        }

        public async Task<DataTable> GetGroupElecMeterCount()
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spGroupElecMeterCountList");

        }

        public async Task<DataTable> GetElecMeterListBySiteId(int siteid)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spNjjySiteElecList", new SugarParameter("@SITEID", siteid));

        }


        public async Task<DataTable> GetElecRtdData(int siteid,string emetercode)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spNjjySiteElecRtd", new SugarParameter("@SITEID", siteid), new SugarParameter("@emetercode", emetercode));

        }

        public async Task<DataTable> GetSiteElecLatestWarnList(int siteid, string emetercode)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteElecLatestWarnList", new SugarParameter("@SITEID", siteid), new SugarParameter("@emetercode", emetercode));

        }

        public async Task<DataTable> GetElecHisData(int siteid, string emetercode,DateTime time)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteElecHisForApp", new SugarParameter("@SITEID", siteid), new SugarParameter("@emetercode", emetercode), new SugarParameter("@startdate", time));

        }

        public async Task<PageOutput<GCElecMeterPageListOutput>> GetList(int GROUPID, string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _baseRepository.Db.Queryable<GCElecMeterEntity, GCSiteEntity>((e, s) => new JoinQueryInfos(JoinType.Left, e.SITEID == s.SITEID))
                .Where((e, s) => e.bdel == 0)
                .WhereIF(GROUPID > 0, (e, s) => e.GROUPID == GROUPID && s.GROUPID == GROUPID)
                .WhereIF(!string.IsNullOrEmpty(keyword), (e, s) => s.sitename.Contains(keyword) || s.siteshortname.Contains(keyword) || e.emetercode.Contains(keyword) || e.emetername.Contains(keyword))
                .OrderBy((e, s) => new { s.siteshortname, e.emetercode })
            .Select<GCElecMeterPageListOutput>().ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<GCElecMeterPageListOutput>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }

        public async Task<List<GCElecMeterEntity>> GetListForJob()
        {
            var list = await _baseRepository.Db.Queryable<GCElecMeterEntity>()
                .Where(e => e.bdel == 0).ToListAsync();
           
            return list;
        }

        /// <summary>
        /// 实时数据插入
        /// </summary>
        /// <param name="param">数据信息</param>
        /// <returns></returns>
        public async Task<int> RtdInsert(EmeterDataInput input)
        {
            SgParams sp = new SgParams(true);
            sp.SetParams(input);
            await _baseRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spElecDataInsert", sp.Params);
            return sp.ReturnValue;
        }


        /// <summary>
        /// 获取指定设备的时间区间实时值
        /// </summary>
        /// <param name="updatetime">时间</param>
        /// <returns>数据集</returns>
        public async Task<List<GCElecMeterEntity>> GetDistinctElecMeterList(DateTime updatetime)
        {
            return await _baseRepository.Db.Queryable<GCElecMeterEntity>().Where(a => SqlFunc.Subqueryable<GCElecMeterEntity>().Where(b => b.operatedate >= updatetime).Any()).ToListAsync();
        }

        public async Task<DataTable> GetSiteElecWarnChart(int siteid, DateTime startdate, DateTime enddate)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spNjjySiteElecWarnChart", new SugarParameter("@SITEID", siteid), new SugarParameter("@startdate", startdate), new SugarParameter("@enddate", enddate));

        }

        public async Task<DataTable> GetSiteElecWarnTypeChart(int siteid, DateTime startdate, DateTime enddate)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spNjjySiteElecWarnTypeChart", new SugarParameter("@SITEID", siteid), new SugarParameter("@startdate", startdate), new SugarParameter("@enddate", enddate));

        }


        public async Task<DataTable> GetSiteElecWarnList(int siteid, string emetercode, DateTime startdate, DateTime enddate,int pageindex ,int pagesize)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spNjjySiteElecWarnList", new SugarParameter("@SITEID", siteid), new SugarParameter("@emetercode", emetercode), new SugarParameter("@startdate", startdate), new SugarParameter("@enddate", enddate), new SugarParameter("@pageindex", pageindex), new SugarParameter("@pagesize", pagesize));

        }
    }
}
