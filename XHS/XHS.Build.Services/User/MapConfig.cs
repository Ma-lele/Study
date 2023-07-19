using AutoMapper;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;

namespace XHS.Build.Services.User
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            CreateMap<SysUserEntity, LoginUserOutput>();
            CreateMap<SysUserEntity, SysUserListOutput>();
            CreateMap<UserAddInput, SysUserEntity>();
        }
    }
}
