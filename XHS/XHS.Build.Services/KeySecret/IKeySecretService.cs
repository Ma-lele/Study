using System.Threading.Tasks;
using XHS.Build.Common.Configs;
using XHS.Build.Model;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.KeySecretConfig
{
    public interface IKeySecretService : IBaseServices<TCCKeySecretConfig>
    {
        Task<PageOutput<TCCKeySecretConfig>> GetList(string keyword, int page, int size);
    }
}
