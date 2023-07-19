using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Fog
{
    public class FogJobService : BaseServices<GCFogEntity>, IFogJobService
    {
        private readonly IBaseRepository<GCFogEntity> _dal;
        public FogJobService(IBaseRepository<GCFogEntity> dal)
        {
            _dal = dal;
            BaseDal = dal;
        }
        public async Task<DataTable> GetFogKickerDataTable()
        {
            string SQL = @"SELECT d.[SITEID]
                                          ,d.[devicecode]
                                          ,d.[fogkickline]
                                          ,d.[status]
                                          ,d.[checkintime]
                                          ,d.[checkouttime]
                                          ,d.[bdel]
	                                      ,dr.pm10
	                                      ,dr.updatetime
	                                      ,f.[fogcode]
                                          ,f.[fogname]
                                          ,f.[fogstatus]
                                          ,f.[switchno]
                                          ,f.[delay]
                                          ,f.[bwaterauto]
                                          ,f.[checkintime] fcheckintime
                                          ,f.[checkouttime] fcheckouttime
                                      FROM [T_GC_Device] d INNER JOIN T_GC_DeviceRtd dr 
                                      ON d.devicecode=dr.devicecode AND DATEADD(MINUTE,1,dr.updatetime) > GETDATE() AND d.bdel=0 AND d.[status]=1 AND d.fogkickline > 0 
                                      INNER JOIN T_GC_Fog f
                                      ON f.SITEID = d.SITEID AND f.fogstatus=0 AND f.bwaterauto=1
                                      WHERE dbo.fnGetSystemSettingValue('S148')=1
	                                    AND dr.pm10 >= d.fogkickline AND dr.pm10 < 2000";
            DataTable dt =await _dal.Db.Ado.GetDataTableAsync(SQL);
            return dt;
        }
    }
}
