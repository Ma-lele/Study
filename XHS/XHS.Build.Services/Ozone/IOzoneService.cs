using System.Collections.Generic;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Model.NetModels.Dtos;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Ozone
{
    public interface IOzoneService : IBaseServices<GCOzone>
    {
        Task<int> doRtdInsert(OzoneRtdDataInput input);
       
/// <summary>
        /// Group数据
        /// </summary>
        /// <returns></returns>
        Task<List<GroupHelmetBeaconCount>> GetGroupCount();


        /// <summary>
        /// 检查code是否重复
        /// </summary>
        /// <param name="code"></param>
        /// <param name="groupid"></param>
        /// <returns></returns>
        Task<bool> CheckCode(string code, int groupid, int ozid);


        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="keyword"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        Task<PageOutput<OzoneDto>> GetOzPageList(int groupid, string keyword, int page, int size);


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<bool> Delete(GCOzone input);    }
}
