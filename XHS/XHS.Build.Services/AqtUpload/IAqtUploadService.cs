using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.AqtUpload
{
    public interface IAqtUploadService : IBaseServices<BaseEntity>
    {

        /// <summary>
        /// 省对接接口数据获取（2号文档）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListById();

        /// <summary>
        /// 省对接接口数据获取（2号文档）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListMinute();

        /// <summary>
        /// 苏州对接接口数据获取（2号文档）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListForSuzhou();

        /// <summary>
        /// 苏州对接接口数据获取（2号文档3.16）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListForSuzhouMinute();

        /// <summary>
        /// 省对接接口数据获取（3号文档）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetCityBelongtoList();

        /// <summary>
        /// 省对接接口数据获取（4号文档灌南）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListsForGuannan();

        /// <summary>
        /// 省对接接口分钟数据获取（4号文档阜宁）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListsForGuannanMinute();

        /// <summary>
        /// 省对接接口数据获取（4号文档如皋）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListsForRugao();

        /// <summary>
        /// 省对接接口分钟数据获取（4号文档如皋）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListsForRugaoMinute();

        /// <summary>
        /// 新吴区对接接口数据获取（4号文档）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListsForXinwuqu();

        /// <summary>
        /// 新吴区对接接口分钟数据获取（4号文档）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListsForXinwuquMinute();

        /// <summary>
        /// 省对接接口数据获取（4号文档阜宁）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListsForFuning();

        /// <summary>
        /// 省对接接口分钟数据获取（4号文档阜宁）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListsForFuningMinute();

        /// <summary>
        /// 获取分组别监督备案号
        /// </summary>
        /// <returns></returns>
        Task<DataTable> GetGroupSiteajcodes();

        /// <summary>
        /// 获取最近更新时间
        /// </summary>
        /// <returns></returns>
        Task<DataTable> GetLastCityUploadTime(string uploadurl, string post);

        /// <summary>
        /// 更新报警记录id
        /// </summary>
        /// <returns></returns>
        Task<int> UpdateWarnAlarmId(int warnId, string alarmId);


        /// <summary>
        /// 更新上传时间
        /// </summary>
        /// <returns></returns>
        Task<int> UpdateCityUploadDate(string uploadurl, string post, DateTime uploadtime);

        /// <summary>
        /// 上传信息看板
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<int> doUpdateBoard(object param);

        /// <summary>
        /// 3.11 上传检查单信息
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<int> doUpdateChecklist(object param);

        /// <summary>
        /// 3.12 上传移动巡检点
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<int> doUpdateCheckPoints(object param);

        /// <summary>
        /// 3.13 上传移动巡检信息
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<int> doUpdateMobileCheck(object param);

        /// <summary>
        /// 3.14 上传扬尘设备信息
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<int> doUpdateDustDeviceInfo(object param);



        /// <summary>
        /// 3.17 上传视频监控点信息
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<int> doUpdateUploadVideo(object param);


        /// <summary>
        /// 3.19  3.22  上传塔吊升降机基本信息
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<int> doUpdateUploadMachineryInfos(object param);



        /// <summary>
        /// 3.24 上传卸料平台基本信息
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<int> doUpdateUploadDeviceInfo(object param);

        /// <summary>
        /// 3.26 上传深基坑设备基本信息
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<int> doUpdateDeppPitDeviceInfo(object param);

        /// <summary>
        /// 3.28 上传高支模设备基本信息
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<int> doUpdateHighFormworkDeviceInfo(object param);

        /// <summary>
        /// 智慧工地对接智慧监管平台
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<DataSet> GetListsForNanjing();

        /// <summary>
        /// 智慧工地对接智慧监管平台  分钟
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListsForNanjingMinute();

        /// <summary>
        /// 插入项目自查数据
        /// </summary>
        /// <returns></returns>
        Task<int> doAqtSelfInspectCountInsert(List<SugarParameter> param);

        /// <summary>
        /// 插入项目月评总数
        /// </summary>
        /// <returns></returns>
        Task<int> doAqtMonthReviewCountInsert(List<SugarParameter> param);

        /// <summary>
        /// 插入项目每月自评状态及结果
        /// </summary>
        /// <returns></returns>
        Task<int> doAqtMonthReviewResultInsert(List<SugarParameter> param);

        /// <summary>
        /// 插入项目安标考评结果
        /// </summary>
        /// <returns></returns>
        Task<int> doAqtSafetyStandardResultInsert(List<SugarParameter> param);


        /// <summary>
        /// 插入项目数据
        /// </summary>
        /// <returns></returns>
        Task<int> doAqtProjectInsert(List<SugarParameter> param);

        /// <summary>
        /// 插入超危项目数据
        /// </summary>
        /// <returns></returns>
        Task<int> doAqtSuperDangerInsert(List<SugarParameter> param);

        /// 省对接接口数据获取（4号文档盐城）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListsForYancheng();

        /// 省对接接口实时数据获取（4号文档盐城）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListsForYanchengMinute();

        /// <summary>
        /// 获取视频信息对应GROUPID,SITEID
        /// </summary>
        /// <param name="recordNumber">安全监督备案号</param>
        /// <returns></returns>
        Task<GCSiteEntity> GetVideoInfo(string recordNumber);

        /// <summary>
        /// 插入视频信息数据
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<int> doAqtCameraInsert(SgParams sp);

        /// <summary>
        /// 更新视频信息数据
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<int> doAqtCameraUpdate(SgParams sp);

        /// <summary>
        /// 是否存在此条数据
        /// </summary>
        /// <param name="cameracode">表中videoId字段</param>
        /// <returns></returns>
        Task<bool> GetExists(string cameracode, int channel, int cameratype, int SITEID);

        /// <summary>
        /// 省对接接口数据获取（无锡）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListsForWuxi();

        /// <summary>
        /// 省对接接口数据获取分钟（无锡）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListsForWuxiMinute();

        /// <summary>
        /// 对接徐圩（小时）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListForXuwei();

        /// <summary>
        /// 对接徐圩（分钟）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListsForXuweiMinute();


        /// <summary>
        /// 获取更新安监通安监号、GROUPID、SITEID
        /// </summary>
        /// <returns></returns>
        Task<DataTable> GetAqtInspectSiteList();

        /// <summary>
        /// 更新安监通数据
        /// </summary>
        /// <param name="param">param</param>
        /// <returns></returns>
        Task<int> doAqtInspectSave(List<SugarParameter> param);

        /// <summary>
        /// 获取更新安监通区属编号、备案号、通知书编号、同名字段
        /// </summary>
        /// <returns></returns>
        Task<DataTable> GetAqtInspectNameListForUpdate();

        /// <summary>
        /// 安监通同步数据追加
        /// </summary>
        /// <param name="param">param</param>
        /// <returns></returns>
        Task<int> doAqtInspectNameUpdate(List<SugarParameter> param);

        /// <summary>
        /// 3.2.6 随手拍数据上传
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<int> doDownFourFreeToShoot(SgParams sp);

        /// <summary>
        /// 3.2.7 随手拍完成数据上传
        /// </summary>
        /// <param name="param">情报</param>
        /// <returns></returns>
        Task<int> doDownFourFreeToShootRectify(SgParams sp);

        /// <summary>
        /// 对接徐州（小时）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListForXuzhou();

        /// <summary>
        /// 对接徐州（分钟）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListForXuzhouMinute();

        /// <summary>
        /// 华瑞数据上传
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListForHuarui();

        /// <summary>
        /// 广联达数据上传
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListForGuanglianda();

        /// <summary>
        /// 对接新合盛
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListForXinhesheng();

        /// <summary>
        /// 修改数据推送跟新时间
        /// </summary>
        /// <param name="month">加减月份</param>
        /// <param name="uploadumonthrl">推送地址</param>
        /// <returns></returns>
        Task<int> UpdateMonthDate(int month, string uploadurl);

        /// <summary>
        /// 奥途数据上传（徐州中骏东湖上景）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListForAuto();

        /// <summary>
        /// 华润数据上传（江阴2020-C-19项目）
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListForHuarun();

       /// <summary>
       /// 方洋车冲上传盐城
       /// </summary>
       /// <param name="datetime">推送时间</param>
       /// <param name="uploadurl">地址</param>
       /// <returns></returns>
        Task<DataTable> GetListForCarUndeveloped(string datetime,string uploadurl);

        Task<DataSet> GetListForYancheng();

        /// <summary>
        /// 仪征智慧工地数据上传
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetListsForYiZheng();
    }
}
