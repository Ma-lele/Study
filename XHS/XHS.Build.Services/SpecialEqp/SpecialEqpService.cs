using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Cache;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Util;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;
using System.Linq;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.SystemSetting;
using XHS.Build.Common;

namespace XHS.Build.Services.SpecialEqp
{
    public class SpecialEqpService : BaseServices<GCSpecialEqpEntity>, ISpecialEqpService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<GCSpecialEqpEntity> _baseRepository;
        private readonly IConfiguration _configuration;
        private readonly ICache _cache;
        private static readonly Mutex mutex = new Mutex();
        private readonly ILogger<SpecialEqpService> _logger;
        private readonly IHpSystemSetting _hpSystemSetting;

        public SpecialEqpService(IUser user, IBaseRepository<GCSpecialEqpEntity> baseRepository, IConfiguration configuration,
            ICache cache, ILogger<SpecialEqpService> logger,  IHpSystemSetting hpSystemSetting)
        {
            _user = user;
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
            _configuration = configuration;
            _cache = cache;
            _logger = logger;
            _hpSystemSetting = hpSystemSetting;
        }

        public async Task<List<GroupHelmetBeaconCount>> GetGroupCount()
        {
            return await _baseRepository.Db.Ado.SqlQueryAsync<GroupHelmetBeaconCount>("select G.GROUPID,G.groupshortname,isnull(A.count,0) as count from T_GC_Group G LEFT JOIN (SELECT S.GROUPID, count(1) as count from T_GC_SpecialEqp D  LEFT JOIN T_GC_Site S ON D.SITEID = S.SITEID GROUP BY S.GROUPID) A on A.GROUPID = G.GROUPID where G.status = 0  ORDER BY isnull(A.count,0) desc,G.city, G.district, G.GROUPID");
        }

        public async Task<PageOutput<SpecialEqpListOutput>> GetSiteSpecialEqpPageList(int groupid, string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _baseRepository.Db.Queryable<GCSpecialEqpEntity, GCSpecialEqpTypeEntity, GCSiteEntity>((p, t, s) => new JoinQueryInfos(JoinType.Left, p.SETYPEID == t.SETYPEID, JoinType.Left, p.SITEID == s.SITEID))
                .WhereIF(groupid > 0, (p, t, s) => s.GROUPID == groupid)
                .WhereIF(!string.IsNullOrEmpty(keyword), (p, t, s) => s.siteshortname.Contains(keyword) || p.secode.Contains(keyword) || p.sename.Contains(keyword))
                .Select((p, t, s) => new SpecialEqpListOutput()
                {
                    alarmdate = p.alarmdate,
                    alarmstate = p.alarmstate,
                    bdel = p.bdel,
                    checkouttime = p.checkouttime,
                    hasreport = p.hasreport,
                    lastcheckday = p.lastcheckday,
                    liftovercode = p.liftovercode,
                    maker = p.maker,
                    operatedate = p.operatedate,
                    @operator = p.@operator,
                    paramjson = p.paramjson,
                    productdate = p.productdate,
                    remark = p.remark,
                    secode = p.secode,
                    SEID = p.SEID,
                    sename = p.sename,
                    sestatus = p.sestatus,
                    setype = p.setype,
                    SETYPEID = p.SETYPEID,
                    SITEID = p.SITEID,
                    setypename = t.setypename,
                    GROUPID = s.GROUPID,
                    siteshortname = s.siteshortname,
                    checkintime = SqlFunc.Subqueryable<GCSpecialEqpRtdDataEntity>().Where(d => d.secode == p.secode).Max(d => d.updatedate),
                    nextcheckday = SqlFunc
                                    .IF(t.checktype == 1).Return(SqlFunc.DateAdd(Convert.ToDateTime(p.lastcheckday), t.checktime, DateType.Year))
                                    .ElseIF(t.checktype == 2).Return(SqlFunc.DateAdd(Convert.ToDateTime(p.lastcheckday), t.checktime, DateType.Month))
                                    .ElseIF(t.checktype == 3).Return(SqlFunc.DateAdd(Convert.ToDateTime(p.lastcheckday), t.checktime, DateType.Day))
                                    .End<DateTime?>(null),
                    daycount = 0//DateTime.Now.Subtract(nextcheckday).Days,

                }).ToPageListAsync(page, size, totalCount);
            list.ForEach(item => { item.daycount = item.nextcheckday == null ? 0 : DateTime.Now.Subtract(Convert.ToDateTime(item.nextcheckday)).Days; });
            var data = new PageOutput<SpecialEqpListOutput>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }

        public async Task<DataTable> GetWXDYSpecialList(string setype, string secode)
        {
            if (string.IsNullOrEmpty(secode))
            {
                var SQL = @"select ddy.*,a.paramjson,a.secode,a.ID FROM T_GC_DeviceDY ddy 
inner join T_GC_Device d on d.DEVICEID=ddy.DEVICEID  
inner join T_GC_SpecialEqp a on a.SITEID=d.SITEID where a.setype=@setype and a.bdel=0 AND LEN(ddy.appid)>0 AND LEN(ddy.[prj_id])>0 ";
                return await _baseRepository.Db.Ado.GetDataTableAsync(SQL, new { setype = setype });
            }
            else
            {
                var SQL = @"select ddy.*,a.paramjson,a.secode,a.ID FROM T_GC_DeviceDY ddy 
inner join T_GC_Device d on d.DEVICEID=ddy.DEVICEID  
inner join T_GC_SpecialEqp a on a.SITEID=d.SITEID where a.setype=@setype and a.bdel=0 AND LEN(ddy.appid)>0 AND LEN(ddy.[prj_id])>0 and a.secode=@secode";
                return await _baseRepository.Db.Ado.GetDataTableAsync(SQL, new { setype = setype, secode = secode });
            }

        }


        public async Task<string> GetWXDYToken(string appid = "sysfdas2fvdasf33dag", bool isForce = false ,string SECRTE= "cag4adg412fa2dc2", string AES="da2gaf4afdasfea1",string url ="" )
        {
            if (string.IsNullOrEmpty(url))
            {
                url = _configuration.GetSection("WXDY").GetValue<string>("PushUrl");
            }
            string SUCCESS = "0000";
            string token_url = "http://{0}/rest/Token/get/{1}/";
            string TOKEN_URL_Format = string.Format(token_url, url, appid);
            string ACCESS_TOKEN = null;
            if (!isForce)
            {
                ACCESS_TOKEN = await _cache.GetAsync<string>(appid);
                if (!string.IsNullOrEmpty(ACCESS_TOKEN))
                {
                    _logger.LogInformation(appid + "获取到的token：" + ACCESS_TOKEN);
                    return ACCESS_TOKEN;
                }
            }
            string pass = "{\"pass\":\"" + SpecialEqpHelp.GetPass(SECRTE, AES) + "\"}";
            string tokenresp = UHttp.Post(TOKEN_URL_Format, pass, UHttp.CONTENT_TYPE_JSON);
            JObject jToken = JObject.Parse(tokenresp);
            if (Convert.ToString(jToken["flag"]) != SUCCESS)
            {
                return "";
            }
            ACCESS_TOKEN = Convert.ToString(jToken["result"]["access_token"]);
            await _cache.SetAsync(appid, ACCESS_TOKEN, TimeSpan.FromMinutes(60));
            
            _logger.LogInformation(appid + "获取到的token：" + ACCESS_TOKEN);
            return ACCESS_TOKEN;
        }

        public async Task<GCSpecialEqpRtdDataEntity> GetLastOneMinFirst(string secode)
        {
            return await _baseRepository.Db.Queryable<GCSpecialEqpRtdDataEntity>().OrderBy(a => a.updatedate, OrderByType.Desc).FirstAsync(a => a.secode == secode && a.updatedate > DateTime.Now.AddMinutes(1));
        }
        public async Task<GCSpecialEqpRtdDataEntity> GetLastOneFirst(string secode)
        {
            return await _baseRepository.Db.Queryable<GCSpecialEqpRtdDataEntity>().OrderBy(a => a.updatedate, OrderByType.Desc).FirstAsync(a => a.secode == secode);
        }

        public async Task<int> AddSEDoc(List<GCSpecialEqpDoc> list)
        {
            return await _baseRepository.Db.Insertable(list).ExecuteCommandAsync();
        }


        public async Task<int> DeleteSEDoc(int SEID,string[] list)
        {
            return await _baseRepository.Db.Deleteable<GCSpecialEqpDoc>()
                .Where(ii => ii.SEID == SEID && list.Contains(ii.SEDOCID.ToString()))
                .ExecuteCommandAsync();
        }

        public async Task<List<SpecialEqpDocOutputDto>> GetSEPics(int SEID)
        {
            string domain = _hpSystemSetting.getSettingValue(Const.Setting.S034);
            string folder = _hpSystemSetting.getSettingValue(Const.Setting.S056);

            return await _baseRepository.Db.Queryable<GCSpecialEqpDoc>()
                .Where(ii => ii.SEID == SEID)
                .Select(ii => new SpecialEqpDocOutputDto
                {
                    url = "http://" + domain + "/" + folder + "/" + ii.SEID.ToString() + "/" + ii.SEDOCID.ToString()
                    + "/" + ii.filename,
                    filetype = ii.filetype,
                    name = ii.filename
                })
                .ToListAsync();
        }

        /// <summary>
        /// 获取监测对象的设备数
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="setype"></param>
        /// <returns></returns>
        public async Task<DataRow> GetCountAsync(int SITEID, int setype)
        {
            DataTable dt = await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2SpecialCount", new { SITEID = SITEID, setype = setype });
            if (dt == null || dt.Rows.Count == 0)
                return null;

            return dt.Rows[0];
        }

        public async Task<DataRow> GetRtdAsync(int SITEID, string secode)
        {
            DataTable dt = await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2SpecialRtd", new { SITEID = SITEID, secode = secode });
            if (dt == null || dt.Rows.Count == 0)
                return null;

            return dt.Rows[0];
        }

        public async Task<DataTable> GetRtdListAsync(int SITEID, string secode)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2SpecialRtdList", new { SITEID = SITEID, secode = secode });
        }

        public async Task<DataTable> GetListAsync(int SITEID, int setype)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2SpecialList", new { SITEID = SITEID, setype = setype });
        }

        public async Task<DataTable> GetEmpListAsync(int SITEID, string secode)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2SpecialEmpList", new { SITEID = SITEID, secode = secode });
        }
    }
}
