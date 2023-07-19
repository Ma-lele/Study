using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Photo
{
    public class PhotoService : BaseServices<BaseEntity>, IPhotoService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<BaseEntity> _photoRepository;
        public PhotoService(IBaseRepository<BaseEntity> photoRepository, IUser user)
        {
            _user = user;
            _photoRepository = photoRepository;
        }
        public async Task<int> doDelete(int PHOTOID)
        {
            return await _photoRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spSitePhotoDelete", new { PHOTOID = PHOTOID });
        }

        public async Task<int> doDeleteByDate(int SITEID, DateTime createddate)
        {
            return await _photoRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spSitePhotoDeleteByDate", new { SITEID = SITEID, createddate = createddate });
        }

        public async Task<int> doInsert(object param)
        {
            return await _photoRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spSitePhotoInsert", param);
        }

        public async Task<DataSet> getList()
        {
            return await _photoRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spSitePhotoList", new { USERID = _user.Id });
        }

        public async Task<DataTable> getOne(int SITEID, DateTime createddate)
        {
            return await _photoRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spSitePhotoGet", new { SITEID = SITEID, createddate = createddate });
        }
    }
}
