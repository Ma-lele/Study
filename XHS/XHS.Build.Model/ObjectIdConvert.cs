﻿using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model
{
    class ObjectIdConvert : JsonConverter
    {
        //重写序列化
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());

        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.String)
            {
                throw new Exception($"Unexpected token parsing ObjectId. Expected String.get{reader.TokenType}");
            }
            var value = (string)reader.Value;
            return string.IsNullOrEmpty(value) ? ObjectId.Empty : ObjectId.Parse(value);
        }
        //重新反序列化
        public override bool CanConvert(Type objectType)
        {
            return typeof(ObjectId).IsAssignableFrom(objectType);
        }


    }
}
