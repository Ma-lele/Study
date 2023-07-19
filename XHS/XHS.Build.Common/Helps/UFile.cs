using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace XHS.Build.Common.Helps
{
    /// <summary> 
    /// 文件操作类 
    /// </summary> 
    public class UFile
    {
        public const string ALL = "*";
        public static bool SaveFile(IFormFile file, string fileName)
        {
            try
            {
                string dictory = Path.GetDirectoryName(fileName);
                CreateDirectory(dictory);
                if (IsExistDirectory(dictory))
                {
                    //file.SaveAs(fileName);
                    using (var stream = new FileStream(fileName, FileMode.Create))
                    {
                         file.CopyTo(stream);
                    }
                }
                    
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #region 检测指定目录是否存在
        /// <summary> 
        /// 检测指定目录是否存在 
        /// </summary> 
        /// <param name="directoryPath">目录的绝对路径</param>         
        public static bool IsExistDirectory(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }
        #endregion

        #region 检测指定文件是否存在
        /// <summary> 
        /// 检测指定文件是否存在,如果存在则返回true。 
        /// </summary> 
        /// <param name="filePath">文件的绝对路径</param>         
        public static bool IsExistFile(string filePath)
        {
            return File.Exists(filePath);
        }
        #endregion

        #region 检测指定目录是否为空
        /// <summary> 
        /// 检测指定目录是否为空 
        /// </summary> 
        /// <param name="directoryPath">指定目录的绝对路径</param>         
        public static bool IsEmptyDirectory(string directoryPath)
        {
            try
            {
                //判断是否存在文件 
                string[] fileNames = GetFileNames(directoryPath);
                if (fileNames.Length > 0)
                {
                    return false;
                }
                //判断是否存在文件夹 
                string[] directoryNames = GetDirectories(directoryPath);
                if (directoryNames.Length > 0)
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                return true;
            }
        }
        #endregion
        #region 检测指定目录中是否存在指定的文件
        /// <summary> 
        /// 检测指定目录中是否存在指定的文件,若要搜索子目录请使用重载方法. 
        /// </summary> 
        /// <param name="directoryPath">指定目录的绝对路径</param> 
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。 
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>         
        public static bool Contains(string directoryPath, string searchPattern)
        {
            try
            {
                //获取指定的文件列表 
                string[] fileNames = GetFileNames(directoryPath, searchPattern, false);
                //判断指定文件是否存在 
                if (fileNames.Length == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary> 
        /// 检测指定目录中是否存在指定的文件 
        /// </summary> 
        /// <param name="directoryPath">指定目录的绝对路径</param> 
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。 
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>  
        /// <param name="isSearchChild">是否搜索子目录</param> 
        public static bool Contains(string directoryPath, string searchPattern, bool isSearchChild)
        {
            try
            {
                //获取指定的文件列表 
                string[] fileNames = GetFileNames(directoryPath, searchPattern, true);
                //判断指定文件是否存在 
                if (fileNames.Length == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
        #region 创建一个目录
        /// <summary> 
        /// 创建一个目录 
        /// </summary> 
        /// <param name="directoryPath">目录的绝对路径</param> 
        public static void CreateDirectory(string directoryPath)
        {
            //如果目录不存在则创建该目录 
            if (!IsExistDirectory(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
        #endregion
        #region 创建一个文件
        /// <summary> 
        /// 创建一个文件。 
        /// </summary> 
        /// <param name="filePath">文件的绝对路径</param> 
        public static void CreateFile(string filePath)
        {
            try
            {
                //如果文件不存在则创建该文件 
                if (!IsExistFile(filePath))
                {
                    //创建一个FileInfo对象 
                    FileInfo file = new FileInfo(filePath);
                    //创建文件 
                    FileStream fs = file.Create();
                    //关闭文件流 
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary> 
        /// 创建一个文件,并将字节流写入文件。 
        /// </summary> 
        /// <param name="filePath">文件的绝对路径</param> 
        /// <param name="buffer">二进制流数据</param> 
        public static void CreateFile(string filePath, byte[] buffer)
        {
            try
            {
                //如果文件不存在则创建该文件 
                if (!IsExistFile(filePath))
                {
                    //创建一个FileInfo对象 
                    FileInfo file = new FileInfo(filePath);
                    //创建文件 
                    FileStream fs = file.Create();
                    //写入二进制流 
                    fs.Write(buffer, 0, buffer.Length);
                    //关闭文件流 
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
        #region 获取文本文件的行数
        /// <summary> 
        /// 获取文本文件的行数 
        /// </summary> 
        /// <param name="filePath">文件的绝对路径</param>         
        public static int GetLineCount(string filePath)
        {
            //将文本文件的各行读到一个字符串数组中 
            string[] rows = File.ReadAllLines(filePath);
            //返回行数 
            return rows.Length;
        }
        #endregion
        #region 获取一个文件的长度
        /// <summary> 
        /// 获取一个文件的长度,单位为Byte 
        /// </summary> 
        /// <param name="filePath">文件的绝对路径</param>         
        public static int GetFileSize(string filePath)
        {
            //创建一个文件对象 
            FileInfo fi = new FileInfo(filePath);
            //获取文件的大小 
            return (int)fi.Length;
        }
        /// <summary> 
        /// 获取一个文件的长度,单位为KB 
        /// </summary> 
        /// <param name="filePath">文件的路径</param>         
        public static double GetFileSizeByKB(string filePath)
        {
            //创建一个文件对象 
            FileInfo fi = new FileInfo(filePath);
            //获取文件的大小 
            return Convert.ToDouble(Convert.ToDouble(fi.Length) / 1024);
        }
        /// <summary> 
        /// 获取一个文件的长度,单位为MB 
        /// </summary> 
        /// <param name="filePath">文件的路径</param>         
        public static double GetFileSizeByMB(string filePath)
        {
            //创建一个文件对象 
            FileInfo fi = new FileInfo(filePath);
            //获取文件的大小 
            return Convert.ToDouble(Convert.ToDouble(fi.Length) / 1024 / 1024);
        }
        #endregion

        #region 获取指定目录中的文件列表
        /// <summary> 
        /// 获取指定目录中所有文件列表 
        /// </summary> 
        /// <param name="directoryPath">指定目录的绝对路径</param>         
        public static string[] GetFileNames(string directoryPath)
        {
            //如果目录不存在，则抛出异常 
            if (!IsExistDirectory(directoryPath))
            {
                throw new FileNotFoundException();
            }
            //获取文件列表 
            return Directory.GetFiles(directoryPath);
        }

        /// <summary> 
        /// 获取指定目录及子目录中所有文件列表 
        /// </summary> 
        /// <param name="directoryPath">指定目录的绝对路径</param> 
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。 
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param> 
        /// <param name="isSearchChild">是否搜索子目录</param> 
        public static string[] GetFileNames(string directoryPath, string searchPattern, bool isSearchChild)
        {
            //如果目录不存在，则抛出异常 
            if (!IsExistDirectory(directoryPath))
            {
                throw new FileNotFoundException();
            }
            try
            {
                if (isSearchChild)
                {
                    return Directory.GetFiles(directoryPath, searchPattern, SearchOption.AllDirectories);
                }
                else
                {
                    return Directory.GetFiles(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }
        #endregion
        #region 获取指定目录中的子目录列表
        /// <summary> 
        /// 获取指定目录中所有子目录列表,若要搜索嵌套的子目录列表,请使用重载方法. 
        /// </summary> 
        /// <param name="directoryPath">指定目录的绝对路径</param>         
        public static string[] GetDirectories(string directoryPath)
        {
            try
            {
                return Directory.GetDirectories(directoryPath);
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }
        /// <summary> 
        /// 获取指定目录及子目录中所有子目录列表 
        /// </summary> 
        /// <param name="directoryPath">指定目录的绝对路径</param> 
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。 
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param> 
        /// <param name="isSearchChild">是否搜索子目录</param> 
        public static string[] GetDirectories(string directoryPath, string searchPattern, bool isSearchChild)
        {
            try
            {
                if (isSearchChild)
                {
                    return Directory.GetDirectories(directoryPath, searchPattern, SearchOption.AllDirectories);
                }
                else
                {
                    return Directory.GetDirectories(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 向文本文件写入内容
        /// <summary> 
        /// 向文本文件中写入内容 
        /// </summary> 
        /// <param name="filePath">文件的绝对路径</param> 
        /// <param name="content">写入的内容</param>         
        public static void WriteText(string filePath, string content)
        {
            //向文件写入内容 
            File.WriteAllText(filePath, content);
        }
        #endregion

        #region 向文本文件的尾部追加内容
        /// <summary> 
        /// 向文本文件的尾部追加内容 
        /// </summary> 
        /// <param name="filePath">文件的绝对路径</param> 
        /// <param name="content">写入的内容</param> 
        public static void AppendText(string filePath, string content)
        {
            File.AppendAllText(filePath, content);
        }
        #endregion

        #region 将现有文件的内容复制到新文件中
        /// <summary> 
        /// 将源文件的内容复制到目标文件中 
        /// </summary> 
        /// <param name="sourceFilePath">源文件的绝对路径</param> 
        /// <param name="destFilePath">目标文件的绝对路径</param> 
        public static void Copy(string sourceFilePath, string destFilePath)
        {
            string dictory = Path.GetDirectoryName(destFilePath);
            CreateDirectory(dictory);
            if (IsExistDirectory(dictory))
                File.Copy(sourceFilePath, destFilePath, true);
        }
        #endregion

        #region 将文件移动到指定目录
        /// <summary> 
        /// 将指定文件移到新位置，并提供指定新文件名的选项。 
        /// </summary> 
        /// <param name="sourceFileName">要移动的文件的名称</param> 
        /// <param name="destFileName">文件的新路径</param> 
        public static void Move(string sourceFileName, string destFileName)
        {
            Move(sourceFileName, destFileName, false);
        }

        /// <summary> 
        /// 将指定文件移到新位置，并提供指定新文件名的选项。 
        /// </summary> 
        /// <param name="sourceFileName">要移动的文件的名称</param> 
        /// <param name="destFileName">文件的新路径</param> 
        /// <param name="isOverwrite">是否强制覆盖</param> 
        public static void Move(string sourceFileName, string destFileName, bool isOverwrite)
        {
            if (IsExistFile(destFileName))
            {
                if (isOverwrite)
                    File.Delete(destFileName);
            }
            File.Move(sourceFileName, destFileName);
        }
        #endregion

        #region 修改文件后缀
        /// <summary> 
        /// 修改文件后缀名。 
        /// </summary> 
        /// <param name="sourceFileName">对象文件名</param> 
        /// <param name="ext">文件后缀名</param> 
        public static void ChangeSuffix(string sourceFileName, string ext)
        {
            if (IsExistFile(sourceFileName))
            {
                File.Move(sourceFileName, Path.ChangeExtension(sourceFileName, ext));
            }
        }
        #endregion

        #region 将流读取到缓冲区中
        /// <summary> 
        /// 将流读取到缓冲区中 
        /// </summary> 
        /// <param name="stream">原始流</param> 
        public static byte[] StreamToBytes(Stream stream)
        {
            try
            {
                //创建缓冲区 
                byte[] buffer = new byte[stream.Length];
                //读取流 
                stream.Read(buffer, 0, Convert.ToInt32(stream.Length));
                //返回流 
                return buffer;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //关闭流 
                stream.Close();
            }
        }
        #endregion
        #region 将文件读取到缓冲区中
        /// <summary> 
        /// 将文件读取到缓冲区中 
        /// </summary> 
        /// <param name="filePath">文件的绝对路径</param> 
        public static byte[] FileToBytes(string filePath)
        {
            //获取文件的大小  
            int fileSize = GetFileSize(filePath);
            //创建一个临时缓冲区 
            byte[] buffer = new byte[fileSize];
            //创建一个文件流 
            FileInfo fi = new FileInfo(filePath);
            FileStream fs = fi.Open(FileMode.Open);
            try
            {
                //将文件流读入缓冲区 
                fs.Read(buffer, 0, fileSize);
                return buffer;
            }
            catch (IOException ex)
            {
                throw ex;
            }
            finally
            {
                //关闭文件流 
                fs.Close();
            }
        }
        #endregion
        #region 将文件读取到字符串中
        /// <summary> 
        /// 将文件读取到字符串中 
        /// </summary> 
        /// <param name="filePath">文件的绝对路径</param> 
        public static string FileToString(string filePath)
        {
            return FileToString(filePath, Encoding.Unicode);
        }
        /// <summary> 
        /// 将文件读取到字符串中 
        /// </summary> 
        /// <param name="filePath">文件的绝对路径</param> 
        /// <param name="encoding">字符编码</param> 
        public static string FileToString(string filePath, Encoding encoding)
        {
            //创建流读取器 
            StreamReader reader = new StreamReader(filePath, encoding);
            try
            {
                //读取流 
                return reader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //关闭流读取器 
                reader.Close();
            }
        }
        #endregion
        #region 从文件的绝对路径中获取文件名( 包含扩展名 )
        /// <summary> 
        /// 从文件的绝对路径中获取文件名( 包含扩展名 ) 
        /// </summary> 
        /// <param name="filePath">文件的绝对路径</param>         
        public static string GetFileName(string filePath)
        {
            //获取文件的名称 
            FileInfo fi = new FileInfo(filePath);
            return fi.Name;
        }
        #endregion
        #region 从文件的绝对路径中获取文件名( 不包含扩展名 )
        /// <summary> 
        /// 从文件的绝对路径中获取文件名( 不包含扩展名 ) 
        /// </summary> 
        /// <param name="filePath">文件的绝对路径</param>         
        public static string GetFileNameNoExtension(string filePath)
        {
            //获取文件的名称 
            FileInfo fi = new FileInfo(filePath);
            return fi.Name.Split('.')[0];
        }
        #endregion
        #region 从文件的绝对路径中获取扩展名
        /// <summary> 
        /// 从文件的绝对路径中获取扩展名 
        /// </summary> 
        /// <param name="filePath">文件的绝对路径</param>         
        public static string GetExtension(string filePath)
        {
            //获取文件的名称 
            FileInfo fi = new FileInfo(filePath);
            return fi.Extension;
        }
        #endregion
        #region 清空指定目录
        /// <summary> 
        /// 清空指定目录下所有文件及子目录,但该目录依然保存. 
        /// </summary> 
        /// <param name="directoryPath">指定目录的绝对路径</param> 
        public static void ClearDirectory(string directoryPath)
        {
            if (IsExistDirectory(directoryPath))
            {
                //删除目录中所有的文件 
                string[] fileNames = GetFileNames(directoryPath);
                for (int i = 0; i < fileNames.Length; i++)
                {
                    DeleteFile(fileNames[i]);
                }
                //删除目录中所有的子目录 
                string[] directoryNames = GetDirectories(directoryPath);
                for (int i = 0; i < directoryNames.Length; i++)
                {
                    DeleteDirectory(directoryNames[i]);
                }
            }
        }
        #endregion
        #region 清空文件内容
        /// <summary> 
        /// 清空文件内容 
        /// </summary> 
        /// <param name="filePath">文件的绝对路径</param> 
        public static void ClearFile(string filePath)
        {
            //删除文件 
            File.Delete(filePath);
            //重新创建该文件 
            CreateFile(filePath);
        }
        #endregion
        #region 删除指定文件
        /// <summary> 
        /// 删除指定文件 
        /// </summary> 
        /// <param name="filePath">文件的绝对路径</param> 
        public static void DeleteFile(string filePath)
        {
            if (IsExistFile(filePath))
            {
                File.Delete(filePath);
            }
        }
        #endregion
        #region 删除指定目录
        /// <summary> 
        /// 删除指定目录及其所有子目录 
        /// </summary> 
        /// <param name="directoryPath">指定目录的绝对路径</param> 
        public static void DeleteDirectory(string directoryPath)
        {
            if (IsExistDirectory(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }
        }
        #endregion

        /// <summary>
        /// 判断文件是否过期
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="dayDiff">过期期限，单位：天</param>
        /// <returns></returns>
        public static bool IsExpired(string filePath, int dayDiff)
        {
            FileInfo fi = new FileInfo(filePath);
            DateTime now = DateTime.Now;
            TimeSpan ts = now.Subtract(fi.CreationTime);
            return ts.TotalDays >= dayDiff;
        }

        /// <summary>
        /// 清理过期文件
        /// </summary>
        /// <param name="directory">目标目录</param>
        /// <param name="dayDiff">时间间隔（单位：天）</param>
        /// <param name="pattern">文件名匹配字符串</param>
        /// <param name="isSearchChild">是否包含子文件夹</param>
        /// <param name="notFoundThrow">目录不存在是否需要抛异常</param>
        public static void ClearExpiredFile(string directory, int dayDiff, string pattern, bool isSearchChild, bool notFoundThrow)
        {

            if (!UFile.IsExistDirectory(directory))
                return;

            //删除过期文件
            string[] tmpFiles = UFile.GetFileNames(directory, pattern, isSearchChild);
            foreach (string file in tmpFiles)
            {
                if (UFile.IsExpired(file, dayDiff))
                    UFile.DeleteFile(file);
            }

            //删除空文件夹
            DirectoryInfo dir = new DirectoryInfo(directory);
            DirectoryInfo[] subdirs = dir.GetDirectories(ALL, SearchOption.AllDirectories);
            foreach (DirectoryInfo subdir in subdirs)
            {
                FileSystemInfo[] subFiles = subdir.GetFileSystemInfos();
                if (subFiles.Count().Equals(0))
                    subdir.Delete();
            }
        }

        /// <summary>  
        /// 生成缩略图  
        /// </summary>  
        /// <param name="buffer">源图字节数组</param>  
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>  
        /// <param name="width">缩略图宽度</param>  
        /// <param name="height">缩略图高度</param>  
        /// <param name="mode">生成缩略图的方式</param>  
        public static void MakeThumbnail(byte[] buffer, string thumbnailPath, int width, int height, string mode, string PicExtension)
        {
            MemoryStream ms = new MemoryStream(buffer);
            MakeThumbnail(ms, thumbnailPath, width, height, mode, PicExtension);
        }
        /// <summary>  
        /// 生成缩略图  
        /// </summary>  
        /// <param name="PicStream">源图流</param>  
        /// <param name="thumbnailPath">缩略图路径（物理路径）</param>  
        /// <param name="width">缩略图宽度</param>  
        /// <param name="height">缩略图高度</param>  
        /// <param name="mode">生成缩略图的方式</param>  
        public static void MakeThumbnail(Stream PicStream, string thumbnailPath, int width, int height, string mode, string PicExtension)
        {
            System.Drawing.Image originalImage = System.Drawing.Image.FromStream(PicStream);
            int towidth = width;
            int toheight = height;

            int x = 0;
            int y = 0;
            int ow = originalImage.Width;
            int oh = originalImage.Height;

            switch (mode.ToUpper())
            {
                case "HW"://指定高宽缩放（可能变形）   
                    break;
                case "W"://指定宽，高按比例   
                    toheight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H"://指定高，宽按比例  
                    towidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "CUT"://指定高宽裁减（不变形）   
                    if ((double)originalImage.Width / (double)originalImage.Height > (double)towidth / (double)toheight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * towidth / toheight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / towidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
                default:
                    break;
            }

            //新建一个bmp图片  
            System.Drawing.Image bitmap = new System.Drawing.Bitmap(towidth, toheight);

            //新建一个画板  
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap);

            //设置高质量插值法  
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;

            //设置高质量，低速度呈现平滑程度  昆明卓联科技（网站、软件开发）  
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            //清空画布并以透明背景色填充  
            g.Clear(System.Drawing.Color.Transparent);

            //在指定位置并且按指定大小绘制原图片的指定部分  
            g.DrawImage(originalImage, new System.Drawing.Rectangle(0, 0, towidth, toheight),
            new System.Drawing.Rectangle(x, y, ow, oh),
            System.Drawing.GraphicsUnit.Pixel);

            try
            {
                //根据原来的图片格式，保存为原图片格式  
                switch (PicExtension.ToLower())
                {
                    case ".jpg":
                        //以jpg格式保存缩略图  
                        bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".jpeg":
                        bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".pjpeg":
                        bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".gif":
                        bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Gif);
                        break;
                    case ".bmp":
                        bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                    case ".png":
                        bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    default:
                        bitmap.Save(thumbnailPath, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                }

            }
            catch (System.Exception e)
            {
                throw e;
            }
            finally
            {
                originalImage.Dispose();
                bitmap.Dispose();
                g.Dispose();
            }
        }

        /// <summary>
        /// 文件名过长截取（保留扩展名）
        /// </summary>
        /// <param name="fileName">原文件名</param>
        /// <param name="length">新文件名长度</param>
        /// <returns>新文件名</returns>
        public static string GetLimitFileName(string fileName, int length)
        {
            string result = "";

            if (fileName.Length <= length)
            {
                result = fileName;
            }
            else
            {
                int dotindex = fileName.LastIndexOf('.');

                int fileNameLen = 50 - (fileName.Length - dotindex);

                result = fileName.Substring(0, fileNameLen) + fileName.Substring(dotindex);
            }

            return result;
        }

        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片地址</param>
        /// <param name="dFile">压缩后保存图片地址</param>
        /// <param name="size">压缩后图片的最大大小</param>
        /// <param name="flag">压缩质量（数字越小压缩率越高）1-100</param>
        /// <param name="sfsc">是否是第一次调用</param>
        /// <returns></returns>
        public static bool CompressImage(string sFile, string dFile, int size = 300, int flag = 90, bool sfsc = true)
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
                        CompressImage(sFile, dFile, size, flag, false);
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

        /// <summary>
        /// 删除指定文件夹内的所有空文件夹，包括空的子文件夹
        /// </summary>
        /// <param name="parentFolder"></param>
        public static void DeleteEmptyFolders(string parentFolder)
        {
            var dir = new DirectoryInfo(parentFolder);
            var subdirs = dir.GetDirectories("*.*", SearchOption.AllDirectories);

            foreach (var subdir in subdirs)
            {
                if (!Directory.Exists(subdir.FullName)) continue;

                var subFiles = subdir.GetFileSystemInfos("*.*", SearchOption.AllDirectories);

                var findFile = false;
                foreach (var sub in subFiles)
                {
                    findFile = (sub.Attributes & FileAttributes.Directory) == 0;

                    if (findFile) break;
                }

                if (!findFile) subdir.Delete(true);
            }
        }
    }
}
