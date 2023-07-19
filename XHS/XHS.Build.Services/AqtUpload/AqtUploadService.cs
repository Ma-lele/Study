using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.AqtUpload
{
    public class AqtUploadService : BaseServices<BaseEntity>, IAqtUploadService
    {
        public readonly IUser _user;
        public readonly IBaseRepository<BaseEntity> _attendRepository;
        public AqtUploadService(IUser user, IBaseRepository<BaseEntity> attendRepository)
        {
            _user = user;
            base.BaseDal = attendRepository;
            _attendRepository = attendRepository;
            _attendRepository.CurrentDb = "XJ_Env";

        }

        public async Task<DataSet> GetListById()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpHour", new { });
        }

        public async Task<DataSet> GetListMinute()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpMinute", new { });
        }

        public async Task<DataSet> GetListForSuzhou()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpHourSuzhou", new { });

        }

        public async Task<DataSet> GetListForSuzhouMinute()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpMinuteSiteSuzhou", new { });

        }

        public async Task<DataSet> GetCityBelongtoList()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spAqtGroupList", new { });

        }

        public async Task<DataSet> GetListsForGuannan()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpHourSiteGuannan", new { });

        }

        public async Task<DataSet> GetListsForGuannanMinute()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpMinuteSiteGuannan", new { });

        }

        public async Task<DataSet> GetListsForRugao()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpHourSiteRugao", new { });

        }

        public async Task<DataSet> GetListsForRugaoMinute()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpMinuteSiteRugao", new { });

        }

        public async Task<DataSet> GetListsForFuningMinute()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpMinuteSiteFuning", new { });

        }

        public async Task<DataSet> GetListsForFuning()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpHourSiteFuning", new { });

        }
        public async Task<DataTable> GetGroupSiteajcodes()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spGetGroupSiteajcodes", new { });

        }

        public async Task<DataTable> GetLastCityUploadTime(string uploadurl, string post)
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spLastCityUploadTime", new { uploadurl = uploadurl, post = post });

        }


        public async Task<int> UpdateWarnAlarmId(int warnId, string alarmId)
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spWarnAlarmIdUpdate", new { warnid = warnId, alarmid = alarmId });

        }

        public async Task<int> UpdateCityUploadDate(string uploadurl, string post, DateTime uploadtime)
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spCityUpLoadDateUpdate", new { uploadurl = uploadurl, post = post, uploadtime = uploadtime });

        }

        public async Task<int> doUpdateBoard(object param)
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDownBoard", param);
        }

        public async Task<int> doUpdateChecklist(object param)
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDownChecklist", param);
        }

        public async Task<int> doUpdateCheckPoints(object param)
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDownCheckPoints", param);
        }

        public async Task<int> doUpdateMobileCheck(object param)
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDownMobileCheck", param);
        }

        public async Task<int> doUpdateDustDeviceInfo(object param)
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDownDustDeviceInfo", param);
        }

        public async Task<int> doUpdateUploadVideo(object param)
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDownVideo", param);
        }

        public async Task<int> doUpdateUploadMachineryInfos(object param)
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDownMachineryInfos", param);
        }

        public async Task<int> doUpdateUploadDeviceInfo(object param)
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDownUploadDeviceInfo", param);
        }

        public async Task<int> doUpdateHighFormworkDeviceInfo(object param)
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDownHighFormworkDeviceInfo", param);
        }


        public async Task<int> doUpdateDeppPitDeviceInfo(object param)
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDownDeppPitDeviceInfo", param);
        }

        public async Task<DataSet> GetListsForNanjing()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpHourSiteJianye", new { });

        }

        public async Task<DataSet> GetListsForNanjingMinute()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpMinuteSiteJianye", new { });

        }

        public async Task<DataSet> GetListsForXinwuqu()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpHourSiteXinwuqu", new { });

        }

        public async Task<DataSet> GetListsForXinwuquMinute()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpMinuteSiteXinwuqu", new { });

        }
        public async Task<int> doAqtSelfInspectCountInsert(List<SugarParameter> param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spAqtSelfInspectCountInsert", ps);
            return output.Value.ObjToInt();
        }


        public async Task<int> doAqtMonthReviewCountInsert(List<SugarParameter> param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spAqtMonthReviewCountInsert", ps);
            return output.Value.ObjToInt();
        }


        public async Task<int> doAqtMonthReviewResultInsert(List<SugarParameter> param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spAqtMonthReviewResultInsert", ps);
            return output.Value.ObjToInt();
        }


        public async Task<int> doAqtSafetyStandardResultInsert(List<SugarParameter> param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spAqtSafetyStandardResultInsert", ps);
            return output.Value.ObjToInt();
        }

        public async Task<int> doAqtProjectInsert(List<SugarParameter> param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spAqtProjInfoInsert", ps);
            return output.Value.ObjToInt();
        }


        public async Task<int> doAqtSuperDangerInsert(List<SugarParameter> param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spSiteSuperDangerInsert", ps);
            return output.Value.ObjToInt();
        }

        public async Task<DataSet> GetListsForYancheng()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpHourSiteyancheng", new { });
        }

        public async Task<GCSiteEntity> GetVideoInfo(string recordNumber)
        {
            GCSiteEntity data = await _attendRepository.Db.Queryable<GCSiteEntity>().FirstAsync(it => it.siteajcode == recordNumber);
            return data;
        }

        public async Task<int> doAqtCameraInsert(SgParams sp)
        {
            await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spCameraInsert", sp.Params);
            return sp.ReturnValue;
        }

        public async Task<int> doAqtCameraUpdate(SgParams sp)
        {
            await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spCameraUpdateByCode", sp.Params);
            return sp.ReturnValue;
        }

        public async Task<bool> GetExists(string cameracode, int channel, int cameratype, int SITEID)
        {
            return await _attendRepository.Db.Queryable<GCCameraEntity>().Where(it => it.cameracode == cameracode && it.channel == channel && it.cameratype == cameratype && it.SITEID == SITEID).AnyAsync();
        }

        public async Task<DataSet> GetListsForWuxi()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpHourSitewuxi", new { });
        }

        public async Task<DataSet> GetListsForWuxiMinute()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpMinuteSiteWuxi", new { });

        }

        public async Task<DataSet> GetListsForYanchengMinute()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpMinuteSiteyancheng", new { });

        }

        public async Task<DataSet> GetListForXuwei()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpHourSiteXuwei", new { });
        }

        public async Task<DataSet> GetListsForXuweiMinute()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpMinuteSiteXuwei", new { });
        }

        public async Task<DataTable> GetAqtInspectSiteList()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spAqtInspectSiteList", new { });
        }

        public async Task<int> doAqtInspectSave(List<SugarParameter> param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spAqtInspectSave", ps);
            return output.Value.ObjToInt();
        }

        public async Task<DataTable> GetAqtInspectNameListForUpdate()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spAqtInspectNameListForUpdate", new { });
        }

        public async Task<int> doAqtInspectNameUpdate(List<SugarParameter> param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spAqtInspectNameUpdate", ps);
            return output.Value.ObjToInt();
        }

        public async Task<int> doDownFourFreeToShoot(SgParams sp)
        {
            await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDownFourFreeToShoot", sp.Params);
            return sp.ReturnValue;
        }

        public async Task<int> doDownFourFreeToShootRectify(SgParams sp)
        {
            await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDownFourFreeToShootRectify", sp.Params);
            return sp.ReturnValue;
        }

        public async Task<DataSet> GetListForXuzhou()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpHourSiteXuzhou", new { });
        }

        public async Task<DataSet> GetListForXuzhouMinute()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpMinuteSiteXuzhou", new { });
        }

        public async Task<DataSet> GetListForHuarui()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpSitehuarui", new { });

        }

        public async Task<DataSet> GetListForGuanglianda()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpSiteGuanglianda", new { });
        }

        public async Task<DataSet> GetListForXinhesheng()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpHourSiteXinhesheng", new { });

        }
        public async Task<int> UpdateMonthDate(int month, string uploadurl)
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spCityUpLoadMonthDateUpdate", new { month = month, uploadurl = uploadurl });
        }

        public async Task<DataSet> GetListForAuto()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpSiteAoTu", new { });
        }

        public async Task<DataSet> GetListForHuarun()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpSiteHuarun", new { });

        }
        public async Task<DataTable> GetListForCarUndeveloped(string datetime, string uploadurl)
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spYcCarUndeveloped", new { starttime = datetime, uploadurl = uploadurl });
        }

        public async Task<DataSet> GetListForYancheng()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpSiteyancheng", new { });
        }

        public async Task<DataSet> GetListsForYiZheng()
        {
            return await _attendRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spUpSiteYiZheng", new { });
        }
    }
}
