using SqlSugar;
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
using XHS.Build.Common.Sqlsugar;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.Services.SystemSetting;

namespace XHS.Build.Services.Attend
{
    public partial class HpAttend:IHpAttend
    {
        /// <summary>
        /// 允许上传文件扩展名
        /// </summary>
        private static string[] FILE_EXT = new string[] { ".jpg", ".jpeg", ".png" };
        /// <summary>
        /// 缩略图宽
        /// </summary>
        private const int THUMB_WIDTH = 80;
        /// <summary>
        /// 缩略图高
        /// </summary>
        private const int THUMB_HEIGHT = 45;
        /// <summary>
        /// 缩略图前缀
        /// </summary>
        private const string THUMB_PREFIX = "tmb_";

        private readonly IHpSystemSetting _hpSystemSetting;
        private readonly IBaseRepository<BaseEntity> _attendRepository;
        private readonly IUser _user;
        public HpAttend(IHpSystemSetting hpSystemSetting, IUser user, IBaseRepository<BaseEntity> attendRepository)
        {
            _hpSystemSetting = hpSystemSetting;
            _user = user;
            _attendRepository = attendRepository;
        }

        /// <summary>
        /// 上传签到照片
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="filename">文件名</param>
        /// <param name="fileString">文件源字符串</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        public async Task<long> doUpload( int SITEID,float longitude, float latitude, string address, string filename, string fileString, string remark)
        {
            long result = 0;
            string fileExt = Path.GetExtension(filename).ToLower();//文件扩展名
            string tmbFileName = string.Empty;
            DateTime now = DateTime.Now;
            long ATTENDID = 0;
            string fileName = "";

            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    _hpSystemSetting.getSettingValue(Const.Setting.S030),
                    _hpSystemSetting.getSettingValue(Const.Setting.S149),
                    _user.GroupId.ToString(), SITEID.ToString());

                if (!FILE_EXT.Contains(fileExt))
                    return -37;//上传文件格式非法！

                SgParams sp = new SgParams();
                sp.Add("GROUPID", _user.GroupId);
                sp.Add("SITEID", SITEID);
                sp.Add("USERID", _user.Id);
                sp.Add("longitude", longitude);
                sp.Add("latitude", latitude);
                sp.Add("address", address);
                sp.Add("remark", remark);
                sp.NeetReturnValue();
                await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spUserAttendInsert", sp.Params);
               
                //文件ID
                ATTENDID= sp.ReturnValue;
                

                if (ATTENDID.Equals(0))
                    return 0;

                //检查/创建目录
                UFile.CreateDirectory(path);
                fileName = ATTENDID + fileExt;
                //生成缩略图 tmb_XXXXXX.jpg
                tmbFileName = Path.Combine(path, THUMB_PREFIX + fileName);
                byte[] buff = Convert.FromBase64String(fileString);
                UFile.MakeThumbnail(buff, tmbFileName, THUMB_WIDTH, THUMB_HEIGHT, "HW", fileExt);
                //保存原图
                fileName = Path.Combine(path, fileName);
                UFile.CreateFile(Path.Combine(fileName), buff);//保存文件

                result = ATTENDID;
            }
            catch (Exception ex)
            {
                UFile.DeleteFile(tmbFileName);
                UFile.DeleteFile(fileName);
                var output = new SugarParameter("@return", null, System.Data.DbType.Int32, ParameterDirection.ReturnValue);
                await _attendRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spUserAttendDelete", new SugarParameter("ATTENDID", ATTENDID), output);
                
                //await _attendRepository.doDelete(ATTENDID);
                //ULog.WriteError(ex.Message, Properties.Settings.Default.AppName);
                return 0;
            }

            return result;
        }

    }
}
