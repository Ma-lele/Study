using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Attend
{
    public interface IAttendService : IBaseServices<BaseEntity>
    {
        /// <summary>
        /// 插入考勤记录
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        Task<long> DoInsert(int SITEID, float longitude, float latitude, string address, string remark);

        /// <summary>
        /// 插入考勤便签
        /// </summary>
        /// <param name="attenddate">考勤日期</param>
        /// <param name="operatenote">便签内容</param>
        /// <returns></returns>
        Task<int> DoNoteRegist(DateTime attenddate, string operatenote);

        /// <summary>
        /// 根据考勤ID获取数据
        /// </summary>
        /// <param name="ATTENDID">考勤ID</param>
        /// <returns></returns>
        Task<DataTable> GetListById(ulong ATTENDID);

        /// <summary>
        /// 获取考勤
        /// </summary>
        /// <param name="operatedate">操作日期</param>
        /// <returns></returns>
        Task<DataTable> GetNoteList( DateTime operatedate);

        /// <summary>
        /// 取得一个月的考勤汇总数据
        /// </summary>
        /// <param name="GROUPID">组ID</param>
        /// <param name="datestart">开始日期</param>
        /// <param name="dateend">结束日期</param>
        /// <returns>结果集</returns>
        Task<DataTable> GetAttendMonth(int GROUPID, string datestart, string dateend);

        /// <summary>
        /// 取得某一天每个员工的考勤列表
        /// </summary>
        /// <param name="GROUPID">组ID</param>
        /// <param name="billdate">指定日</param>
        /// <returns>结果集</returns>
        Task<DataTable> GetAttendDaily(int GROUPID, string billdate);

        /// <summary>
        ///  取得某一天一个员工的考勤列表
        /// </summary>
        /// <param name="billdate">指定日</param>
        /// <returns>结果集</returns>
        Task<DataTable> GetAttendDailyOne( string billdate);

        /// <summary>
        /// 切换考勤记录有效无效
        /// </summary>
        /// <param name="ATTENDID">主键</param>
        /// <param name="type">1:切换成有效 0：切换成无效</param>
        /// <returns>结果</returns>
        Task<int> DoAttendSwitch(long ATTENDID, int type);

        Task<DataTable> spUserAttendListForUser(DateTime billdate);
    }
}
