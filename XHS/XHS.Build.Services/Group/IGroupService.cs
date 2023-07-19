using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Group
{
    public interface IGroupService:IBaseServices<GcGroupEntity>
    {
        /// <summary>
        /// 获取分组列表
        /// </summary>
        /// <param name="GROUPID">分组ID</param>
        /// <returns></returns>
        Task<List<GcGroupEntity>> GetAll(int GROUPID);
        /// <summary>
        /// 取得各监测内容的预警线
        /// </summary>
        /// <returns>结果集</returns>
        Task<DataTable> getWarnline();

        Task<DataTable> GroupGet();

        Task<string> GetAttendUserPsd(int GROUPID);

        Task<List<GroupSiteCount>> GetGroupSiteCountAll();

        Task<List<GroupSiteCount>> GetGroupSiteCountOutZero();

        Task<List<GCGroupSettingEntity>> GetGroupSetting(int groupid);



        /// <summary>
        /// 设置扬尘告警线
        /// </summary>
        /// <param name="updata"></param>
        /// <returns></returns>
        Task<bool> SetLine(List<GCGroupSettingEntity> updata, List<GCGroupSettingEntity> insertdata);

        /// <summary>
        /// 根据分组获取市区街道
        /// </summary>
        /// <param name="GROUPID">分组编号</param>
        /// <returns></returns>
        Task<DataSet> GetAreas(int GROUPID);


        /// <summary>
        /// 根据城市code获取group
        /// </summary>
        /// <param name="cityCode"></param>
        /// <param name="regionid"></param>
        /// <returns></returns>
        Task<List<GCRegionEntity>> GetGroupSelect(string cityCode, string regionid);
    }
}
