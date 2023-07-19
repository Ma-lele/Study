using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Board
{
    public interface IBoardService : IBaseServices<CtBoardGroupDto>
    {
        Task<DataTable> GetGroupListAsync(int GROUPID);

        Task<DataSet> GetGeneralAsync(int GROUPID);

        Task<DataRow> GetDevSiteCountAsync(int GROUPID);

        Task<DataTable> GetGeneralListAsync(int GROUPID,int pageindex,int pagesize,string keyword);
        Task<DataTable> GetMapAsync(int GROUPID);

        Task<DataTable> GetDevSiteListAsync(int GROUPID, int type);

        Task<DataTable> GetWeekWarnAsync(int GROUPID);

        Task<DataTable> GetWarnAsync(int GROUPID, int type);

        Task<DataRow> GetTopWeatherAsync(string city);

        Task<DataTable> GetAttendBoardListAsync(int GROUPID);

        Task<DataTable> GetOnlineRank(int GROUPID);
    }
}
