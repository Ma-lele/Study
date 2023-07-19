using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Unload
{
    public interface IUnloadService:IBaseServices<GCUnloadEntity>
    {

        /// <summary>
        /// 获取卸料平台列表
        /// </summary>
        /// <returns>卸料平台数据集</returns>
        Task<DataTable> getListForSite(int SITEID);

        Task<int> doWarn(params SugarParameter[] param);

        /// <summary>
        /// 增加实时数据
        /// </summary>
        /// <param name="unloadInput"></param>
        /// <returns></returns>
        Task<int> AddRealData(UnloadInput unloadInput);

        Task<PageOutput<GCUnloadPageListOutput>> GetSiteUnloadPageList(int groupid, string keyword, int page, int size);

        Task<List<GroupHelmetBeaconCount>> GetGroupCount();

        /// <summary>
        /// 获取获取实时值
        /// </summary>
        /// <param name="unloadid">设备id</param>
        /// <returns>实时值数据集</returns>
        Task<string> GetListRealUnloadData(string unloadid);


        /// <summary>
        /// 获取指定设备的时间区间实时值
        /// </summary>
        /// <param name="unloadid">设备id</param>
        /// <param name="startTime">开始时间, yyyyMMddHHmmss格式</param>
        /// <param name="endTime">结束时间, yyyyMMddHHmmss格式</param>
        /// <returns>数据集</returns>
        Task<string> GetListUnloadDataScheduleCurrent(string unloadid,string startTime,string endTime);

        /// <summary>
        /// 获取指定设备的时间区间实时值(MongoDB)
        /// </summary>
        /// <param name="unloadid">设备id</param>
        /// <param name="startTime">开始时间, yyyyMMddHHmmss格式</param>
        /// <param name="endTime">结束时间, yyyyMMddHHmmss格式</param>
        /// <returns>数据集</returns>
        Task<List<UnloadInput>> GetListUnloadDataCurrent(string unloadid, DateTime startTime, DateTime endTime);


        Task<List<GCUnloadEntity>> GetDistinctUnloadList(DateTime updatetime);


        /// <summary>
        /// 卸料平台-左上角统计
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <returns></returns>
        Task<DataTable> spV2UnloadStats(int SITEID);


        /// <summary>
        /// 卸料平台-下拉框监测数据
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <returns></returns>
        Task<DataTable> spV2UnloadSelect(int SITEID);

        /// <summary>
        /// 卸料平台-历史数据
        /// </summary>
        /// <param name="unloadid">设备id</param>
        /// <param name="searchDate">查找日期</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">行数</param>
        /// <param name="total">总行数</param>
        /// <returns></returns>
        List<UnloadInput> UnloadHistory(string unloadid, DateTime searchDate, int pageIndex, int pageSize, ref long total);

        /// <summary>
        /// 卸料平台-设备重量趋势
        /// </summary>
        /// <param name="unloadid">设备id</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <returns></returns>
        List<UnloadInput> UnloadTrend(string unloadid, DateTime startDate, DateTime endDate);
    }
}
