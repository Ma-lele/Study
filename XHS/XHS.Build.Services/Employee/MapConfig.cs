using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Models;

namespace XHS.Build.Services.Employee
{
    public class MapConfig : Profile
    {
        public MapConfig()
        {
            CreateMap<EmployeeAddEditInput, GCEmployeeEntity>();
        }
    }
}
