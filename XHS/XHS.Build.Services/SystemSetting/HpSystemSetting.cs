using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using XHS.Build.Repository.SystemSetting;

namespace XHS.Build.Services.SystemSetting
{
    public partial class HpSystemSetting:IHpSystemSetting
    {
        private readonly ISystemSettingRepository _systemSettingRepository;
        public HpSystemSetting(ISystemSettingRepository systemSettingRepository)
        {
            _systemSettingRepository = systemSettingRepository;
            _systemSettingRepository.CurrentDb = "XJ_Env";
        }
        /// <summary>
        /// 系统设置
        /// </summary>
        private  DataTable _setting = null;

        private  DataTable _Setting
        {
            get { return getAll(); }
            set { _setting = value; }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public  void init()
        {
            if (_setting == null)
            {
                reset();
            }
        }

        /// <summary>
        /// 系统设置重置
        /// </summary>
        public  void reset()
        {
            _setting = _systemSettingRepository.getAll();
        }

        /// <summary>
        /// 获取所有系统设置
        /// </summary>
        /// <returns></returns>
        public  DataTable getAll()
        {
            if (_setting == null)
            {
                reset();
            }
            return _setting;
        }

        /// <summary>
        /// 根据设置键获取设置
        /// </summary>
        /// <param name="SETTINGID">设置键</param>
        /// <returns></returns>
        public  DataRow getSetting(string SETTINGID)
        {
            DataRow[] rows = _Setting.Select("SETTINGID='" + SETTINGID + "'");
            return rows[0];
        }


        /// <summary>
        /// 根据设置键获取设置的Value
        /// </summary>
        /// <param name="SETTINGID">设置键</param>
        /// <returns></returns>
        public  string getSettingValue(string SETTINGID)
        {
            DataRow[] rows = _Setting.Select("SETTINGID='" + SETTINGID + "'");
            if (rows.Length > 0)
                return Convert.ToString(rows[0]["value"]);

            return null;
        }

        /// <summary>
        /// 更新系统设置
        /// </summary>
        /// <param name="SETTINGID">设置ID</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public  int doUpdate(string SETTINGID, string value)
        {

            int result = _systemSettingRepository.doUpdate(SETTINGID, value);

            //如果更新成功，重置系统设置
            if (result > 0)
                reset();

            return result;
        }
    }
}
