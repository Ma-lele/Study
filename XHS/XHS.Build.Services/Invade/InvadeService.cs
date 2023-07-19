using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Invade
{
    public class InvadeService : BaseServices<GCInvadeEntity>, IInvadeService
    {
        private readonly IBaseRepository<GCInvadeEntity> _dal;
        public InvadeService(IBaseRepository<GCInvadeEntity> dal)
        {
            _dal = dal;
            BaseDal = dal;
        }

        public async Task<PageOutput<GCInvadeListOutput>> GetInvadePageList(int groupid, string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _dal.Db.Queryable<GCInvadeEntity, GCSiteEntity>((e, s) => new JoinQueryInfos(JoinType.Inner, e.SITEID == s.SITEID && e.GROUPID == s.GROUPID))
                .WhereIF(groupid > 0, (e, s) => e.GROUPID == groupid)
                .WhereIF(!string.IsNullOrEmpty(keyword), (e, s) => e.invadename.Contains(keyword) || e.invadecode.Contains(keyword) || s.siteshortname.Contains(keyword))
                .OrderBy((e, s) => e.updatedate, OrderByType.Desc).Select<GCInvadeListOutput>()
                .ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<GCInvadeListOutput>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }

        public async Task<List<GroupHelmetBeaconCount>> GetGroupCount()
        {
            return await _dal.Db.Ado.SqlQueryAsync<GroupHelmetBeaconCount>("  SELECT [GROUPID]  ,[groupname] ,[groupshortname] , (SELECT COUNT(1) FROM [T_GC_Invade] sc WHERE sc.GROUPID = g.GROUPID) count FROM[T_GC_Group] g  ORDER BY count DESC,[groupshortname]");
        }

        public async Task<int> SPWarnInsertForInvade(InvadeWarnInsertInput input)
        {
            List<SugarParameter> param = new List<SugarParameter>();
            param.Add(new SugarParameter("@invadecode", input.invadecode));
            param.Add(new SugarParameter("@createtime", input.createtime));
            param.Add(new SugarParameter("@jsonall", JsonConvert.SerializeObject(input)));
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            param.Add(output);
            await _dal.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spWarnInsertForInvade", param);
            return output.Value.ObjToInt();
        }

        public async Task<List<GCInvadeEntity>> GetDistinctInvadeList(DateTime updatetime)
        {
            return await _dal.Db.Queryable<GCInvadeEntity>().Where(a => SqlFunc.Subqueryable<GCInvadeEntity>().Where(b => b.updatedate >= updatetime).Any()).ToListAsync();
        }
    }
}
