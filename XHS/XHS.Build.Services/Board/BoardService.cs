using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Model.ModelDtos;
using XHS.Build.Repository.Base;
using XHS.Build.Services.Base;

namespace XHS.Build.Services.Board
{
    public class BoardService : BaseServices<CtBoardGroupDto>, IBoardService
    {
        private readonly IBaseRepository<CtBoardGroupDto> _boardRepository;
        public readonly IUser _user;

        public BoardService(IBaseRepository<CtBoardGroupDto> boardRepository, IUser user)
        {
            _boardRepository = boardRepository;
            _user = user;
        }

        public async Task<DataRow> GetDevSiteCountAsync(int GROUPID)
        {
            DataTable dt = await _boardRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtBoardDevSiteCount", new { GROUPID = GROUPID });
            if (dt == null || dt.Rows.Count == 0)
                return null;

            return dt.Rows[0];
        }

        public Task<DataTable> GetDevSiteListAsync(int GROUPID, int type)
        {
            return _boardRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtBoardDevSiteList", new { GROUPID = GROUPID,type=type});
        }

        public Task<DataSet> GetGeneralAsync(int GROUPID)
        {
            return _boardRepository.Db.Ado.UseStoredProcedure().GetDataSetAllAsync("spCtBoardGeneral",new { GROUPID=GROUPID});

        }

        public Task<DataTable> GetGeneralListAsync(int GROUPID,int pageindex,int pagesize,string keyword)
        {
            return _boardRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtBoardGeneralList", new { GROUPID = GROUPID,pageindex=pageindex,pagesize=pagesize,keyword=keyword});
        }

        public Task<DataTable> GetGroupListAsync(int GROUPID)
        {
            if (GROUPID == -1)
            {
                GROUPID = _user.GroupId;
            }
            return _boardRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtBoardGroupList", new { GROUPID = GROUPID });
        }

        public Task<DataTable> GetMapAsync(int GROUPID)
        {
            return _boardRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtBoardMap", new { GROUPID = GROUPID });
        }

        public async Task<DataRow> GetTopWeatherAsync(string city)
        {
            DataTable dt = await _boardRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtTopWeather", new { city = city });
            if (dt == null || dt.Rows.Count == 0)
                return null;
            return dt.Rows[0];
        }

        public Task<DataTable> GetWarnAsync(int GROUPID, int type)
        {
            return _boardRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtBoardWarn", new { GROUPID = GROUPID, type = type });

        }

        public Task<DataTable> GetWeekWarnAsync(int GROUPID)
        {
            return _boardRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtBoardWeekWarn", new { GROUPID = GROUPID});

        }


        public Task<DataTable> GetAttendBoardListAsync(int GROUPID)
        {
            return _boardRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtAttendProjectList", new { GROUPID = GROUPID });

        }

        public Task<DataTable> GetOnlineRank(int GROUPID)
        {
            return _boardRepository.Db.Ado.UseStoredProcedure().GetDataTableAsync("spCtBoardOnlineRank", new { GROUPID = GROUPID});
        }
    }
}
