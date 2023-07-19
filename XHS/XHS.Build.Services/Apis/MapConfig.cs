using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;

namespace XHS.Build.Services.Apis
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            CreateMap<ApisInputDto, SysApisEntity>();
        }
    }
}
