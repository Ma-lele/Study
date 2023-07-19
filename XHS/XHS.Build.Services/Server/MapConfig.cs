using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;

namespace XHS.Build.Services.Server
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            CreateMap<ServerEntity, ServerRequestOutput>();
        }
    }
}
