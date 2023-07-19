using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Helps;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Ozone
{
    public class OzoneService : BaseServices<GCOzone>, IOzoneService
    {
        private readonly IBaseRepository<GCOzone> _repository;

        public OzoneService(IBaseRepository<GCOzone> repository)
        {
            BaseDal = repository;
            _repository = repository;
        }

        /// <summary>
        /// 插入实时数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<int> doRtdInsert(OzoneRtdDataInput input)
        {
            List<SugarParameter> param = new List<SugarParameter>();
            param.Add(new SugarParameter("@ozcode", input.ozcode));
            param.Add(new SugarParameter("@o3", input.o3)); 
            param.Add(new SugarParameter("@collectionTime", input.collectionTime));
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var PList = param.ToList();
            PList.Add(output);
            await _repository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spOzoneRtdInsert", PList);
            return output.Value.ObjToInt();
        }

        /// <summary>
        /// Group数据
        /// </summary>
        /// <returns></returns>
        public async Task<List<GroupHelmetBeaconCount>> GetGroupCount()
        {
            return await _repository.Db.Queryable<GcGroupEntity, GCOzone>((g, o) => new JoinQueryInfos(
                    JoinType.Left, g.GROUPID == o.GROUPID && o.bdel == 0
                ))
                .Where((g, o) => g.status == 0)
                .GroupBy((g, o) => new { g.GROUPID, g.groupname, g.groupshortname })
                .Select<GroupHelmetBeaconCount>(" g.[GROUPID]  ,g.[groupname] ,g.[groupshortname] ,count(o.OZID) count ")
                .OrderBy("count desc")
                .ToListAsync();
        }


        public async Task<bool> CheckCode(string code, int groupid, int ozid)
        {
            Expression<Func<GCOzone, bool>> whereExpression = o => o.ozcode == code && o.GROUPID == groupid && o.bdel == 0;
            if (ozid > 0)
            {
                whereExpression = whereExpression.And(o => o.OZID != ozid);
            }
            var exists = await _repository.Query(whereExpression);
            if (!exists.Any())
            {
                return true;
            }
            return false;
        }


        public async Task<PageOutput<OzoneDto>> GetOzPageList(int groupid, string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _repository.Db.Queryable<GCOzone, GCSiteEntity, GcGroupEntity>((o, s, g) => new JoinQueryInfos(
                     JoinType.Inner, o.SITEID == s.SITEID,
                     JoinType.Inner, o.GROUPID == g.GROUPID
                 ))
                .Where((o, s, g) => o.bdel == 0)
                .WhereIF(groupid > 0, (o, s, g) => o.GROUPID == groupid)
                .WhereIF(!string.IsNullOrEmpty(keyword), (o, s, g) => o.ozname.Contains(keyword))
                .Select((o, s, g) => new OzoneDto
                {
                    bdel = o.bdel,
                    createtime = o.createtime,
                    GROUPID = o.GROUPID,
                    ozcode = o.ozcode,
                    OZID = o.OZID,
                    o3 = o.o3,
                    ozname = o.ozname,
                    operatedate = o.operatedate,
                    @operator = o.@operator,
                    SITEID = o.SITEID,
                    siteshortname = s.siteshortname,
                    groupshortname = g.groupshortname
                })
                .ToPageListAsync(page, size, totalCount);

            var data = new PageOutput<OzoneDto>()
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
        public async Task<bool> Delete(GCOzone input)
        {
            return await _repository.Db.Updateable(input)
                .UpdateColumns(ii => new { ii.bdel, ii.@operator, ii.operatedate })
                .ExecuteCommandHasChangeAsync();
        }    }
}
