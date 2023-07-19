using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;

namespace XHS.Build.Repository.SystemSetting
{
    public interface ISystemSettingRepository:IBaseRepository<CCSystemSettingEntity>
    {
        /// <summary>
        /// 获取所有的系统设置
        /// </summary>
        /// <returns></returns>
        DataTable getAll();

        /// <summary>
        /// 更新系统设置
        /// </summary>
        /// <param name="SETTINGID">设置ID</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        int doUpdate(string SETTINGID, string value);

        /// <summary>
        /// 获取设定值
        /// </summary>
        /// <param name="USERID"></param>
        /// <returns></returns>
        DataTable getValue(string keys);
    }
}
