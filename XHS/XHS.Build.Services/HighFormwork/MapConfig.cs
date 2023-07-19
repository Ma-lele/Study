using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;

namespace XHS.Build.Services.HighFormwork
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            CreateMap<HighFormworkAreaDto, GCHighFormworkArea>();
            CreateMap<HighFormworkAreaUpdateDto, GCHighFormworkArea>();
        }
    }
}
