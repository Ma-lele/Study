using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Server
{
    public interface IServerService : IBaseServices<ServerEntity>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Task<List<ServerRequestOutput>> GetServerList();
    }
}
