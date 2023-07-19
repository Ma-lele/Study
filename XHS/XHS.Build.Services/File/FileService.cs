using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Services.File
{
    public class FileService : BaseServices<FileEntity>, IFileService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<FileEntity> _FileRepository;
        private readonly IHpSystemSetting _hpSystemSetting;
        public FileService(IBaseRepository<FileEntity> FileRepository, IUser user, IHpSystemSetting hpSystemSetting)
        {
            _user = user;
            _FileRepository = FileRepository;
            BaseDal = FileRepository;
            _hpSystemSetting = hpSystemSetting;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="FILEID">文件ID</param>
        /// <returns>结果</returns>
        public async Task<int> doDelete(string FILEID)
        {
            return await _FileRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spFileDelete", new { FILEID = FILEID });

        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="linkid">链接ID</param>
        /// <returns>结果</returns>
        public async Task<int> doDeleteByLinkid(string linkid)
        {
            return await _FileRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spFileDeleteByLinkid", new { linkid = linkid });

        }

        public string doInsert(DBParam param)
        {
            return _FileRepository.Db.Ado.UseStoredProcedure().GetString("spFileInsert", new
            {
                GROUPID = param.Get("GROUPID"),
                SITEID = param.Get("SITEID"),
                linkid = param.Get("linkid"),
                filetype = param.Get("filetype"),
                exparam = param.Get("exparam"),
                filename = param.Get("filename"),
                filesize = param.Get("filesize"),
                creater = param.Get("creater")
            });
        }


        public int doUpdate(DBParam param)
        {
            return _FileRepository.Db.Ado.UseStoredProcedure().GetInt("spFileUpdate", new
            {
                FILEID = param.Get("FILEID"),
                GROUPID = param.Get("GROUPID"),
                SITEID = param.Get("SITEID"),
                linkid = param.Get("linkid"),
                filetype = param.Get("filetype"),
                exparam = param.Get("exparam"),
                creater = param.Get("creater")
            });
        }

        public async Task<List<FileEntity>> GetFileListByLindId(string linkid)
        {
            return await _FileRepository.Query(f => f.linkid == linkid && f.bdel == 0);
        }

        public string GetImageUrl(FileEntity fileEntity, bool tmb = false)
        {
            string S034 = _hpSystemSetting.getSettingValue("S034");
            if (!string.IsNullOrEmpty(S034) && !string.IsNullOrEmpty(fileEntity.filename) && fileEntity.filename.Split('.').Length == 2)
            {
                return "http://" + S034 + "/resourse/" + fileEntity.GROUPID + "/" + fileEntity.filetype + "/" + fileEntity.SITEID + "/" + fileEntity.linkid + "/" + (tmb ? "tmb_" : "") + fileEntity.FILEID + "." + fileEntity.filename.Split('.')[1];
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetImageTempUrl(FileEntity fileEntity, bool tmb = false)
        {
            string S034 = _hpSystemSetting.getSettingValue("S034");
            if (!string.IsNullOrEmpty(S034) && !string.IsNullOrEmpty(fileEntity.FILEID) && fileEntity.filename.Split('.').Length == 2)
            {
                return "http://" + S034 + "/imgtmp/" + (tmb ? "tmb_" : "") + fileEntity.FILEID + "." + fileEntity.filename.Split('.')[1];
            }
            else
            {
                return string.Empty;
            }
        }

        public async Task DeleteFile()
        {
            var Files = await _FileRepository.Query(f => f.bdel == 1 && f.createdate < DateTime.Now.AddDays(-1));
            if (Files.Any())
            {
                string STR_RESOURSE = "resourse";
                string THUMB_PREFIX = "tmb_";
                var S030 = _hpSystemSetting.getSettingValue(Const.Setting.S030);
                var S018 = _hpSystemSetting.getSettingValue(Const.Setting.S018);
                string RootPath = Path.Combine(S030, S018);
                foreach (var file in Files)
                {
                    //删库
                    if (await _FileRepository.DeleteById(file.FILEID))
                    {
                        //删文件
                        string fileExt = Path.GetExtension(file.filename).ToLower();
                        string fname = Path.Combine(RootPath, file.FILEID + fileExt);
                        string tmbfname = Path.Combine(RootPath, THUMB_PREFIX + file.FILEID + fileExt);
                        UFile.DeleteFile(fname);
                        UFile.DeleteFile(tmbfname);
                        if (!string.IsNullOrEmpty(file.linkid))
                        {
                            fname = Path.Combine(S030, STR_RESOURSE, Convert.ToString(file.GROUPID), Convert.ToString(file.filetype), Convert.ToString(file.SITEID), Convert.ToString(file.linkid), file.FILEID + fileExt);
                            tmbfname = Path.Combine(S030, STR_RESOURSE, Convert.ToString(file.GROUPID), Convert.ToString(file.filetype), Convert.ToString(file.SITEID), Convert.ToString(file.linkid), THUMB_PREFIX + file.FILEID + fileExt);
                            UFile.DeleteFile(fname);
                            UFile.DeleteFile(tmbfname);
                        }
                    }
                }
                //删除RootPath下空文件夹
                //UFile.DeleteEmptyFolders(RootPath);
                UFile.DeleteEmptyFolders(Path.Combine(S030, STR_RESOURSE));
            }
        }
    }
}
