using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;
using XHS.Build.Common.Helps;
using XHS.Build.Services.File;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Services.HighFormwork
{
    public class HighFormworkService : BaseServices<GCHighFormwork>, IHighFormworkService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<GCHighFormwork> _repository;
        private readonly IHpFileDoc _hpfiledoc;
        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IMongoDBRepository<HighFormworkData> _mongoRepository;
        public HighFormworkService(IUser user, IBaseRepository<GCHighFormwork> repository, IMongoDBRepository<HighFormworkData> mongoRepository, IHpFileDoc hpfiledoc, IHpSystemSetting hpSystemSetting)
        {
            _user = user;
            _repository = repository;
            _hpfiledoc = hpfiledoc;
            BaseDal = repository;
            _hpSystemSetting = hpSystemSetting;
            _mongoRepository = mongoRepository;
        }


        public async Task<bool> CheckCode(string code, int groupid, int hfwid)
        {
            Expression<Func<GCHighFormwork, bool>> whereExpression = h => h.hfwcode == code && h.GROUPID == groupid && h.bdel == 0;
            if (hfwid > 0)
            {
                whereExpression = whereExpression.And(h => h.HFWID != hfwid);
            }
            var exists = await _repository.Query(whereExpression);
            if (!exists.Any())
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<int> doRtdInsert(HighFormworkData dto)
        {
            await _mongoRepository.InsertAsync(dto);

            List<SugarParameter> param = new List<SugarParameter>();
            param.Add(new SugarParameter("@recordNumber", dto.recordNumber));
            param.Add(new SugarParameter("@deviceId", dto.deviceId));
            param.Add(new SugarParameter("@pointId", dto.pointId));
            param.Add(new SugarParameter("@collectionTime", dto.collectionTime));
            param.Add(new SugarParameter("@power", dto.power));
            param.Add(new SugarParameter("@temperature", dto.temperature));
            param.Add(new SugarParameter("@load", dto.load));
            param.Add(new SugarParameter("@horizontalAngle", dto.horizontalAngle));
            param.Add(new SugarParameter("@coordinate", dto.coordinate));
            param.Add(new SugarParameter("@translation", dto.translation));
            param.Add(new SugarParameter("@settlement", dto.settlement));
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _repository.Db.Ado.UseStoredProcedure().GetIntAsync("spDownHighFormworkRtdData", ps);
            return output.Value.ObjToInt();
        }


        /// <summary>
        /// Group数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<GroupHelmetBeaconCount>> GetGroupCount()
        {
            return await _repository.Db.Queryable<GcGroupEntity, GCHighFormwork>((g, f) => new JoinQueryInfos(
                    JoinType.Left, g.GROUPID == f.GROUPID && f.bdel == 0
                ))
                .Where((g, f) => g.status == 0)
                .GroupBy((g, f) => new { g.GROUPID, g.groupname, g.groupshortname })
                .Select<GroupHelmetBeaconCount>(" g.[GROUPID]  ,g.[groupname] ,g.[groupshortname] ,count(f.HFWID) count ")
                .OrderBy("count desc")
                .ToListAsync();
        }


        public async Task<PageOutput<HighformworkDto>> GetHfwPageList(int groupid, string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _repository.Db.Queryable<GCHighFormwork, GCSiteEntity, GcGroupEntity>((h, s, g) => new JoinQueryInfos(
                     JoinType.Inner, h.SITEID == s.SITEID,
                     JoinType.Inner, h.GROUPID == g.GROUPID
                 ))
                .Where((h, s, g) => h.bdel == 0)
                .WhereIF(groupid > 0, (h, s, g) => h.GROUPID == groupid)
                .WhereIF(!string.IsNullOrEmpty(keyword), (h, s, g) => h.hfwname.Contains(keyword) || h.hfwcode.Contains(keyword) || s.siteshortname.Contains(keyword))
                .Select((h, s, g) => new HighformworkDto
                {
                    bdel = h.bdel,
                    createtime = h.createtime,
                    GROUPID = h.GROUPID,
                    hfwcode = h.hfwcode,
                    HFWID = h.HFWID,
                    hfwname = h.hfwname,
                    operatedate = h.operatedate,
                    @operator = h.@operator,
                    SITEID = h.SITEID,
                    siteshortname = s.siteshortname,
                    groupshortname = g.groupshortname
                })
                .ToPageListAsync(page, size, totalCount);

            var data = new PageOutput<HighformworkDto>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> Delete(GCHighFormwork input)
        {
            return await _repository.Db.Updateable(input)
                .UpdateColumns(ii => new { ii.bdel, ii.@operator, ii.operatedate })
                .ExecuteCommandHasChangeAsync();
        }


        public async Task<GCHighFormworkArea> AddArea(GCHighFormworkArea input)
        {
            var result = await _repository.Db.Insertable(input).ExecuteReturnEntityAsync();
            if (result == null || result.HFWAID <= 0)
            {
                return null;
            }

            if (input.bactive == 1)
            {
                await _repository.Db.Updateable(new GCHighFormworkArea { bactive = 0 })
                    .UpdateColumns(ii => ii.bactive)
                    .Where(ii => ii.HFWID == input.HFWID && ii.HFWAID != result.HFWAID)
                    .ExecuteCommandHasChangeAsync();
            }
            return result;
        }


        /// <summary>
        /// 高支模Group数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<GroupHelmetBeaconCount>> GetAreaGroupCount()
        {
            return await _repository.Db.Queryable<GcGroupEntity, GCHighFormworkArea>((g, f) => new JoinQueryInfos(
                    JoinType.Left, g.GROUPID == f.GROUPID && f.bdel == 0
                ))
                .Where((g, f) => g.status == 0)
                .GroupBy((g, f) => new { g.GROUPID, g.groupname, g.groupshortname })
                .Select<GroupHelmetBeaconCount>(" g.[GROUPID]  ,g.[groupname] ,g.[groupshortname] ,count(f.HFWAID) count ")
                .OrderBy("count desc")
                .ToListAsync();
        }


        public async Task<PageOutput<HighFormworkAreaOutputDto>> GetHfwaPageList(int groupid, string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            string sql = " select a.*,b.hfwcode,b.hfwname,s.siteshortname,c.filetype,c.linkid,c.FILEID "
                + " from T_GC_HighFormworkArea a "
                + " inner join T_GC_Site s on a.SITEID = s.SITEID "
                + " inner join T_GC_HighFormwork b on a.HFWID = b.HFWID "
                + " outer apply ("
                + " select top 1filetype,linkid,FILEID "
                + " from T_GC_File "
                + " where linkid = CONVERT(nvarchar(200), a.HFWAID) and filetype = 'HighFormwork' "
                + " order by createdate desc ) c "
                + "  where a.bdel = 0  ";



            var list = await _repository.Db.SqlQueryable<HighFormworkAreaOutputDto>(sql)
                .WhereIF(groupid > 0, ii => ii.GROUPID == groupid)
                .WhereIF(!string.IsNullOrEmpty(keyword), ii => ii.hfwaname.Contains(keyword) || ii.hfwname.Contains(keyword) || ii.hfwcode.Contains(keyword) || ii.siteshortname.Contains(keyword))
                .Select(ii => new HighFormworkAreaOutputDto
                {
                    bactive = ii.bactive,
                    bdel = ii.bdel,
                    createdate = ii.createdate,
                    GROUPID = ii.GROUPID,
                    HFWAID = ii.HFWAID,
                    hfwaname = ii.hfwaname,
                    HFWID = ii.HFWID,
                    operatedate = ii.operatedate,
                    @operator = ii.@operator,
                    SITEID = ii.SITEID,
                    siteshortname = ii.siteshortname,
                    hfwcode = ii.hfwcode,
                    hfwname = ii.hfwname,
                    filepath = "http://" + _hpSystemSetting.getSettingValue("S034") + "/resourse/" + ii.GROUPID.ToString() + "/HighFormwork/" + ii.SITEID.ToString() + "/" + ii.HFWAID.ToString() + "/" + ii.FILEID.ToString() + ".jpg"
                })
                .OrderBy(ii => ii.createdate, OrderByType.Desc)
                .ToPageListAsync(page, size, totalCount);

            var data = new PageOutput<HighFormworkAreaOutputDto>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }


        public async Task<bool> UpdateHFWA(GCHighFormworkArea input)
        {
            var result = await _repository.Db.Updateable(input)
               .UpdateColumns(ii => new { ii.GROUPID, ii.HFWID, ii.SITEID, ii.bactive, ii.hfwaname, ii.@operator, ii.operatedate })
               .ExecuteCommandHasChangeAsync();

            if (result && input.bactive == 1)
            {
                await _repository.Db.Updateable(new GCHighFormworkArea { bactive = 0 })
                    .UpdateColumns(ii => ii.bactive)
                    .Where(ii => ii.HFWID == input.HFWID && ii.HFWAID != input.HFWAID)
                    .ExecuteCommandHasChangeAsync();
            }

            return result;
        }


        /// <summary>
        /// 删除高支模区域
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<bool> DeleteArea(GCHighFormworkArea input)
        {
            return await _repository.Db.Updateable(input)
                .UpdateColumns(ii => new { ii.bdel, ii.@operator, ii.operatedate })
                .ExecuteCommandHasChangeAsync();
        }


        /// <summary>
        /// 获取指定设备的时间区间实时值
        /// </summary>
        /// <param name="updatetime">时间</param>
        /// <returns>数据集</returns>
        public async Task<List<GCHighFormwork>> GetDistinctHighFormworkList(DateTime updatetime)
        {
            return await _repository.Db.Queryable<GCHighFormwork>().Where(a => SqlFunc.Subqueryable<GCHighFormwork>().Where(b => b.operatedate >= updatetime).Any()).ToListAsync();
        }

        /// <summary>
        /// 获取项目区域，设备信息
        /// </summary>
        /// <param name="GROUPID">分组ID</param>
        /// <param name="SITEID">项目ID</param>
        /// <returns>数据集</returns>
        public async Task<DataSet> GetSiteHighFormworkList(int GROUPID, int SITEID)
        {

            DataSet ds = await _repository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spNjjySiteHighFormworkList", new { GROUPID = GROUPID, SITEID = SITEID });
            return ds;
        }

        /// <summary>
        /// 获取项目高支模告警信息
        /// </summary>
        /// <param name="SITEID">项目ID</param>
        /// <returns>数据集</returns>
        public async Task<DataSet> GetSiteHighFormworkWarnCount(int SITEID)
        {
            DataSet ds = await _repository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spSiteHighFormworkWarnCount", new { SITEID = SITEID });
            return ds;
        }

        /// <summary>
        /// 获取项目高支模告警信息
        /// </summary>
        /// <param name="hfwcode">项目ID</param>
        /// <param name="startdate">项目ID</param>
        /// <param name="enddate">项目ID</param>
        /// <returns>数据集</returns>
        public async Task<DataTable> GetSiteHighFormworkWarnHis(string hfwcode, DateTime startdate, DateTime enddate)
        {
            DataTable dt = await _repository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spNjjySiteHighFormworkWarnHis", new { hfwcode = hfwcode, startdate = startdate, enddate = enddate });
            return dt;
        }

        /// <summary>
        /// 获取项目高支模图表信息app
        /// </summary>
        /// <param name="SITEID">项目ID</param>
        /// <returns>数据集</returns>
        public async Task<DataSet> GetSiteHighFormworkAppChart(int SITEID, int HFWAID, string hfwcode, string spotcode)
        {
            DataSet ds = await _repository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spSiteHighFormworkAppChart", new { SITEID = SITEID, HFWAID = HFWAID, hfwcode = hfwcode, spotcode = spotcode });
            return ds;
        }

        /// <summary>
        /// 获取项目高支模历史数据
        /// </summary>
        /// <param name="SITEID">项目ID</param>
        /// <returns>数据集</returns>
        public async Task<DataTable> GetSiteHighFormworkHisData(int SITEID, int HFWAID, string hfwcode, string spotcode, DateTime startdate, DateTime enddate)
        {
            DataTable dt = await _repository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteHighFormworkHisData", new { SITEID = SITEID, HFWAID = HFWAID, hfwcode = hfwcode, spotcode = spotcode, startdate = startdate, enddate = enddate });
            return dt;
        }

        public async Task<DataTable> spV2HfwStats(int SITEID)
        {
            return await _repository.Db.Ado.UseStoredProcedure()
               .GetDataTableAsync("spV2HfwStats",
               new
               {
                   SITEID = SITEID
               });
        }

        public async Task<DataTable> spV2HfwSelect(int SITEID)
        {
            return await _repository.Db.Ado.UseStoredProcedure()
               .GetDataTableAsync("spV2HfwSelect",
               new
               {
                   SITEID = SITEID
               });
        }

        public async Task<DataTable> spV2HfwMonitor(int SITEID, string hfwcode, int HFWAID, string spotcode)
        {
            return await _repository.Db.Ado.UseStoredProcedure()
               .GetDataTableAsync("spV2HfwMonitor",
               new
               {
                   SITEID = SITEID,
                   hfwcode = hfwcode,
                   HFWAID = HFWAID,
                   spotcode = spotcode
               });
        }

        public async Task<DataTable> spV2HfwMonitorHistory(int SITEID, string hfwcode, int HFWAID, string spotcode, DateTime startDate, DateTime endDate, int pageindex, int pagesize)
        {
            return await _repository.Db.Ado.UseStoredProcedure()
               .GetDataTableAsync("spV2HfwMonitorHistory",
               new
               {
                   SITEID = SITEID,
                   hfwcode = hfwcode,
                   HFWAID = HFWAID,
                   spotcode = spotcode,
                   startdate = startDate,
                   enddate = endDate,
                   pageindex = pageindex,
                   pagesize = pagesize
               });
        }
    }
}
