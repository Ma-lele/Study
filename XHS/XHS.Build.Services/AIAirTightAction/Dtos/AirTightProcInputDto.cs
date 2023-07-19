﻿using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Services.AIAirTightAction.Dtos
{
    public class AirTightProcInputDto
    {
        /// <summary>
        /// 项目id
        /// </summary>
        public string projid { get; set; }
        /// <summary>
        /// 车辆信息
        /// </summary>
        public string cartype { get; set; }
        /// <summary>
        /// 车牌号
        /// </summary>
        public string carno { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? createtime { get; set; }
    }
}
