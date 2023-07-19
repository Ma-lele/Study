using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Apis
{
    public interface IApiService:IBaseServices<SysApisEntity>
    {
        Task<IResponseOutput> AsyncApi(List<ApisInputDto> dtos);
        /// <summary>
        /// 同步swagger到sysapi表
        /// </summary>
        /// <param name="apis"></param>
        /// <returns></returns>
        Task<IResponseOutput> AsyncApiJson(JArray apis);

    }

}
