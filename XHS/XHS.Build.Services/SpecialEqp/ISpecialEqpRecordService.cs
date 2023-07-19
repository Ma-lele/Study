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
    public interface ISpecialEqpRecordService : IBaseServices<GCSpecialEqpRecordEntity>
    {
        Task<PageOutput<SpecialEqpRecordListOutput>> GetSiteSpecialEqpPageList(int groupid, string keyword, int page, int size);

        Task<List<GroupHelmetBeaconCount>> GetGroupCount();

        Task<List<GCSpecialEqpEntity>> GetEqp(int GROUPID, int SITEID, int setype);

        Task<int> AddProof(List<GCSpecialEqpRecordProof> input);

        Task<List<ImgDto>> GetImgs(int SERID);

        Task<int> DelImgs(List<GCSpecialEqpRecordProof> list);

        /// <summary>
        /// 获取单台特种设备的备案（1条备案+图片列表）
        /// </summary>
        /// <param name="SITEID"></param>
        /// <param name="secode"></param>
        /// <returns></returns>
        Task<DataSet> GetOneAsync(int SITEID, string secode);
    }
}
