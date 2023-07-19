using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.AISoilAction.Dtos;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.AISoilAction
{
    public class AISoilService : BaseServices<AISoilActionEntity>, IAISoilService
    {
        public readonly IBaseRepository<AISoilActionEntity> _repository;
        public readonly IBaseRepository<GCSiteEntity> _siteRepository;
        public AISoilService(IBaseRepository<AISoilActionEntity> repository, IBaseRepository<GCSiteEntity> siteRepository)
        {
            base.BaseDal = repository;
            _repository = repository;
            _siteRepository = siteRepository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projid"></param>
        /// <returns></returns>
        public async Task<AISoilActionEntity> GetSoilLastSoilrate(string projid)
        {
            var sites = await _siteRepository.Query(s => s.soilprojid == projid);
            if (sites.Any())
            {
                var soil = await _repository.Db.Queryable<AISoilActionEntity>().OrderBy(a => a.createtime, OrderByType.Desc).FirstAsync(a => a.SITEID == sites[0].SITEID && a.GROUPID == sites[0].GROUPID && a.createtime.ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"));
                if (soil == null)
                {
                    return new AISoilActionEntity() { SITEID = sites[0].SITEID, GROUPID = sites[0].GROUPID };
                }
                else
                {
                    return soil;
                }

            }
            return null;
        }

        public async Task<int> WarnInsertForSoil(AISoilProcInputDto input)
        {
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _repository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spWarnInsertForSoil",
                new SugarParameter("@projid", input.projid),
                new SugarParameter("@soilrate", input.soilrate),
                new SugarParameter("@createtime", input.createtime),
                new SugarParameter("@jsonall", JsonConvert.SerializeObject(input)),
                returnvalue);
            return returnvalue.Value.ObjToInt();
        }

        public async Task<DataTable> GetSoilList(int SITEID, DateTime date)
        {
            return await _repository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spNjjySiteSoilList", new { SITEID = SITEID, startdate = date, enddate = date });
        }

        public async Task<DataTable> GetAiSoilRecordListAsync(int SiteId,string month, int pageindex, int pagesize)
        {
            return await _repository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiSoilRecordList", new {siteid= SiteId, month = month, pageindex = pageindex, pagesize = pagesize });
        }

        public async Task<DataRow> GetAiSoilDataCompareAsync(int SiteId)
        {
             DataTable dt=  await  _repository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiSoilDataCompare", new { siteid= SiteId });
            if (dt == null || dt.Rows.Count == 0)
                return null;
            return dt.Rows[0];
        }

        public async Task<DataTable> GetAiSoilDataCountAsync(int SiteId,int type)
        {
            return await _repository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiSoilDataCount", new {siteid=SiteId,type=type});
        }

        public async Task<DataTable> GetAiSoilDuringAnalysisAsync(int SiteId, int type)
        {
            return await _repository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2AiSoilDuringAnalysis", new { siteid = SiteId, type = type });
        }
    }
}
