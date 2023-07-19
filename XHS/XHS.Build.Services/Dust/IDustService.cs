using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Dust
{
    public interface IDustService : IBaseServices<BaseEntity>
    {
        /// <summary>
        /// 根据设备种类获取监测对象
        /// </summary>
        /// <param name="linktype">设备种类</param>
        /// <returns></returns>
        Task<DataTable> getListByDeviceType(int linktype);


        /// <summary>
        /// 扬尘工程航拍
        /// </summary>
        /// <returns></returns>
        Task<List<BnSiteVideo>> GetFlyVideoList();

        /// <summary>
        /// 工程视频 获取特种设备列表（单个监测）
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        Task<List<BnSpecialEqp>> getListForSite(int SITEID);

        /// <summary>
        /// 获取五方人员
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        Task<List<BnUserFivePart>> GetListFivePart(int SITEID);

        /// <summary>
        /// 注册五方人员
        /// </summary>
        /// <param name="param">注册信息</param>
        /// <returns>是否成功</returns>
        Task<int> doRegistFivePart(object param);



        /// <summary>
        /// 检索预警
        /// </summary>
        /// <param name="GROUPID">分组ID</param>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="type">种类(0:全部,1:离线报警,2:超标报警,31:车辆冲洗设备离线报警,32:车辆未冲洗报警,4:特种设备报警,5:临边围挡报警)</param>
        /// <param name="startdate">开始日期</param>
        /// <param name="enddate">结束日期</param>
        /// <returns>预警信息数据集</returns>
        Task<DataTable> GetWarningList(int SITEID, int type, DateTime startdate, DateTime enddate);

        /// <summary>
        /// 获取用户负责的所有雾泡
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        Task<DataTable> GetFogListByUser();

        /// <summary>
        /// 获取有扬尘监测点的group列表
        /// </summary>
        /// <returns></returns>
        Task<DataSet> GetGroupListforSpot();

        Task<PageOutput<DustListOutput>> GetDustPageList(int groupid, string keyword, int page, int size, string order = "", string ordertype = "");

        Task<List<GroupHelmetBeaconCount>> GetGroupCount();
    }
}
