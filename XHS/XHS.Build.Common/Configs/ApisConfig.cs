using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Common.Configs
{
    public class KeySecretList
    {
        public List<KeySecret> Items { get; set; }
    }
    public class Apis
    {
        public string Api { get; set; }
        public string Method { get; set; }
    }

    public class KeySecret
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int KSCID { get; set; }
        public string Key { get; set; }
        public string UserId { get; set; }
        public string Secret { get; set; }
        public string Name { get; set; }
        public string GroupId { get; set; }
        public string[] Ips { get; set; }        

        public string[] Apis { get; set; }

        public string[] Domains { get; set; }
    }

    [SugarTable("T_CC_KeySecretConfig")]
    public class TCCKeySecretConfig
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int KSCID { get; set; }
        public string Key { get; set; }
        public string UserId { get; set; }
        public string Secret { get; set; }
        public string Name { get; set; }
        public string GroupId { get; set; }
        public string Ips { get; set; }

        public string Apis { get; set; }

        public string Domains { get; set; }
    }
    public class KeyApis
    {
        public string Key { get; set; }
        public string Api { get; set; }
    }
}
