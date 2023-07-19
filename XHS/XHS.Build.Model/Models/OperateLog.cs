using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class OperateLog
    {
        /// <summary>
        /// 
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// api名称
        /// </summary>
        public string ApiMethod { get;set;}

        /// <summary>
        /// 接口路径
        /// </summary>
        public string ApiPath { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 接口msg
        /// </summary>
        public string NoteMsg { get; set; }

        /// <summary>
        /// 接口耗时
        /// </summary>
        public long ElapsedMilliseconds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? Status { get; set; }
    }

    public class MongoOperateLog : MongoBase.BaseEntity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// api名称
        /// </summary>
        public string ApiMethod { get; set; }

        /// <summary>
        /// 接口路径
        /// </summary>
        public string ApiPath { get; set; }

        /// <summary>
        /// IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 接口msg
        /// </summary>
        public string NoteMsg { get; set; }

        /// <summary>
        /// 接口耗时
        /// </summary>
        public long ElapsedMilliseconds { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? Status { get; set; }

        public string Body { get; set; }

        public string UrlQueryString { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreateTime { get; set; }

        public string GroupId { get; set; }
    }
}
