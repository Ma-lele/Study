using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.UavData
{
    public interface IUavDataService : IBaseServices<BaseEntity>
    { /// <summary>
      /// 检索飞行数据
      /// <param name="SITEID">监测点ID</param>
      /// <param name="createddate">拍摄日</param>
      /// <returns>结果集</returns>
        Task<DataTable> getList(int SITEID, DateTime createddate);
    }
}
