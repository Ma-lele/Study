using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.MongoBase;

namespace XHS.Build.Model.MongoModels
{
    /// <summary>
    /// 
    /// </summary>
    public class TestEntity :BaseEntity
    {
        //public string id { get;set; }
        public string name { get; set; }

        public string code { get; set; }

        public string test { get; set; }
    }
}
