using SqlSugar;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Configs;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.KeySecretConfig
{
    public class KeySecretService : BaseServices<TCCKeySecretConfig>, IKeySecretService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<TCCKeySecretConfig> _keySecretRepository;
        public KeySecretService(IUser user, IBaseRepository<TCCKeySecretConfig> keySecretRepository)
        {
            _user = user;
            _keySecretRepository = keySecretRepository;
            BaseDal = keySecretRepository;
        }


        public async Task<PageOutput<TCCKeySecretConfig>> GetList(string keyword, int page, int size)
        {
            RefAsync<int> totalCount = 0;
            var list = await _keySecretRepository.Db.Queryable<TCCKeySecretConfig>()
               .WhereIF(!string.IsNullOrEmpty(keyword), u => u.Name.Contains(keyword))
                .OrderBy(" Name desc")
            .Select<TCCKeySecretConfig>().ToPageListAsync(page, size, totalCount);
            var data = new PageOutput<TCCKeySecretConfig>()
            {
                data = list,
                dataCount = totalCount
            };
            return data;
        }

    }
}
