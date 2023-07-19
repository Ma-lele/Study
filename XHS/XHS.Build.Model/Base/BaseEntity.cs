using MongoDB.Bson;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace XHS.Build.Model.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class BaseEntity
    {
        private string _id;

        /// <summary>
        /// ID
        /// </summary>
        [SugarColumn(IsNullable = false, IsPrimaryKey = true)]
        public string Id
        {
            get
            {
                if (string.IsNullOrEmpty(_id))
                {
                    return ObjectId.GenerateNewId().ToString();
                }
                else
                {
                    return _id;
                }
            }
            set
            {
                _id = value;
            }
        }

    }
}
