using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.DeepPit
{
    public interface IDeepPitService : IBaseServices<GCDeepPit>
    {
        /// <summary>
        /// 插入深基坑结构物和设备数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> Insert(SgParams param);

        /// <summary>
        /// 插入设备实时数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> RtdInsert(SgParams param);


        /// <summary>
        /// Group数据
        /// </summary>
        /// <returns></returns>
        Task<List<GroupHelmetBeaconCount>> GetGroupCount();


        /// <summary>
        /// 检查code是否重复
        /// </summary>
        /// <param name="code"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        Task<bool> CheckCode(string code, int groupid, int dpid);


        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PageOutput<DeepPitDto>> GetDpPageList(int groupid, string keyword, int page, int size);


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        new Task<bool> Delete(GCDeepPit input);

        /// <summary>
        /// 获取指定设备的时间区间实时值
        /// </summary>
        /// <param name="updatetime">时间</param>
        /// <returns>数据集</returns>
        Task<List<GCDeepPit>> GetDistinctDeepPitList(DateTime updatetime);


        /// <summary>
        /// 新增深基坑设备
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> AddDevice(GCDeepPitDevice input);


        /// <summary>
        /// 编辑深基坑设备
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<bool> EditDevice(GCDeepPitDevice input);


        /// <summary>
        /// 删除深基坑设备
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> DeleteDevice(GCDeepPitDevice entity);

        /// <summary>
        /// 获取深基坑设备分页数据
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PageOutput<DeepPitDeviceDto>> GetDevicePageList(string keyword, int page, int size);


        /// <summary>
        /// 检查深基坑设备编号是否重复
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Task<bool> CheckDeviceCode(string code);


        /// <summary>
        /// 深基坑-左上角统计
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <returns></returns>
        Task<DataTable> spV2DpStats(int SITEID);


        /// <summary>
        /// 深基坑-下拉框数据
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <returns></returns>
        Task<DataSet> spV2DpSelect(int SITEID);


        /// <summary>
        /// 深基坑-监控信息
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="DPID"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="monitorType"></param>
        /// <param name="deviceid"></param>
        /// <returns></returns>
        Task<DataTable> spV2DpMonitor(int SITEID, int DPID, DateTime startTime, DateTime endTime, string monitorType, string deviceid);



        /// <summary>
        /// 深基坑-实时数据
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <param name="DPID">深基坑ID</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">行数</param>
        /// <returns></returns>
        Task<DataTable> spV2DpLive(int SITEID, int DPID, int pageindex, int pagesize);


        /// <summary>
        /// 深基坑-历史数据
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="DPID"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="monitorType"></param>
        /// <param name="deviceid"></param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">行数</param>
        /// <returns></returns>
        Task<DataTable> spV2DpHis(int SITEID, int DPID, DateTime startTime, DateTime endTime, string monitorType, string deviceid, int pageindex, int pagesize);
    }
}
