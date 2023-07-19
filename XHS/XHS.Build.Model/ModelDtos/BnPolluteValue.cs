using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    /// <summary>
    /// 污染数据Bean
    /// </summary>
    public class BnPolluteValue
    {
        public DateTime datatime { get; set; }

        public Decimal value { get; set; }
    }
}
