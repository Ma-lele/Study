using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Model.Models;
using XHS.Build.Services.SystemSetting;
using XHS.Build.Common.Util;
using static XHS.Build.Model.Models.FileEntity;

namespace XHS.Build.Services.File
{
    public class HpFileDoc : IHpFileDoc
    {
        /// <summary>
        /// 允许上传文件扩展名
        /// </summary>
        private static string[] IMG_EXT = new string[] { ".jpg", ".jpeg", ".png" };
        private const string STR_RESOURSE = "resourse";
        private const string STR_PLANE = "plane";
        private const string STR_STATIC = "static";
        /// <summary>
        /// 缩略图宽
        /// </summary>
        private const int THUMB_WIDTH = 120;
        /// <summary>
        /// 缩略图高
        /// </summary>
        private const int THUMB_HEIGHT = 80;
        /// <summary>
        /// 缩略图前缀
        /// </summary>
        private const string THUMB_PREFIX = "tmb_";
        private readonly ILogger<HpFileDoc> _logger;


        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IFileService _FileService;
        public HpFileDoc(IHpSystemSetting hpSystemSetting, IHostingEnvironment hostingEnvironment, IFileService FileService, ILogger<HpFileDoc> logger)
        {
            _hpSystemSetting = hpSystemSetting;
            _hostingEnvironment = hostingEnvironment;
            _FileService = FileService;
            _logger = logger;
        }

        /// <summary>
        /// 添加文档
        /// </summary>
        /// <param name="fep">文件参数</param>
        /// <param name="creater">创建者</param>
        /// <returns></returns>
        public async Task<string> doRegist(IFormFile file, FileEntity.FileEntityParam fep, string creater)
        {
            string FILEID = Guid.NewGuid().ToString();
            string savePath = Path.Combine(_hpSystemSetting.getSettingValue(Const.Setting.S030), _hpSystemSetting.getSettingValue(Const.Setting.S018));
            string fileExt = Path.GetExtension(file.FileName).ToLower();//文件扩展名
            try
            {
                //第一次创建目录，需要添加文件夹下所有文件的web访问的配置文件
                UFile.CreateDirectory(savePath);

                FileEntity fe = new FileEntity();
                fe.FILEID = FILEID;
                fe.GROUPID = fep.GROUPID;
                fe.SITEID = fep.SITEID;
                fe.linkid = fep.linkid;
                fe.filetype = fep.filetype.ToString();
                fe.exparam = fep.exparam;
                fe.filename = file.FileName;
                fe.filesize = file.Length;
                fe.creater = creater;
                int ret = await _FileService.Add(fe);
                if (ret > 0)
                {
                    string filename = Path.Combine(savePath, FILEID + fileExt);
                    using (var stream = new FileStream(filename, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    // byte[] buff = Convert.FromBase64String(fep.fileString);
                    // UFile.CreateFile(Path.Combine(filename), buff);//保存文件                   
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
            return FILEID + fileExt;
        }

        public async Task<FileOutput> AddBase64Img(string base64string, FileEntityParam fep = null)
        {
            string FILEID = Guid.NewGuid().ToString();
            //2021年1月12日 增加是否需要入库
            if (fep == null)
            {
                string savePath = Path.Combine(_hpSystemSetting.getSettingValue(Const.Setting.S030), _hpSystemSetting.getSettingValue(Const.Setting.S018)) + "/";
                bool suc = ImgHelper.Base64ToImage(base64string, savePath, FILEID);
                if (suc)
                {
                    var ret = await _FileService.AddEntity(new FileEntity() { FILEID = FILEID, bdel = 1, creater = "", filename = FILEID + ".jpg", filetype = "", filesize = 0 });
                    if (ret != null)
                    {
                        return new FileOutput() { FileId = FILEID, FileUrl = _FileService.GetImageTempUrl(ret) };
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                //不入库，生成文件和tmb文件
                string rootPath = Path.Combine(_hpSystemSetting.getSettingValue(Const.Setting.S030), STR_RESOURSE, fep.GROUPID.ToString(), fep.filetype.ToString());
                string path = Path.Combine(rootPath, fep.SITEID.ToString(), fep.linkid.ToString());//项目目录
                bool suc = ImgHelper.Base64ToImage(base64string, path + "/", FILEID);
                if (suc)
                {
                    Image iSource = Image.FromFile(Path.Combine(path, FILEID + ".jpg"));
                    try
                    {
                        FileStream pFileStream = null;
                        //生成缩略图 tmb_XXXXXX.jpg
                        string tmbFileName = Path.Combine(path, THUMB_PREFIX + FILEID + ".jpg");
                        pFileStream = new FileStream(Path.Combine(path, FILEID + ".jpg"), FileMode.Open, FileAccess.Read);
                        BinaryReader r = new BinaryReader(pFileStream);
                        r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开
                        byte[] pReadByte = r.ReadBytes((int)r.BaseStream.Length);

                        if (iSource.Width > iSource.Height)
                        {
                            UFile.MakeThumbnail(pReadByte, tmbFileName, THUMB_WIDTH, THUMB_HEIGHT, "CUT", ".jpg");
                        }
                        else
                        {
                            UFile.MakeThumbnail(pReadByte, tmbFileName, THUMB_HEIGHT, THUMB_WIDTH, "CUT", ".jpg");
                        }
                        r.Close();
                        pFileStream.Close();
                    }
                    catch
                    {

                    }
                    finally
                    {
                        iSource.Dispose();
                    }
                    return new FileOutput() { FileId = FILEID, FileUrl = "http://" + _hpSystemSetting.getSettingValue(Const.Setting.S034) + "/resourse/" + fep.GROUPID + "/" + fep.filetype + "/" + fep.SITEID + "/" + fep.linkid + "/" + "tmb_" + FILEID + ".jgp" };

                }
                else
                {
                    return null;
                }
            }

        }

        /// <summary>
        /// 删除文档
        /// </summary>
        /// <param name="GROUPID">组编号</param>
        /// <param name="SITEID">监测点编号</param>
        /// <param name="linkid">链接ID</param>
        /// <param name="filetype">功能类型</param>
        /// <returns></returns>
        public async Task<int> doDelete(int GROUPID, int SITEID, string linkid, string filetype)
        {
            int result = 0;
            result = await _FileService.doDeleteByLinkid(linkid);
            if (result == 0)
            {
                return result;
            }

            string path = Path.Combine(_hpSystemSetting.getSettingValue(Const.Setting.S030), STR_RESOURSE, GROUPID.ToString(), filetype);

            string linkidpath = Path.Combine(path, SITEID.ToString(), linkid);
            try
            {
                UFile.DeleteDirectory(linkidpath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return 0;
            }

            return result;
        }

        /// <summary>
        /// 添加文档
        /// </summary>
        /// <param name="file">POST提交的文件名</param>
        /// <param name="fep">文件参数</param>
        /// <param name="creater">创建者</param>
        /// <returns></returns>
        public async Task<bool> doUpdate(string file, FileEntity.FileEntityParam fep, string creater)
        {
            string rootPath = Path.Combine(_hpSystemSetting.getSettingValue(Const.Setting.S030), STR_RESOURSE, fep.GROUPID.ToString(), fep.filetype.ToString());
            string tmpPath = Path.Combine(_hpSystemSetting.getSettingValue(Const.Setting.S030), _hpSystemSetting.getSettingValue(Const.Setting.S018));
            string tmpFileName = Path.Combine(tmpPath, file);
            bool ret = false;
            try
            {
                string path = Path.Combine(rootPath, fep.SITEID.ToString(), fep.linkid.ToString());//项目目录
                //创建目录
                UFile.CreateDirectory(path);

                string FILEID = file.Substring(0, file.LastIndexOf("."));
                var files = await _FileService.Query(x => x.FILEID == FILEID);

                if (files.Count > 0)
                {
                    var fileparam = files.First();
                    fileparam.bdel = 0;
                    fileparam.linkid = fep.linkid;
                    fileparam.filetype = fep.filetype.ToString();
                    fileparam.exparam = fep.exparam;
                    if (fep.GROUPID > 0)
                    {
                        fileparam.GROUPID = fep.GROUPID;
                    }
                    if (fep.SITEID > 0)
                    {
                        fileparam.SITEID = fep.SITEID;
                    }
                    ret = await _FileService.Update(fileparam, new List<string> { "bdel", "linkid", "filetype", "exparam", "groupid", "siteid" }, null, string.Format("FILEID='{0}'", FILEID));
                }

                if (ret)
                {
                    string fileExt = Path.GetExtension(file);//文件扩展名
                    string realFileName = Path.Combine(path, file);

                    if (IMG_EXT.Contains(fileExt))
                    {
                        //是图片的情况，压缩图片
                        UFile.CompressImage(tmpFileName, realFileName);
                        Image iSource = Image.FromFile(tmpFileName);
                        try
                        {
                            FileStream pFileStream = null;
                            //生成缩略图 tmb_XXXXXX.jpg
                            string tmbFileName = Path.Combine(path, THUMB_PREFIX + file);
                            pFileStream = new FileStream(tmpFileName, FileMode.Open, FileAccess.Read);
                            BinaryReader r = new BinaryReader(pFileStream);
                            r.BaseStream.Seek(0, SeekOrigin.Begin);    //将文件指针设置到文件开
                            byte[] pReadByte = r.ReadBytes((int)r.BaseStream.Length);

                            if (iSource.Width > iSource.Height)
                            {
                                UFile.MakeThumbnail(pReadByte, tmbFileName, THUMB_WIDTH, THUMB_HEIGHT, "CUT", fileExt);
                            }
                            else
                            {
                                UFile.MakeThumbnail(pReadByte, tmbFileName, THUMB_HEIGHT, THUMB_WIDTH, "CUT", fileExt);
                            }
                            r.Close();
                            pFileStream.Close();
                        }
                        catch
                        {

                        }
                        finally
                        {
                            iSource.Dispose();
                        }
                    }
                    else
                    {
                        UFile.Move(tmpFileName, realFileName);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return ret;
            }

            return ret;
        }

        public async Task<FileOutput> UploadImgWithTmp(IFormFile file, string creater = "")
        {
            string FILEID = Guid.NewGuid().ToString();
            string savePath = Path.Combine(_hpSystemSetting.getSettingValue(Const.Setting.S030), _hpSystemSetting.getSettingValue(Const.Setting.S018));
            string fileExt = Path.GetExtension(file.FileName).ToLower();//文件扩展名
            try
            {
                UFile.CreateDirectory(savePath);

                string filename = Path.Combine(savePath, FILEID + fileExt);
                string tmbFileName = Path.Combine(savePath, THUMB_PREFIX + FILEID + fileExt);
                using (var stream = new FileStream(filename, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                if (ImgHelper.GetPicture(filename, tmbFileName, 0, 0, 50))
                {
                    FileEntity fe = new FileEntity() { FILEID = FILEID, filename = file.FileName, filesize = file.Length, creater = creater, filetype = "" };
                    int ret = await _FileService.Add(fe);
                    if (ret > 0)
                    {
                        return new FileOutput() { FileId = FILEID, FileUrl = _FileService.GetImageTempUrl(fe), FileUrlTemp = _FileService.GetImageTempUrl(fe, true) };
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdateUploadImgTmp(string fileid, FileEntity.FileEntityParam fep, string creater = "")
        {
            var FileEntitys = await _FileService.Query(f => f.FILEID == fileid);//.QueryById(fileid);
            if (FileEntitys == null || !FileEntitys.Any())
            {
                return false;
            }
            var FileEntity = FileEntitys.FirstOrDefault();
            string rootPath = Path.Combine(_hpSystemSetting.getSettingValue(Const.Setting.S030), STR_RESOURSE, fep.GROUPID.ToString(), fep.filetype.ToString());
            string tmpPath = Path.Combine(_hpSystemSetting.getSettingValue(Const.Setting.S030), _hpSystemSetting.getSettingValue(Const.Setting.S018));
            string fileExt = Path.GetExtension(FileEntity.filename);//文件扩展名
            string FileName = Path.Combine(tmpPath, fileid + fileExt);//临时原图
            string tmpFileName = Path.Combine(tmpPath, THUMB_PREFIX + fileid + fileExt);//临时压缩图

            try
            {
                string path = Path.Combine(rootPath, fep.SITEID.ToString(), fep.linkid.ToString());//项目目录
                //创建目录
                UFile.CreateDirectory(path);
                //先移动文件
                string RealFileName = Path.Combine(path, fileid + fileExt);//目标原图
                string tmpRealFileName = Path.Combine(path, THUMB_PREFIX + fileid + fileExt);//目标压缩图
                UFile.Copy(FileName, RealFileName);
                UFile.Copy(tmpFileName, tmpRealFileName);

                //更新数据库
                FileEntity.bdel = 0;
                FileEntity.creater = creater;
                FileEntity.linkid = fep.linkid;
                FileEntity.filetype = fep.filetype.ToString();
                FileEntity.exparam = fep.exparam;
                if (fep.GROUPID > 0)
                {
                    FileEntity.GROUPID = fep.GROUPID;
                }
                if (fep.SITEID > 0)
                {
                    FileEntity.SITEID = fep.SITEID;
                }
                var ret = await _FileService.Update(FileEntity);
                return ret;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SaveSiteMapPlane(IFormFile file, int GROUPID, int SITEID)
        {
            string fileExt = Path.GetExtension(file.FileName).ToLower();//文件扩展名
            string savePath = Path.Combine(_hpSystemSetting.getSettingValue(Const.Setting.S023), GROUPID.ToString());
            try
            {
                if (!UFile.IsExistDirectory(savePath))
                {
                    UFile.CreateDirectory(savePath);
                }

                string filename = Path.Combine(savePath, SITEID.ToString() + fileExt);
                string tmbFileName = Path.Combine(savePath, THUMB_PREFIX + SITEID.ToString() + fileExt);
                using (var stream = new FileStream(filename, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                if (ImgHelper.GetPicture(filename, tmbFileName, 0, 0, 50))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 把平面图复制到临时文件夹
        /// </summary>
        /// <param name="GROUPID"></param>
        /// <param name="SITEID"></param>
        /// <param name="isForce"></param>
        /// <returns></returns>
        public async Task<string> GetSiteMapPlane(int GROUPID, int SITEID, bool isForce = false)
        {
            if (SITEID <= 0 || GROUPID <=0)
                return "";
            string tmpPath = Path.Combine(_hostingEnvironment.ContentRootPath, _hpSystemSetting.getSettingValue(Const.Setting.S169));
            string tmpFile = UEncrypter.EncryptByMD5(UEncrypter.EncryptBySHA1(SITEID + Const.Symbol.COLON + DateTime.Now.DayOfYear.ToString())) + Const.FileEx.JPG;
            if (isForce || !UFile.IsExistFile(Path.Combine(tmpPath, tmpFile)))//如果临时文件还没有,就生成
            {
                string sourceFile = Path.Combine(_hostingEnvironment.ContentRootPath, _hpSystemSetting.getSettingValue(Const.Setting.S023), GROUPID.ToString(), SITEID.ToString() + Const.FileEx.JPG);
                if (!UFile.IsExistFile(sourceFile))//连源文件都没有就拉倒
                    return "";
                //考个过来
                UFile.Copy(sourceFile, Path.Combine(tmpPath, tmpFile));
                //清掉3天以前的
                UFile.ClearExpiredFile(tmpPath, 3, Const.FileEx.ALL, true, false);
            }
            return STR_STATIC + Const.Symbol.SLASH + _hpSystemSetting.getSettingValue(Const.Setting.S018) + Const.Symbol.SLASH + tmpFile; ;
        }
        
        public async Task<bool> DeleteSiteMapPlane(int GROUPID, int SITEID)
        {
            if (SITEID <= 0 || GROUPID <=0)
                return false;

            string sourceFile = Path.Combine(_hostingEnvironment.ContentRootPath, _hpSystemSetting.getSettingValue(Const.Setting.S023), GROUPID.ToString(), SITEID.ToString() + Const.FileEx.JPG);
            string tmpFile = Path.Combine(_hostingEnvironment.ContentRootPath, _hpSystemSetting.getSettingValue(Const.Setting.S169),
                UEncrypter.EncryptByMD5(UEncrypter.EncryptBySHA1(SITEID + Const.Symbol.COLON + DateTime.Now.DayOfYear.ToString())) + Const.FileEx.JPG);
            try
            {
                //删除临时图
                UFile.DeleteFile(Path.Combine(tmpFile));
                //删除原图
                UFile.DeleteFile(sourceFile);
                return true;
            }
            catch (Exception ex)
            {
                
                return false;
            }
        }
    }
}
