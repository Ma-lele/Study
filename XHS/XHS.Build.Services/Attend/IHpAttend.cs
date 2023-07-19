using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace XHS.Build.Services.Attend
{
    public interface IHpAttend
    {
        Task<long> doUpload(int SITEID, float longitude, float latitude, string address, string filename, string fileString, string remark);
    }
}
