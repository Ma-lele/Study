using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Server
{
    public class ServerService : BaseServices<ServerEntity>, IServerService
    {
        private readonly IBaseRepository<ServerEntity> _baseRepository;
        public ServerService(IBaseRepository<ServerEntity> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task<List<ServerRequestOutput>> GetServerList()
        {
            var DbList =await _baseRepository.Db.Queryable<ServerEntity>().Where(a=>a.bdeployed==1).Select<ServerRequestOutput>().ToListAsync(); ;
            return DbList;
        }
    }
}
