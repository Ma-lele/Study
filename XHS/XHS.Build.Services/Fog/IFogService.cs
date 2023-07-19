using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Fog
{
    public interface IFogService:IBaseServices<GCFogEntity>
    {
        /// <summary>
        /// 检索雾炮
        /// </summary>
        /// <returns>雾炮数据集</returns>
        Task<DataSet> getList();

        /// <summary>
        /// 获取所有雾泡
        /// </summary>
        /// <returns></returns>
        Task<DataTable> getServerList();

        /// <summary>
        /// 获取监测点下雾泡
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <returns></returns>
        Task<DataTable> getListBySite(int SITEID);

        /// <summary>
        /// 获取用户负责的所有雾泡
        /// </summary>
        /// <returns></returns>
        Task<DataTable> getListByUser();

        /// <summary>
        /// 上线
        /// </summary>
        /// <param name="fogcode">雾泡编号</param>
        /// <returns></returns>
        Task<int> doCheckin(string fogcode);

        /// <summary>
        /// 下线
        /// </summary>
        /// <param name="fogcode">雾泡编号</param>
        /// <returns></returns>
        Task<int> doCheckout(string fogcode);

        /// <summary>
        /// 开启
        /// </summary>
        /// <param name="fogcode">雾泡编号</param>
        /// <param name="switchno">开关号</param>
        /// <returns></returns>
        Task<int> doTurnOn(string fogcode, string switchno);

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="fogcode">雾泡编号</param>
        /// <param name="switchno">开关号</param>
        /// <returns></returns>
        Task<int> doTurnOff(string fogcode, string switchno);

        /// <summary>
        /// 插入设备操作日志
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> doLogInsert(object param);



        Task<PageOutput<GCFogSitePageListOutput>> GetSiteFogPageList(int groupid, string keyword, int page, int size);

        Task<List<GroupHelmetBeaconCount>> GetGroupCount();
    }
}
