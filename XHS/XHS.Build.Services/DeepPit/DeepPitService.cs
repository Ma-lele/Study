using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.DeepPit
{
    public class DeepPitService : BaseServices<GCDeepPit>, IDeepPitService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<GCDeepPit> _repository;
        public DeepPitService(IUser user, IBaseRepository<GCDeepPit> repository)
        {
            _user = user;
            _repository = repository;
            BaseDal = repository;
        }


        /// <summary>
        /// 插入深基坑结构物和设备数据
        /// </summary>
        /// <param name="sp"></param>
        /// <returns></returns>
        public async Task<int> Insert(SgParams sp)
        {
            await _repository.Db.Ado.UseStoredProcedure().GetIntAsync("spDownDeepPitData", sp.Params);
            return sp.ReturnValue;
        }

        /// <summary>
        /// 插入设备实时数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<int> RtdInsert(SgParams sp)
        {
            await _repository.Db.Ado.UseStoredProcedure().GetIntAsync("spDownDeepPitRtdData", sp.Params);
            return sp.ReturnValue;
        }


        /// <summary>
        /// Group数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<GroupHelmetBeaconCount>> GetGroupCount()
        {
            return await _repository.Db.Queryable<GcGroupEntity, GCDeepPit>((g, d) => new JoinQueryInfos(
                    JoinType.Left, g.GROUPID == d.GROUPID && d.bdel == 0
                ))
                .Where((g, d) => g.status == 0)
                .GroupBy((g, d) => new { g.GROUPID, g.groupname, g.groupshortname })
                .Select<GroupHelmetBeaconCount>(" g.[GROUPID]  ,g.[groupname] ,g.[groupshortname] ,count(d.DPID) count ")
                .OrderBy("count desc")
                .ToListAsync();
        }


        public async Task<bool> CheckCode(string code, int groupid, int dpid)
        {
            Expression<Func<GCDeepPit, bool>> whereExpression = d => d.dpcode == code && d.GROUPID == groupid && d.bdel == 0;
            if (dpid > 0)
            {
                whereExpression = whereExpression.And(h => h.DPID != dpid);
            }
            var exists = await _repository.Query(whereExpression);
            if (!exists.Any())
            {
                return true;
            }
            return false;
        }


        public async Task<PageOutput<DeepPitDto>> GetDpPageList(int groupid, string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _repository.Db.Queryable<GCDeepPit, GCSiteEntity, GcGroupEntity>((d, s, g) => new JoinQueryInfos(
                     JoinType.Inner, d.SITEID == s.SITEID,
                     JoinType.Inner, d.GROUPID == g.GROUPID
                 ))
                .Where((d, s, g) => d.bdel == 0)
                .WhereIF(groupid > 0, (d, s, g) => d.GROUPID == groupid)
                .WhereIF(!string.IsNullOrEmpty(keyword), (d, s, g) => s.siteshortname.Contains(keyword) || d.dpname.Contains(keyword) || d.dpcode.Contains(keyword))
                .Select((d, s, g) => new DeepPitDto
                {
                    bdel = d.bdel,
                    createtime = d.createtime,
                    GROUPID = d.GROUPID,
                    dpcode = d.dpcode,
                    DPID = d.DPID,
                    dpname = d.dpname,
                    operatedate = d.operatedate,
                    @operator = d.@operator,
                    SITEID = d.SITEID,
                    siteshortname = s.siteshortname,
                    groupshortname = g.groupshortname,
                    dpurl = d.dpurl,
                    latitude = d.latitude,
                    longitude = d.longitude
                })
                .ToPageListAsync(page, size, totalCount);

            var data = new PageOutput<DeepPitDto>()
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
        public new async Task<bool> Delete(GCDeepPit input)
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
        public async Task<List<GCDeepPit>> GetDistinctDeepPitList(DateTime updatetime)
        {
            return await _repository.Db.Queryable<GCDeepPit>().Where(a => SqlFunc.Subqueryable<GCDeepPit>().Where(b => b.operatedate >= updatetime).Any()).ToListAsync();
        }

        public async Task<int> AddDevice(GCDeepPitDevice input)
        {
            return await _repository.Db.Insertable(input).ExecuteCommandAsync();
        }

        public async Task<bool> EditDevice(GCDeepPitDevice input)
        {
            return await _repository.Db.Updateable(input).ExecuteCommandHasChangeAsync();
        }

        public async Task<bool> DeleteDevice(GCDeepPitDevice entity)
        {
            return await _repository.Db.Updateable(entity)
                .WhereColumns(ii => ii.DPDID)
                .UpdateColumns(ii => new { ii.bdel, ii.operatedate, ii.@operator })
                .ExecuteCommandHasChangeAsync();
        }

        public async Task<PageOutput<DeepPitDeviceDto>> GetDevicePageList(string keyword, int page, int size)
        {
            _repository.CurrentDb = "XJ_Env";
            RefAsync<int> totalCount = 0;
            var list = await _repository.Db.Queryable<GCDeepPitDevice, GCDeepPit>((d, p) => new JoinQueryInfos(
                 JoinType.Inner, d.DPID == p.DPID
                 ))
                .Where((d, p) => d.bdel == 0)
                .WhereIF(!string.IsNullOrEmpty(keyword), (d, p) => d.devicename.Contains(keyword) || d.deviceid.Contains(keyword)
                )
                .Select((d, p) => new DeepPitDeviceDto
                {
                    bdel = d.bdel,
                    createtime = d.createtime,
                    DPID = d.DPID,
                    deviceid = d.deviceid,
                    devicename = d.devicename,
                    dpcode = d.dpcode,
                    DPDID = d.DPDID,
                    DpName = p.dpname,
                    GROUPID = p.GROUPID,
                    operatedate = d.operatedate,
                    @operator = d.@operator,
                    SITEID = p.SITEID
                })
                .OrderBy(d => d.createtime, OrderByType.Desc)
                .ToPageListAsync(page, size, totalCount);

            var data = new PageOutput<DeepPitDeviceDto>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }

        public async Task<bool> CheckDeviceCode(string code)
        {
            Expression<Func<GCDeepPitDevice, bool>> whereExpression = d => d.deviceid == code && d.bdel == 0;

            var exists = await _repository.Db.Queryable<GCDeepPitDevice>()
                .Where(whereExpression)
                .ToListAsync();
            if (!exists.Any())
            {
                return true;
            }
            return false;
        }

        public async Task<DataTable> spV2DpStats(int SITEID)
        {
            return await _repository.Db.Ado.UseStoredProcedure()
               .GetDataTableAsync("spV2DpStats",
               new
               {
                   SITEID = SITEID
               });
        }

        public async Task<DataSet> spV2DpSelect(int SITEID)
        {
            return await _repository.Db.Ado.UseStoredProcedure()
               .GetDataSetAllAsync("spV2DpSelect",
               new
               {
                   SITEID = SITEID
               });
        }

        public async Task<DataTable> spV2DpMonitor(int SITEID, int DPID, DateTime startTime, DateTime endTime, 
            string monitorType, string deviceid)
        {
            return await _repository.Db.Ado.UseStoredProcedure()
               .GetDataTableAsync("spV2DpMonitor",
               new
               {
                   SITEID = SITEID,
                   DPID = DPID,
                   startTime = startTime,
                   endTime = endTime,
                   monitorType = monitorType,
                   deviceid = deviceid
               });
        }

        public async Task<DataTable> spV2DpLive(int SITEID, int DPID, int pageindex, int pagesize)
        {
            return await _repository.Db.Ado.UseStoredProcedure()
               .GetDataTableAsync("spV2DpLive",
               new
               {
                   SITEID = SITEID,
                   DPID = DPID,
                   pageindex = pageindex,
                   pagesize = pagesize
               });
        }

        public async Task<DataTable> spV2DpHis(int SITEID, int DPID, DateTime startTime, DateTime endTime,
          string monitorType, string deviceid, int pageindex, int pagesize)
        {
            return await _repository.Db.Ado.UseStoredProcedure()
               .GetDataTableAsync("spV2DpHis",
               new
               {
                   SITEID = SITEID,
                   DPID = DPID,
                   startTime = startTime,
                   endTime = endTime,
                   monitorType = monitorType,
                   deviceid = deviceid,
                   pageindex = pageindex,
                   pagesize = pagesize
               });
        }
    }
}
