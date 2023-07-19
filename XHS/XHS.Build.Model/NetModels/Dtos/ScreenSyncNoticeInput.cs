using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.NetModels.Dtos
{
    /// <summary>
    /// 
    /// </summary>
    public class ScreenSyncNoticeInput
    {
        public string screencode { get; set; }
        public string defaultnotice { get; set; }
        public string projectctrllevel { get; set; }
        public string projectopenstate { get; set; }

    }
}
