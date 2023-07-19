using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.FrontUser
{
    public interface IFrontUserService:IBaseServices<GCUserEntity>
    {
        /// <summary>
        /// 获取一页
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PageOutput<GCUserListEntity>> GetPageList(string keyword, int page, int size, int GROUPID);

        /// <summary>
        /// 新建用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> UserAdd(GCUserEntity input);

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> UserUpdate(GCUserEntity input);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> UserDelete(GCUserEntity input);
        /// <summary>
        /// 获取每个分组用户数量
        /// </summary>
        /// <returns></returns>
        Task<DataTable> GetGroupCount();
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> ResetPwd(GCUserEntity input);
        /// <summary>
        /// 获取单个工地的负责人列表（勾和没勾都获取）
        /// </summary>
        /// <param name="USERID"></param>
        /// <returns></returns>
        Task<DataTable> getUserSiteList(int USERID);
        /// <summary>
        /// 保存单个工地的负责人
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> saveUserSite(UserSiteInput input);
    }
}
