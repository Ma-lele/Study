using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Cable
{
    public interface ICableService:IBaseServices<GCCableEntity>
    {
        /// <summary>
        /// 获取钢丝绳列表
        /// </summary>
        /// <returns>数据集</returns>
        DataSet GetList();

        /// <summary>
        /// 评估风险等级
        /// </summary>
        /// <param name="sensorid">设备ID</param>
        /// <param name="risklevel">风险等级</param>
        /// <returns></returns>
        int DoRiskLevelCheck(string sensorid, int risklevel);

        /// <summary>
        /// 获取监测点下钢丝绳列表
        /// </summary>
        /// <returns>数据集</returns>
        Task<DataTable> GetListForSite(int SITEID);

        /// <summary>
        /// 获取最大损伤数据
        /// </summary>
        /// <param name="sensorid">设备id</param>
        /// <returns>实时值数据集</returns>
        Task<string> GetMaxDamage(string sensorid);


        /// <summary>
        /// 获取完整的损伤数据
        /// </summary>
        /// <param name="sensorid">设备id</param>
        /// <returns>数据集</returns>
        Task<string> GetAllDamage(string sensorid);


        /// <summary>
        /// 获取完整断丝预测
        /// </summary>
        /// <param name="sensorid">设备id</param>
        /// <returns>数据集</returns>
        Task<string> GetBroken(string sensorid);


        Task<PageOutput<GCCablePageListOutput>> GetSiteCablePageList(int groupid, string keyword, int page, int size);

        Task<List<GroupHelmetBeaconCount>> GetGroupCount();


    }
}
