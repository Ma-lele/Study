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
    public class SystemSettingRepository : BaseRepository<CCSystemSettingEntity>, ISystemSettingRepository
    {
        public SystemSettingRepository(IUnitOfWork unitOfWork) : base(unitOfWork) { }

        public int doUpdate(string SETTINGID, string value)
        {
            return Db.Ado.UseStoredProcedure().GetInt("spSystemSettingUpdate", new { SETTINGID = SETTINGID, value = value });
        }

        public DataTable getAll()
        {
            return  Db.Ado.UseStoredProcedure().GetDataTable("spSystemSettingAll");
        }

        public DataTable getValue(string keys)
        {
            return  Db.Ado.UseStoredProcedure().GetDataTable("spSystemSettingGet", new { IDS = keys });
        }
    }
}
