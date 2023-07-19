using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 安全帽
    /// </summary> 
    public class BnHelmet
    {
        /// <summary>
        /// 安全帽ID
        /// </summary> 
        public string HELMETID { get; set; }

        /// <summary>
        /// 位置名称
        /// </summary> 
        public string beaconname { get; set; }

        /// <summary>
        /// 信标编号
        /// </summary> 
        public string beaconcode { get; set; }

        /// <summary>
        /// 安全帽编号
        /// </summary> 
        public string helmetcode { get; set; }

        /// <summary>
        /// 人员姓名
        /// </summary> 
        public string realname { get; set; }

        /// <summary>
        /// 人员照片
        /// </summary> 
        public string image { get; set; }



        /// <summary>
        /// 电量
        /// </summary> 
        public string power { get; set; }

        /// <summary>
        /// 位置
        /// </summary> 
        public string position { get; set; }

        /// <summary>
        /// 所在层
        /// </summary> 
        public string floor { get; set; }
    }
}
