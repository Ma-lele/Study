using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using XHS.Build.Model.MongoBase;

namespace XHS.Build.Model.MongoModels
{
    public class AppExceptionLog : BaseEntity
    {
        //public string id { get;set; }
        public string name { get; set; }

        public string code { get; set; }

        public string test { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime reportDate { get; set; }
    }
}
