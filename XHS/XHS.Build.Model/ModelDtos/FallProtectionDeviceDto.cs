using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.Models;

namespace XHS.Build.Model.ModelDtos
{
    public class FallProtectionDeviceDto : GCFallProtectionDevice
    {
        public string siteshortname { get; set; }
    }
}
