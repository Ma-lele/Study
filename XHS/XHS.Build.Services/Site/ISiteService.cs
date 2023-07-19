using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Site
{
   public  interface ISiteService:IBaseServices<GCSiteEntity>
    {
        /// <summary>
        /// 获取该监测对象的负责人
        /// </summary>
        /// <param name="SITEID">监测对象ID</param>
        /// <returns></returns>
        Task<DataTable> getAsignListForApp(int SITEID);

        /// <summary>
        /// 获取用户所属组下的工地进度统计
        /// </summary>
        /// <returns></returns>
        Task<DataSet> getPhaseCount();

        /// <summary>
        /// 获取用户所属组下的监测对象
        /// </summary>
        /// <returns></returns>
        Task<DataTable> getListForMobile();

        /// <summary>
        /// 获取用户所属组下的子监测对象
        /// </summary>
        /// <returns></returns>
        Task<DataTable> getChildListForMobile();

        /// <summary>
        /// 获取监测对象的子监测对象
        /// </summary>
        /// <returns></returns>
        Task<DataTable> getChildListById(int SITEID);

        /// <summary>
        /// 获取监测点信息
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <returns></returns>
        Task<DataRow> getOneApp(int SITEID);

        /// <summary>
        /// 获得一段时间内的临边防护报警次数
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <returns></returns>
        Task<DataRow> getSitefenceCount(int SITEID);


        /// <summary>
        /// 获得塔吊的预警、报警次数
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <returns></returns>
        Task<DataRow> getSiteCraneAlarmCount(int SITEID,string deviceId);

        /// <summary>
        /// 获得施工升降机的预警、报警次数
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <returns></returns>
        Task<DataRow> getSiteElevatorAlarmCount(int SITEID, string deviceId);

        /// <summary>
        /// 获得卸料平台的预警、报警次数
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <returns></returns>
        Task<DataRow> getSiteUploadAlarmCount(int SITEID, string deviceId);

        /// <summary>
        /// 获取监测点信息
        /// </summary>
        /// <param name="recordNumber">监督备案号</param>
        /// <returns></returns>
        Task<DataRow> getOneByRecordNumber(string recordNumber);

        /// <summary>
        /// 获取监测点设备绑定情况
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <returns></returns>
        Task<DataTable> getMenu(int SITEID);

        /// <summary>
        /// 获取监测对象24小时内的主要污染数据
        /// </summary>
        /// <param name="SITEID">监测对象ID</param>
        /// <returns></returns>
        Task<DataTable> get24HoursData(int SITEID);

        /// <summary>
        /// 获取监测对象近60分钟内的主要污染数据
        /// </summary>
        /// <param name="SITEID">监测对象ID</param>
        /// <returns></returns>
        Task<DataTable> get60MinutesData(int SITEID);


        /// <summary>
        /// 根据设备种类获取监测对象
        /// </summary>
        /// <param name="linktype">设备种类</param>
        /// <returns></returns>
        Task<DataTable> getListByDeviceType(int linktype);


        /// <summary>
        /// 根据功能ID获取监测对象
        /// </summary>
        /// <param name="MENUID">功能ID</param>
        /// <returns></returns>
        Task<DataTable> getListByMenuId(string MENUID);

        /// <summary>
        /// 检索监测对象
        /// <param name="operatedate">最后更新时间</param>
        /// <returns>结果集</returns>
        Task<List<BnSiteFly>> getFlyList(DateTime operatedate);

        /// <summary>
        /// 获取监测点的渣土车白名单
        /// </summary>
        /// <param name="SITEID">监测点编号</param>
        /// <returns>结果集</returns>
        Task<DataTable> getTruckList(int SITEID);


        Task<DataTable> getListForApi(int GROUPID, int index, int size);
        Task<DataTable> getUserForApi(int SITEID);

        #region SiteDoc
        /// <summary>
        /// 获取监测点文件列表
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <returns>监测点数据集</returns>
        Task<DataSet> getSiteDocList(int SITEID);

        /// <summary>
        /// 插入监测点文件
        /// </summary>
        /// <param name="param">监测点文件信息</param>
        /// <returns>设置结果</returns>
        Task<string> doInsertSiteDoc(object param);

        /// <summary>
        /// 删除监测点文件
        /// </summary>
        /// <param name="SITEDOCID">监测点文件ID</param>
        /// <returns>结果</returns>
        Task<DataRow> doDeleteSiteDoc(string SITEDOCID);
        #endregion
        /// <summary>
        /// 插入数据 
        /// </summary>
        /// <returns>返回siteid</returns>
        Task<int> Insert(GCSiteEntity entity);

        Task<bool> QuerySiteExist(Expression<Func<GCSiteEntity, bool>> whereExpression);

        /// <summary>
        /// 获取监测对象简介
        /// </summary>
        /// <param name="groupid">分组编号</param>
        /// <param name="siteid">项目编号</param>
        /// <returns></returns>
        Task<GCSiteSummaryEntity> GetSummary(int groupid, int siteid);

        /// <summary>
        /// 保存监测对象简介
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns>结果</returns>
        Task<int> SaveSummary(GCSiteSummaryEntity entity);


        /// <summary>
        /// 根据用户ID获取监测对象
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <returns></returns>
        Task<DataTable> getListByUserId(int userid);


        /// <summary>
        /// 根据用户ID获取监测对象
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <returns></returns>
        Task<DataTable> getV2ListByUserId(int userid);


        /// <summary>
        /// 获取监测对象详情
        /// </summary>
        /// <param name="SITEID">用户ID</param>
        /// <returns></returns>
        Task<DataTable> getV2SiteInfo(int SITEID);

        /// <summary>
        /// 获取监测对象五方单位信息
        /// </summary>
        /// <param name="SITEID">用户ID</param>
        /// <returns></returns>
        Task<DataTable> getV2SiteFiveParts(int SITEID);

        /// <summary>
        /// 插入工地基本数据
        /// </summary>
        /// <param name="param">工地数据</param>
        /// <returns></returns>
        Task<int> doDySiteUpdate(List<SugarParameter> param);

        /// <summary>
        /// 插入工地升降机数据
        /// </summary>
        /// <param name="param">升降机数据</param>
        /// <returns></returns>
        Task<int> doDySiteLiftData(List<SugarParameter> param);

        /// <summary>
        /// 插入工地扬尘数据
        /// </summary>
        /// <param name="param">扬尘数据</param>
        /// <returns></returns>
        Task<int> doDySitePmData(List<SugarParameter> param);


        /// <summary>
        /// 删除危大工程基本信息
        /// </summary>
        /// <param name="param">升降机数据</param>
        /// <returns></returns>
        Task<int> doSiteDangerDelete(List<SugarParameter> param);


        /// <summary>
        /// 插入危大工程基本信息
        /// </summary>
        /// <param name="param">升降机数据</param>
        /// <returns></returns>
        Task<int> doSiteDangerInsert(List<SugarParameter> param);

        /// <summary>
        /// 大运项目列表
        /// </summary>
        /// <returns></returns>
        Task<DataTable> GetDySiteList();

        /// <summary>
        /// 大运接口更新时间列表
        /// </summary>
        /// <returns></returns>
        Task<DataTable> GetDyUpdatedate(string uploadurl);


        /// <summary>
        /// 获取监测对象图片
        /// </summary>
        /// <param name="siteid"></param>
        /// <returns></returns>
        Task<List<SiteDocOutPut>> GetPic(int siteid);


        /// <summary>
        /// 新增监测对象图片
        /// </summary>
        /// <param name="inputs"></param>
        /// <returns></returns>
        Task<int> AddSiteDoc(List<GCSiteDoc> inputs);


        /// <summary>
        /// 删除监测对象图片
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="SITEDOCID"></param>
        /// <returns></returns>
        Task<int> DeleteSiteDoc(int SITEID, string[] SITEDOCID);

        /// <summary>
        /// 第三方绑定保存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<int> doSaveBind(GCSiteBindEntity entity);

        /// <summary>
        /// 关闭项目
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<int> doSiteComplete(int SITEID, string username);

        /// <summary>
        /// 移除项目
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<int> doSiteRemove(int SITEID, string username);
        /// <summary>
        /// 获取工地和五方
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        Task<GCSiteAndCompanyEntity> getSiteCompany(int SITEID);
        /// <summary>
        /// 更新监测对象信息
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        Task<int> doSiteCompanyUpdate(GCSiteAndCompanyEntity site);
        /// <summary>
        /// 插入监测对象信息及五方信息
        /// </summary>
        /// <param name="siteandcompany"></param>
        /// <returns></returns>
        Task<int> doSiteCompanyInsert(GCSiteAndCompanyEntity siteandcompany);


        /// <summary>
        /// 获取监测点臭氧最近信息
        /// </summary>
        /// <param name="SITEID">监测点编号</param>
        /// <returns>结果集</returns>
        Task<DataSet> getSiteO3Latest(int SITEID);

        /// <summary>
        /// 获取监测点臭氧图表信息
        /// </summary>
        /// <param name="SITEID">监测点编号</param>
        /// <returns>结果集</returns>
        Task<DataSet> getSiteO3Chart(int SITEID);


        /// <summary>
        /// 获取监测点臭氧历史数据
        /// </summary>
        /// <param name="SITEID">监测点编号</param>
        /// <param name="datatype">1：分钟 2：小时 3：日均</param>
        /// <param name="startdate">开始时间</param>
        /// <param name="enddate">结束时间</param>
        /// <param name="pageIndex">页索引</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>结果集</returns>
        Task<DataTable> getSiteO3His(int SITEID, int datatype, DateTime startdate, DateTime enddate, int pageIndex, int pageSize);

        /// <summary>
        /// 获取单个工地的负责人列表（勾和没勾都获取）
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        Task<DataTable> getSiteUserList(int SITEID);
        /// <summary>
        /// 保存单个工地的负责人
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> saveSiteUser(SiteUserInput input);

        /// <summary>
        /// 保存单个工地
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<int> saveSite(SiteDto input);

        /// <summary>
        /// 获取安监通里有，site里没有的项目列表
        /// </summary>
        /// <returns></returns>
        Task<DataTable> GetSiteUninput();

        /// <summary>
        /// 获取安监通项目信息
        /// </summary>
        /// <param name="siteajcode"></param>
        /// <returns></returns>
        Task<DataTable> GetAqtProj(string siteajcode);

        /// <summary>
        /// 设备地图
        /// </summary>
        /// <param name="type">1 扬尘，2 摄像头，3 塔吊，4 升降机，5 卸料平台，6 深基坑，7 高支模，8 临边防护,9 实名制</param>
        /// <param name="SITEID">siteid</param>
        /// <returns></returns>
        Task<DataTable> spV2CommonMap(int type, int SITEID);

        /// <summary>
        /// 获取未绑定扬尘的监测对象列表
        /// </summary>
        /// <param name="GROUPID"></param>
        /// <returns></returns>
        Task<DataTable> GetSiteNoDevice(int GROUPID);
    }
}
