using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.SpecialEqp
{
    public interface ISpecialEqpService : IBaseServices<GCSpecialEqpEntity>
    {
        Task<PageOutput<SpecialEqpListOutput>> GetSiteSpecialEqpPageList(int groupid, string keyword, int page, int size);

        Task<List<GroupHelmetBeaconCount>> GetGroupCount();

        Task<string> GetWXDYToken(string appid= "sysfdas2fvdasf33dag", bool isForce = false, string SECRTE = "cag4adg412fa2dc2", string AES = "da2gaf4afdasfea1", string url = "");

        Task<GCSpecialEqpRtdDataEntity> GetLastOneMinFirst(string secode);
        Task<GCSpecialEqpRtdDataEntity> GetLastOneFirst(string secode);

        /// <summary>
        /// 无锡大运  特种设备数据
        /// </summary>
        /// <param name="setype"></param>
        /// <returns></returns>
        Task<DataTable> GetWXDYSpecialList(string setype, string secode);


        /// <summary>
        /// 插入特种设备图片
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<int> AddSEDoc(List<GCSpecialEqpDoc> list);


        /// <summary>
        /// 删除特种设备图片
        /// </summary>
        /// <param name="SEID"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        Task<int> DeleteSEDoc(int SEID,string[] list);


        Task<List<SpecialEqpDocOutputDto>> GetSEPics(int SEID);

        /// <summary>
        /// 获取监测对象的设备数
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="setype"></param>
        /// <returns></returns>
        Task<DataRow> GetCountAsync(int SITEID, int setype);

        /// <summary>
        /// 获取某台特种设备最新一条实时数据
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="secode"></param>
        /// <returns></returns>
        Task<DataRow> GetRtdAsync(int SITEID, string secode);

        /// <summary>
        /// 获取某台特种设备最近N条实时数据用于回放
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="secode"></param>
        /// <returns></returns>
        Task<DataTable> GetRtdListAsync(int SITEID, string secode);

        /// <summary>
        /// 获取单个工地特种设备列表
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="setype"></param>
        /// <returns></returns>
        Task<DataTable> GetListAsync(int SITEID, int setype);

        /// <summary>
        /// 获取单台特种设备的操作员列表
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="secode"></param>
        /// <returns></returns>
        Task<DataTable> GetEmpListAsync(int SITEID, string secode);
    }
}
