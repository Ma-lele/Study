using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace XHS.Build.Services.SystemSetting
{
   public  interface IHpSystemSetting
    {
        void init();
        void reset();
        DataTable getAll();
        DataRow getSetting(string SETTINGID);
        string getSettingValue(string SETTINGID);
        int doUpdate(string SETTINGID, string value);
    }
}
