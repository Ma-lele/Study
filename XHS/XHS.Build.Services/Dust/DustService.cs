using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Dust
{
    public class DustService : BaseServices<BaseEntity>, IDustService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<BaseEntity> _dustRepository;
        public DustService(IUser user, IBaseRepository<BaseEntity> dustRepository)
        {
            _user = user;
            _dustRepository = dustRepository;
            base.BaseDal = dustRepository;
        }

        public async Task<List<BnSiteVideo>> GetFlyVideoList()
        {
            DataSet ds = await _dustRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spSiteVideoList", new { USERID = _user.Id });
            if (ds == null || ds.Tables.Count.Equals(0) || ds.Tables[0].Rows.Count.Equals(0))
                return null;
            List<BnSiteVideo> result = new List<BnSiteVideo>();
            DataTable dt = ds.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                BnSiteVideo bs = new BnSiteVideo();
                bs.VIDEOID = Convert.ToInt32(dt.Rows[i]["VIDEOID"]);
                bs.SITEID = Convert.ToInt32(dt.Rows[i]["SITEID"]);
                bs.path = Convert.ToString(dt.Rows[i]["path"]);
                bs.filename = Convert.ToString(dt.Rows[i]["filename"]);
                bs.filesize = Convert.ToInt32(dt.Rows[i]["filesize"]);
                bs.remark = Convert.ToString(dt.Rows[i]["remark"]);
                bs.username = Convert.ToString(dt.Rows[i]["operator"]);
                bs.createddate = Convert.ToDateTime(dt.Rows[i]["createddate"]);
                bs.operatedate = Convert.ToDateTime(dt.Rows[i]["operatedate"]);

                result.Add(bs);
            }
            return result;
        }

        public async Task<DataTable> getListByDeviceType(int linktype)
        {
            return await _dustRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteListByDeviceType", new { USERID = _user.Id, linktype = linktype });
        }

        public async Task<List<BnUserFivePart>> GetListFivePart(int SITEID)
        {
            DataSet ds = await _dustRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spSiteUserListFivePart", new { SITEID = SITEID });
            if (ds == null || ds.Tables.Count.Equals(0))
                return null;
            List<BnUserFivePart> result = new List<BnUserFivePart>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                BnUserFivePart bufp = new BnUserFivePart();
                bufp.USERID = Convert.ToInt32(ds.Tables[0].Rows[i]["USERID"]);
                bufp.username = Convert.ToString(ds.Tables[0].Rows[i]["username"]);
                bufp.mobile = Convert.ToString(ds.Tables[0].Rows[i]["mobile"]);
                bufp.position = Convert.ToString(ds.Tables[0].Rows[i]["position"]);
                bufp.company = Convert.ToString(ds.Tables[0].Rows[i]["company"]);

                result.Add(bufp);
            }
            return result;
        }

        public async Task<int> doRegistFivePart(object param)
        {
            return await _dustRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spUserUpdateFivePart", param);
        }

        public async Task<List<BnSpecialEqp>> getListForSite(int SITEID)
        {
            DataSet ds = await _dustRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spSpecialEqpListForSite", new { SITEID = SITEID });
            if (ds == null || ds.Tables[0].Rows.Count.Equals(0))
                return null;
            List<BnSpecialEqp> result = new List<BnSpecialEqp>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                BnSpecialEqp bse = new BnSpecialEqp();
                bse.SEID = Convert.ToInt32(ds.Tables[0].Rows[i]["SEID"]);
                bse.sename = Convert.ToString(ds.Tables[0].Rows[i]["sename"]);
                bse.setypename = Convert.ToString(ds.Tables[0].Rows[i]["setypename"]);

                result.Add(bse);
            }
            return result;
        }



        public async Task<DataTable> GetWarningList(int SITEID, int type, DateTime startdate, DateTime enddate)
        {
            return await _dustRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spWarnListType", new { GROUPID = _user.GroupId, SITEID = SITEID, USERID = _user.Id, type = type, startdate = startdate, enddate = enddate });
        }

        public async Task<DataTable> GetFogListByUser()
        {
            return await _dustRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spFogListByUser", new { USERID = _user.Id });
        }

        public async Task<DataSet> GetGroupListforSpot()
        {
            return await _dustRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spGroupListforSpot");
        }

        public async Task<PageOutput<DustListOutput>> GetDustPageList(int groupid, string keyword, int page, int size, string order = "", string ordertype = "")
        {
            RefAsync<int> totalCount = 0;
            var list = await _dustRepository.Db.SqlQueryable<DustListOutput>("select  S.SITEID , S.siteshortname , D.devicecode , S.GROUPID , G.groupshortname , P.siteshortname as parentshortname from T_GC_Site S INNER JOIN T_GC_Group G ON S.GROUPID = G.GROUPID INNER JOIN T_GC_Site P ON S.PARENTSITEID = P.SITEID LEFT JOIN T_GC_Device D ON D.SITEID = S.SITEID where S.[status] <=1 AND S.PARENTSITEID <> S.SITEID ")
                .WhereIF(groupid > 0, f => f.GROUPID == groupid)
                .WhereIF(!string.IsNullOrEmpty(keyword), (f) => f.siteshortname.Contains(keyword) || f.devicecode.Contains(keyword) || f.parentshortname.Contains(keyword))
                .OrderByIF(!string.IsNullOrWhiteSpace(order), order + " " + ordertype)
            .Select<DustListOutput>().ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<DustListOutput>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }

        public async Task<List<GroupHelmetBeaconCount>> GetGroupCount()
        {
            return await _dustRepository.Db.Ado.SqlQueryAsync<GroupHelmetBeaconCount>("select  G.suffix , G.GROUPID , isnull(A.count, 0) count , G.groupshortname from T_GC_Group G LEFT JOIN( select GROUPID, count(1) as count from T_GC_Site where PARENTSITEID <> SITEID AND status <= 2 GROUP BY GROUPID ) A ON G.GROUPID = A.GROUPID where G.status = 0 ORDER BY A.count desc, G.GROUPID ");
        }
    }
}
