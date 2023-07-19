using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.AssetInspection
{
    public interface IAssetInspectionService : IBaseServices<BaseEntity>
    {
        /// <summary>
        /// 新增租户类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<bool> AddTenantType(CATenantType input);


        /// <summary>
        /// 获取租户类型分页
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PageOutput<CATenantType>> GetTenantTypeList(Expression<Func<CATenantType, bool>> whereExpression, 
            int page, int size);


        /// <summary>
        /// 编辑租户类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<bool> EditTenantType(CATenantType input);


        /// <summary>
        /// 删除租户类型
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<bool> DeleteTenantType(CATenantType input);


        /// <summary>
        /// 新增模板
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<CACheckModel> AddTemplate(CACheckModel input);


        /// <summary>
        /// 编辑模板
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<bool> EditTemplate(CACheckModel input);


        /// <summary>
        /// 新增检查项配置
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        Task<int> AddTemplateItems(List<CACheckList> inputs);


        /// <summary>
        /// 分页查询检查模板
        /// </summary>
        /// <param name="whereExpression"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PageOutput<InsTmpOutputDto>> GetInsTmpListAsync(Expression<Func<CACheckModel, bool>> whereExpression,
            int page, int size);


        /// <summary>
        /// 获取模板检查项列表
        /// </summary>
        /// <param name="CMID"></param>
        /// <returns></returns>
        Task<List<CACheckList>> GetCheckListAsync(int CMID);


        /// <summary>
        /// 删除检查模板
        /// </summary>
        /// <param name="CMID"></param>
        /// <param name="operator"></param>
        /// <returns></returns>
        Task<bool> DelTemplate(int CMID, string @operator);


        /// <summary>
        /// 模板查重
        /// </summary>
        /// <param name="TTID"></param>
        /// <returns></returns>
        Task<bool> CheckTemplate(int TTID);


        /// <summary>
        /// 模板查重
        /// </summary>
        /// <param name="TTID"></param>
        /// <param name="CMID"></param>
        /// <returns></returns>
        Task<bool> CheckTemplate(int TTID, int CMID);



        /// <summary>
        /// 租户列表分页
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PageOutput<TenantDto>> GetTenantListAsync(int groupid,string keyword, int page, int size);


        /// <summary>
        /// 登录名查重
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<bool> CheckUsername(string username);


        /// <summary>
        /// 登录名查重
        /// </summary>
        /// <param name="TEID"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<bool> CheckUsername(int TEID, string username);


        /// <summary>
        /// 分组count
        /// </summary>
        /// <returns></returns>
        Task<List<GroupHelmetBeaconCount>> GetTenantGroupCount();


        /// <summary>
        /// 根据分类查询模板ID
        /// </summary>
        /// <param name="TTID"></param>
        /// <returns></returns>
        Task<int> GetCMID(int TTID);


        /// <summary>
        /// 新增租户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<bool> AddTenant(CATenant input);


        /// <summary>
        /// 编辑租户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<bool> EditTenant(CATenant input);


        /// <summary>
        /// 自定义检查项
        /// </summary>
        /// <param name="TEID"></param>
        /// <returns></returns>
        Task<List<CACheckList>> GetCustom(int TEID);


        /// <summary>
        /// 删除自定义检查项
        /// </summary>
        /// <param name="TEID"></param>
        /// <param name="operator"></param>
        /// <returns></returns>
        Task<bool> DelCustom(int TEID, string @operator);


        /// <summary>
        /// 获取资产巡检SITES
        /// </summary>
        /// <param name="GROUPID"></param>
        /// <returns></returns>
        Task<List<GCSiteEntity>> GetTenantSites(int GROUPID);


        /// <summary>
        /// 更新用户模板
        /// </summary>
        /// <param name="cmid"></param>
        /// <param name="ttid"></param>
        void UpdateUser(int cmid, int ttid);
    }
}
