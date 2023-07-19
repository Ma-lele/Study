using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.ModelDtos
{
    public class RealNameDto
    {
        public string resultMessage { get; set; }
        public dynamic Data { get; set; }
        public string resultCode { get; set; }
        public string ResultReason { get; set; }
        public string ResultState { get; set; }
    }
}
