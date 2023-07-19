using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;

namespace XHS.Build.Services.DeepPit
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            CreateMap<DeepPitInputDto, GCDeepPit>();
        }
    }
}
