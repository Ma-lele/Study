using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Models;

namespace XHS.Build.Services.SpecialEqp
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            CreateMap<AuthData, GCSpecialEqpAuthHisEntity>().ForMember(entity=>entity.ID, dto => dto.MapFrom(a => a.DriverCardNo)).ForMember(entity => entity.realname, dto => dto.MapFrom(a => a.DriverName));
        }
    }
}
