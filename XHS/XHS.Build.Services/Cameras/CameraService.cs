using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Cameras
{
    public class CameraService : BaseServices<GCCameraEntity>, ICameraService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<GCCameraEntity> _cameraRepository;
        public CameraService(IUser user, IBaseRepository<GCCameraEntity> cameraRepository)
        {
            _user = user;
            _cameraRepository = cameraRepository;
            BaseDal = cameraRepository;
        }

        public async Task<DataTable> getCameraList(int siteid)
        {
            return await _cameraRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCameraListBySite", new { SITEID = siteid });
        }
        public async Task<DataTable> getCameraListByDevCode(string  devcode)
        {
            return await _cameraRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spV2CameraListByDevcode", new { devcode = devcode });
        }

        public async Task<List<GroupHelmetBeaconCount>> GetGroupCount()
        {
            return await _cameraRepository.Db.Ado.SqlQueryAsync<GroupHelmetBeaconCount>("SELECT [GROUPID] ,[groupname]  ,[groupshortname]  , (SELECT COUNT(1) FROM[vSiteCamera] sc WHERE sc.GROUPID = g.GROUPID) count FROM[T_GC_Group] g where status = 0 ORDER BY count DESC,[groupshortname]");
        }

        public async Task<PageOutput<VSiteCamera>> GetSiteCameraPageList(int groupid, string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _cameraRepository.Db.Queryable<VSiteCamera>()
                .WhereIF(groupid > 0, (v) => v.GROUPID == groupid)
                .WhereIF(!string.IsNullOrEmpty(keyword), (v) => v.cameraname.Contains(keyword) || v.cameracode.Contains(keyword) || 
                v.siteshortname.Contains(keyword) || v.devcode.Contains(keyword))
                .OrderBy((v) => v.operatedate)
                .Select<VSiteCamera>()
                .ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<VSiteCamera>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }
        public async Task<List<VSiteCamera>> GetCameraInfoByCameracode(string cameracode)
        {
            RefAsync<int> totalCount = 0;
            var list = await _cameraRepository.Db.Queryable<VSiteCamera>()
                .WhereIF(!string.IsNullOrEmpty(cameracode), (v) => v.cameracode == cameracode)
                .OrderBy((v) => v.operatedate)
                .Select<VSiteCamera>()
                .ToListAsync();
            return list;
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<int> doDyCameraInsert(List<SugarParameter> param)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            var ps = param.ToList();
            ps.Add(output);
            await _cameraRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDyCameraUpdate", ps);
            return output.Value.ObjToInt();
        }

        public async Task<string> CameraCodeSpliceAsync(string type)
        {
            DataTable dt = await _cameraRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCameraCodeSplice", new { type = type });
            if (dt == null || dt.Rows.Count == 0)
                return null;
            return dt.Rows[0][0].ToString();
        }

        public async Task<int> CamerabonlineUpdateAsync(string cameracode, int bonline, int upstatehis = 0)
        {
            try
            {
                SgParams sp = new SgParams();
                
                sp.Add("cameracode", cameracode);
                sp.Add("bonline", bonline);
                sp.Add("upstatehis", upstatehis);
                sp.NeetReturnValue();
                await _cameraRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spCamerabonlineUpdate", sp.Params);
                var fsda = sp.ReturnValue;
                return fsda;

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

    }
}
