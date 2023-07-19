using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.HighFormwork
{
    public interface IHighFormworkService : IBaseServices<GCHighFormwork>
    {

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> doRtdInsert(HighFormworkData dto);


        /// <summary>
        /// 检查code是否重复
        /// </summary>
        /// <param name="code"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        Task<bool> CheckCode(string code, int groupid, int hfwid);


        /// <summary>
        /// Group数据
        /// </summary>
        /// <returns></returns>
        Task<List<GroupHelmetBeaconCount>> GetGroupCount();


        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PageOutput<HighformworkDto>> GetHfwPageList(int groupid, string keyword, int page, int size);


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<bool> Delete(GCHighFormwork input);


        /// <summary>
        /// 新增区域
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<GCHighFormworkArea> AddArea(GCHighFormworkArea input);


        /// <summary>
        /// 高支模Group数据
        /// </summary>
        /// <returns></returns>
        Task<List<GroupHelmetBeaconCount>> GetAreaGroupCount();


        /// <summary>
        /// 获取区域分页数据
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PageOutput<HighFormworkAreaOutputDto>> GetHfwaPageList(int groupid, string keyword, int page, int size);


        /// <summary>
        /// 更新高支模区域
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<bool> UpdateHFWA(GCHighFormworkArea input);


        /// <summary>
        /// 删除高支模区域
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<bool> DeleteArea(GCHighFormworkArea input);


        /// <summary>
        /// 获取指定设备的时间区间实时值
        /// </summary>
        /// <param name="updatetime">时间</param>
        /// <returns>数据集</returns>
        Task<List<GCHighFormwork>> GetDistinctHighFormworkList(DateTime updatetime);

        /// <summary>
        /// 获取项目区域，设备信息
        /// </summary>
        /// <param name="GROUPID">分组ID</param>
        /// <param name="SITEID">项目ID</param>
        /// <returns>数据集</returns>
        Task<DataSet> GetSiteHighFormworkList(int GROUPID, int SITEID);


        /// <summary>
        /// 获取告警统计
        /// </summary>
        /// <param name="SITEID">项目ID</param>
        /// <returns>数据集</returns>
        Task<DataSet> GetSiteHighFormworkWarnCount(int SITEID);

        /// <summary>
        /// 获取告警记录
        /// </summary>
        /// <param name="hfwcode">项目ID</param>
        /// <param name="startdate">项目ID</param>
        /// <param name="enddate">项目ID</param>
        /// <returns>数据集</returns>
        Task<DataTable> GetSiteHighFormworkWarnHis(string hfwcode, DateTime startdate, DateTime enddate);

        /// <summary>
        /// 获取项目高支模图表信息app
        /// </summary>
        /// <param name="SITEID">项目ID</param>
        /// <returns>数据集</returns>
        Task<DataSet> GetSiteHighFormworkAppChart(int SITEID, int HFWAID, string hfwcode, string spotcode);

        Task<DataTable> GetSiteHighFormworkHisData(int SITEID, int HFWAID, string hfwcode, string spotcode, DateTime startdate, DateTime enddate);

        /// <summary>
        /// 智慧工地2.0项目端-高支模-左上角统计
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <returns></returns>
        Task<DataTable> spV2HfwStats(int SITEID);

        /// <summary>
        /// 智慧工地2.0项目端-高支模-下拉框数据
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <returns></returns>
        Task<DataTable> spV2HfwSelect(int SITEID);

        /// <summary>
        /// 智慧工地2.0项目端-高支模-监控信息
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <param name="hfwcode">设备编号</param>
        /// <param name="HFWAID">区域ID</param>
        /// <param name="spotcode">点位编号</param>
        /// <returns></returns>
        Task<DataTable> spV2HfwMonitor(int SITEID, string hfwcode, int HFWAID, string spotcode);

        /// <summary>
        /// 智慧工地2.0项目端-高支模-监控信息-历史数据
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <param name="hfwcode">设备编号</param>
        /// <param name="HFWAID">区域ID</param>
        /// <param name="spotcode">点位编号</param>
        /// <param name="startDate">查询开始日期</param>
        /// <param name="endDate">查询结束日期</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">行数</param>
        /// <returns></returns>
        Task<DataTable> spV2HfwMonitorHistory(int SITEID, string hfwcode, int HFWAID,
            string spotcode, DateTime startDate, DateTime endDate, int pageindex, int pagesize);
    }
}
