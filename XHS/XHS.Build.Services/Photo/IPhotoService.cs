using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Photo
{
    public interface IPhotoService:IBaseServices<BaseEntity>
    {
        /// <summary>
        /// 获取照片主列表
        /// <returns>结果集</returns>
        Task<DataSet> getList();
        /// <summary>
        /// 检索照片
        /// <param name="SITEID">监测点ID</param>
        /// <param name="createddate">拍摄日</param>
        /// <returns>结果集</returns>
        Task<DataTable> getOne(int SITEID, DateTime createddate);
        /// <summary>
        /// 照片插入
        /// </summary>
        /// <param name="param">参数</param>
        /// <returns></returns>
        Task<int> doInsert(object param);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="PhotoID">照片ID</param>
        /// <returns></returns>
        Task<int> doDelete(int PHOTOID);
        /// <summary>
        /// 删除当天全部
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="createddate">拍摄日</param>
        /// <returns></returns>
        Task<int> doDeleteByDate(int SITEID, DateTime createddate);
    }
}
