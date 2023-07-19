using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Water
{
    public interface IWaterService : IBaseServices<BaseEntity>
    {


        Task<int> rtdInsert(params SugarParameter[] param);

    }
}
