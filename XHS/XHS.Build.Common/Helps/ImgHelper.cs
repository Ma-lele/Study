using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Text;

namespace XHS.Build.Common.Helps
{
    public class ImgHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="base64Str"></param>
        /// <param name="path"></param>
        /// <param name="imgName">不带后缀</param>
        /// <returns></returns>
        public static bool Base64ToImage(string base64Str, string path, string imgName)
        {
            try
            {
                path = path + imgName + ".jpg";
                string tmpRootDir = Path.GetDirectoryName(path); //获取目录 
                if (!Directory.Exists(tmpRootDir))
                {
                    Directory.CreateDirectory(tmpRootDir);
                }
                byte[] photoBytes = Convert.FromBase64String(base64Str);
                //创建一个新文件，将指定的字节数组写入该文件，然后关闭文件。如果目标文件已存在，则将覆盖它。
                File.WriteAllBytes(path, photoBytes);
                return true;
            }
            catch
            {
                return false;
            }

        }

        #region Image To base64
        private static Image UrlToImage(string url)
        {
            WebClient mywebclient = new WebClient();
            byte[] Bytes = mywebclient.DownloadData(url);
            using (MemoryStream ms = new MemoryStream(Bytes))
            {
                Image outputImg = Image.FromStream(ms);
                return outputImg;
            }
        }
        /// <summary>
        /// Image 转成 base64
        /// </summary>
        /// <param name="fileFullName"></param>
        private static string ImageToBase64(Image img)
        {
            try
            {
                Bitmap bmp = new Bitmap(img);
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                return Convert.ToBase64String(arr);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static string ImageToBase64(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }
            return ImageToBase64(UrlToImage(url));
        }
        #endregion

        /// <summary>
        /// 根据文件流压缩图片（如果不想保留原文件）
        /// </summary>
        /// <param name="stream">上传文件</param>
        /// <param name="dHeight">等比例压缩高</param>
        /// <param name="dWidth"></param>
        /// <param name="flag">压缩 1-100 越小压缩越多</param>
        /// <param name="outstream">返回文件</param>
        /// <returns></returns>
        public static bool GetPictureStreem(Stream stream, int dHeight, int dWidth, int flag, Stream outstream)
        {
            //可以直接从流里边得到图片,这样就可以不先存储一份了
            System.Drawing.Image iSource = System.Drawing.Image.FromStream(stream);

            //如果为参数为0就保持原图片
            if (dHeight == 0)
            {
                dHeight = iSource.Height;
            }
            if (dWidth == 0)
            {
                dWidth = iSource.Width;
            }

            ImageFormat tFormat = iSource.RawFormat;
            int sW, sH;

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
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

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
                    //可以存储在流里边;
                    ob.Save(outstream, jpegICIinfo, ep);

                }
                else
                {
                    ob.Save(outstream, tFormat);
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

        /// <summary>
        /// 图片压缩（需要原图，根据原图压缩）
        /// </summary>
        /// <param name="sFile">原图片位置</param>
        /// <param name="dFile">压缩后图片位置</param>
        /// <param name="dHeight">图片压缩后的高度</param>
        /// <param name="dWidth">图片压缩后的宽度</param>
        /// <param name="flag">图片压缩比0-100,数值越小压缩比越高，失真越多</param>
        /// <returns></returns>
        public static bool GetPicture(string sFile, string dFile, int dHeight, int dWidth, int flag)
        {
            System.Drawing.Image iSource = System.Drawing.Image.FromFile(sFile);
            //如果为参数为0就保持原图片的高宽嘛（不然想保持原图外面还要去读取一次）
            if (dHeight == 0)
            {
                dHeight = iSource.Height;
            }
            if (dWidth == 0)
            {
                dWidth = iSource.Width;
            }

            ImageFormat tFormat = iSource.RawFormat;
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
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

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
