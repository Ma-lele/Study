using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using XHS.Build.Common;
using XHS.Build.Common.Helps;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Services.SpecialEqpDoc
{
    public class HpSpecialEqpDoc: IHpSpecialEqpDoc
    {
        private const string STR_FILEEX = ".file";
        private const string STR_TMPEX = ".tmp";
        private const string STR_DELEX = ".del";
        private const string STR_DOWNLOAD = "download";
        private const string STR_SEDOCID = "SEDOCID";
        private const string STR_SEID = "SEID";
        private const string STR_FILENAME = "filename";
        private const string STR_RESOURSE = "resourse";
        private const string STR_WEB_CONFIG = "Web.config";

        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ISpecialEqpDocService _specialEqpDocService;
        public HpSpecialEqpDoc(IHpSystemSetting hpSystemSetting, IHostingEnvironment hostingEnvironment, ISpecialEqpDocService specialEqpDocService)
        {
            _hpSystemSetting = hpSystemSetting;
            _hostingEnvironment = hostingEnvironment;
            _specialEqpDocService = specialEqpDocService;
        }
        /// <summary>
        /// 添加文档
        /// </summary>
        /// <param name="file">POST提交的文件</param>
        /// <param name="param">其他信息</param>
        /// <param name="limitsize">压缩后最大size（k）</param>
        /// <returns></returns>
        public int doRegist(IFormFile file, DBParam param, int limitsize)
        {
            int result = 0;
            string DOCUMENTID = string.Empty;
            string rootPath = Path.Combine(_hpSystemSetting.getSettingValue(Const.Setting.S030), _hpSystemSetting.getSettingValue(Const.Setting.S056));

            try
            {
                //第一次创建目录，需要添加文件夹下所有文件的web访问的配置文件
                if (!UFile.IsExistDirectory(rootPath))
                {
                    UFile.CreateDirectory(rootPath);
                    UFile.Copy(Path.Combine(_hostingEnvironment.ContentRootPath, STR_RESOURSE, STR_WEB_CONFIG), Path.Combine(rootPath, STR_WEB_CONFIG));
                }

                string path = Path.Combine(rootPath, param.Get(STR_SEID).ToString());//项目目录
                string tmpFileName = Path.Combine(path, Path.GetRandomFileName() + Path.GetExtension(file.FileName));
                if (UFile.SaveFile(file, tmpFileName))
                {
                    DOCUMENTID = _specialEqpDocService.doInsert(param);
                    if (string.IsNullOrEmpty(DOCUMENTID))
                        UFile.DeleteFile(tmpFileName);
                    else
                    {
                        //UFile.Move(tmpFileName, Path.Combine(path, DOCUMENTID.ToLower() + Path.GetExtension(file.FileName)));
                        CompressImage(tmpFileName, Path.Combine(path, DOCUMENTID.ToLower() + Path.GetExtension(file.FileName)), 80, limitsize);
                        UFile.DeleteFile(tmpFileName);
                        result = 1;
                    }
                }
            }
            catch (Exception ex)
            {
                //ULog.WriteError(ex.Message, Properties.Settings.Default.AppName);
                return 0;
            }

            return result;
        }

        /// <summary>
        /// 删除文档
        /// </summary>
        /// <param name="SEDOCID">文档ID</param>
        /// <returns></returns>
        public int doDelete(string id)
        {
            int result = 0;
            DataRow dr = _specialEqpDocService.doDelete(id);
            if (dr == null)
                return result;
            else
                result = 1;

            string SEDOCID = dr[STR_SEDOCID].ToString();
            string SEID = dr[STR_SEID].ToString();
            string FILENAME = dr[STR_FILENAME].ToString();
            string path = Path.Combine(_hpSystemSetting.getSettingValue(Const.Setting.S030), _hpSystemSetting.getSettingValue(Const.Setting.S056), SEID);
            string sourceFileName = Path.Combine(path, SEDOCID + Path.GetExtension(FILENAME));
            try
            {
                UFile.DeleteFile(sourceFileName);
            }
            catch (Exception ex)
            {
                //ULog.WriteError(ex.Message, Properties.Settings.Default.AppName);
                return 0;
            }

            return result;
        }

        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片地址</param>
        /// <param name="dFile">压缩后保存图片地址</param>
        /// <param name="flag">压缩质量（数字越小压缩率越高）1-100</param>
        /// <param name="size">压缩后图片的最大大小</param>
        /// <param name="sfsc">是否是第一次调用</param>
        /// <returns></returns>
        public bool CompressImage(string sFile, string dFile, int flag = 90, int size = 300, bool sfsc = true)
        {
            //如果是第一次调用，原始图像的大小小于要压缩的大小，则直接复制文件，并且返回true
            FileInfo firstFileInfo = new FileInfo(sFile);
            if (sfsc == true && firstFileInfo.Length < size * 1024)
            {
                firstFileInfo.CopyTo(dFile);
                return true;
            }
            Image iSource = Image.FromFile(sFile);
            ImageFormat tFormat = iSource.RawFormat;
            int dHeight = iSource.Height / 2;
            int dWidth = iSource.Width / 2;
            int sW = 0, sH = 0;
            //按比例缩放
            Size tem_size = new Size(iSource.Width, iSource.Height);
            if (tem_size.Width > dHeight || tem_size.Width > dWidth)
            {
                if ((tem_size.Width * dHeight) > (tem_size.Width * dWidth))
                {
                    sW = dWidth;
                    sH = (dWidth * tem_size.Height) / tem_size.Width;
                }
                else
                {
                    sH = dHeight;
                    sW = (tem_size.Width * dHeight) / tem_size.Height;
                }
            }
            else
            {
                sW = tem_size.Width;
                sH = tem_size.Height;
            }

            Bitmap ob = new Bitmap(dWidth, dHeight);
            Graphics g = Graphics.FromImage(ob);

            g.Clear(Color.WhiteSmoke);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            g.DrawImage(iSource, new Rectangle((dWidth - sW) / 2, (dHeight - sH) / 2, sW, sH), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);

            g.Dispose();

            //以下代码为保存图片时，设置压缩质量
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;

            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int x = 0; x < arrayICI.Length; x++)
                {
                    if (arrayICI[x].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[x];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径
                    FileInfo fi = new FileInfo(dFile);
                    if (fi.Length > 1024 * size)
                    {
                        flag = flag - 10;
                        CompressImage(sFile, dFile, flag, size, false);
                    }
                }
                else
                {
                    ob.Save(dFile, tFormat);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                iSource.Dispose();
                ob.Dispose();
            }
        }
    }
}
