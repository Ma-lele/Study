using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Helmet;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 安全帽定位
    /// </summary>
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HelmetController : ControllerBase
    {
        private readonly IHelmetService _helmetService;
        public HelmetController(IHelmetService helmetService)
        {
            _helmetService = helmetService;
        }

        /// <summary>
        /// 获取楼宇，区域，安全帽信息列表
        /// </summary>
        /// <param name="SITEID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("getlistbysiteid")]
        public async Task<IResponseOutput> GetListBySiteId(int SITEID)
        {
            DataSet ds = await _helmetService.GetListBySiteId(SITEID);
            DataTable beacondt;
            DataTable builddt;
            DataTable helmetdt;
            List<BnHelmetResult<BnHelmet>> resultlist = new List<BnHelmetResult<BnHelmet>>();
            if (ds!=null && ds.Tables.Count > 2)
            {
                beacondt = ds.Tables[0]; //区域
                builddt = ds.Tables[1]; //楼宇
                helmetdt = ds.Tables[2]; //安全帽列表

                //区域
                if (beacondt.Rows.Count > 0 && helmetdt.Rows.Count > 0)
                {
                    for (int i = 0; i < beacondt.Rows.Count; i++)
                    {
                        DataRow dr = beacondt.Rows[i];
                        BnHelmetResult<BnHelmet> result = new BnHelmetResult<BnHelmet>();
                        result.beaconcode = dr["beaconcode"].ToString();
                        result.beaconname = dr["beaconname"].ToString();
                        result.equiptype = dr["equiptype"].ToString();
                        List<BnHelmet> list = new List<BnHelmet>();
                        int count = 0;
                        for (int j = 0; j < helmetdt.Rows.Count; j++)
                        {
                            DataRow hdr = helmetdt.Rows[j];
                            if (result.beaconcode.Equals(hdr["beaconcode"].ToString()))
                            {
                                count++;
                                BnHelmet bh = new BnHelmet();
                                bh.beaconname = result.beaconname;
                                bh.beaconcode = hdr["beaconcode"].ToString();
                                bh.helmetcode = hdr["helmetcode"].ToString();
                                bh.image = hdr["image"].ToString();
                                if (!string.IsNullOrEmpty(hdr["path"].ToString()))
                                {
                                    bh.image = hdr["path"].ToString();
                                } 
                                bh.power = hdr["power"].ToString();
                                bh.realname = hdr["realname"].ToString();
                                bh.HELMETID = hdr["HELMETID"].ToString();
                                bh.position = "";
                                bh.floor = "";
                                list.Add(bh);
                            }
                            
                        }
                        result.beaconcount = count;
                        result.data = list;
                        resultlist.Add(result);
                    }
                }
                //楼宇
                if (builddt.Rows.Count > 0 && helmetdt.Rows.Count > 0)
                {
                    for (int i = 0; i < builddt.Rows.Count; i++)
                    {
                        DataRow dr = builddt.Rows[i];
                        BnHelmetResult<BnHelmet> result = new BnHelmetResult<BnHelmet>();
                        result.beaconcode = dr["inbeaconcode"].ToString();
                        result.beaconname = dr["buildname"].ToString();
                        result.equiptype = dr["equiptype"].ToString();
                        Decimal floorheight = (Decimal) dr["floorheight"];
                        int buildfloor = (int)dr["buildfloor"];
                        int underfloor = (int)dr["underfloor"];
                        List<BnHelmet> list = new List<BnHelmet>();
                        int count = 0;
                        for (int j = 0; j < helmetdt.Rows.Count; j++)
                        {
                            int floor = 1;
                            DataRow hdr = helmetdt.Rows[j];
                            if (result.beaconcode.Equals(hdr["beaconcode"].ToString()))
                            {
                                count++;
                                BnHelmet bh = new BnHelmet();
                                bh.beaconname = result.beaconname;
                                bh.beaconcode = hdr["beaconcode"].ToString();
                                bh.helmetcode = hdr["helmetcode"].ToString();
                                bh.image = hdr["image"].ToString();
                                if (!string.IsNullOrEmpty(hdr["path"].ToString()))
                                {
                                    bh.image = hdr["path"].ToString();
                                }
                                bh.power = hdr["power"].ToString();
                                bh.realname = hdr["realname"].ToString();
                                bh.HELMETID = hdr["HELMETID"].ToString();
                                Decimal hightdiff = (Decimal)hdr["hightdiff"];
                                if (hightdiff >= 0)
                                {
                                    floor = (int)(hightdiff / floorheight) + 1;
                                }
                                else
                                {
                                    floor = (int)(hightdiff / floorheight) - 1;
                                }
                                if (floor > 0 && floor > buildfloor)
                                {
                                    floor = buildfloor;
                                }
                                if (floor < 0 && floor < -underfloor)
                                {
                                    floor = -underfloor;
                                }
                                bh.position = "" + floor + "层/" + buildfloor + "层";
                                bh.floor = "" + floor + "F";
                                list.Add(bh);
                            }
                        }
                        result.beaconcount = count;
                        result.data = list;
                        resultlist.Add(result);
                    }
                }

                //范围外
                if (helmetdt.Rows.Count > 0)
                {
                    BnHelmetResult<BnHelmet> result = new BnHelmetResult<BnHelmet>();
                    result.beaconcode = "";
                    result.beaconname = "范围外";
                    result.equiptype = "99";
                    List<BnHelmet> list = new List<BnHelmet>();
                    int count = 0;
                    for (int j = 0; j < helmetdt.Rows.Count; j++)
                    {
                        DataRow hdr = helmetdt.Rows[j];
                        if (string.IsNullOrEmpty(hdr["beaconcode"].ToString()))
                        {
                            count++;
                            BnHelmet bh = new BnHelmet();
                            bh.beaconname = result.beaconname;
                            bh.beaconcode = hdr["beaconcode"].ToString();
                            bh.helmetcode = hdr["helmetcode"].ToString();
                            bh.image = hdr["image"].ToString();
                            if (!string.IsNullOrEmpty(hdr["path"].ToString()))
                            {
                                bh.image = hdr["path"].ToString();
                            }
                            bh.power = hdr["power"].ToString();
                            bh.realname = hdr["realname"].ToString();
                            bh.HELMETID = hdr["HELMETID"].ToString();
                            bh.position = "";
                            bh.floor = "";
                            list.Add(bh);
                        }
                    }
                    result.beaconcount = count;
                    result.data = list;
                    resultlist.Add(result);
                    
                }
            }
            return ResponseOutput.Ok(resultlist);
        }
    }
}