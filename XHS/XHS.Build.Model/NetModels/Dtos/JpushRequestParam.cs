using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.NetModels.Dtos
{
    public class JpushRequestParam
    {
        //用户uuid（多用户逗号分隔）
        public string RegistrationId { get; set; }
        public string uuids { get; set; } = "";
        public string templateCode { get; set; }
        public string templateParam { get; set; }
        public string title { get; set; }
        public Dictionary<string, object> Extras { get; set; }
    }
}
