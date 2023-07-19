using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Device
{
    public class DeviceService : BaseServices<GCDeviceEntity>, IDeviceService
    {
        private readonly IBaseRepository<GCDeviceEntity> _baseRepository;
        public DeviceService(IBaseRepository<GCDeviceEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
        }

        /// <summary>
        /// 设备登出
        /// </summary>
        /// <param name="devicecode">设备编号</param>
        /// <param name="bwarn">是否需要报警(默认是)</param>
        /// <returns></returns>
        public async Task<int> doCheckout(string devicecode, int bwarn)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _baseRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spDeviceCheckout", new SugarParameter("@devicecode", devicecode), new SugarParameter("@bwarn", bwarn), output);
            return output.Value.ObjToInt();
        }

        public async Task<List<GroupHelmetBeaconCount>> GetGroupCount()
        {
            return await _baseRepository.Db.Ado.SqlQueryAsync<GroupHelmetBeaconCount>("select G.GROUPID,G.groupshortname,isnull(A.count,0) as count , dbo.fnGetSystemSettingValue('S148') hasfogkickline from T_GC_Group G LEFT JOIN (SELECT D.GROUPID, count(D.DEVICEID) as count from T_GC_Device D GROUP BY D.GROUPID) A on A.GROUPID = G.GROUPID where G.status = 0 ORDER BY case when A.count is null then 0 else A.count end desc,G.city,G.district,G.GROUPID");
        }

        public async Task<PageOutput<GCDeviceEntity>> GetSiteDevicePageList(int groupid, string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _baseRepository.Db.Queryable<GCDeviceEntity, GCSiteEntity>((d, s) => new JoinQueryInfos(JoinType.Left, d.SITEID == s.SITEID && d.GROUPID == s.GROUPID))
                .WhereIF(groupid > 0, (d, s) => d.GROUPID == groupid && d.bdel == 0)
                .WhereIF(!string.IsNullOrEmpty(keyword), (d, s) => d.devicecode.Contains(keyword) || d.supplier.Contains(keyword) || s.siteshortname.Contains(keyword))
                .OrderBy(" checkintime desc,bDel desc")
                .Select<GCDeviceEntity>().ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<GCDeviceEntity>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }


        public async Task<DataTable> GetDevOnline(DateTime startdate, DateTime enddate)
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spApiDevOnline", new { startdate = startdate, enddate = enddate });
        }

        public async Task<bool> DeleteRtd(string devicecode, int siteid)
        {
            return await _baseRepository.Db.Deleteable<GCDeviceRtd>()
                .Where(ii => ii.devicecode == devicecode && ii.SITEID == siteid)
                .ExecuteCommandHasChangeAsync();
        }

        /// <summary>
        /// 设备注册
        /// </summary>
        /// <param name="dto">设备信息</param>
        /// <returns></returns>
        public async Task<int> AddDeviceFacture(DeviceDto dto)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _baseRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spSiteDeviceAdd", new SugarParameter("@recordNumber", dto.recordNumber), new SugarParameter("@deviceType", dto.deviceType), new SugarParameter("@deviceId", dto.deviceId), new SugarParameter("@deviceName", dto.deviceName), new SugarParameter("@operator", dto.updater), output);
            return output.Value.ObjToInt();
        }

        /// <summary>
        /// 设备删除
        /// </summary>
        /// <param name="dto">设备信息</param>
        /// <returns></returns>
        public async Task<int> deleteDeviceFacture(DeviceDto dto)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _baseRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spSiteDeviceDemolish", new SugarParameter("@recordNumber", dto.recordNumber), new SugarParameter("@deviceType", dto.deviceType), new SugarParameter("@deviceId", dto.deviceId), new SugarParameter("@operator", dto.updater), output);
            return output.Value.ObjToInt();
        }

        public async Task<int> doRtdDelete()
        {
            return await _baseRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDeviceRtdDelete");
        }

        public async Task<bool> AddDevDelHis(GCDevDelHis input)
        {
            int result = await _baseRepository.Db.Insertable(input).ExecuteCommandAsync();
            return result > 0 ? true : false;
        }

        /// <summary>
        /// 设备注册
        /// </summary>
        /// <param name="dto">设备信息</param>
        /// <returns></returns>
        public async Task<int> AddAYDeviceFacture(DeviceDto dto)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            await _baseRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spAYDeviceAdd", new SugarParameter("@deviceType", dto.deviceType), new SugarParameter("@deviceId", dto.deviceId), new SugarParameter("@deviceName", dto.deviceName), new SugarParameter("@operator", dto.updater), output);
            return output.Value.ObjToInt();
        }
    }
}
