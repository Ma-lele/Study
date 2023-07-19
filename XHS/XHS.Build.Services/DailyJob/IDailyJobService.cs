using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.DailyJob
{
    public interface IDailyJobService:IBaseServices<BaseEntity>
    {
        void SparkcnDoSync();

        void CableDoSync();

        void Excute(string fullPath, string dbName, int timeout);
    }
}
