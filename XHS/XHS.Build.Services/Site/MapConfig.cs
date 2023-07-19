using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Models;

namespace XHS.Build.Services.Site
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            CreateMap<DustSiteAddInput, GCSiteEntity>();
        }
    }
}
