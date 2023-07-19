using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Common.Auth
{
    public interface IUserAqtKey
    {
        public string Appkey { get;  }

        public string Attenduserpsd { get; }

        public int SiteId { get; }

        public string RecordNumber { get; }
    }
}
