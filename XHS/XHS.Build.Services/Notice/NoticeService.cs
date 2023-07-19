using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Notice
{
    public class NoticeService:BaseServices<BaseEntity>,INoticeService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<BaseEntity> _noticeRepository;
        public NoticeService(IUser user, IBaseRepository<BaseEntity> noticeRepository)
        {
            _user = user;
            _noticeRepository = noticeRepository;
            BaseDal = noticeRepository;
        }

        public async Task<int> doInsert(object param)
        {
            return await _noticeRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spNoticeInsert", param);
        }

        public async Task<DataTable> getFileList(int NOTICEID)
        {
            return await _noticeRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spNoticeFileList", new { NOTICEID = NOTICEID });
        }

        public async Task<DataTable> getList()
        {
            return await _noticeRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spNoticeList", new { GROUPID = _user.GroupId });
        }

        public async Task<DataTable> getListByUser( DateTime operatedate)
        {
            return await _noticeRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spNoticeListByUser", new { USERID = _user.Id, operatedate = operatedate });
        }

        public async Task<DataSet> getOne(int NOTICEID)
        {
            return await _noticeRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spNoticeGet", new { NOTICEID = NOTICEID });
        }

        public async Task<DataTable> getUserBySite(int GROUPID,int SITEID)
        {
            return await _noticeRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spNoticeUserList", new { GROUPID = GROUPID, SITEID = SITEID });
        }

        public async Task<DataTable> getWebNoticeActiveListByGroup()
        {
            return await _noticeRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spWebNoticeActiveList", new { GROUPID = _user.GroupId });
        }

        public async Task<DataSet> getWebNoticeOne(int WEBNOTICEID)
        {
            return await _noticeRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spWebNoticeGet", new { WEBNOTICEID = WEBNOTICEID });
        }
    }
}
