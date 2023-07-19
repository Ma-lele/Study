using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 
    /// </summary>
    public class DeviceBindInputDto
    {
        /// <summary>
        /// 域名
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        public string DeviceType { get; set; }

        /// <summary>
        /// Code
        /// </summary>
        public string DeviceCode { get; set; }
        public DateTime? UpdateDate { get; set; }


    }

    public class SpecialEntity
    {
        public int SITEID { get; set; }
        public string sename { get; set; }
        public string secode { get; set; }
        public int setype { get; set; }

        public DateTime operatedate { get; set; }
    }
   

    public class SiteDeviceEntity
    {
        public string attendprojid { get; set; }
        public string parkkey { get; set; }
        public string helmetprojid { get; set; }
        public string trespasserprojid { get; set; }
        public string strangerprojid { get; set; }
        public string fireprojid { get; set; }
        public string liftoverprojid { get; set; }
        public string amdiscloseprojid { get; set; }
        public string airtightprojid { get; set; }
        public string sprayprojid { get; set; }
        public string soilprojid { get; set; }
        public string vestprojid { get; set; }
        public DateTime? apiupdatedate { get; set; }

        public DateTime operatedate { get; set; }
    }

    public class SiteDeviceTypeDto
    {
        public string attend { get; set; }
        public string carwash { get; set; }
        public string helmet { get; set; }
        public string trespasser { get; set; }
        public string stranger { get; set; }
        public string fire { get; set; }
        public string liftover { get; set; }
        public string amdisclose { get; set; }
        public string airtight { get; set; }
        public string spray { get; set; }
        public string soil { get; set; }

        public string vest { get; set; }
        public DateTime operatedate { get; set; }
        public DateTime? apiupdatedate { get; set; }
    }
}
