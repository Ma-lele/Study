using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using XHS.Build.Common;
using XHS.Build.Common.Response;
using XHS.Build.Services.SystemSetting;

namespace xhs.build.app.Controllers
{
    /// <summary>
    /// 配置
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class SettingController : ControllerBase
    {
        private readonly IHpSystemSetting _hpSystemSetting;
        public SettingController(IHpSystemSetting hpSystemSetting)
        {
            _hpSystemSetting = hpSystemSetting;
        }

        /// <summary>
        /// 配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IResponseOutput GetList()
        {
            try
            {
                JObject result = new JObject{
                     { Const.Setting.S015,"http://" +  _hpSystemSetting.getSettingValue(Const.Setting.S015) },
                     { Const.Setting.S034, "http://" + _hpSystemSetting.getSettingValue(Const.Setting.S034) },
                     
                    //{ Const.Setting.S020, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S020), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S027, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S027), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S028, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S028), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S037, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S037), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S038, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S038), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S041, _hpSystemSetting.getSettingValue(Const.Setting.S041) },
                    //{ Const.Setting.S051, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S051), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S052, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S052), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S053, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S053), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S054, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S054), Const.Encryp.PUBLIC_KEY) },
                    { Const.Setting.S057, _hpSystemSetting.getSettingValue(Const.Setting.S057) },
                    { Const.Setting.S059, _hpSystemSetting.getSettingValue(Const.Setting.S059) },
                    //{ Const.Setting.S060, _hpSystemSetting.getSettingValue(Const.Setting.S060) },
                    //{ Const.Setting.S064, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S064), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S068, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S068), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S070, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S070), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S074, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S074), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S075, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S075), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S076, _hpSystemSetting.getSettingValue(Const.Setting.S076) },
                    { Const.Setting.S078, _hpSystemSetting.getSettingValue(Const.Setting.S078) },
                    //{ Const.Setting.S083, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S083), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S084, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S084), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S085, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S085), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S086, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S086), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S089, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S089), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S090, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S090), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S091, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S091), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S093, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S093), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S094, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S094), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S095, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S095), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S096, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S096), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S097, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S097), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S098, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S098), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S099, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S099), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S107, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S107), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S108, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S108), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S109, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S109), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S110, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S110), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S118, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S118), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S124, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S124), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S125, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S125), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S126, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S126), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S127, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S127), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S128, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S128), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S129, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S129), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S133, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S133), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S134, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S134), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S135, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S135), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S137, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S137), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S141, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S141), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S142, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S142), Const.Encryp.PUBLIC_KEY) },
                    //{ Const.Setting.S143, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S143), Const.Encryp.PUBLIC_KEY) },
                    { Const.Setting.S144, _hpSystemSetting.getSettingValue(Const.Setting.S144) },
                    { Const.Setting.S150, _hpSystemSetting.getSettingValue(Const.Setting.S150) },
                    { Const.Setting.S151, _hpSystemSetting.getSettingValue(Const.Setting.S151) },
                    { Const.Setting.S152, _hpSystemSetting.getSettingValue(Const.Setting.S152) },
                    { Const.Setting.S153, _hpSystemSetting.getSettingValue(Const.Setting.S153) },
                    { Const.Setting.S155, _hpSystemSetting.getSettingValue(Const.Setting.S155) },
                    { Const.Setting.S156, _hpSystemSetting.getSettingValue(Const.Setting.S156) },
                    { Const.Setting.S163, _hpSystemSetting.getSettingValue(Const.Setting.S163) },

                    //{ Const.Setting.S147, UEncrypter.EncryptByRSA(_hpSystemSetting.getSettingValue(Const.Setting.S147), Const.Encryp.PUBLIC_KEY) }
                };

                return ResponseOutput.Ok(result);
            }
            catch (Exception ex)
            {
                return ResponseOutput.NotOk();
            }
        }
    }
}