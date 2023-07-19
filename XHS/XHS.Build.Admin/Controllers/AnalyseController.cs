using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Services.Analyse;
using XHS.Build.Common.Helps;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 风险分析配置
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [Permission]
    public class AnalyseController : ControllerBase
    {
        private readonly IAnalyseService _analyseService;
        public AnalyseController(IAnalyseService analyseService)
        {
            _analyseService = analyseService;
        }


        /// <summary>
        /// 设置PL算法
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> SetPLAlgorithm(JObject input)
        {
            if (input == null || !input.ContainsKey("tData"))
            {
                return ResponseOutput.NotOk("请填写信息");
            }

            List<AYEventLevel> aYEvents = new List<AYEventLevel>();
            input["tData"].ToList().ForEach(ii => {
                aYEvents.Add(new AYEventLevel
                {
                    FTCODE = "A",
                    srlevel = int.Parse(ii["level"].ToString()),
                    eventlevel = int.Parse(ii["A"].ToString())
                });
                aYEvents.Add(new AYEventLevel
                {
                    FTCODE = "B",
                    srlevel = int.Parse(ii["level"].ToString()),
                    eventlevel = int.Parse(ii["B"].ToString())
                });
                aYEvents.Add(new AYEventLevel
                {
                    FTCODE = "C",
                    srlevel = int.Parse(ii["level"].ToString()),
                    eventlevel = int.Parse(ii["C"].ToString())
                });
                aYEvents.Add(new AYEventLevel
                {
                    FTCODE = "D",
                    srlevel = int.Parse(ii["level"].ToString()),
                    eventlevel = int.Parse(ii["D"].ToString())
                });
                aYEvents.Add(new AYEventLevel
                {
                    FTCODE = "E",
                    srlevel = int.Parse(ii["level"].ToString()),
                    eventlevel = int.Parse(ii["E"].ToString())
                });
            });

            var result = await _analyseService.SetPLAlgorithm(aYEvents);

            return result ? ResponseOutput.Ok() : ResponseOutput.NotOk("设置失败");
        }


        /// <summary>
        /// 获取PL算法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetPLAlgorithm()
        {
            var list = await _analyseService.GetAllPLAlgorithm();
            JArray arr = new JArray();
            list.GroupBy(gg=>gg.srlevel).ToList().ForEach(ii => {
                JObject obj = new JObject();
                obj["level"] = ii.Key;
                obj["A"] = list.Find(ff => ff.srlevel == ii.Key && ff.FTCODE == "A").eventlevel;
                obj["B"] = list.Find(ff => ff.srlevel == ii.Key && ff.FTCODE == "B").eventlevel;
                obj["C"] = list.Find(ff => ff.srlevel == ii.Key && ff.FTCODE == "C").eventlevel;
                obj["D"] = list.Find(ff => ff.srlevel == ii.Key && ff.FTCODE == "D").eventlevel;
                obj["E"] = list.Find(ff => ff.srlevel == ii.Key && ff.FTCODE == "E").eventlevel;
                arr.Add(obj);
            });
            return ResponseOutput.Ok(arr);
        }


        /// <summary>
        /// 获取严重度数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetSeverity()
        {
            var list = await _analyseService.GetSeverityData();
            JArray arr = new JArray();
            list.Where(ww => ww.ETID.Length > 2)
                .GroupBy(gg => gg.ETID)
                .ToList()
                .ForEach(ii =>
                {
                    JObject obj = new JObject();

                    var tmp = list.Where(ff => ff.ETID == ii.Key);
                    obj["ETID"] = ii.Key;
                    obj["etname"] = tmp.FirstOrDefault().etname;
                    obj["ettypename"] = list.Find(ff => ff.ETID == ii.Key.Substring(0, 2)).etname;
                    obj["etdatatype"] = tmp.FirstOrDefault().etdatatype;
                    obj["etsrtype"] = tmp.FirstOrDefault().etsrtype;
                    obj["type2level"] = tmp.FirstOrDefault().srlevel;
                    tmp.ToList().ForEach(jj => {
                        obj["from"+jj.srlevel] = jj.srfrom;
                        obj["to"+ jj.srlevel] = jj.srto;
                    });
                  
                    arr.Add(obj);
                });


            return ResponseOutput.Ok(arr);
        }


        /// <summary>
        /// 设置严重度数据
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> SetSeverity(JObject input)
        {
            if (input == null || !input.ContainsKey("ETID"))
            {
                return ResponseOutput.NotOk("请填写信息");
            }

            List<AYSeverity> list = new List<AYSeverity>();
            List<AYSeverity> delList = new List<AYSeverity>();

            int etsrtype = input["etsrtype"].ToInt();
            if (etsrtype == 1)
            {
                input.Properties().Where(ii => ii.Name.Contains("from") || ii.Name.Contains("to"))
                    .ToList().ForEach(ii =>
                    {
                        int _srlevel = ii.Name.Substring(ii.Name.Length - 1, 1).ToInt();
                        var entity = list.Find(ff => ff.srlevel == _srlevel);

                        if (entity != null)
                        {
                            if (ii.Name.Contains("from"))
                            {
                                entity.srfrom = ii.Value.ToDecimal();
                            }
                            else if (ii.Name.Contains("to"))
                            {
                                entity.srto = ii.Value.ToDecimal();
                            }
                        }
                        else
                        {
                            AYSeverity severity = new AYSeverity();
                            severity.ETID = input["ETID"].ToString();
                            if (ii.Name.Contains("from"))
                            {
                                severity.srlevel = ii.Name.Split("from")[1].ToInt();
                                severity.srfrom = ii.Value.ToDecimal();
                            }
                            else if (ii.Name.Contains("to"))
                            {
                                severity.srlevel = ii.Name.Split("to")[1].ToInt();
                                severity.srto = ii.Value.ToDecimal();
                            }
                            list.Add(severity);
                        }
                    });

              
                list.ForEach(ii => {
                    if (ii.srfrom == 0 && ii.srto == 0)
                    {
                        delList.Add(ii);
                    }
                });
                delList.ForEach(ii => {
                    list.Remove(ii);
                });
            }
            else if (etsrtype == 2)
            {
                AYSeverity severity = new AYSeverity{ 
                    ETID = input["ETID"].ToString(),
                    srfrom = 0,
                    srto = 0,
                    srlevel = input["type2level"].ToInt()
                };

                list.Add(severity);
            }

            bool isDel = false;
            if (list.Count == 0)
            {
                if (delList.Count == 0)
                {
                    return ResponseOutput.NotOk("无数据");
                }
                else
                {
                    isDel = true;
                    list = delList;
                }
            }

            bool result = await _analyseService.SetSeverity(list,isDel);


            return result ? ResponseOutput.Ok() : ResponseOutput.NotOk("设置失败");
        }


        /// <summary>
        /// 获取可能性配置数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetFrequency()
        {
            var list = await _analyseService.GetFrequencyData();
            return ResponseOutput.Ok(list);
        }


        /// <summary>
        /// 设置可能性配置
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> SetFrequency(JObject input)
        {
            if (input == null || !input.ContainsKey("tData"))
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var list = input["tData"].ToString().ConvertObject<List<AYFrequencyType>>();
            var result = await _analyseService.SetFrequency(list);

            return result ? ResponseOutput.Ok("设置成功") : ResponseOutput.NotOk("设置失败");
        }
    }
}
