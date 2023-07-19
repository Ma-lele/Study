using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Models;

namespace XHS.Build.Services.Group
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            CreateMap<GroupInputDto, GcGroupEntity>();
            CreateMap<GcGroupEntity, GroupInputDto>();
        }
    }
}
