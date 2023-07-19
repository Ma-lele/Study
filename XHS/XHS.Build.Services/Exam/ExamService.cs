using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model.Base;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Exam
{
    public class ExamService : BaseServices<BaseEntity>, IExamService
    {
        private readonly IUser _user;
        private readonly IBaseRepository<BaseEntity> _examRepository;
        public ExamService(IBaseRepository<BaseEntity> examRepository, IUser user)
        {
            _examRepository = examRepository;
            BaseDal = examRepository;
            _user = user;
        }
        public async Task<DataSet> GetRandomQuestionList(int pageIndex, int pageSize)
        {
            return await _examRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spRandomQuestionList", new { GROUPID = _user.GroupId, pageIndex = pageIndex , pageSize = pageSize });
        }
        public async Task<DataSet> GetRandomQuestionListByTag(int SITEID, string examtype, int pageIndex, int pageSize)
        {
            return await _examRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spRandomQuestionListByTag", new { GROUPID = _user.GroupId, SITEID = SITEID, qtags= examtype,  pageIndex = pageIndex, pageSize = pageSize });
        }

        public async Task<DataSet> GeQuestionList(int pageIndex, int pageSize)
        {
           // return await _examRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spQuestionList", new { GROUPID = _user.GroupId, pageIndex = pageIndex, pageSize = pageSize });
            return await _examRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spRandomQuestionList", new { GROUPID = _user.GroupId, pageIndex = pageIndex, pageSize = pageSize });
        }

        public async Task<DataTable> GetExamResultListByUser()
        {
            return await _examRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spExamResultListForUser", new { USERID = _user.Id });
        }

        public async Task<DataTable> GetTagList()
        {
            return await _examRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spTagListForApp", new { GROUPID = _user.GroupId, USERID = _user.Id });
        }

        public async Task<int> DoExamResultSave(int SITEID, string examtype, int usetime, float score, int totalcount, int wrongcount, int rightcount)
        {
            return await _examRepository.Db.Ado.UseStoredProcedure().GetIntAsync("spExamResultInsert", new { GROUPID = _user.GroupId, SITEID = SITEID, USERID = _user.Id, examtype = examtype , usetime = usetime , score = score , totalcount = totalcount , wrongcount = wrongcount , rightcount = rightcount });
        }
    }
}
