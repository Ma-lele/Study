using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Exam
{
    public interface IExamService : IBaseServices<BaseEntity>
    {
        /// <summary>
        /// 获取考题
        /// </summary>
        /// <returns>结果集</returns>
        Task<DataSet> GetRandomQuestionList(int pageIndex, int pageSize);

        /// <summary>
        /// 获取考题
        /// </summary>
        /// <returns>结果集</returns>
        Task<DataSet> GetRandomQuestionListByTag(int SITEID, string examtype, int pageIndex, int pageSize);

        /// <summary>
        /// 获取培训题目
        /// </summary>
        /// <returns>结果集</returns>
        Task<DataSet> GeQuestionList(int pageIndex, int pageSize);

        /// <summary>
        /// 获取考试结果
        /// </summary>
        /// <returns>结果集</returns>
        Task<DataTable> GetExamResultListByUser();

        /// <summary>
        /// 获取考试类别
        /// </summary>
        /// <returns>结果集</returns>
        Task<DataTable> GetTagList();

        /// <summary>
        /// 保存考试结果
        /// </summary>
        /// <returns>结果集</returns>
        Task<int> DoExamResultSave(int SITEID, string examtype, int usetime, float score, int totalcount, int wrongcount, int rightcount);
    }
}
