using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.MongoBase
{
    public abstract partial class BaseEntity
    {
        protected BaseEntity()
        {
            _id = ObjectId.GenerateNewId().ToString();
        }

        [JsonConverter(typeof(ObjectIdConvert))]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id
        {
            get { return _id; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    _id = ObjectId.GenerateNewId().ToString();
                else
                    _id = value;
            }
        }

        private string _id;
    }
}
