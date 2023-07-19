using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Group
{
    public class GroupService : BaseServices<GcGroupEntity>, IGroupService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<GcGroupEntity> _groupRepository;
        private readonly IBaseRepository<GCGroupSettingEntity> _groupSysRepository;
        public GroupService(IBaseRepository<GcGroupEntity> groupRepository, IBaseRepository<GCGroupSettingEntity> groupSysRepository, IUser user)
        {
            _groupRepository = groupRepository;
            _groupSysRepository = groupSysRepository;
            BaseDal = groupRepository;
            _user = user;
        }

        /// <summary>
        /// 获取分组列表
        /// </summary>
        /// <param name="GROUPID">分组ID</param>
        /// <returns></returns>
        public async Task<List<GcGroupEntity>> GetAll(int GROUPID)
        {
            return await BaseDal.Db.Queryable<GcGroupEntity>()
                .WhereIF(GROUPID > 0, g => g.GROUPID == GROUPID)
                .OrderBy(g => g.groupshortname, OrderByType.Asc).ToListAsync();
        }

        public async Task<List<GCGroupSettingEntity>> GetGroupSetting(int groupid)
        {
            return await _groupSysRepository.Query(ii=>ii.GROUPID == groupid);
        }

        public async Task<List<GroupSiteCount>> GetGroupSiteCountAll()
        {
            var list = await _groupRepository.Db.Queryable<GcGroupEntity>().Select(it => new GroupSiteCount()
            {
                GROUPID = it.GROUPID,
                groupshortname = it.groupshortname,
                suffix = it.suffix,
                count = SqlFunc.Subqueryable<GCSiteEntity>().Where(s => it.GROUPID == s.GROUPID && s.status != 3 && s.SITEID == s.PARENTSITEID).Count()
            }).OrderBy((it) => new { it.count }, OrderByType.Desc)
            .ToListAsync();
            return list;
        }


        public async Task<string> GetAttendUserPsd(int GROUPID)
        {
            var list = await _groupRepository.Db.Queryable<GcGroupEntity>().Where(s => s.GROUPID == GROUPID).ToListAsync();
            if (list != null && list.Count > 0)
            {
                GcGroupEntity entity = list[0];
                return entity.attenduserpsd;
            }
            return null;
        }

        public async Task<List<GroupSiteCount>> GetGroupSiteCountOutZero()
        {
            var list = (await GetGroupSiteCountAll()).Where(a => a.count > 0).ToList();
            return list;
        }

        public async Task<DataTable> getWarnline()
        {
            return await _groupRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spWarnline", new { GROUPID = _user.GroupId });
        }

        public async Task<DataTable> GroupGet()
        {
            return await _groupRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spGroupGet", new { GROUPID = _user.GroupId });
        }

        public new async Task<int> Add(GcGroupEntity model)
        {
            int result = 0;
            var row = await BaseDal.AddEntity(model);
            if (row != null)
            {
                result = 1;
            }
            List<GCGroupSettingEntity> groupSys = await _groupRepository.Db.Queryable<GCGroupSettingEntity>().Where(q => q.GROUPID == row.GROUPID).ToListAsync();

            if (!groupSys.Any())
            {
                List<GCGroupSettingEntity> list = new List<GCGroupSettingEntity>();
                string[] keys = { "noisedayprewarnline", "noisedaywarnline", "noisenightprewarnline", "noisenightwarnline"
                        , "pm10pm25over", "pm10prewarnline", "pm10warnline", "pm2_5prewarnline", "pm2_5warnline", "tspprewarnline", "tspwarnline" };
                string[] vals = { "60", "70", "50", "55", "1", "100", "150", "90", "100", "400", "500" };
                for (int i = 0; i < keys.Length; i++)
                {
                    GCGroupSettingEntity item = new GCGroupSettingEntity();
                    item.GROUPID = row.GROUPID;
                    item.key = keys[i];
                    item.value = vals[i];
                    list.Add(item);
                }

                await _groupSysRepository.Add(list);
            }
            return result;
        }

        public async Task<bool> SetLine(List<GCGroupSettingEntity> updata, List<GCGroupSettingEntity> insertdata)
        {
            bool suc = true;
            if (updata.Count > 0)
            {
                suc = await _groupSysRepository.Db.Updateable(updata)
                     .UpdateColumns(new string[] { "value" })
                     .WhereColumns(new string[] { "GROUPID", "key" })
                    .ExecuteCommandHasChangeAsync();
            }
            if (insertdata.Count > 0 && suc)
            {
                suc = (await _groupSysRepository.Add(insertdata)) > 0 ? true : false;
            }

            return suc;
        }


        /// <summary>
        /// 根据分组获取市区街道
        /// </summary>
        /// <param name="GROUPID">分组编号</param>
        /// <returns></returns>
        public async Task<DataSet> GetAreas(int GROUPID)
        {
            DataSet result = await _groupRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spGetRegionByGroup", new { GROUPID = GROUPID });
            result.Tables[0].TableName = "citys";
            result.Tables[1].TableName = "regions";
            result.Tables[2].TableName = "blocks";
            return result;

        }

        public Task<bool> AddGroupSetting(GCGroupSettingEntity entity)
        {
            throw new NotImplementedException();
        }

        public async Task<List<GCRegionEntity>> GetGroupSelect(string cityCode, string regionid)
        { 
            StringBuilder sql = new StringBuilder();
            sql.Append(" select a.* from XJ_Env.dbo.T_GC_Region a left join XJ_Env.dbo.T_GC_Region b on a.parentid = b.REGIONID ");
            sql.AppendFormat(" where (b.regioncode = '{0}' or a.regioncode = '{0}') ", cityCode);
             
            if (!string.IsNullOrWhiteSpace(regionid) && regionid != "0")
            {
                sql.AppendFormat(" and a.REGIONID in ({0}) ", regionid);
            }

            var list = await _groupRepository.Db.SqlQueryable<GCRegionEntity>(sql.ToString()).ToListAsync();
            return list;
        }
    }
}
