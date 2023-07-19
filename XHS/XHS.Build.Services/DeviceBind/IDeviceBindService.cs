using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.DeviceBind
{
    public interface IDeviceBindService : IBaseServices<DeviceBindEntity>
    {
        /// <summary>
        /// 获取列表（cache）
        /// </summary>
        /// <returns></returns>
        Task<IResponseOutput<List<DeviceBindOutput>>> GetDeviceBindList();

        /// <summary>
        /// 新增或修改
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<IResponseOutput> AddOrEditDeviceBind(List<DeviceBindInputDto> dtolist);

        /// <summary>
        /// 根据code获取绑定设备信息
        /// </summary>
        /// <returns></returns>
        Task<List<DeviceBindOutput>> GetDeviceBindByCode(string devicecode);

        /// <summary>
        /// 根据code获取绑定设备信息
        /// </summary>
        /// <returns></returns>
        Task<List<DeviceBindOutput>> GetDeviceBindByCodeType(string devicecode,string devicetype);

        /// <summary>
        /// 获取特种设备查询时间
        /// </summary>
        /// <returns></returns>
        Task<DateTime> GetBindDatetime(string domain,string type);

        Task<DateTime> GetInvadeDatetime(string domain);

        /// <summary>
        /// 水表设备查询时间
        /// </summary>
        /// <returns></returns>
        Task<DateTime> GetWaterDatetime(string domain);

        /// <summary>
        /// 电表设备查询时间
        /// </summary>
        /// <returns></returns>
        Task<DateTime> GetElectricDatetime(string domain);

        /// <summary>
        /// 获取其他设备查询时间
        /// </summary>
        /// <returns></returns>
        Task<DateTime> GetSiteDatetime(string domain);

        /// <summary>
        /// 获取特种设备列表
        /// </summary>
        /// <param name="updatedate"></param>
        /// <returns></returns>
        Task<IResponseOutput> GetSpecialEqpApiBindList(DateTime updatedate);

        /// <summary>
        /// 获取特种设备之外的设备列表
        /// </summary>
        /// <param name="updatedate"></param>
        /// <returns></returns>
        Task<IResponseOutput> GetSiteApiBindList(DateTime updatedate);

        /// <summary>
        /// 获取水表设备列表
        /// </summary>
        /// <param name="updatedate"></param>
        /// <returns></returns>
        Task<IResponseOutput> GetWaterEqpApiBindList(DateTime updatedate);

        /// <summary>
        /// 获取电表设备列表
        /// </summary>
        /// <param name="updatedate"></param>
        /// <returns></returns>
        Task<IResponseOutput> GetElectricEqpApiBindList(DateTime updatedate);
    }
}
