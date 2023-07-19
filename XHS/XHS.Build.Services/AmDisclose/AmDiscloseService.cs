using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using XHS.Build.Common.Util;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.AmDisclose
{
    public class AmDiscloseService:BaseServices<GCAmDiscloseEntity>, IAmDiscloseService
    {
        private readonly IBaseRepository<GCAmDiscloseEntity> _baseRepository;
        public AmDiscloseService(IBaseRepository<GCAmDiscloseEntity> baseRepository)
        {
            _baseRepository = baseRepository;
            BaseDal = baseRepository;
        }

        public async Task<string> GetCount(string projId)
        {
            DataSet ds = await _baseRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spAmDiscloseCount", new { projId = projId });
            string result = JsonTransfer.dataSet2Json(ds);
            if (!string.IsNullOrEmpty(result))
            {
                JObject retObj = JObject.Parse(result);
                JObject json = new JObject();
                json.Add("months", retObj["0"]);
                json.Add("years", retObj["1"]);
                result = json.ToString();
            }
            return result;

        }

        public async Task<string> GetAmDiscloseByDay(string projId, string date, int page, int limit)
        {
            DataTable dt = await _baseRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spAmDiscloseDayForMobile", new { projId = projId, billdate = date, page = page, limit = limit });
            string result = JsonTransfer.dataTable2Json(dt);
            if (!string.IsNullOrEmpty(result))
            {
                JArray retObj = JArray.Parse(result);
                JObject json = new JObject();
                if (retObj.Count > 0)
                {
                    JObject amjson = (JObject)retObj[0];
                    json.Add("total", amjson["totalcount"]);
                }
                else
                {
                    json.Add("total", 0);
                }

                json.Add("records", retObj);
                result = json.ToString();
            }
            return result;
        }
    }
}
