using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.CtUser
{
    public interface ICtUserService : IBaseServices<CTUserEntity>
    {
        /// <summary>
        /// 获取一页
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PageOutput<CTUserListEntity>> GetPageList(string keyword, int page, int size, int GROUPID);

        /// <summary>
        /// 新建用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> UserAdd(CTUserEntity input);

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> UserUpdate(CTUserEntity input);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> UserDelete(CTUserEntity input);
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
        Task<int> ResetPwd(CTUserEntity input);

        /// <summary>
        /// 获取分组下一级的区域列表（分组到市，返回区列表；分组到区，返回街道列表）
        /// </summary>
        /// <param name="GROUPID"></param>
        /// <returns></returns>
        Task<DataTable> GetRegions(int GROUPID);
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> ChangePwd(CTUserPwd input);
        // <summary>
        /// 更新验证码
        /// </summary>
        /// <param name="mobile">手机号</param>
        /// <param name="verifycode">验证码</param>
        /// <returns></returns>
        Task<int> SetVerifyCode(string mobile, string verifycode);
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        Task<DataTable> GetUserLogin(string username, string pwd);
    }
}
