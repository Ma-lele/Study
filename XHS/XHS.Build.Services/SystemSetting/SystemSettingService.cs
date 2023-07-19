using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Repository.SystemSetting;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.SystemSetting
{
    public class SystemSettingService : BaseServices<CCSystemSettingEntity>, ISystemSettingService
    {
        private readonly ISystemSettingRepository _systemSettingRepository;
        public SystemSettingService(ISystemSettingRepository systemSettingRepository)
        {
            _systemSettingRepository = systemSettingRepository;
            BaseDal = systemSettingRepository;
        }
        public int doUpdate(string SETTINGID, string value)
        {
            return _systemSettingRepository.doUpdate(SETTINGID, value);
        }

        public DataTable getAll()
        {
            return _systemSettingRepository.getAll();
        }

        public DataTable getValue(string keys)
        {
            return _systemSettingRepository.getValue(keys);
        }

    }
}
