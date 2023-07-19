using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.UserService
{
     public interface IUserService : IBaseServices<GCUserEntity>
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        Task<DataTable> GetUserByLogin(LoginRequest loginRequest);

        /// <summary>
        /// 租户登录
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <returns></returns>
        Task<DataTable> GetTenantUserByLogin(LoginRequest loginRequest);

        Task<int> changePwd(string USERID, string pwd);


        Task<int> changeTenantUserPwd(string USERID, string pwd);

        /// <summary>
        /// 更新用户极光推送ID
        /// </summary>
        /// <param name="RegistrationId"></param>
        /// <returns></returns>
        Task<int> UserJpushIdSave(string RegistrationId, string USERUUID);

        /// <summary>
        /// 获取用户极光推送ID
        /// </summary>
        /// <param name="uuids"></param>
        /// <returns></returns>
        Task<string> GetUserJpushIds(string uuids);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="GROUPID"></param>
        ///  <param name="updatetime">差分时间</param>
        /// <returns></returns>
        Task<DataTable> GetUserList(int GROUPID,DateTime? updatetime);

        /// <summary>
        /// API登录
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        Task<GCUserEntity> GetUserByUUID(string uuid);

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="loginName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<GCUserEntity> QueryUser(string loginName, string password);

    }

}
