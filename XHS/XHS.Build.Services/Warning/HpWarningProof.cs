using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XHS.Build.Common;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Helps;
using XHS.Build.Repository.Round;
using XHS.Build.Repository.SystemSetting;
using XHS.Build.Repository.Warning;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Services.Warning
{
    public class HpWarningProof: IHpWarningProof
    {
        private readonly IUser _user;
        private readonly ISvWarningRepository _svWarningRepository;
        private readonly ISvRoundRepository _svRoundRepository;
        private readonly ISystemSettingRepository _systemSettingRepository;
        private readonly IHpSystemSetting _hpSystemSetting;
        public HpWarningProof(IUser user, ISvWarningRepository svWarningRepository, ISvRoundRepository svRoundRepository, ISystemSettingRepository systemSettingRepository, IHpSystemSetting hpSystemSetting)
        {
            _svWarningRepository = svWarningRepository;
            _user = user;
            _svRoundRepository = svRoundRepository;
            _systemSettingRepository = systemSettingRepository;
            _hpSystemSetting = hpSystemSetting;
        }
        /// <summary>
        /// 允许上传文件扩展名
        /// </summary>
        private static string[] FILE_EXT = new string[] { ".jpg", ".jpeg", ".png" };
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
        /// 上传任务相关文件
        /// </summary>
        /// <param name="SITEID">SITEID</param>
        /// <param name="WPID">WPID</param>
        /// <param name="filename">文件名</param>
        /// <param name="fileString">文件源字符串</param>
        /// <param name="filesize">文件大小</param>
        /// <returns></returns>
        public int doUpload(int SITEID, long WPID, int bsolved, string filename, string fileString, long filesize)
        {
            int result = 0;
            string fileName = string.Empty;
            string tmbFileName = string.Empty;
            string PROOFID = string.Empty;
            try
            {
                string path = Path.Combine(
                    _hpSystemSetting.getSettingValue(Const.Setting.S030),
                    _hpSystemSetting.getSettingValue(Const.Setting.S138),
                    _user.GroupId.ToString(), SITEID.ToString(), WPID.ToString());
                string fileExt = Path.GetExtension(filename).ToLower();//文件扩展名

                if (!FILE_EXT.Contains(fileExt))
                    return -5;//上传文件非法！

                //先保存文件信息到数据库
                PROOFID = _svWarningRepository.doInsert(_user.GroupId,SITEID,_user.Id,WPID,bsolved,filename,filesize,_user.Name);
                if (string.IsNullOrEmpty(PROOFID))
                    return 0;

                //检查/创建目录
                UFile.CreateDirectory(path);
                //生成缩略图 tmb_XXXXXX.jpg
                tmbFileName = Path.Combine(path, THUMB_PREFIX + PROOFID.ToLower() + Const.FileEx.JPG);
                byte[] buff = Convert.FromBase64String(fileString);
                UFile.MakeThumbnail(buff, tmbFileName, THUMB_WIDTH, THUMB_HEIGHT, "CUT", fileExt);
                //保存原图
                fileName = Path.Combine(path, PROOFID.ToLower() + fileExt);
                UFile.CreateFile(Path.Combine(fileName), buff);//保存文件

                result = 1;
            }
            catch (Exception ex)
            {
                UFile.DeleteFile(fileName);
                if (!string.IsNullOrEmpty(PROOFID))
                    _svRoundRepository.doDelete(PROOFID);//删掉记录
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// 删除巡检目录下所有文件
        /// </summary>
        /// <param name="GROUPID">分组ID</param>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="WPID">WPID</param>
        /// <returns></returns>
        public int doDeleteAll(int GROUPID, int SITEID, long WPID)
        {
            if (GROUPID < 1 || SITEID < 1 || WPID < 1)
                return 0;
            int result = 0;
            try
            {
                string savePath = Path.Combine(
                 _hpSystemSetting.getSettingValue(Const.Setting.S030),
                _hpSystemSetting.getSettingValue(Const.Setting.S138),
                GROUPID.ToString(), SITEID.ToString(), WPID.ToString());//对应目录

                //连根拔起
                UFile.DeleteDirectory(savePath);
                result = 1;

            }
            catch (Exception ex)
            {
                return 0;
            }

            return result;
        }

        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="PROOFID">照片ID</param>
        /// <param name="SITEID">监测点ID</param>
        /// <param name="WPID">WPID</param>
        /// <param name="PROOFID">PROOFID</param>
        /// <param name="createddate">拍摄日期</param>
        /// <param name="filename">文件名</param>
        /// <returns></returns>
        public int doDelete(int GROUPID, int SITEID, long WPID, string PROOFID, string filename)
        {
            if (PROOFID == "" || SITEID < 1 || string.IsNullOrEmpty(filename))
                return 0;

            int result = 0;

            string fileExt = Path.GetExtension(filename).ToLower();//文件扩展名

            try
            {
                //先删表记录
                result = _svRoundRepository.doDelete(PROOFID);
                if (result > 0)
                {
                    string savePath = Path.Combine(
                    _hpSystemSetting.getSettingValue(Const.Setting.S030),
                    _hpSystemSetting.getSettingValue(Const.Setting.S138),
                    GROUPID.ToString(), SITEID.ToString(), WPID.ToString());//对应目录
                    string realFileName = Path.Combine(savePath, PROOFID.ToLower() + fileExt);
                    string thumbFileName = Path.Combine(savePath, THUMB_PREFIX + PROOFID.ToLower() + fileExt);

                    //删除缩略图
                    UFile.DeleteFile(thumbFileName);
                    //删除原图
                    UFile.DeleteFile(realFileName);
                }
            }
            catch (Exception ex)
            {
                return 0;
            }

            return result;
        }
    }
}
