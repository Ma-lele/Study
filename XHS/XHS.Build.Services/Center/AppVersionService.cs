using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Center
{
    public class AppVersionService : BaseServices<BaseEntity>, IAppVersionService
    {
        private readonly IBaseRepository<BaseEntity> _defenceRepository;
        public AppVersionService(IBaseRepository<BaseEntity> defenceRepository)
        {
            
            _defenceRepository = defenceRepository;
            BaseDal = defenceRepository;
        }
        public async Task<DataTable> GetAppVersion(string domain, string type)
        {
          //  string sql = "select domain ,isforce,discription,androidurl as downloadurl,androidver as version from T_Server where domain = '" + domain + "'";
           // if (type == "ios")
          //  {
         //       sql = "select domain ,isforce,discription,iosurl as downloadurl,iosver as version from T_Server where domain = '" + domain + "'";
          //  }
             return await _defenceRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spAppVerForDomain", new { domain = domain });
      
            //return await _defenceRepository.QuerySql(sql);
        }

      
    }
}
