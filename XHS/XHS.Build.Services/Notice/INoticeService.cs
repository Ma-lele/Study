using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Notice
{
    public interface INoticeService:IBaseServices<BaseEntity>
    {
        /// <summary>
        /// 检索公告
        /// </summary>
        /// <param name="operatedate">操作时间</param>
        /// <returns></returns>
        Task<DataTable> getListByUser( DateTime operatedate);

        /// <summary>
        /// 获取改分组下所有公告
        /// </summary>
        /// <returns></returns>
        Task<DataTable> getList();

        /// <summary>
        /// 获取指定监测点的考勤人员
        /// </summary>
        /// <param name="GROUPID">组ID</param>
        /// <param name="SITEID">监测点ID</param>
        /// <returns>结果集</returns>
        Task<DataTable> getUserBySite(int GROUPID, int SITEID);

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="param">注册信息</param>
        /// <returns>公告ID</returns>
        Task<int> doInsert(object param);

        /// <summary>
        /// 获取单条记录
        /// </summary>
        /// <param name="NOTICEID">公告ID</param>
        /// <returns></returns>
        Task<DataSet> getOne(int NOTICEID);

        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="NOTICEID">公告ID</param>
        /// <returns></returns>
        Task<DataTable> getFileList(int NOTICEID);

        /// <summary>
        /// 获取单个公告及图像资源
        /// </summary>
        /// <param name="WEBNOTICEID">公告ID</param>
        /// <returns></returns>
        Task<DataSet> getWebNoticeOne(int WEBNOTICEID);

        /// <summary>
        /// 通过分组获取有效公告列表
        /// </summary>
        /// <param name="GROUPID">公告ID</param>
        /// <returns></returns>
        Task<DataTable> getWebNoticeActiveListByGroup();
    }
}
