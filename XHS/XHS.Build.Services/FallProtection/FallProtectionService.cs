using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;
using System.Linq;
using XHS.Build.Model.ModelDtos;
using System.Data;

namespace XHS.Build.Services.FallProtection
{
    public class FallProtectionService : BaseServices<BaseEntity>, IFallProtectionService
    {
        private readonly IBaseRepository<BaseEntity> _repository;

        public FallProtectionService(IBaseRepository<BaseEntity> repository)
        {
            _repository = repository;
            BaseDal = repository;
        }

        public async Task<bool> AddDevice(GCFallProtectionDevice input)
        {
            int result = await _repository.Db.Insertable(input).ExecuteCommandAsync();
            return result > 0 ? true : false;
        }

        public async Task<bool> CheckCode(string code, int FPDID)
        {
            var results = await _repository.Db.Queryable<GCFallProtectionDevice>()
                .Where(ii => ii.deviceId == code && ii.bdel == false)
                .WhereIF(FPDID > 0, ii => ii.FPDID != FPDID)
                .ToListAsync();

            return results.Any() ? false : true;
        }

        public async Task<bool> DeleteDevice(int FPDID, string @operator)
        {
            return await _repository.Db.Updateable<GCFallProtectionDevice>()
                .SetColumns(ii => new GCFallProtectionDevice { bdel = true, @operator = @operator, operatedate = DateTime.Now })
                .Where(ii => ii.FPDID == FPDID)
                .ExecuteCommandHasChangeAsync();
        }

        public async Task<bool> EditDevice(GCFallProtectionDevice input)
        {
            return await _repository.Db.Updateable(input)
                .UpdateColumns(ii => new { ii.GROUPID, ii.SITEID, ii.deviceId, ii.name, ii.@operator, ii.operatedate })
                .WhereColumns(ii => ii.FPDID)
                .ExecuteCommandHasChangeAsync();
        }

        public async Task<GCFallProtectionDevice> FindByCode(string deviceId)
        {
            return await _repository.Db.Queryable<GCFallProtectionDevice>()
                .Where(ii => ii.deviceId == deviceId)
                .FirstAsync();
        }

        public async Task<int> FindnSetOffline()
        {
            return await _repository.Db.Updateable<GCFallProtectionDevice>()
                .SetColumns(ii => new GCFallProtectionDevice
                {
                    onlinestatus = 0,
                    @operator = "自动检测",
                    operatedate = DateTime.Now

                })
                .Where(ii => SqlFunc.DateAdd(ii.lastpushtime, 50, DateType.Hour) < DateTime.Now)
                .ExecuteCommandAsync();
        }

        public async Task<List<GroupHelmetBeaconCount>> GetGroupCount()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(" SELECT [GROUPID]  ,[groupname]  ,[groupshortname] ,  ");
            sql.Append(" (SELECT COUNT(1) FROM T_GC_FallProtectionDevice F WHERE F.bdel = 0 and F.GROUPID = g.GROUPID) count ");
            sql.Append(" FROM[T_GC_Group] g WHERE status = 0 ORDER BY count DESC,[groupshortname] ");
            return await _repository.Db.Ado.SqlQueryAsync<GroupHelmetBeaconCount>(sql.ToString());
        }

        public async Task<PageOutput<FallProtectionDeviceDto>> GetPageListAsync(int groupid, string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _repository.Db.Queryable<GCFallProtectionDevice, GCSiteEntity, GcGroupEntity,CCDataDictionaryEntity>((f, s, g, d) => new JoinQueryInfos(
                     JoinType.Inner, f.SITEID == s.SITEID,
                     JoinType.Inner, f.GROUPID == g.GROUPID,
                     JoinType.Left,f.alarmId == d.dcode && d.datatype == "fpcode"
                 ))
                .Where((f, s, g, d) => f.bdel == false)
                .WhereIF(groupid > 0, (f, s, g, d) => f.GROUPID == groupid)
                .WhereIF(!string.IsNullOrEmpty(keyword), (f, s, g, d) => f.name.Contains(keyword) ||
                f.deviceId == keyword || s.siteshortname.Contains(keyword))
                .Select((f, s, g, d) =>new FallProtectionDeviceDto { 
                    battery=f.battery,
                    bdel = f.bdel,
                    creationtime = f.creationtime,
                    deviceId = f.deviceId,
                    FPDID = f.FPDID,
                    GROUPID = f.GROUPID,
                    lat = f.lat,
                    @long = f.@long,
                    name = f.name,
                    onlinestatus = f.onlinestatus,
                    operatedate = f.operatedate,
                    @operator = f.@operator,
                    SITEID = f.SITEID,
                    siteshortname = s.siteshortname,
                    alarmId = d.dataitem
                })
                .ToPageListAsync(page, size, totalCount);

            var data = new PageOutput<FallProtectionDeviceDto>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }

        public async Task<List<CCSystemSettingEntity>> GetPushUrl()
        {
            return await _repository.Db.Queryable<CCSystemSettingEntity>()
                .Where(ii => ii.SETTINGID == "S196" || ii.SETTINGID == "S197")
                .ToListAsync();
        }

        public async Task<bool> SetAlarm(int FPDID, string alarmId)
        {
            return await _repository.Db.Updateable<GCFallProtectionDevice>()
                .SetColumns(ii => new GCFallProtectionDevice { 
                    alarmId = alarmId, 
                    @operator = "数据推送", 
                    operatedate = DateTime.Now,
                    lastpushtime = DateTime.Now
                })
                .Where(ii => ii.FPDID == FPDID)
                .ExecuteCommandHasChangeAsync();
        }

        public async Task<bool> SetDeviceStatus(GCFallProtectionDevice input)
        {
            return await _repository.Db.Updateable(input)
                 .UpdateColumns(ii => new { ii.battery, ii.lat, ii.@long, ii.onlinestatus, ii.@operator, ii.operatedate,ii.lastpushtime })
                .Where(ii => ii.FPDID == input.FPDID)
                .ExecuteCommandHasChangeAsync();
        }

        public async Task<DataSet> spV2FallProStats(int SITEID)
        {
            return await _repository.Db.Ado.UseStoredProcedure()
               .GetDataSetAllAsync("spV2FallProStats",
               new
               {
                   SITEID = SITEID
               });
        }

        public async Task<DataTable> spV2FallProWarnType(int SITEID, DateTime startTime, DateTime endTime)
        {
            return await _repository.Db.Ado.UseStoredProcedure()
               .GetDataTableAsync("spV2FallProWarnType",
               new
               {
                   SITEID = SITEID,
                   startTime = startTime,
                   endTime = endTime
               });
        }

        public async Task<DataTable> spV2FallProDevStats(int SITEID, string keyword, int online, int alarm)
        {
            return await _repository.Db.Ado.UseStoredProcedure()
              .GetDataTableAsync("spV2FallProDevStats",
              new
              {
                  SITEID = SITEID,
                  keyword = keyword,
                  online = online,
                  alarm = alarm
              });
        }
    }
}
