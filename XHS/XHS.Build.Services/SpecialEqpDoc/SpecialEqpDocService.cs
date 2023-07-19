using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.SpecialEqpDoc
{
    public class SpecialEqpDocService:BaseServices<BaseEntity>,ISpecialEqpDocService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<BaseEntity> _specialEqpDocRepository;
        private readonly IMongoDBRepository<SpecialEqpData> _specialMongoService;
        public SpecialEqpDocService(IBaseRepository<BaseEntity> specialEqpDocRepository, IMongoDBRepository<SpecialEqpData> specialMongoService, IUser user)
        {
            _user = user;
            _specialEqpDocRepository = specialEqpDocRepository;
            BaseDal = specialEqpDocRepository;
            _specialMongoService = specialMongoService;
        }

        public DataRow doDelete(string SEDOCID)
        {
            DataTable dt = _specialEqpDocRepository.Db.Ado.UseStoredProcedure().GetDataTable("spSpecialEqpDocDelete", new { SEDOCID = SEDOCID });
            if (dt == null || dt.Rows.Count == 0)
                return null;

            return dt.Rows[0];
        }

        public string doInsert(object param)
        {
            return _specialEqpDocRepository.Db.Ado.UseStoredProcedure().GetString("spSpecialEqpDocInsert", param);
        }

        public async Task<DataSet> getList(int SEID)
        {
            return await _specialEqpDocRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spSpecialEqpDocList", new { SEID = SEID });
        }

        public DataSet getNjjySiteSpec(int SITEID)
        {
            return _specialEqpDocRepository.Db.Ado.UseStoredProcedure().GetDataSetAll("spNjjySiteSpec", new { SITEID = SITEID });
        }

        public DataSet getRealOne(int SEID)
        {
            return _specialEqpDocRepository.Db.Ado.UseStoredProcedure().GetDataSetAll("spSpecialEqpRealList", new { SEID = SEID });
        }

        /// <summary>
        /// 获取指定设备的时间区间实时值
        /// </summary>
        /// <param name="SeCode">设备id</param>
        /// <param name="startTime">开始时间, yyyyMMddHHmmss格式</param>
        /// <param name="endTime">结束时间, yyyyMMddHHmmss格式</param>
        /// <returns>数据集</returns>
        public async Task<List<SpecialEqpData>> GetListDataHis(string SeCode, DateTime startTime, DateTime endTime, int pageIndex, int pageSize = 20)
        {
            FilterDefinition<SpecialEqpData> filter = Builders<SpecialEqpData>.Filter.Empty;
            filter = filter & Builders<SpecialEqpData>.Filter.Eq(a => a.SeCode, SeCode);
            filter = filter & Builders<SpecialEqpData>.Filter.Gte(a => a.CreateTime, startTime);
            filter = filter & Builders<SpecialEqpData>.Filter.Lt(a => a.CreateTime, endTime);
            SortDefinition<SpecialEqpData> sort = Builders<SpecialEqpData>.Sort.Ascending(a => a.CreateTime);
            long total = 0;
            List<SpecialEqpData> retString = (List<SpecialEqpData>)_specialMongoService.FindByFilterWithPage(filter, pageIndex, pageSize,sort);
            return retString;
        }
    }
}
