using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.IPWhiteList
{
    public class IPWhiteListService : BaseServices<SFIPWhiteList>, IIPWhiteListService
    {
        private readonly IBaseRepository<SFIPWhiteList> _dal;
        public IPWhiteListService(IBaseRepository<SFIPWhiteList> dal)
        {
            _dal = dal;
            _dal.CurrentDb = "XJ_Env_bak";
            BaseDal = dal;
        }

    }
}
