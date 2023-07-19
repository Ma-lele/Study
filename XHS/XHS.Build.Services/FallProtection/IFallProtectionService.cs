using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.FallProtection
{
    public interface IFallProtectionService : IBaseServices<BaseEntity>
    {
        Task<List<CCSystemSettingEntity>> GetPushUrl();

        //  Task<int> SetPushUrl(List<CCSystemSettingEntity> input);

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PageOutput<FallProtectionDeviceDto>> GetPageListAsync(int groupid, string keyword, int page, int size);


        /// <summary>
        /// 分组count
        /// </summary>
        /// <returns></returns>
        Task<List<GroupHelmetBeaconCount>> GetGroupCount();


        /// <summary>
        /// 新增设备
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<bool> AddDevice(GCFallProtectionDevice input);


        /// <summary>
        /// 检查编号是否重复
        /// </summary>
        /// <param name="code"></param>
        /// <param name="FPDID"></param>
        /// <returns></returns>
        Task<bool> CheckCode(string code, int FPDID);


        /// <summary>
        /// 编辑设备
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<bool> EditDevice(GCFallProtectionDevice input);


        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="FPDID"></param>
        /// <param name="operator"></param>
        /// <returns></returns>
        Task<bool> DeleteDevice(int FPDID, string @operator);


        /// <summary>
        /// 查询单台设备
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        Task<GCFallProtectionDevice> FindByCode(string deviceId);


        /// <summary>
        /// 更新设备状态
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<bool> SetDeviceStatus(GCFallProtectionDevice input);


        /// <summary>
        /// 更新报警id
        /// </summary>
        /// <param name="FPDID"></param>
        /// <param name="alarmId"></param>
        /// <returns></returns>
        Task<bool> SetAlarm(int FPDID, string alarmId);


        /// <summary>
        /// 查找并设置离线设备状态(50小时未推送数据)
        /// </summary>
        /// <returns></returns>
        Task<int> FindnSetOffline();


        /// <summary>
        /// 智慧工地2.0项目端-临边防护-左上角统计
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        Task<DataSet> spV2FallProStats(int SITEID);


        /// <summary>
        /// 智慧工地2.0项目端-临边防护-告警类型统计分析
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        Task<DataTable> spV2FallProWarnType(int SITEID, DateTime startTime, DateTime endTime);


        /// <summary>
        /// 智慧工地2.0项目端-临边防护-设备统计
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <param name="keyword">关键字</param>
        /// <param name="online">在线状态 -1:全部  0:离线  1:在线</param>
        /// <param name="alarm">报警状态 -1:全部  0:正常  1:异常</param>
        /// <returns></returns>
        Task<DataTable> spV2FallProDevStats(int SITEID, string keyword, int online, int alarm);
    }
}
