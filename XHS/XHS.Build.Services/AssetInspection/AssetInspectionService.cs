using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;
using System.Linq;

namespace XHS.Build.Services.AssetInspection
{
    public class AssetInspectionService : BaseServices<BaseEntity>, IAssetInspectionService
    {
        private readonly IBaseRepository<BaseEntity> _repository;

        public AssetInspectionService(IBaseRepository<BaseEntity> repository)
        {
            _repository = repository;
            BaseDal = repository;
        }


        public async Task<bool> AddTenantType(CATenantType input)
        {
            var exist = await _repository.Db.Queryable<CATenantType>()
                .Where(ii => ii.name == input.name && ii.Status == 0)
                .FirstAsync();
            if (exist != null && exist.TTID > 0)
            {
                return false;
            }

            int result = await _repository.Db.Insertable(input).ExecuteCommandAsync();
            return result > 0 ? true : false;
        }

        public async Task<bool> EditTenantType(CATenantType input)
        {
            var exist = await _repository.Db.Queryable<CATenantType>()
                   .Where(ii => ii.name == input.name && ii.Status == 0)
                   .FirstAsync();
            if (exist != null && exist.TTID > 0 && input.TTID != exist.TTID)
            {
                return false;
            }
            return await _repository.Db.Updateable(input).WhereColumns(ii=>ii.TTID).ExecuteCommandHasChangeAsync();
        }

        public async Task<PageOutput<CATenantType>> GetTenantTypeList(Expression<Func<CATenantType, bool>> whereExpression, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _repository.Db.Queryable<CATenantType>()
                .WhereIF(whereExpression != null, whereExpression)
                .OrderBy(ii => ii.operatedate, OrderByType.Desc)
                .ToPageListAsync(page, size, totalCount);

            return new PageOutput<CATenantType>() { data = list, dataCount = totalCount };
        }

        public async Task<bool> DeleteTenantType(CATenantType input)
        { 
            return await _repository.Db.Updateable(input)
                .WhereColumns(ii => ii.TTID)
                .UpdateColumns(ii=>ii.Status)
                .ExecuteCommandHasChangeAsync();
        }

        public async Task<CACheckModel> AddTemplate(CACheckModel input)
        {
            return await _repository.Db.Insertable(input).ExecuteReturnEntityAsync();
        }

        public async Task<bool> EditTemplate(CACheckModel input)
        {
            bool result = await _repository.Db.Updateable(input)
                .WhereColumns(ii => ii.CMID)
                .UpdateColumns(ii => new { ii.name, ii.TTID, ii.operatedate, ii.@operator })
                .ExecuteCommandHasChangeAsync();
            if (!result)
            {
                return false;
            }
             
 
            await _repository.Db.Updateable<CACheckList>()
                .SetColumns(ii=>new CACheckList { bdel = 1,@operator = input.@operator,operatedate = input.operatedate})
                .Where(ii => ii.type == 1 && ii.linkid == input.CMID)
                .ExecuteCommandHasChangeAsync();

            return result;
        }

        public async Task<int> AddTemplateItems(List<CACheckList> inputs)
        {
            return await _repository.Db.Insertable(inputs).ExecuteCommandAsync();
        }

        public async Task<PageOutput<InsTmpOutputDto>> GetInsTmpListAsync(Expression<Func<CACheckModel, bool>> whereExpression, 
            int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _repository.Db.Queryable<CACheckModel, CACheckList, CATenantType>((model, list, type) => new JoinQueryInfos(
                JoinType.Left, model.CMID == list.linkid && list.bdel == 0,
                JoinType.Left, model.TTID == type.TTID
                ))
                .Where((model, list, type) => model.bdel == 0 && type.Status == 0)
                .WhereIF(whereExpression != null, whereExpression)
                .GroupBy((model, list, type) => new { model.CMID, model.TTID,TmpName = model.name, model.operatedate, model.@operator, TenantTypeName = type.name })
                .Select((model, list, type) => new InsTmpOutputDto
                {
                    CMID = model.CMID,
                    CheckCount = SqlFunc.AggregateCount(list.CLID),
                    operatedate = model.operatedate,
                    @operator = model.@operator,
                    TenantTypeName = type.name,
                    TmpName = model.name,
                    TTID = model.TTID
                })
                .OrderBy(model => model.operatedate, OrderByType.Desc)
                .ToPageListAsync(page, size, totalCount);

            return new PageOutput<InsTmpOutputDto>() { data = list, dataCount = totalCount };
        }

        public async Task<List<CACheckList>> GetCheckListAsync(int CMID)
        {
            var list = await _repository.Db.Queryable<CACheckList>()
                .Where(ii => ii.linkid == CMID && ii.bdel == 0)
                .ToListAsync();
            return list;
        }

        public async Task<bool> DelTemplate(int CMID, string @operator)
        {
            var result = await _repository.Db.Updateable<CACheckModel>()
                .SetColumns(ii => new CACheckModel { bdel = 1, @operator = @operator, operatedate = DateTime.Now })
                .Where(ii => ii.CMID == CMID)
                .ExecuteCommandHasChangeAsync();

            await _repository.Db.Updateable<CACheckList>()
                .SetColumns(ii => new CACheckList { bdel = 1, @operator = @operator, operatedate = DateTime.Now })
                .Where(ii => ii.type == 1 && ii.linkid == CMID)
                .ExecuteCommandHasChangeAsync();

            return result;
        }

        public async Task<bool> CheckTemplate(int TTID)
        {
            var exists = await _repository.Db.Queryable<CACheckModel>().Where(ii => ii.TTID == TTID && ii.bdel == 0).ToListAsync();
            return exists.Any() ? false : true;
        }

        public async Task<bool> CheckTemplate(int TTID, int CMID)
        {
            var exists = await _repository.Db.Queryable<CACheckModel>()
                .Where(ii => ii.TTID == TTID && ii.CMID != CMID && ii.bdel == 0)
                .ToListAsync();
            return exists.Any() ? false : true;
        }

        public async Task<PageOutput<TenantDto>> GetTenantListAsync(int groupid,string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _repository.Db.Queryable<CATenant, CATenantType, CACheckModel, CACheckList, CACheckList>(
                (tenant, type, model, tmplist, customlist) => new JoinQueryInfos(
                    JoinType.Left, tenant.TTID == type.TTID,
                    JoinType.Left, tenant.CMID == model.CMID && model.bdel == 0,
                    JoinType.Left, model.CMID == tmplist.linkid && tmplist.type == 1 && tmplist.bdel == 0,
                    JoinType.Left, tenant.TEID == customlist.linkid && customlist.type == 2 && customlist.bdel == 0
                    ))
                .WhereIF(groupid > 0, tenant => tenant.GROUPID == groupid)
                .WhereIF(!string.IsNullOrWhiteSpace(keyword), (tenant, type, model, tmplist, customlist) =>
                    tenant.contact.Contains(keyword) || tenant.name.Contains(keyword) || tenant.tel.Contains(keyword) ||
                    tenant.username.Contains(keyword))
                .GroupBy((tenant, type, model, tmplist, customlist) => new
                {
                    tenant.TEID,
                    tenant.GROUPID,
                    tenant.SITEID,
                    tenant.TTID,
                    tenant.doorplate,
                    tenant.name,
                    tenant.username,
                    typename = type.name,
                    tenant.contact,
                    tenant.tel,
                    tenant.status
                })
                .Select((tenant, type, model, tmplist, customlist) => new TenantDto
                {
                    TEID = tenant.TEID,
                    GROUPID = tenant.GROUPID,
                    SITEID = tenant.SITEID,
                    TTID = tenant.TTID,
                    doorplate = tenant.doorplate,
                    name = tenant.name,
                    username = tenant.username,
                    typename = type.name,
                    contact = tenant.contact,
                    tel = tenant.tel,
                    tmpcount = SqlFunc.AggregateDistinctCount(tmplist.CLID),
                    customizecount = SqlFunc.AggregateDistinctCount(customlist.CLID),
                    status = tenant.status
                })
                .OrderBy(tenant => tenant.TEID, OrderByType.Desc)
                .ToPageListAsync(page, size, totalCount);

            return new PageOutput<TenantDto>() { data = list, dataCount = totalCount };
        }

        public async Task<bool> CheckUsername(string username)
        {
            var exist = await _repository.Db.Queryable<CATenant>()
                .Where(ii => ii.username == username)
                .ToListAsync();
            return exist.Any() ? false : true;
        }

        public async Task<bool> CheckUsername(int TEID, string username)
        {
            var exist = await _repository.Db.Queryable<CATenant>()
                .Where(ii => ii.username == username && ii.TEID != TEID)
                .ToListAsync();
            return exist.Any() ? false : true;
        }

        public async Task<List<GroupHelmetBeaconCount>> GetTenantGroupCount()
        {
            return await _repository.Db.Queryable<GcGroupEntity, CATenant>((g, c) => new JoinQueryInfos(
                    JoinType.Left, g.GROUPID == c.GROUPID
                ))
                .Where((g, c) => g.status == 0)
                .GroupBy((g, c) => new { g.GROUPID, g.groupname, g.groupshortname })
                .Select<GroupHelmetBeaconCount>(" g.[GROUPID]  ,g.[groupname] ,g.[groupshortname] ,count(c.TEID) count ")
                .OrderBy("count desc")
                .ToListAsync();
        }

        public async Task<int> GetCMID(int TTID)
        {
            var entity = await _repository.Db.Queryable<CACheckModel>().Where(ii => ii.TTID == TTID && ii.bdel == 0).FirstAsync();

            return (entity == null || entity.CMID <= 0) ? 0 : entity.CMID;
        }

        public async Task<bool> AddTenant(CATenant input)
        {
            var result = await _repository.Db.Insertable(input).ExecuteCommandAsync();

            return result > 0 ? true : false;
        }

        public async Task<bool> EditTenant(CATenant input)
        {
            return await _repository.Db.Updateable(input)
                .UpdateColumns(ii => new {
                    ii.GROUPID,
                    ii.SITEID,
                    ii.name,
                    ii.contact,
                    ii.tel,
                    ii.doorplate,
                    ii.status,
                    ii.TTID,
                    ii.CMID,
                    ii.operatedate,
                    ii.@operator,
                    ii.username
                })
                .WhereColumns(ii => ii.TEID)
                .ExecuteCommandHasChangeAsync();
        }


        public async Task<List<CACheckList>> GetCustom(int TEID)
        {
            var list = await _repository.Db.Queryable<CACheckList>()
                .Where(ii => ii.linkid == TEID && ii.type == 2 && ii.bdel == 0)
                .ToListAsync();
            return list;
        }


        public async Task<bool> DelCustom(int TEID, string @operator)
        {
            var result = await _repository.Db.Updateable<CACheckList>()
                 .SetColumns(ii => new CACheckList { bdel = 1, @operator = @operator, operatedate = DateTime.Now })
                .Where(ii => ii.linkid == TEID && ii.type == 2)
                .ExecuteCommandHasChangeAsync();

            return result;
        }


        public async Task<List<GCSiteEntity>> GetTenantSites(int GROUPID)
        {
            return await _repository.Db.Queryable<GCSiteEntity, CCDataDictionaryEntity>((s, d) => new JoinQueryInfos(
                     JoinType.Inner, s.sitetype == d.DDID
                 ))
                .Where((s, d) => s.GROUPID == GROUPID && s.status == 0 && d.dataitem == "资产巡查项目")
                .ToListAsync();
        }


        public async void UpdateUser(int cmid, int ttid)
        {
            await _repository.Db.Updateable<CATenant>()
                .SetColumns(ii => new CATenant { CMID = cmid })
                .Where(ii => ii.CMID == 0 && ii.TTID == ttid)
                .ExecuteCommandHasChangeAsync();
        }
    }
}
