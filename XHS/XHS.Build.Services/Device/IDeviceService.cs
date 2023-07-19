using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Device
{
    public interface IDeviceService:IBaseServices<GCDeviceEntity>
    {
        Task<int> doCheckout(string devicecode, int bwarn);

        Task<PageOutput<GCDeviceEntity>> GetSiteDevicePageList(int groupid, string keyword, int page, int size);

        Task<List<GroupHelmetBeaconCount>> GetGroupCount();

        Task<DataTable> GetDevOnline(DateTime startdate, DateTime enddate);

        Task<bool> DeleteRtd(string devicecode, int siteid);

        Task<int> AddDeviceFacture(DeviceDto dto);


        Task<int> deleteDeviceFacture(DeviceDto dto);


        Task<int> doRtdDelete();

        Task<bool> AddDevDelHis(GCDevDelHis input);


        Task<int> AddAYDeviceFacture(DeviceDto dto);
    }
}
