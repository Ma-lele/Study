using System;
using XHS.Build.Model.TaskJobs;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.TaskQz
{
    public class TasksQzServices : BaseServices<TasksQz>, ITasksQzServices
    {
        IBaseRepository<TasksQz> _dal;
        public TasksQzServices(IBaseRepository<TasksQz> dal)
        {
            this._dal = dal;
            base.BaseDal = dal;
        }

    }
}
