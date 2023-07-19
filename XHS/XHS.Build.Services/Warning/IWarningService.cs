using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model.Base;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Warning
{
    public interface IWarningService:IBaseServices<BaseEntity>
    {
        /// <summary>
        /// 检索当天预警
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <returns></returns>
        Task<DataTable> getTodayList(int SITEID);

        /// <summary>
        /// 检索预警
        /// </summary>
        /// <param name="GROUPID">分组ID</param>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="type">种类(0:全部,1:离线报警,2:超标报警,31:车辆冲洗设备离线报警,32:车辆未冲洗报警,4:特种设备报警,5:临边围挡报警)</param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <returns>预警信息数据集</returns>
        Task<DataTable> getListType(int SITEID, int type, DateTime startdate, DateTime enddate);

        /// <summary>
        /// 报警数据检索
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="type">种类(0:全部,1:离线报警,2:超标报警,31:车辆冲洗设备离线报警,32:车辆未冲洗报警,4:特种设备报警,5:临边围挡报警)</param>
        /// <param name="date">指定日期</param>
        /// <returns>预警信息数据集</returns>
        Task<DataTable> getListByType(int SITEID, int type, DateTime startdate, DateTime enddate);


        /// <summary>
        /// 报警数据检索
        /// </summary>
        /// <param name="devicecode">设备编号</param>
        /// <param name="type">种类(0:全部,1:离线报警,2:超标报警,31:车辆冲洗设备离线报警,32:车辆未冲洗报警,4:特种设备报警,5:临边围挡报警)</param>
        /// <param name="date">指定日期</param>
        /// <returns>预警信息数据集</returns>
        Task<DataTable> getListByDevicecode(string devicecode, int type, DateTime startdate, DateTime enddate);

        /// <summary>
        /// 获取预警统计
        /// </summary>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <returns></returns>
        Task<DataTable> getSumList( DateTime startdate, DateTime enddate);

        /// <summary>
        /// 获取重要告警
        /// </summary>
        /// <param name="GROUPID">分组ID</param>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <returns>预警信息数据集</returns>
        Task<DataTable> getListPrime(int SITEID,  DateTime startdate, DateTime enddate);

        /// <summary>
        /// 获取待发送警告列表
        /// </summary>
        /// <returns></returns>
        Task<DataTable> getSendList();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WARNID"></param>
        /// <returns></returns>
        Task<DataTable> getSendList(int WARNID);

        Task<DataTable> getAISendList(int WARNID);

        /// <summary>
        /// 取得单条记录
        /// </summary>
        /// <param name="WARNID">预警ID</param>
        /// <returns>预警信息</returns>
        Task<DataRow> getOne(int WARNID);

        /// <summary>
        /// 更新警告状态
        /// </summary>
        /// <param name="WARNID">警告ID</param>
        /// <param name="type">警告type</param>
        /// <returns></returns>
        Task<int> doUpdate(int WARNID, int type);

        /// <summary>
        /// 更新警告处理未处理状态
        /// </summary>
        /// <param name="WARNID">警告ID</param>
        /// <param name="warnstatus">更新状态</param>
        /// <returns></returns>
        Task<int> doChangeStatus(int WARNID, int warnstatus );


        /// <summary>
        /// 获取警告列表（APP专用）
        /// </summary>
        /// <param name="SITEID">监测对象编号</param>
        /// <param name="startdate">开始时间</param>
        /// <param name="enddate">结束时间</param>
        /// <returns></returns>
        Task<DataTable> getListApp(int SITEID, DateTime startdate, DateTime enddate);

        /// <summary>
        /// 获取待发送喷水命令列表
        /// </summary>
        /// <returns></returns>
        Task<DataTable> getSendCmdList();
        /// <summary>
        /// 更新雾炮喷水状态
        /// </summary>
        /// <param name="WARNID">预警编号</param>
        /// <returns></returns>
        Task<int> doUpdateCmd(int WARNID);

        /// <summary>
        /// 车辆冲洗设备下线
        /// </summary>
        /// <param name="parkkey">停车场编号</param>
        /// <param name="gatename">车道名称</param>
        /// <returns></returns>
        Task<int> doCarWashOffline(string parkkey, string gatename);

        /// <summary>
        /// 车辆未冲洗
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> doCarUnWashed(SgParams sp);


        /// <summary>
        /// 安全帽佩戴识别
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> doHelmet(params SugarParameter[] param);

        /// <summary>
        /// 陌生人进场识别
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> doStranger(params SugarParameter[] param);

        /// <summary>
        /// 高支模
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> doHighFormwork(HighFormworkAlarmInfoDto dto);

        /// <summary>
        /// 深基坑
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> doDeepPit(SgParams sp);

        /// <summary>
        /// 人车分流识别
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> doTrespasser(params SugarParameter[] param);
        /// <summary>
        /// 火警
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> doFire(params SugarParameter[] param);

        /// <summary>
        /// 烟雾
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> doSmoke(params SugarParameter[] param);

        /// <summary>
        /// 反光衣
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> doVest(params SugarParameter[] param);

        /// <summary>
        /// 智慧电表
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> doElectric(EmeterWarnDataInput input);

        /// <summary>
        /// 升降机人数超载
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> doOverload(params SugarParameter[] param);


        /// <summary>
        /// 车辆冲洗设备下线超时
        /// </summary>
        /// <param name="parkkey">停车场编号</param>
        /// <param name="gatename">车道名称</param>
        /// <param name="type">报警种类</param>
        /// <returns></returns>
        Task<int> doCarWashOfflineTimeout(string parkkey, string gatename, int type);

        /// <summary>
        /// 分页获取预警
        /// </summary>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="normalai"></param>
        /// <param name="warntype"></param>
        /// <param name="warnstatus"></param>
        /// <param name="wpcode"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        Task<DataTable> GetWarnListByCondition(DateTime startdate, DateTime enddate, int normalai, int warntype, int warnstatus, string wpcode, int pageindex, int pagesize);


        Task<int> doNjjyWarnAddRemark(object param);

        Task<DataTable> doNjjyWarnSolve(object param);

        Task<int> doNjjyWarnFinish(object param);
        /// <summary>
        /// 插入提醒
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> doInsert(object param);

        /// <summary>
        /// 获取某条告警对应的监测对象对应的警告接收者（南京建邺专用）
        /// </summary>
        /// <param name="WARNID">告警编号</param>
        /// <returns>结果集</returns>
        Task<DataSet> getNjjyWarnDetail(int WARNID, int type);

        /// <summary>
        /// 获取一个工地AI的30天和1年的统计件数
        /// </summary>
        /// <param name="SITEID">工地编号</param>
        /// <param name="type">AI种类 61:安全帽佩戴识别,62:陌生人进场识别,63:人车分流识别,64:火警</param>
        /// <returns>结果集</returns>
        Task<DataSet> getSiteAiCount(int SITEID, int type);

        /// <summary>
        /// 获取一个工地指定日的统计件数
        /// </summary>
        /// <param name="SITEID">工地编号</param>
        /// <param name="type">AI种类 61:安全帽佩戴识别,62:陌生人进场识别,63:人车分流识别,64:火警</param>
        /// <param name="billdate">指定日</param>
        /// <returns>结果集</returns>
        Task<DataTable> getSiteAiDayCount(int SITEID, int type, DateTime billdate);

        /// <summary>
        /// 获取工地列表
        /// </summary>
        /// <param name="SITEID">工地编号</param>
        /// <param name="type">AI种类 61:安全帽佩戴识别,62:陌生人进场识别,63:人车分流识别,64:火警</param>
        /// <param name="billdate">指定日</param>
        /// <param name="ENDWARNID">本页最后一条数据的WARNID（用于手机分页）</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">一页件数</param>
        /// <returns>结果集</returns>
        Task<DataSet> getSiteAiList(int SITEID, int type, DateTime billdate, int ENDWARNID, int pageindex, int pagesize);

        Task<DataTable> spWarnCountForAppTop(DateTime dt);

        /// <summary>
        /// 接口  车辆冲洗
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<int> CarWashInsert(CarWashInsertDto dto);

        /// <summary>
        /// 获取区间内预警数
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <returns></returns>
        Task<DataTable> GetWarnListForTaihu(int SITEID, DateTime startdate, DateTime? enddate);

        /// <summary>
        /// 新临边报警
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        Task<int> FallProtectionInsert(CCWarning input);

        /// <summary>
        /// 告警统计
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="SITEID">siteid</param>
        /// <returns></returns>
        Task<DataSet> spV2WarnStats(string startTime, string endTime, int SITEID);

        /// <summary>
        /// 告警分析-每日数据
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="SITEID">siteid</param>
        /// <returns></returns>
        Task<DataTable> spV2WarnStatsDaily(string startTime, string endTime, int SITEID);

        /// <summary>
        /// 告警分析-分类统计每日告警数
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="SITEID">siteid</param>
        /// <param name="type">分类</param>
        /// <returns></returns>
        Task<DataTable> spV2WarnTypeDaily(string startTime, string endTime, int SITEID, int type);

        /// <summary>
        /// 告警分析-分类统计每日告警数
        /// </summary>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="SITEID">siteid</param>
        /// <returns></returns>
        Task<DataTable> spV2WarnTypeRank(string startDate, string endDate, int SITEID);

        /// <summary>
        /// 通用告警-实时告警数据列表
        /// </summary>
        /// <param name="type"></param>
        /// <param name="SITEID"></param>
        /// <param name="keyword"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        Task<DataTable> spV2WarnLiveList(int type, int SITEID, string keyword, DateTime startTime, DateTime endTime, int pageSize, int pageIndex);

        /// <summary>
        /// 告警分析-分类统计
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        Task<DataSet> spV2WarnTypeStats(int SITEID, DateTime startTime, DateTime endTime);

        /// <summary>
        /// 告警分析-实时数据下拉框
        /// </summary>
        /// <returns></returns>
        Task<DataTable> spV2WarnLiveSelect();

        /// <summary>
        /// 告警分析-实时数据(按模块分)
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <param name="keyword">关键词(设备编号)</param>
        /// <param name="MPID">模块ID</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">结束时间</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">行数</param>
        /// <returns></returns>
        Task<DataTable> spV2WarnTypeLive(int SITEID, string keyword, int MPID, DateTime startDate, DateTime endDate,
            int pageIndex, int pageSize);
    }
}
