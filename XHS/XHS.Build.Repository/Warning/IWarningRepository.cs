using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;

namespace XHS.Build.Repository.Warning
{
    public interface IWarningRepository:IBaseRepository<BaseEntity>
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
        /// <param name="USERID">用户ID</param>
        /// <param name="type">种类(0:全部,1:离线报警,2:超标报警,31:车辆冲洗设备离线报警,32:车辆未冲洗报警,4:特种设备报警,5:临边围挡报警)</param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <returns>预警信息数据集</returns>
        Task<DataTable> getListType(int GROUPID, int SITEID, string USERID, int type, DateTime startdate, DateTime enddate);

        /// <summary>
        /// 检索预警
        /// </summary>
        /// <param name="GROUPID">分组ID</param>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="USERID">用户ID</param>
        /// <param name="type">种类(0:全部,1:离线报警,2:超标报警,31:车辆冲洗设备离线报警,32:车辆未冲洗报警,4:特种设备报警,5:临边围挡报警)</param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <returns>预警信息数据集</returns>
        Task<DataTable> getListByType(int GROUPID, int SITEID, string USERID, int type, DateTime startdate, DateTime enddate);


        /// <summary>
        /// 获取预警统计
        /// </summary>
        /// <param name="GROUPID">分组ID</param>
        /// <param name="USERID">用户</param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <returns></returns>
        Task<DataTable> getSumList(int GROUPID, string USERID, DateTime startdate, DateTime enddate);

        /// <summary>
        /// 获取重要告警
        /// </summary>
        /// <param name="GROUPID">分组ID</param>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="USERID">用户ID</param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <returns>预警信息数据集</returns>
        Task<DataTable> getListPrime(int GROUPID, int SITEID, string USERID, DateTime startdate, DateTime enddate);

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
        Task<DataTable> getOne(int WARNID);

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
        /// <param name="username">更新者</param>
        /// <returns></returns>
        Task<int> doChangeStatus(int WARNID, int warnstatus, string username);


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
        /// <param name="userid"></param>
        /// <param name="groupid"></param>
        /// <param name="startdate"></param>
        /// <param name="enddate"></param>
        /// <param name="normalai"></param>
        /// <param name="warntype"></param>
        /// <param name="warnstatus"></param>
        /// <param name="wpcode"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        Task<DataTable> GetWarnListByCondition(string userid,string groupid,DateTime startdate,DateTime enddate,int normalai,int warntype,int warnstatus,string wpcode,int pageindex,int pagesize);

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
        /// <param name="type">AI种类 61:安全帽佩戴识别,62:陌生人进场识别,63:人车分流识别,64:火警,65:区域闯入,66:黄土裸露,67:密闭运输,68:反光衣</param>
        /// <param name="billdate">指定日</param>
        /// <param name="ENDWARNID">本页最后一条数据的WARNID（用于手机分页）</param>
        /// <param name="pageindex">页码</param>
        /// <param name="pagesize">一页件数</param>
        /// <returns>结果集</returns>
        Task<DataSet> getSiteAiList(int SITEID, int type, DateTime billdate, int ENDWARNID, int pageindex, int pagesize);

        Task<DataTable> spWarnCountForAppTop(string userid, int groupid, DateTime dt);


        Task<DataTable> GetWarnListForTaihu(int groupid,int SITEID, DateTime startdate, DateTime? enddate);
    }
}
