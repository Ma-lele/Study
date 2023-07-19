using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;

namespace XHS.Build.Services.Role
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            CreateMap<SysRoleEntityDto, RoleListOutput>();
            CreateMap<RoleAddInput, SysRoleEntity>();
            CreateMap<RoleUpdateInput, SysRoleEntity>();
        }
    }
}
