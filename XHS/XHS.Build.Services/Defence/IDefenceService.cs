using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Defence
{
    public interface IDefenceService:IBaseServices<DefenceEntity>
    {
        /// <summary>
        /// 检索临边围挡
        /// </summary>
        /// <returns>临边围挡数据集</returns>
        Task<DataSet> getList();

        /// <summary>
        /// 获取所有临边围挡
        /// </summary>
        /// <returns></returns>
        Task<DataTable> getServerList();

        /// <summary>
        /// 获取监测点下临边围挡
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <returns></returns>
        Task<DataTable> getListBySite(int SITEID);

        /// <summary>
        /// 设置临边围挡信息
        /// </summary>
        /// <param name="param">临边围挡信息</param>
        /// <returns>设置结果</returns>
        Task<int> doUpdate(object param);

        /// <summary>
        /// 获取临边围挡单条记录
        /// </summary>
        /// <param name="dfcode">临边围挡设备编号</param>
        /// <returns></returns>
        Task<DataRow> getOne(string dfcode);


        /// <summary>
        /// 临边围挡插入
        /// </summary>
        /// <param name="param">临边围挡情报</param>
        /// <returns>结果集</returns>
        Task<int> doInsert(object param);

        /// <summary>
        /// 删除临边围挡
        /// </summary>
        /// <param name="DEFENCEID">临边围挡ID</param>
        /// <returns>成功件数</returns>
        Task<int> doDelete(int DEFENCEID);

        /// <summary>
        /// 上线
        /// </summary>
        /// <param name="dfcode">设备编号</param>
        /// <returns></returns>
        Task<int> doCheckin(string dfcode);

        /// <summary>
        /// 下线
        /// </summary>
        /// <param name="dfcode">设备编号</param>
        /// <returns></returns>
        Task<int> doCheckout(string dfcode);

        /// <summary>
        /// 拆除
        /// </summary>
        /// <param name="dfcode">设备编号</param>
        /// <param name="switchno">开关号</param>
        /// <returns></returns>
        Task<int> doDisconnect(string dfcode, string switchno);

        /// <summary>
        /// 拆除省4号
        /// </summary>
        /// <param name="dfcode">设备编号</param>
        /// <param name="switchno">开关号</param
        /// <param name="defectPosition">缺失位置</param>
        /// <param name="defectDate">发生时间</param>
        /// <returns></returns>
        Task<int> doFourDisconnect(string dfcode, string switchno, string defectPosition, DateTime defectDate);

        /// <summary>
        /// 恢复省4号
        /// </summary>
        /// <param name="dfcode">设备编号</param>
        /// <param name="recoveryDate">设备编号</param>
        /// <returns></returns>
        Task<int> doFourRecover(string dfcode, DateTime recoveryDate);

        /// <summary>
        /// 布防
        /// </summary>
        /// <param name="dfcode">设备编号</param>
        /// <returns></returns>
        Task<int> doShield(string dfcode );

        /// <summary>
        /// 撤防
        /// </summary>
        /// <param name="dfcode">设备编号</param>
        /// <returns></returns>
        Task<int> doUnshield(string dfcode );

        /// <summary>
        /// 区域布防/撤防
        /// </summary>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="dfzone">片区</param>
        /// <param name="bsheild">0:撤防 1:布防</param> 
        /// <returns></returns>
        Task<int> doZoneShield(int SITEID, string dfzone, int bsheild );

        /// <summary>
        /// 恢复
        /// </summary>
        /// <param name="dfcode">设备编号</param>
        /// <returns></returns>
        Task<int> doRecover(string dfcode);

        /// <summary>
        /// 插入设备操作日志
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<int> doLogInsert(object param);


        Task<PageOutput<DefenceEntity>> GetSiteDefencePageList(int groupid, string keyword, int page, int size);

        Task<List<GroupHelmetBeaconCount>> GetGroupCount();

        /// <summary>
        /// 安全监测-临边围挡
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <param name="dfstatus">设备状态0:闭合 1::断线 2:拆除 3:主机拆除 4:围挡缺失</param>
        /// <param name="bsheild">警戒状态0:撤防 1:布防</param>
        /// <param name="SITEID">siteid</param>
        /// <returns></returns>
        Task<DataSet> spV2DefenceStats(string keyword, int dfstatus, int bsheild, int SITEID);

        /// <summary>
        /// 临边围挡-左上角统计
        /// </summary>
        /// <param name="SITEID">siteid</param>
        /// <returns></returns>
        Task<DataSet> spV2DefenceTotal(int SITEID);

        
    }
}
