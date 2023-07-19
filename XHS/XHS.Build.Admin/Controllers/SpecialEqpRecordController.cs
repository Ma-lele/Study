using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XHS.Build.Admin.Attributes;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Common.Response;
using XHS.Build.Model.Models;
using XHS.Build.Services.SpecialEqp;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Admin.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Permission]
    [Authorize]
    public class SpecialEqpRecordController : ControllerBase
    {
        private readonly IUser _user;
        private readonly ISpecialEqpRecordService _specialEqpService;
        private readonly IHpSystemSetting _hpSystemSetting;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="specialEqpService"></param>
        /// <param name="hpSystemSetting"></param>
        public SpecialEqpRecordController(IUser user, ISpecialEqpRecordService specialEqpService, IHpSystemSetting hpSystemSetting)
        {
            _specialEqpService = specialEqpService;
            _user = user;
            _hpSystemSetting = hpSystemSetting;
        }

        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="groupid"></param>
        /// <param name="keyword">搜索名称</param>
        /// <param name="size"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetList(int groupid = 0, string keyword = "", int page = 1, int size = 20)
        {
            if (_user.GroupId > 0)
            {
                groupid = _user.GroupId;
            }
            var data = await _specialEqpService.GetSiteSpecialEqpPageList(groupid, keyword, page, size);

            return ResponseOutput.Ok(data);
        }

        /// <summary>
        /// 分组
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Groups()
        {
            return ResponseOutput.Ok(await _specialEqpService.GetGroupCount());
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IResponseOutput> Post(SpecialEqpRecordInputDto input)
        {
            if (input == null)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var dbExist = await _specialEqpService.Query(f => f.rightno == input.rightno);
            if (dbExist.Any())
            {
                return ResponseOutput.NotOk("该产权备案号已存在");
            }
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            var entity = await _specialEqpService.AddEntity(input);

            //string domain = _hpSystemSetting.getSettingValue(Const.Setting.S016);
            //string folder = _hpSystemSetting.getSettingValue(Const.Setting.S106);
            //string tmpPath = Path.Combine(_hpSystemSetting.getSettingValue(Const.Setting.S030), 
            //    _hpSystemSetting.getSettingValue(Const.Setting.S018));
            //string savePath = Path.Combine(domain, folder, input.GROUPID.ToString(),input.SITEID.ToString(), entity.SERID.ToString());
            //UFile.CreateDirectory(savePath);
            //input.fileList.ForEach(async ii => {
            //    if (UFile.IsExistFile(tmpPath + "/" + ii.filename))
            //    {
            //        UFile.Move(tmpPath + "/" + ii.filename, savePath + "/" + ii.filename);
            //    }
            //    GCSpecialEqpRecordProof proof = new GCSpecialEqpRecordProof { 
            //        SERPROOFID = Guid.Parse(ii.filename.Split('.').FirstOrDefault()),
            //        SERID = entity.SERID,
            //        filename = ii.name,
            //        filesize = UFile.GetFileSize(savePath + "/" + ii.filename),
            //        createtime = DateTime.Now
            //    };

            //    await _specialEqpService.AddProof(proof);
            //});
            var proofList = MoveFile(input.GROUPID, input.SITEID, entity.SERID, input.fileList);
            await _specialEqpService.AddProof(proofList);

            return entity == null ? ResponseOutput.NotOk("添加数据错误") : ResponseOutput.Ok("新增成功");
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IResponseOutput> Delete(int id)
        {
            if (id <= 0)
            {
                return ResponseOutput.NotOk("请选择需要删除的数据");
            }

            bool isDel = await _specialEqpService.DeleteById(id);
            if (isDel)
            {
                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("删除失败");
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPut]
        public async Task<IResponseOutput> Put(SpecialEqpRecordUpdateDto input)
        {
            if (input == null || input.SERID <= 0)
            {
                return ResponseOutput.NotOk("请填写信息");
            }
            var dbExist = await _specialEqpService.Query(f => f.rightno == input.rightno && f.SERID != input.SERID);
            if (dbExist.Any())
            {
                return ResponseOutput.NotOk("该产权备案号已存在");
            }
            input.operatedate = DateTime.Now;
            input.@operator = _user.Name;
            bool suc = await _specialEqpService.Update(input);
            if (suc)
            {
                if (input.fileDelList != null && input.fileDelList.Count > 0)
                {
                    var delList = new List<GCSpecialEqpRecordProof>();
                    input.fileDelList.ForEach(ii =>
                    {
                        var pf = new GCSpecialEqpRecordProof
                        {
                            SERPROOFID = Guid.Parse(ii),//Guid.Parse(ii.Replace('.' + ii.Split('.').LastOrDefault(), "").Split('/').LastOrDefault()),
                            SERID = input.SERID
                        };
                        delList.Add(pf);
                    });
                    await _specialEqpService.DelImgs(delList);
                }
                if (input.fileList != null && input.fileList.Where(ii => ii.status == "ready").Any())
                {
                    var addFileList = MoveFile(input.GROUPID, input.SITEID, input.SERID, input.fileList.Where(ii => ii.status == "ready").ToList());
                    await _specialEqpService.AddProof(addFileList);
                }

                return ResponseOutput.Ok();
            }
            else
            {
                return ResponseOutput.NotOk("修改失败");
            }
        }

        /// <summary>
        /// 获取详细
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> Get(string id)
        {
            return ResponseOutput.Ok(await _specialEqpService.QueryById(id));
        }


        /// <summary>
        /// 获取监测对象下的特种设备列表
        /// </summary>
        /// <param name="GROUPID"></param>
        /// <param name="SITEID"></param>
        /// <param name="setype"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetEqp(int GROUPID,int SITEID,int setype)
        {
            return ResponseOutput.Ok(await _specialEqpService.GetEqp(GROUPID, SITEID, setype));
        }


        /// <summary>
        /// 获取图片
        /// </summary>
        /// <param name="GROUPID"></param>
        /// <param name="SITEID"></param>
        /// <param name="SERID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IResponseOutput> GetEqpProof(int GROUPID,int SITEID, int SERID)
        {
            var result = await _specialEqpService.GetImgs(SERID);
            string path = $"http://{_hpSystemSetting.getSettingValue(Const.Setting.S015)}/"
                    + $"{ _hpSystemSetting.getSettingValue(Const.Setting.S106)}/"
                    + GROUPID.ToString() + "/"
                    + SITEID.ToString() + "/"
                    + SERID.ToString();

            result.ForEach(ii => {
                ii.id = ii.url;
                if (ii.name.StartsWith("http"))
                {
                    ii.url = ii.name;
                }
                else
                {
                    ii.url = path + "/" + ii.url + "." + ii.name.Split('.').LastOrDefault();
                }              
            });


            return ResponseOutput.Ok(result);
        }


        private List<GCSpecialEqpRecordProof> MoveFile(int groupid,int siteid,int serid,List<FileInput> fileList)
        {
            List<GCSpecialEqpRecordProof> result = new List<GCSpecialEqpRecordProof>();
            string domain = _hpSystemSetting.getSettingValue(Const.Setting.S016);
            string folder = _hpSystemSetting.getSettingValue(Const.Setting.S106);
            string tmpPath = Path.Combine(_hpSystemSetting.getSettingValue(Const.Setting.S030),
                _hpSystemSetting.getSettingValue(Const.Setting.S018));
            string savePath = Path.Combine(domain, folder, groupid.ToString(), siteid.ToString(), serid.ToString());
            UFile.CreateDirectory(savePath);

            fileList.ForEach(ii => {
                if (UFile.IsExistFile(tmpPath + "/" + ii.filename))
                {
                    UFile.Move(tmpPath + "/" + ii.filename, savePath + "/" + ii.filename);
                }

                GCSpecialEqpRecordProof proof = new GCSpecialEqpRecordProof
                {
                    SERPROOFID = Guid.Parse(ii.filename.Split('.').FirstOrDefault()),
                    SERID = serid,
                    filename = ii.name,
                    filesize = UFile.GetFileSize(savePath + "/" + ii.filename),
                    createtime = DateTime.Now
                };

                result.Add(proof);
            });

            return result;
        }
    }
}
