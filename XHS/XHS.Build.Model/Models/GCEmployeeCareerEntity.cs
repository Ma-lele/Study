using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    [SugarTable("T_GC_EmployeeCareer")]
    public class GCEmployeeCareerEntity
    {
        [SugarColumn(IsPrimaryKey = true)]
        public int ECID { get; set; }
        public string ID { get; set; }
        public string Papertype { get; set; }
        public string Papername { get; set; }
        public string Papercode { get; set; }

        private string _Startdate;
        public string Startdate
        {
            get
            {
                if (!string.IsNullOrEmpty(_Startdate))
                {
                    return _Startdate.Split(' ')[0];
                }
                else
                {
                    return _Startdate;
                }

            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _Startdate = value.Split(' ')[0];
                }
                else
                {
                    _Startdate = value;
                }

            }
        }

        private string _Enddate;
        public string Enddate
        {
            get
            {
                if (!string.IsNullOrEmpty(_Enddate))
                {
                    return _Enddate.Split(' ')[0];
                }
                else
                {
                    return _Enddate;
                }

            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _Enddate = value.Split(' ')[0];
                }
                else
                {
                    _Enddate = value;
                }

            }
        }

        [SugarColumn(IsIgnore = true)]
        public string Image { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string FileName { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string[] ImageUrls { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string ImageTmpUrls { get; set; }
    }
}
