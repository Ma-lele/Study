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
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.DeviceCN
{
    public class DeviceCNService : BaseServices<BaseEntity>, IDeviceCNService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<BaseEntity> _deviceCNRepository;
        public DeviceCNService(IUser user, IBaseRepository<BaseEntity> deviceCNRepository)
        {
            _user = user;
            _deviceCNRepository = deviceCNRepository;
            BaseDal = deviceCNRepository;
        }

        public async Task<DataSet> getSiteDataHis(object param)
        {
            return await _deviceCNRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spSiteDataHis", param);
        }

        /// <summary>
        /// 实时数据插入
        /// </summary>
        /// <param name="param">数据信息</param>
        /// <returns></returns>
        public async Task<int> rtdInsert(DeviceRtdDataInput input)
        {
            SgParams sp = new SgParams();
            sp.SetParams(input);
            sp.NeetReturnValue();

            await _deviceCNRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spDeviceRtdDataInsert", sp.Params);
            return sp.ReturnValue;
        }

        /// <summary>
        /// 获取一个分组下所有监测点最新一条监测数据（分页）
        /// </summary>
        /// <param name="GROUPID">分组编号</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">每页数据条数</param>
        /// <returns>结果集</returns>
        public async Task<DataTable> getSiteAllRtdApi(int GROUPID, int pageIndex, int pageSize)
        {
            DataTable result = await _deviceCNRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteRtdForApi", new { GROUPID = GROUPID, pageIndex = pageIndex, pageSize = pageSize });
            return result;
        }

        /// <summary>
        /// 获取一个监测点最新一条监测数据
        /// </summary>
        /// <param name="SITEID">分组编号</param>
        /// <returns>结果集</returns>
        public async Task<DataTable> getSiteRtdApi(int SITEID)
        {
            DataTable result = await _deviceCNRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteLastRtd", new { SITEID = SITEID });
            return result;
        }

        /// <summary>
        /// 获取一个监测点最近一小时的分钟数据
        /// </summary>
        /// <param name="SITEID">监测点编号</param>
        /// <returns>结果集</returns>
        public async Task<DataTable> getSiteRtdOneHourApi(int SITEID)
        {
            DataTable result = await _deviceCNRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSiteRtdOneHourForApi", new { SITEID = SITEID });
            return result;
        }


        /// <summary>
        /// 批处理
        /// </summary>
        /// <returns></returns>
        public async Task<int> doBatch(int timeout)
        {
            var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
            //_deviceCNRepository.Db.Ado.CommandTimeOut = timeout;
            await _deviceCNRepository.Db.Ado.UseStoredProcedure().ExecuteCommandAsync("spBatchDeviceHourData", output);
            return output.Value.ObjToInt();
        }

        /// <summary>
        /// 获取10分钟内需要推送的单条记录
        /// </summary>
        /// <param name="devicecode">设备编号</param>
        /// <returns></returns>
        public async Task<DataRow> getOneForSend(string devicecode)
        {

            DataTable dt = await _deviceCNRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spDeviceRtdForSend", new { devicecode = devicecode });

            if (dt == null || dt.Rows.Count == 0)
                return null;

            return dt.Rows[0];
        }

        /// <summary>
        /// 检索设备实时信息
        /// </summary>
        /// <returns>设备实时信息数据集</returns>
        public DataTable getRtdList()
        {
            return _deviceCNRepository.Db.Ado.UseStoredProcedure().GetDataTable("spDeviceRtdList");
        }
    }
}
