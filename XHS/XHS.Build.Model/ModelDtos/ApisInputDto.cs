using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class ApisInputDto
    {
        /// <summary>
        /// 接口名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 接口url
        /// </summary>
        public string ApiUrl { get; set; }
    }
}
