using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Helps;
using XHS.Build.Model.Base;
using XHS.Build.Model.Models;
using XHS.Build.Repository.Base;

namespace XHS.Build.Services.Center
{
    public class CenterFile : ICenterFile
    {

        /// <summary>
        /// 允许上传文件扩展名
        /// </summary>
        private static string[] IMG_EXT = new string[] { ".jpg", ".jpeg", ".png" };

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
        /// <summary>
        /// 压缩图前缀
        /// </summary>
        private const string COMPRESS_PREFIX = "comp_";
        private readonly ILogger<CenterFile> _logger;



        private readonly bool isday = false;
        private static string day = string.Empty;
        private readonly string domainName = string.Empty;
        private static string official = string.Empty;
        private static string temporary = string.Empty;
        static string year = DateTime.Now.Year.ToString();
        static string month = DateTime.Now.Month.ToString();
        static string root = Environment.CurrentDirectory.ToString();//获取程序目录
        static string date = string.Empty;
        string rootPath = string.Empty;
        string tempPath = string.Empty;
        private readonly IConfiguration _configuration;
        private readonly IBaseRepository<CenterFileEntity> _centerFileRepository;

        public CenterFile(IConfiguration configuration, IBaseRepository<CenterFileEntity> envRepository)
        {
            isday = configuration.GetSection("Filesupload:isday").Get<bool>();//是否启用日
            day = isday ? DateTime.Now.Day.ToString() : "";
            domainName = configuration.GetSection("Filesupload:domainName").Get<string>();//域名地址
            official = configuration.GetSection("Filesupload:official").Get<string>();
            temporary = configuration.GetSection("Filesupload:temporary").Get<string>();
            date = Path.Combine(year, month, day);//拼日期
            rootPath = Path.Combine(root, official, date);//正式文件夹地址;
            tempPath = Path.Combine(root, temporary, date);//临时文件夹地址
            _configuration = configuration;
            _centerFileRepository = envRepository;
        }

        public async Task<object> upoadFile(IFormFile file)
        {
            string fileExt = Path.GetExtension(file.FileName).ToLower();//文件扩展名
            string url = null;
            if (!string.IsNullOrEmpty(day))
            {
                url = string.Format($"{domainName}/{official}/{year}/{month}/{day}");
            }
            else
            {
                url = string.Format($"{domainName}/{official}/{year}/{month}");
            }
            CenterFileEntity cfile = new CenterFileEntity();
            cfile.fileurl = string.Format($"{url}/{cfile.UUID.ToString().ToLower()}{fileExt}");
            cfile.physicalpath = Path.Combine(rootPath, cfile.UUID.ToString().ToLower() + fileExt);
            cfile.tmburl = string.Format($"{url}/{THUMB_PREFIX}{cfile.UUID.ToString().ToLower()}{fileExt}");
            cfile.compurl = string.Format($"{url}/{COMPRESS_PREFIX}{cfile.UUID.ToString().ToLower()}{fileExt}");

            string filename = cfile.UUID.ToString().ToLower() + fileExt;//文件名
            string tempFileName = Path.Combine(tempPath, filename);//文件临时地址
            string comFileName = Path.Combine(rootPath, COMPRESS_PREFIX + filename);//压缩图地址
            string tmbFileName = Path.Combine(rootPath, THUMB_PREFIX + filename);//缩略图地址
            int result = 0;
            try
            {
                //创建临时文件夹
                UFile.CreateDirectory(tempPath);
                //上传临时文件夹
                using (var stream = new FileStream(tempFileName, FileMode.Create))
                {
                    if (IMG_EXT.Contains(fileExt))
                    {
                        file.CopyTo(stream);
                    }
                    else
                    {
                        await file.CopyToAsync(stream);
                    }
                }

                //创建正式文件夹
                UFile.CreateDirectory(rootPath);
                if (IMG_EXT.Contains(fileExt))
                {
                    //图片
                    //返回自增列
                    result = await _centerFileRepository.Db.Insertable<CenterFileEntity>(cfile).ExecuteReturnIdentityAsync();
                    if (result > 0)
                    {
                        //压缩图片
                        if (await pressPicture(fileExt, tempFileName, cfile.physicalpath, tmbFileName, comFileName, result))
                        {
                            //修改状态
                            await _centerFileRepository.Db.Updateable<CenterFileEntity>().SetColumns(it => new CenterFileEntity() { bsuccess = 1 }).Where(it => it.FILEID == result).ExecuteCommandAsync();
                            return new { fileId = cfile.UUID, fileurl = cfile.fileurl, tmburl = cfile.tmburl, compurl = cfile.compurl };
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        //删除临时文件
                        UFile.DeleteFile(tempFileName);
                        return null;
                    }
                }
                else
                {
                    //文件
                    result = await _centerFileRepository.Db.Insertable<CenterFileEntity>(cfile).IgnoreColumns("tmburl", "compurl").ExecuteReturnIdentityAsync();
                    if (result > 0)
                    {
                        UFile.Move(tempFileName, cfile.physicalpath);
                        //修改状态
                        await _centerFileRepository.Db.Updateable<CenterFileEntity>().SetColumns(it => new CenterFileEntity() { bsuccess = 1 }).Where(it => it.FILEID == result).ExecuteCommandAsync();
                        return new { fileId = cfile.UUID, fileurl = cfile.fileurl };
                    }
                    else
                    {
                        //删除临时文件
                        UFile.DeleteFile(tempFileName);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                //删除压缩到正式文件夹图片
                UFile.DeleteFile(comFileName);
                //删除移正式文件夹图片
                UFile.DeleteFile(cfile.physicalpath);
                //删除临时文件
                UFile.DeleteFile(tempFileName);
                //删除缩略图图
                UFile.DeleteFile(tmbFileName);
                //删除记录
                await _centerFileRepository.Db.Deleteable<CenterFileEntity>().Where(it => it.FILEID == result).ExecuteCommandAsync();
                _logger.LogError(ex.Message);
                return null;
            }
            finally
            {
                //删除临时文件
                UFile.DeleteFile(tempFileName);
            }
        }



        public async Task<object> downloadFile(string fileUrl)
        {
            string url = null;
            if (!string.IsNullOrEmpty(day))
            {
                url = string.Format($"{domainName}/{official}/{year}/{month}/{day}");
            }
            else
            {
                url = string.Format($"{domainName}/{official}/{year}/{month}");
            }
            string fileExt = fileUrl.Remove(0, fileUrl.LastIndexOf('.')).ToLower();//文件扩展名

            CenterFileEntity cfile = new CenterFileEntity();
            cfile.fileurl = string.Format($"{url}/{cfile.UUID.ToString().ToLower()}{fileExt}");
            cfile.physicalpath = Path.Combine(rootPath, cfile.UUID.ToString().ToLower() + fileExt);
            cfile.tmburl = string.Format($"{url}/{THUMB_PREFIX}{cfile.UUID.ToString().ToLower()}{fileExt}");
            cfile.compurl = string.Format($"{url}/{COMPRESS_PREFIX}{cfile.UUID.ToString().ToLower()}{fileExt}");
            cfile.originalurl = fileUrl;
            string filename = cfile.UUID + fileExt;//文件名
            string tempFileName = Path.Combine(tempPath, filename);//文件临时地址
            string comFileName = Path.Combine(rootPath, COMPRESS_PREFIX + filename);//压缩图地址
            string tmbFileName = Path.Combine(rootPath, THUMB_PREFIX + filename);//缩略图地址
            int result = 0;


            try
            {
                //创建临时文件夹
                UFile.CreateDirectory(tempPath);
                //上传临时文件夹
                using (var mywebclient = new WebClient())
                {
                    if (IMG_EXT.Contains(fileExt))
                    {
                        mywebclient.DownloadFile(fileUrl, tempFileName);
                    }
                    else
                    {
                        await mywebclient.DownloadFileTaskAsync(fileUrl, tempFileName);
                    }
                }

                //创建正式文件夹
                UFile.CreateDirectory(rootPath);
                if (IMG_EXT.Contains(fileExt))
                {
                    //图片
                    result = await _centerFileRepository.Db.Insertable<CenterFileEntity>(cfile).ExecuteReturnIdentityAsync();
                    if (result > 0)
                    {
                        //压缩图片
                        if (await pressPicture(fileExt, tempFileName, cfile.physicalpath, tmbFileName, comFileName, result))
                        {
                            //修改状态
                            await _centerFileRepository.Db.Updateable<CenterFileEntity>().SetColumns(it => new CenterFileEntity() { bsuccess = 1 }).Where(it => it.FILEID == result).ExecuteCommandAsync();
                            return new { fileId = cfile.UUID, fileurl = cfile.fileurl, tmburl = cfile.tmburl, compurl = cfile.compurl };
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        //删除临时文件
                        UFile.DeleteFile(tempFileName);
                        return null;
                    }
                }
                else
                {
                    //文件
                    result = await _centerFileRepository.Db.Insertable<CenterFileEntity>(cfile).IgnoreColumns("tmburl", "compurl").ExecuteReturnIdentityAsync();
                    if (result > 0)
                    {
                        UFile.Move(tempFileName, cfile.physicalpath);
                        //修改状态
                        await _centerFileRepository.Db.Updateable<CenterFileEntity>().SetColumns(it => new CenterFileEntity() { bsuccess = 1 }).Where(it => it.FILEID == result).ExecuteCommandAsync();
                        return new { fileId = cfile.UUID, fileurl = cfile.fileurl };
                    }
                    else
                    {
                        //删除临时文件
                        UFile.DeleteFile(tempFileName);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                //删除临时文件
                UFile.DeleteFile(cfile.physicalpath);
                _logger.LogError(ex.Message);
                return null;
            }
            finally
            {
                //删除临时文件
                UFile.DeleteFile(tempFileName);
            }
        }

        //压缩图片
        public async Task<bool> pressPicture(string fileExt, string tempFileName, string realFileName, string tmbFileName, string comFileName, int result)
        {

            Image iSource = null;
            try
            {
                //是图片的情况，压缩图片
                UFile.CompressImage(tempFileName, comFileName);
                UFile.Move(tempFileName, realFileName);
                iSource = Image.FromFile(realFileName);
                FileStream pFileStream = null;

                //生成缩略图 tmb_XXXXXX.jpg
                pFileStream = new FileStream(realFileName, FileMode.Open, FileAccess.Read);
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
            catch (Exception ex)
            {
                //删除压缩到正式文件夹图片
                UFile.DeleteFile(comFileName);
                //删除移正式文件夹图片
                UFile.DeleteFile(realFileName);
                //删除临时文件
                UFile.DeleteFile(tempFileName);
                //删除缩略图
                UFile.DeleteFile(tmbFileName);
                //删除记录
                await _centerFileRepository.Db.Deleteable<CenterFileEntity>().Where(it => it.FILEID == result).ExecuteCommandAsync();
                _logger.LogError(ex.Message);
                return false;
            }
            finally
            {
                if (iSource != null)
                    iSource.Dispose();
            }
            return true;
        }

        //删除文件
        public async Task<bool> deleteFile(string FileId)
        {
            return await _centerFileRepository.Db.Updateable<CenterFileEntity>().SetColumns(it => new CenterFileEntity() { bsuccess = 0 }).Where(it => it.UUID.ToString() == FileId).ExecuteCommandHasChangeAsync();
        }
    }
}
