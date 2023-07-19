using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace XHS.Build.Services.Elevator
{
    /// <summary>
    /// 特种设备全局工具
    /// </summary>
    public class SpecialEqp : ISpecialEqp
    {
        private static ConcurrentDictionary<string, SpecialEqpBean> _list = null;
        private static DateTime _updatedate;
        private static IElevatorService _elevatorService;
        public SpecialEqp(IElevatorService elevatorService)
        {
            _elevatorService = elevatorService;
        }

        public ConcurrentDictionary<string, SpecialEqpBean> List
        {
            get
            {
                if (_list == null || _updatedate == null)
                    init();
                else if (_updatedate.AddMinutes(10) < DateTime.Now)
                    reset();
                return _list;
            }
        }

        /// <summary>
        /// 更新重置
        /// </summary>
        public void reset()
        {
            _updatedate = DateTime.Now;
            DataTable dt =  _elevatorService.getAll();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string secode = Convert.ToString(dt.Rows[i]["secode"]);
                if (string.IsNullOrEmpty(secode))
                {
                    continue;
                }
                if (_list.Keys.Contains(secode))
                {//更新现有设备
                    SpecialEqpBean sew = _list[secode];
                    sew.GROUPID = Convert.ToInt32(dt.Rows[i]["GROUPID"]);
                    sew.setype = Convert.ToInt32(dt.Rows[i]["setype"]);
                    sew.alarmstate = Convert.ToInt32(dt.Rows[i]["alarmstate"]);
                }
                else
                {//添加新设备
                    SpecialEqpBean sewNew = set(dt.Rows[i]);
                    _list[secode] = sewNew;
                    //_list.TryAdd(secode, sewNew);
                }
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void init()
        {
            _list = new ConcurrentDictionary<string, SpecialEqpBean>();
            _updatedate = DateTime.Now;
            DataTable dt =  _elevatorService.getAll();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                SpecialEqpBean sew = set(dt.Rows[i]);
                if (sew != null && !string.IsNullOrEmpty(sew.secode))
                {
                    //_list.Add(sew.secode, sew);
                    _list[sew.secode] = sew;
                }
            }
        }

        /// <summary>
        /// 设置设备
        /// </summary>
        /// <param name="dr"></param>
        /// <returns></returns>
        public SpecialEqpBean set(DataRow dr)
        {
            SpecialEqpBean result = new SpecialEqpBean();
            result.secode = Convert.ToString(dr["secode"]);
            result.GROUPID = Convert.ToInt32(dr["GROUPID"]);
            result.setype = Convert.ToInt32(dr["setype"]);
            result.waitcount = 0;
            result.warndate = DateTime.Parse("2000-01-01");
            result.alarmstate = Convert.ToInt32(dr["alarmstate"]);
            result.alarmdate = Convert.ToDateTime(dr["alarmdate"]);
            return result;
        }
    }

    public class SpecialEqpBean
    {
        public int GROUPID { get; set; }
        public string secode { get; set; }
        public int setype { get; set; }
        public int alarmstate { get; set; }
        public int waitcount { get; set; }
        public DateTime alarmdate { get; set; }
        public DateTime warndate { get; set; }
    }

}
