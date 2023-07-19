using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Services.Site
{
    public class SiteService : BaseServices<GCSiteEntity>, ISiteService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<GCSiteEntity> _siteRepository;
        private readonly IBaseRepository<GCSiteCompanyEntity> _siteCompanyRepository;
        private readonly IBaseRepository<InspectionEntity> _InspectionRepository;
        private readonly IHpSystemSetting _hpSystemSetting;

        public SiteService(IUser user, IBaseRepository<GCSiteEntity> siteRepository,
            IBaseRepository<InspectionEntity> InspectionRepository, IHpSystemSetting hpSystemSetting,
            IBaseRepository<GCSiteCompanyEntity> siteCompanyRepository, IUnitOfWork unitOfWork)
        {
            _user = user;
            _siteRepository = siteRepository;
            _siteCompanyRepository = siteCompanyRepository;
            BaseDal = siteRepository;
            _InspectionRepository = InspectionRepository;
            _hpSystemSetting = hpSystemSetting;

        }

        public async Task<DataTable> get24HoursData(int SITEID)
        {
            return await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSite24HoursDataList", new { SITEID = SITEID });
        }

        public async Task<DataSet> getPhaseCount()
        {
            return await _siteRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spSitePhaseCount", new { USERID = _user.Id });
        }

        public async Task<DataTable> get60MinutesData(int SITEID)
        {
            return await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSite60MinutesDataList", new { SITEID = SITEID });
        }

        public async Task<DataTable> getAsignListForApp(int SITEID)
        {
            return await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteUserListForApp", new { SITEID = SITEID });
        }

        public async Task<DataTable> getListByDeviceType(int linktype)
        {
            return await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteListByDeviceType", new { USERID = _user.Id, linktype = linktype });
        }


        public async Task<DataTable> getListByMenuId(string MENUID)
        {
            return await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteListByMenuId", new { USERID = _user.Id, MENUID = MENUID });
        }

        public async Task<DataTable> getListForMobile()
        {
            return await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteListForMobile", new { USERID = _user.Id });
        }

        public async Task<DataTable> getChildListForMobile()
        {
            return await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteChildListForMobile", new { USERID = _user.Id });
        }
        public async Task<DataTable> getChildListById(int SITEID)
        {
            return await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteSubList", new { SITEID = SITEID });
        }
        public async Task<DataRow> getOneApp(int SITEID)
        {
            DataTable dt = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteGetApp", new { SITEID = SITEID });
            if (dt == null || dt.Rows.Count == 0)
                return null;

            return dt.Rows[0];
        }

        public async Task<DataRow> getOneByRecordNumber(string recordNumber)
        {
            DataTable dt = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteGetByRecordNumber", new { recordNumber = recordNumber });
            if (dt == null || dt.Rows.Count == 0)
                return null;

            return dt.Rows[0];
        }

        public async Task<DataRow> getSitefenceCount(int SITEID)
        {
            DataTable dt = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSitefenceWarnCount", new { SITEID = SITEID });
            if (dt == null || dt.Rows.Count == 0)
                return null;

            return dt.Rows[0];
        }


        public async Task<DataRow> getSiteCraneAlarmCount(int SITEID, string deviceId)
        {
            DataTable dt = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteCraneAlarmCount", new { SITEID = SITEID, deviceId = deviceId });
            if (dt == null || dt.Rows.Count == 0)
                return null;

            return dt.Rows[0];
        }

        public async Task<DataRow> getSiteElevatorAlarmCount(int SITEID, string deviceId)
        {
            DataTable dt = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteElevatorAlarmCount", new { SITEID = SITEID, deviceId = deviceId });
            if (dt == null || dt.Rows.Count == 0)
                return null;

            return dt.Rows[0];
        }

        public async Task<DataRow> getSiteUploadAlarmCount(int SITEID, string deviceId)
        {
            DataTable dt = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteUploadAlarmCount", new { SITEID = SITEID, deviceId = deviceId });
            if (dt == null || dt.Rows.Count == 0)
                return null;

            return dt.Rows[0];
        }
        public async Task<DataTable> getMenu(int SITEID)
        {
            return await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteMenu", new { SITEID = SITEID });
        }

        public async Task<List<BnSiteFly>> getFlyList(DateTime operatedate)
        {
            DataTable dt = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteFlyList", new { USERID = _user.Id, operatedate = operatedate });

            if (dt == null || dt.Rows.Count.Equals(0))
                return null;
            List<BnSiteFly> result = new List<BnSiteFly>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                BnSiteFly bc = new BnSiteFly();
                bc.FLYID = Convert.ToInt32(dt.Rows[i]["FLYID"]);
                bc.SITEID = Convert.ToInt32(dt.Rows[i]["SITEID"]);
                bc.sitename = Convert.ToString(dt.Rows[i]["sitename"]);
                bc.flydate = Convert.ToDateTime(dt.Rows[i]["flydate"]);
                bc.bdel = Convert.ToInt16(dt.Rows[i]["bdel"]);
                bc.operatedate = Convert.ToDateTime(dt.Rows[i]["operatedate"]);

                result.Add(bc);
            }
            return result;
        }



        public async Task<DataTable> getTruckList(int SITEID)
        {
            return await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteTruckList", new { SITEID = SITEID });
        }

        /// <summary>
        /// 接口获取监测点
        /// </summary>
        /// <param name="GROUPID">分组ID</param>
        /// <param name="index">页码</param>
        /// <param name="size">每页件数</param>
        /// <returns></returns>
        public async Task<DataTable> getListForApi(int GROUPID, int index, int size)
        {
            DataTable result = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteListForApi", new { GROUPID = GROUPID, index = index, size = size });
            return result;
        }

        /// <summary>
        /// 根据用户ID获取监测对象
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <returns></returns>
        public async Task<DataTable> getListByUserId(int userid)
        {
            DataTable result = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteListForUser", new { USERID = userid });
            return result;
        }

        /// <summary>
        /// 根据用户ID获取监测对象
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <returns></returns>
        public async Task<DataTable> getV2ListByUserId(int userid)
        {
            DataTable result = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2SiteListForUser", new { USERID = userid });
            return result;
        }

        /// <summary>
        /// 获取监测对象详情
        /// </summary>
        /// <param name="SITEID">用户ID</param>
        /// <returns></returns>
        public async Task<DataTable> getV2SiteInfo(int SITEID)
        {
            DataTable result = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2SiteInfo", new { SITEID = SITEID });
            return result;
        }

        /// <summary>
        /// 获取监测对象五方单位信息
        /// </summary>
        /// <param name="SITEID">用户ID</param>
        /// <returns></returns>
        public async Task<DataTable> getV2SiteFiveParts(int SITEID)
        {
            DataTable result = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2SiteFiveParts", new { SITEID = SITEID });
            return result;
        }

        /// <summary>
        /// 接口获取监测对象下的用户
        /// </summary>
        /// <param name="SITEID">监测对象ID</param>
        /// <returns></returns>
        public async Task<DataTable> getUserForApi(int SITEID)
        {
            DataTable result = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteUserListForApi", new { SITEID = SITEID });
            return result;

        }

        #region SiteDoc
        public async Task<DataSet> getSiteDocList(int SITEID)
        {
            return await _siteRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spSiteDocList", new { SITEID = SITEID });
        }

        public async Task<string> doInsertSiteDoc(object param)
        {
            return await _siteRepository.Db.Ado.UseStoredProcedure().GetStringAsync("spSiteDocInsert", param);
        }

        public async Task<DataRow> doDeleteSiteDoc(string SITEDOCID)
        {
            DataTable dt = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteDocDelete", new { SITEDOCID = SITEDOCID });
            if (dt == null || dt.Rows.Count == 0)
                return null;

            return dt.Rows[0];
        }

        #endregion

        /// <summary>
        /// 插入数据 
        /// </summary>
        /// <returns>返回siteid</returns>
        public async Task<int> Insert(GCSiteEntity entity)
        {
            return await _siteRepository.Db.Insertable(entity).ExecuteReturnIdentityAsync();
        }

        public async Task<bool> QuerySiteExist(Expression<Func<GCSiteEntity, bool>> whereExpression)
        {
            return await _siteRepository.Db.Queryable<GCSiteEntity>().Where(whereExpression).Select(it => it.SITEID).CountAsync() > 0;
        }

        public async Task<GCSiteSummaryEntity> GetSummary(int groupid, int siteid)
        {
            return await _siteRepository.Db.Queryable<GCSiteSummaryEntity>().Where(a => a.SITEID == siteid && a.GROUPID == groupid).SingleAsync();
        }

        public async Task<int> SaveSummary(GCSiteSummaryEntity entity)
        {
            var site = await _siteRepository.Db.Queryable<GCSiteSummaryEntity>()
                .Where(a => a.SITEID == entity.SITEID && a.GROUPID == entity.GROUPID).SingleAsync();
            if (site == null)
            {
                return await _siteRepository.Db.Insertable(entity).ExecuteCommandAsync();
            }
            else
            {
                return await _siteRepository.Db.Updateable(entity).ExecuteCommandAsync();
            }
        }

        public async Task<int> doDySiteUpdate(List<SugarParameter> param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _siteRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDySiteUpdate", ps);
            return output.Value.ObjToInt();
        }

        public async Task<int> doDySiteLiftData(List<SugarParameter> param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _siteRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDyLiftRtdInsert", ps);
            return output.Value.ObjToInt();
        }

        public async Task<int> doDySitePmData(List<SugarParameter> param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _siteRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDyPmUpdate", ps);
            return output.Value.ObjToInt();
        }

        public async Task<int> doSiteDangerDelete(List<SugarParameter> param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _siteRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spSiteDangerDelete", ps);
            return output.Value.ObjToInt();
        }

        public async Task<int> doSiteDangerInsert(List<SugarParameter> param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _siteRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spSiteDangerInsert", ps);
            return output.Value.ObjToInt();
        }
        public async Task<DataTable> GetDySiteList()
        {
            DataTable result = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spDySiteList");
            return result;
        }

        /// <summary>
        /// 大运接口更新时间列表
        /// </summary>
        /// <param name="url">监测对象ID</param>
        /// <returns></returns>
        public async Task<DataTable> GetDyUpdatedate(string uploadurl)
        {
            DataTable result = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spGetCityUploadDate", new { uploadurl = uploadurl });
            return result;

        }

        public async Task<List<SiteDocOutPut>> GetPic(int siteid)
        {
            string domain = _hpSystemSetting.getSettingValue(Const.Setting.S034);
            string folder = _hpSystemSetting.getSettingValue(Const.Setting.S058);

            return await _siteRepository.Db.Queryable<GCSiteDoc>()
                .Where(ii => ii.SITEID == siteid)
                .Select(ii => new SiteDocOutPut
                {
                    url = "http://" + domain + "/" + folder + "/" + ii.SITEID.ToString() + "/" + ii.SITEDOCID.ToString()
                    + "/" + ii.filename,
                    filetype = ii.filetype,
                    name = ii.filename
                })
                .ToListAsync();
        }


        public async Task<int> AddSiteDoc(List<GCSiteDoc> inputs)
        {
            return await _siteRepository.Db.Insertable(inputs).ExecuteCommandAsync();
        }


        public async Task<int> DeleteSiteDoc(int SITEID, string[] SITEDOCID)
        {
            return await _siteRepository.Db.Deleteable<GCSiteDoc>()
                .Where(ii => ii.SITEID == SITEID && SITEDOCID.Contains(ii.SITEDOCID.ToString()))
                .ExecuteCommandAsync();
        }

        /// <summary>
        /// 第三方绑定保存
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> doSaveBind(GCSiteBindEntity entity)
        {
            return await _siteRepository.Db.Updateable(entity).ExecuteCommandAsync();
        }

        /// <summary>
        /// 关闭项目
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<int> doSiteComplete(int SITEID, string username)
        {
            //return await _siteRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spSiteComplete", new { SITEID = SITEID, username = username });
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _siteRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spSiteComplete",
                new SugarParameter("@SITEID", SITEID),
                new SugarParameter("@username", username),
                returnvalue);
            return returnvalue.Value.ObjToInt();
        }

        /// <summary>
        /// 移除项目
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<int> doSiteRemove(int SITEID, string username)
        {
            //return await _siteRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spSiteDataRemove", new { SITEID = SITEID, username = username });
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _siteRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spSiteDataRemove",
                new SugarParameter("@SITEID", SITEID),
                new SugarParameter("@username", username),
                returnvalue);
            return returnvalue.Value.ObjToInt();
        }

        /// <summary>
        /// 获取工地和五方
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        public async Task<GCSiteAndCompanyEntity> getSiteCompany(int SITEID)
        {
            //动态考核地址
            var site = await _siteRepository.Db.Queryable<GCSiteEntity>().Where(a => a.SITEID == SITEID).SingleAsync();
            site.url= _hpSystemSetting.getSettingValue(Const.Setting.S174);
            var company = await _siteRepository.Db.Queryable<GCSiteCompanyEntity>().Where(a => a.SITEID == SITEID).ToListAsync();

            GCSiteAndCompanyEntity result = new GCSiteAndCompanyEntity();
            var sourceType = site.GetType();
            var destType = result.GetType();
            foreach (var source in sourceType.GetProperties())
            {
                foreach (var dest in destType.GetProperties())
                {
                    if (dest.Name == source.Name)
                    {
                        dest.SetValue(result, source.GetValue(site));
                    }
                }
            }
            result.sitecompany = company;

            return result;

        }

        /// <summary>
        /// 更新监测对象信息
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        public async Task<int> doSiteCompanyUpdate(GCSiteAndCompanyEntity siteandcompany)
        {
            //保存site实体
            var site = new GCSiteEntity();
            var sourceType = siteandcompany.GetType();
            var destType = site.GetType();
            foreach (var source in sourceType.GetProperties())
            {
                foreach (var dest in destType.GetProperties())
                {
                    if (dest.Name == source.Name)
                    {
                        dest.SetValue(site, source.GetValue(siteandcompany));
                    }
                }
            }
            try
            {

                var result = await _siteRepository.Db.Updateable(site).ExecuteCommandAsync();

                //保存五方实体
                foreach (var sc in siteandcompany.sitecompany)
                {
                    if (sc.SCID > 0)
                    {
                        if (!string.IsNullOrEmpty(sc.companyname.Trim())
                            || !string.IsNullOrEmpty(sc.companycode.Trim())
                            || !string.IsNullOrEmpty(sc.companycontact.Trim())
                            || !string.IsNullOrEmpty(sc.companytel.Trim()))
                        {
                            //更新 
                            await _siteCompanyRepository.Db.Updateable(sc).ExecuteCommandAsync();
                        }
                        else
                        {//删除
                            await _siteCompanyRepository.Db.Deleteable(sc).ExecuteCommandAsync();
                        }
                    }
                    else//插入
                    {
                        await _siteCompanyRepository.Db.Insertable(sc).ExecuteCommandAsync();
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// 插入监测对象实体及五方实体
        /// </summary>
        /// <param name="siteandcompany"></param>
        /// <returns></returns>
        public async Task<int> doSiteCompanyInsert(GCSiteAndCompanyEntity siteandcompany)
        {
            //保存site实体
            var site = new GCSiteEntity();
            var sourceType = siteandcompany.GetType();
            var destType = site.GetType();
            foreach (var source in sourceType.GetProperties())
            {
                foreach (var dest in destType.GetProperties())
                {
                    if (dest.Name == source.Name)
                    {
                        dest.SetValue(site, source.GetValue(siteandcompany));
                    }
                }
            }
            try
            {

                var SITEID = _siteRepository.Db.Insertable(site).ExecuteReturnIdentity();

                if (SITEID <= 0)
                {
                    return 0;
                }

                site.SITEID = SITEID;
                site.PARENTSITEID = SITEID;
                var result = await _siteRepository.Db.Updateable(site)
                    .UpdateColumns(p => new { p.PARENTSITEID })
                    .WhereColumns(t => new { t.SITEID })
                    .ExecuteCommandAsync();

                if (result <= 0)
                {
                    await _siteRepository.Db.Deleteable(site).ExecuteCommandAsync();
                    return 0;
                }

                //保存五方实体
                List<GCSiteCompanyEntity> companys = new List<GCSiteCompanyEntity>();
                for (int i = 0; i < siteandcompany.sitecompany.Count; i++)
                {
                    if (!string.IsNullOrEmpty(siteandcompany.sitecompany[i].companyname.Trim())
                            || !string.IsNullOrEmpty(siteandcompany.sitecompany[i].companycode.Trim())
                            || !string.IsNullOrEmpty(siteandcompany.sitecompany[i].companycontact.Trim())
                            || !string.IsNullOrEmpty(siteandcompany.sitecompany[i].companytel.Trim()))
                    {
                        siteandcompany.sitecompany[i].SITEID = SITEID;
                        siteandcompany.sitecompany[i].GROUPID = site.GROUPID;
                        companys.Add(siteandcompany.sitecompany[i]);
                    }
                }

                await _siteCompanyRepository.Db.Insertable(companys).ExecuteCommandAsync();

                return SITEID;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        ///  获取监测点臭氧最近信息
        /// </summary>
        /// <param name="SITEID">监测对象ID</param>
        /// <returns></returns>
        public async Task<DataSet> getSiteO3Latest(int SITEID)
        {
            DataSet result = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spSiteO3Latest", new { SITEID = SITEID });
            return result;

        }

        /// <summary>
        ///  获取监测点臭氧图表信息
        /// </summary>
        /// <param name="SITEID">监测对象ID</param>
        /// <returns></returns>
        public async Task<DataSet> getSiteO3Chart(int SITEID)
        {
            DataSet result = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spSiteO3Chart", new { SITEID = SITEID });
            return result;

        }

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
        public async Task<DataTable> getSiteO3His(int SITEID, int datatype, DateTime startdate, DateTime enddate, int pageIndex, int pageSize)
        {
            DataTable result = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteO3His", new { SITEID = SITEID, datatype = datatype, startdate = startdate, enddate = enddate, pageIndex = pageIndex, pageSize = pageSize });
            return result;

        }

        /// <summary>
        /// 获取单个工地的负责人列表（勾和没勾都获取）
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        public async Task<DataTable> getSiteUserList(int SITEID)
        {
            DataTable dt = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spTwoSiteUserList", new { SITEID = SITEID });

            return dt;
        }

        /// <summary>
        /// 保存单个工地的负责人
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> saveSiteUser(SiteUserInput input)
        {
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);

            var param = new List<SugarParameter>();

            foreach (PropertyInfo p in input.GetType().GetProperties())
            {
                param.Add(new SugarParameter("@" + p.Name, p.GetValue(input)));
            }
            param.Add(returnvalue);

            await _siteRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spTwoSiteUserRegist", param);
            return returnvalue.Value.ObjToInt();
        }

        /// <summary>
        /// 保存单个工地的负责人
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> saveSite(SiteDto input)
        {
            var returnvalue = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);

            var param = new List<SugarParameter>();

            input.siteajcode = input.recordNumber;
            foreach (PropertyInfo p in input.GetType().GetProperties())
            {
                if(p.Name == "recordNumber")
                {
                    continue;
                }
                param.Add(new SugarParameter("@" + p.Name, p.GetValue(input)));
            }
            param.Add(returnvalue);
            await _siteRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spSiteInsertFromAnalyse", param);
            return returnvalue.Value.ObjToInt();
        }

        /// <summary>
        /// 获取安监通里有，site里没有的项目列表
        /// </summary>
        /// <returns></returns>
        public async Task<DataTable> GetSiteUninput()
        {
            DataTable result = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spTwoUninputSiteList");
            return result;

        }

        /// <summary>
        /// 获取安监通项目信息
        /// </summary>
        /// <param name="siteajcode"></param>
        /// <returns></returns>
        public async Task<DataTable> GetAqtProj(string siteajcode)
        {
            DataTable result = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spTwoAqtProjGet", new { siteajcode = siteajcode });
            return result;

        }

        public async Task<DataTable> spV2CommonMap(int type, int SITEID)
        {
            return await _siteRepository.Db.Ado.UseStoredProcedure()
               .GetDataTableAsync("spV2CommonMap",
               new
               {
                   type = type,
                   SITEID = SITEID
               });
        }

        /// <summary>
        /// 获取未绑定扬尘的监测对象列表
        /// </summary>
        /// <param name="GROUPID"></param>
        /// <returns></returns>
        public async Task<DataTable> GetSiteNoDevice(int GROUPID)
        {
            DataTable result = await _siteRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spTwoNoDeviceSiteList", new { GROUPID = GROUPID });
            return result;

        }
    }
}
