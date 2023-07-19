using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using XHS.Build.Common.Auth;
using XHS.Build.Common.Response;
using XHS.Build.Services.AISprayAction;
using Newtonsoft.Json;

namespace XHS.Build.SiteFront.Controllers
{
    /// <summary>
    /// 喷淋AI监测
    /// </summary>
    public class AISparyController : ControllerBase
    {


        private readonly IUser _user;
        private readonly IAISprayService _aISprayService;
        public AISparyController(IUser user, IAISprayService aISprayService)
        {
            _aISprayService = aISprayService;
            _user = user;
        }



        /// <summary>
        /// 喷淋统计
        /// </summary>
        /// <param name="type">0:本月;1:上月</param>
        /// <returns></returns>
        [HttpGet]
        [Route("T023/[action]")]
        public async Task<IResponseOutput> GetAiSprayDataCount(int type = 0)
        {
            Dictionary<string, double> dic = new Dictionary<string, double>();
            Dictionary<string, double> thisdic = new Dictionary<string, double>();
            Dictionary<string, double> lastdic = new Dictionary<string, double>();
            DataTable dt = await _aISprayService.GetAiSprayDataCountAsync(type, 1);
            if (type == 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DateTime startTime = Convert.ToDateTime(Convert.ToDateTime(dr[7]));
                    DateTime endTime = Convert.ToDateTime(Convert.ToDateTime(dr[11]));
                    string startTimestr = startTime.ToString("yyyy-MM-dd");
                    string endTimestr = endTime.ToString("yyyy-MM-dd");
                    if (startTimestr == endTimestr)
                    {
                        if (dic.ContainsKey(startTimestr))
                        {
                            dic[startTimestr] = Math.Round(dic[startTimestr] + endTime.Subtract(startTime).TotalHours, 1);
                        }
                        else
                        {
                            dic.Add(startTimestr, Math.Round(endTime.Subtract(startTime).TotalHours, 1));
                        }
                    }
                    else
                    {
                        int days = endTime.Date.Subtract(startTime.Date).Days - 1;
                        if (days > 0)
                        {
                            if (startTime.ToString("yyyy-MM") == DateTime.Now.ToString("yyyy-MM") || startTime.ToString("yyyy-MM") == DateTime.Now.AddMonths(-1).ToString("yyyy-MM"))
                            {
                                if (dic.ContainsKey(startTimestr))
                                {
                                    dic[startTimestr] = Math.Round(dic[startTimestr] + startTime.AddDays(1).Date.Subtract(startTime).TotalHours, 1);
                                }
                                else
                                {
                                    dic.Add(startTimestr, Math.Round(startTime.AddDays(1).Date.Subtract(startTime).TotalHours, 1));
                                }
                            }


                            if (endTime.ToString("yyyy-MM") == DateTime.Now.ToString("yyyy-MM") || endTime.ToString("yyyy-MM") == DateTime.Now.AddMonths(-1).ToString("yyyy-MM"))
                            {
                                if (dic.ContainsKey(endTimestr))
                                {
                                    dic[endTimestr] = Math.Round(dic[endTimestr] + endTime.Subtract(endTime.Date).TotalHours, 1);
                                }
                                else
                                {
                                    dic.Add(endTimestr, Math.Round(endTime.Subtract(endTime.Date).TotalHours, 1));
                                }
                            }
                            for (int i = 1; i <= days; i++)
                            {
                                string time = startTime.AddDays(i).ToString("yyyy-MM");
                                if (time == DateTime.Now.ToString("yyyy-MM") || time == DateTime.Now.AddYears(-1).ToString("yyyy-MM"))
                                {
                                    if (dic.ContainsKey(time))
                                    {
                                        dic[time] = dic[time] + 24;
                                    }
                                    else
                                    {
                                        dic.Add(time, 24);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (startTime.ToString("yyyy-MM") == DateTime.Now.ToString("yyyy-MM") || startTime.ToString("yyyy-MM") == DateTime.Now.AddMonths(-1).ToString("yyyy-MM"))
                            {
                                if (dic.ContainsKey(startTimestr))
                                {
                                    dic[startTimestr] = Math.Round(dic[startTimestr] + startTime.AddDays(1).Date.Subtract(startTime).TotalHours, 1);
                                }
                                else
                                {
                                    dic.Add(startTimestr, Math.Round(startTime.AddDays(1).Date.Subtract(startTime).TotalHours, 1));
                                }
                            }

                            if (endTime.ToString("yyyy-MM") == DateTime.Now.ToString("yyyy-MM") || endTime.ToString("yyyy-MM") == DateTime.Now.AddMonths(-1).ToString("yyyy-MM"))
                            {
                                if (dic.ContainsKey(endTimestr))
                                {
                                    dic[endTimestr] = Math.Round(dic[endTimestr] + endTime.Subtract(endTime.Date).TotalHours, 1);
                                }
                                else
                                {
                                    dic.Add(endTimestr, Math.Round(endTime.Subtract(endTime.Date).TotalHours, 1));
                                }
                            }
                        }
                    }
                }

                int thismonthdays = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                int lastmonthdays = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month - 1);

                for (int i = 1; i <= thismonthdays; i++)
                {
                    if (dic.ContainsKey(new DateTime(DateTime.Now.Year, DateTime.Now.Month, i).ToString("yyyy-MM-dd")))
                        continue;
                    else
                        dic.Add(new DateTime(DateTime.Now.Year, DateTime.Now.Month, i).ToString("yyyy-MM-dd"), 0);
                }

                for (int i = 1; i <= lastmonthdays; i++)
                {
                    if (dic.ContainsKey(new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, i).ToString("yyyy-MM-dd")))
                        continue;
                    else
                        dic.Add(new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, i).ToString("yyyy-MM-dd"), 0);
                }

                foreach (var item in dic)
                {
                    if (item.Key.Contains(DateTime.Now.ToString("yyyy-MM")))
                        thisdic.Add(item.Key, (double)item.Value);
                    else
                        lastdic.Add(item.Key, (double)item.Value);
                }

                var dicSort = from objDic in dic orderby objDic.Key select objDic;
                var thismonth = from objDic in thisdic orderby objDic.Key select objDic;
                var lastmonth = from objDic in lastdic orderby objDic.Key select objDic;

                return ResponseOutput.Ok(new { thismonth = thismonth, lastmonth = lastmonth });


            }
            else if (type == 1)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DateTime startTime = Convert.ToDateTime(Convert.ToDateTime(dr[7]));
                    DateTime endTime = Convert.ToDateTime(Convert.ToDateTime(dr[11]));
                    string startTimestr = startTime.ToString("yyyy-MM-dd");
                    string endTimestr = endTime.ToString("yyyy-MM-dd");
                    string stryearmonth = startTime.ToString("yyyy-MM");
                    string endyearmonth = endTime.ToString("yyyy-MM");
                    if (startTimestr == endTimestr)
                    {
                        if (dic.ContainsKey(stryearmonth))
                        {
                            dic[stryearmonth] = Math.Round(dic[stryearmonth] + endTime.Subtract(startTime).TotalHours, 1);
                        }
                        else
                        {
                            dic.Add(stryearmonth, Math.Round(endTime.Subtract(startTime).TotalHours, 1));
                        }
                    }
                    else
                    {
                        int days = endTime.Date.Subtract(startTime.Date).Days - 1;

                        if (days > 0)
                        {
                            if (startTime.ToString("yyyy") == DateTime.Now.ToString("yyyy") || startTime.ToString("yyyy") == DateTime.Now.AddYears(-1).ToString("yyyy"))
                            {
                                if (dic.ContainsKey(stryearmonth))
                                {
                                    dic[stryearmonth] = Math.Round(dic[stryearmonth] + startTime.AddDays(1).Date.Subtract(startTime).TotalHours, 1);
                                }
                                else
                                {
                                    dic.Add(stryearmonth, Math.Round(startTime.AddDays(1).Date.Subtract(startTime).TotalHours, 1));
                                }
                            }

                            if (endTime.ToString("yyyy") == DateTime.Now.ToString("yyyy") || endTime.ToString("yyyy") == DateTime.Now.AddYears(-1).ToString("yyyy"))
                            {
                                if (dic.ContainsKey(endyearmonth))
                                {
                                    dic[endyearmonth] = Math.Round(dic[endyearmonth] + endTime.Subtract(endTime.Date).TotalHours, 1);
                                }
                                else
                                {
                                    dic.Add(endyearmonth, Math.Round(endTime.Subtract(endTime.Date).TotalHours, 1));
                                }
                            }

                            for (int i = 1; i <= days; i++)
                            {
                                string time = startTime.AddDays(i).ToString("yyyy-MM");
                                if (startTime.AddDays(i).ToString("yyyy") == DateTime.Now.ToString("yyyy") || startTime.AddDays(i).ToString("yyyy") == DateTime.Now.AddYears(-1).ToString("yyyy"))
                                {
                                    if (dic.ContainsKey(time))
                                    {
                                        dic[time] = dic[time] + 24;
                                    }
                                    else
                                    {
                                        dic.Add(time, 24);
                                    }
                                }
                            }

                        }
                        else
                        {
                            if (startTime.ToString("yyyy") == DateTime.Now.ToString("yyyy") || startTime.ToString("yyyy") == DateTime.Now.AddYears(-1).ToString("yyyy"))
                            {
                                if (dic.ContainsKey(stryearmonth))
                                {
                                    dic[stryearmonth] = Math.Round(dic[stryearmonth] + startTime.AddDays(1).Date.Subtract(startTime).TotalHours, 1);
                                }
                                else
                                {
                                    dic.Add(stryearmonth, Math.Round(startTime.AddDays(1).Date.Subtract(startTime).TotalHours, 1));
                                }
                            }

                            if (endTime.ToString("yyyy") == DateTime.Now.ToString("yyyy") || endTime.ToString("yyyy") == DateTime.Now.AddYears(-1).ToString("yyyy"))
                            {
                                if (dic.ContainsKey(endyearmonth))
                                {
                                    dic[endyearmonth] = Math.Round(dic[endyearmonth] + endTime.Subtract(endTime.Date).TotalHours, 1);
                                }
                                else
                                {
                                    dic.Add(endyearmonth, Math.Round(endTime.Subtract(endTime.Date).TotalHours, 1));
                                }
                            }
                        }
                    }
                }

                for (int i = 1; i <= 12; i++)
                {
                    if (dic.ContainsKey(new DateTime(DateTime.Now.Year, i, DateTime.Now.Day).ToString("yyyy-MM")))
                        continue;
                    else
                        dic.Add(new DateTime(DateTime.Now.Year, i, DateTime.Now.Day).ToString("yyyy-MM"), 0);
                }

                for (int i = 1; i <= 12; i++)
                {
                    if (dic.ContainsKey(new DateTime(DateTime.Now.AddYears(-1).Year, i, DateTime.Now.Day).ToString("yyyy-MM")))
                        continue;
                    else
                        dic.Add(new DateTime(DateTime.Now.AddYears(-1).Year, i, DateTime.Now.Day).ToString("yyyy-MM"), 0);
                }

                foreach (var item in dic)
                {
                    if (item.Key.Contains(DateTime.Now.ToString("yyyy")))
                        thisdic.Add(item.Key, (double)item.Value);
                    else
                        lastdic.Add(item.Key, (double)item.Value);
                }
                var dicSort = from objDic in dic orderby objDic.Key select objDic;
                var thismonth = from objDic in thisdic orderby objDic.Key select objDic;
                var lastmonth = from objDic in lastdic orderby objDic.Key select objDic;

                return ResponseOutput.Ok(new { thismonth = thismonth, lastmonth = lastmonth });
            }
            return ResponseOutput.Ok();



        }

        /// <summary>
        /// 喷淋统计
        /// </summary>
        /// <param name="type"></param>                                                                                                                   
        /// <returns></returns>
        [HttpGet]
        [Route("T023/[action]")]
        public async Task<IResponseOutput> GetAiSprayDuringAnalysis(int type = 0)
        {
            Dictionary<int, double> dic = new Dictionary<int, double>();
            Dictionary<int, double> thisdic = new Dictionary<int, double>();
            Dictionary<int, double> lastdic = new Dictionary<int, double>();
            DataTable dt = await _aISprayService.GetAiSprayDuringAnalysisAsync(type, 0);
            if (type == 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DateTime startTime = Convert.ToDateTime(Convert.ToDateTime(dr[7]));
                    DateTime endTime = Convert.ToDateTime(Convert.ToDateTime(dr[11]));
                    string startTimestr = startTime.ToString("yyyy-MM-dd");
                    string endTimestr = endTime.ToString("yyyy-MM-dd");
                    int stahour = Convert.ToInt32(startTime.ToString("HH"));
                    int endhour = Convert.ToInt32(endTime.ToString("HH"));
                    if (startTimestr == endTimestr)
                    {

                        //double min = Math.Round(new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.AddHours(1).Hour, 0, 0, 0).Subtract(startTime).TotalMinutes, 1);

                        //if (dic.ContainsKey(startTime.AddHours(1).Hour))
                        //    dic[startTime.AddHours(1).Hour] += min;
                        //else
                        //    dic.Add(startTime.AddHours(1).Hour, min);

                        //for (int i = stahour + 1; i < endhour; i++)
                        //{

                        //    if (dic.ContainsKey((i+1)))
                        //        dic[(i+1)] += 60;
                        //    else
                        //        dic.Add((i+1), 60);
                        //}

                        //min = Math.Round(new DateTime(endTime.Year, endTime.Month, endTime.Day, endTime.AddHours(1).Hour, 0, 0, 0).Subtract(endTime).TotalMinutes, 1);

                        //if (dic.ContainsKey(endTime.AddHours(1).Hour))
                        //    dic[endTime.AddHours(1).Hour] += min;
                        //else
                        //    dic.Add(endTime.AddHours(1).Hour, min);
                    }
                    else
                    {
                        int days = endTime.Date.Subtract(startTime.Date).Days - 1;
                        if (days > 0)
                        {

                            double min = 0;

                            if (startTime.ToString("yyyy-MM") == DateTime.Now.ToString("yyyy-MM") || startTime.ToString("yyyy-MM") == DateTime.Now.AddMonths(-1).ToString("yyyy-MM"))
                            {

                                min = Math.Round(new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.AddHours(1).Hour, 0, 0, 0).Subtract(startTime).TotalMinutes, 1);

                                if (dic.ContainsKey(startTime.AddHours(1).Hour))
                                    dic[startTime.AddHours(1).Hour] += min;
                                else
                                    dic.Add(startTime.AddHours(1).Hour, min);


                                for (int i = stahour + 1; i < 24; i++)
                                {
                                    if (dic.ContainsKey(i + 1))
                                        dic[i + 1] += 60;
                                    else
                                        dic.Add((i + 1), 60);
                                }
                            }





                            for (int i = 0; ; i++)
                            {
                                if (startTime.AddDays(i).ToString("yyyy-MM") == DateTime.Now.ToString("yyy-MM") || startTime.AddDays(i).ToString("yyyy-") == DateTime.Now.AddMonths(i).ToString("yyy-MM"))
                                {
                                    for (int j = 1; j <= 24; j++)
                                    {
                                        if (dic.ContainsKey(j))
                                            dic[j] += 60 * (days - i + 1);
                                        else
                                            dic.Add(j, 60 * (days - i + 1));
                                    }
                                    break;
                                }
                            }

                            //if (endTime.AddDays(-i).ToString("yyyy-MM") == DateTime.Now.ToString("yyyy-MM") || endTime.AddDays(-i).ToString("yyyy-MM") == DateTime.Now.AddMonths(-1).ToString("yyyy-MM"))
                            //{ }



                            if (endTime.ToString("yyyy-MM") == DateTime.Now.ToString("yyyy-MM") || endTime.ToString("yyyy-MM") == DateTime.Now.AddMonths(-1).ToString("yyyy-MM"))
                            {
                                min = Math.Round(new DateTime(endTime.Year, endTime.Month, endTime.Day, endTime.AddHours(1).Hour, 0, 0, 0).Subtract(endTime).TotalMinutes, 1);

                                if (dic.ContainsKey(endTime.AddHours(1).Hour))
                                    dic[endTime.AddHours(1).Hour] += min;
                                else
                                    dic.Add(endTime.AddHours(1).Hour, min);



                                for (int i = 0; i < endhour; i++)
                                {
                                    int das = endTime.Date.Subtract(startTime.Date).Days;
                                    if (dic.ContainsKey(i + 1))

                                        dic[i + 1] += 60;
                                    else
                                        dic.Add(i + 1, 60);
                                }


                            }



                        }
                        else
                        {
                            double min = 0;
                            if (startTime.ToString("yyyy-MM") == DateTime.Now.ToString("yyyy-MM") || startTime.ToString("yyyy-MM") == DateTime.Now.AddMonths(-1).ToString("yyyy-MM"))
                            {

                                min = Math.Round(new DateTime(startTime.Year, startTime.Month, startTime.Day, startTime.AddHours(1).Hour, 0, 0, 0).Subtract(startTime).TotalMinutes, 1);

                                if (dic.ContainsKey(startTime.AddHours(1).Hour))
                                    dic[startTime.AddHours(1).Hour] += min;
                                else
                                    dic.Add(startTime.AddHours(1).Hour, min);

                                for (int i = stahour + 1; i < 24; i++)
                                {
                                    if (dic.ContainsKey(i + 1))
                                        dic[i + 1] += 60;
                                    else
                                        dic.Add((i + 1), 60);
                                }

                            }

                            if (startTime.ToString("yyyy-MM") == DateTime.Now.ToString("yyyy-MM") || startTime.ToString("yyyy-MM") == DateTime.Now.AddMonths(-1).ToString("yyyy-MM"))
                            {
                                min = Math.Round(new DateTime(endTime.Year, endTime.Month, endTime.Day, endTime.AddHours(1).Hour, 0, 0, 0).Subtract(endTime).TotalMinutes, 1);
                                if (dic.ContainsKey(endTime.AddHours(1).Hour))
                                    dic[endTime.AddHours(1).Hour] += min;
                                else
                                    dic.Add(endTime.AddHours(1).Hour, min);

                                for (int i = 0; i < endhour; i++)
                                {
                                    if (dic.ContainsKey(i + 1))

                                        dic[i + 1] += 60;
                                    else
                                        dic.Add(i + 1, 60);
                                }
                            }







                        }
                    }
                }
            }

            return ResponseOutput.Ok(dic);
        }

        [HttpGet]
        [Route("T023/[action]")]
        public async Task<IResponseOutput> GetAitongji(int type = 0)
        {

            DataTable dt = await _aISprayService.GetAiSprayDuringAnalysisAsync(type, 0);
            Dictionary<string, double> dic = new Dictionary<string, double>();
            Dictionary<int, double> thisdic = new Dictionary<int, double>();
            Dictionary<int, double> lastdic = new Dictionary<int, double>();

            if (type == 0)
            {
                //获取时间范围
                DateTime stascope = DateTime.Now.AddDays(1 - DateTime.Now.Day).AddMonths(-1).Date;
                DateTime endscope = DateTime.Now.AddDays(1 - DateTime.Now.Day).Date.AddMonths(1).AddSeconds(-1);

                foreach (DataRow dr in dt.Rows)
                {
                    DateTime startTime = Convert.ToDateTime(Convert.ToDateTime(dr[7]));
                    DateTime endTime = Convert.ToDateTime(Convert.ToDateTime(dr[11]));
                    string startTimestr = startTime.ToString("yyyy-MM-dd");
                    string endTimestr = endTime.ToString("yyyy-MM-dd");
                    if (startTimestr == endTimestr)//计算喷淋时段一天内
                    {
                        if (dic.ContainsKey(startTimestr))
                            dic[startTimestr] += endTime.Subtract(startTime).TotalHours;
                        else
                            dic.Add(startTimestr, endTime.Subtract(startTime).TotalHours);
                    }
                    else
                    {
                        int days = new DateTime(endTime.Year, endTime.Month, endTime.Day).Subtract(new DateTime(startTime.Year, startTime.Month, startTime.Day)).Days - 1;
                        if (days > 0)
                        {
                            if (startTime > stascope && startTime < endscope)
                            {
                                double hours = startTime.AddDays(1).Date.Subtract(startTime).TotalHours;
                                if (dic.ContainsKey(startTimestr))
                                    dic[startTimestr] += hours;
                                else
                                    dic.Add(startTimestr, hours);
                            }

                            if (endTime > stascope && endTime < endscope)
                            {
                                double hours = endTime.Subtract(endTime.Date).TotalHours;
                                if (dic.ContainsKey(endTimestr))
                                    dic[endTimestr] += hours;
                                else
                                    dic.Add(endTimestr, hours);
                            }

                            for (int i = 1; i <= days; i++)
                            {
                                if (startTime.AddDays(i) > stascope && startTime.AddDays(i) < endscope)//判断追加时间日期是否在范围内
                                {
                                    if (dic.ContainsKey(startTime.AddDays(i).ToString("yyyy-MM-dd")))
                                        dic[startTime.AddDays(i).ToString("yyyy-MM-dd")] += 24;
                                    else
                                        dic.Add(startTime.AddDays(i).ToString("yyyy-MM-dd"), 24);
                                }
                            }
                        }
                        else
                        {
                            if (startTime > stascope && startTime < endscope)
                            {
                                double hours = startTime.AddDays(1).Date.Subtract(startTime).TotalHours;
                                if (dic.ContainsKey(startTimestr))
                                    dic[startTimestr] += hours;
                                else
                                    dic.Add(startTimestr, hours);
                            }

                            if (endTime > stascope && endTime < endscope)
                            {
                                double hours = endTime.Subtract(endTime.Date).TotalHours;
                                if (dic.ContainsKey(endTimestr))
                                    dic[endTimestr] += hours;
                                else
                                    dic.Add(endTimestr, hours);
                            }
                        }
                    }
                }
            }
            else
            {
                //获取时间范围
                DateTime stascope = new DateTime(DateTime.Now.AddYears(-1).Year, 1, 1);
                DateTime endscope = stascope.AddYears(2).AddTicks(-1);

                foreach (DataRow dr in dt.Rows)
                {
                    DateTime startTime = Convert.ToDateTime(Convert.ToDateTime(dr[7]));
                    DateTime endTime = Convert.ToDateTime(Convert.ToDateTime(dr[11]));
                    string startTimestr = startTime.ToString("yyyy-MM-dd");
                    string endTimestr = endTime.ToString("yyyy-MM-dd");
                    if (startTimestr == endTimestr)//计算喷淋时段一天内
                    {
                        if (dic.ContainsKey(startTimestr))
                            dic[startTimestr] += endTime.Subtract(startTime).TotalHours;
                        else
                            dic.Add(startTimestr, endTime.Subtract(startTime).TotalHours);
                    }
                    else
                    {
                        int days = new DateTime(endTime.Year, endTime.Month, endTime.Day).Subtract(new DateTime(startTime.Year, startTime.Month, startTime.Day)).Days - 1;
                        if (days > 0)
                        {
                            if (startTime > stascope && startTime < endscope)
                            {
                                double hours = startTime.AddDays(1).Date.Subtract(startTime).TotalHours;
                                if (dic.ContainsKey(startTimestr))
                                    dic[startTimestr] += hours;
                                else
                                    dic.Add(startTimestr, hours);
                            }

                            if (endTime > stascope && endTime < endscope)
                            {
                                double hours = endTime.Subtract(endTime.Date).TotalHours;
                                if (dic.ContainsKey(endTimestr))
                                    dic[endTimestr] += hours;
                                else
                                    dic.Add(endTimestr, hours);
                            }

                            for (int i = 1; i <= days; i++)
                            {
                                if (startTime.AddDays(i) > stascope && startTime.AddDays(i) < endscope)//判断追加时间日期是否在范围内
                                {
                                    if (dic.ContainsKey(startTime.AddDays(i).ToString("yyyy-MM-dd")))
                                        dic[startTime.AddDays(i).ToString("yyyy-MM-dd")] += 24;
                                    else
                                        dic.Add(startTime.AddDays(i).ToString("yyyy-MM-dd"), 24);
                                }
                            }
                        }
                        else
                        {
                            if (startTime > stascope && startTime < endscope)
                            {
                                double hours = startTime.AddDays(1).Date.Subtract(startTime).TotalHours;
                                if (dic.ContainsKey(startTimestr))
                                    dic[startTimestr] += hours;
                                else
                                    dic.Add(startTimestr, hours);
                            }

                            if (endTime > stascope && endTime < endscope)
                            {
                                double hours = endTime.Subtract(endTime.Date).TotalHours;
                                if (dic.ContainsKey(endTimestr))
                                    dic[endTimestr] += hours;
                                else
                                    dic.Add(endTimestr, hours);
                            }
                        }
                    }





                }
            }




            return ResponseOutput.Ok(dic);
        }

    }
}
