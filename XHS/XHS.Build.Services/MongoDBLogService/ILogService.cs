using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Model.MongoModels;

namespace XHS.Build.Services.TestMongoDBService
{
    public interface ILogService
    {
        Task<AppExceptionLog> InsertAsync(AppExceptionLog testEntity);

        Task<bool> Delete(string id);

        Task<bool> Update(string testEntity);


        Task<IPagedList<BsonDocument>> Pages(string name="", int pageIndex = 0, int pageSize = int.MaxValue);


        Task<List<BsonDocument>> GetTestEntityByName(string name);

        Task<List<BsonDocument>> GetTestEntities();

        Task InsertObjectAsync(string json);

        Task InsertObjectsAsync(string json);


        Task<long> DeleteObjectsAsync(Expression<Func<OpenApiOperateLog, bool>> filter);

        Task<long> DeleteObjectsAsync(Expression<Func<MongoOperateLog, bool>> filter);

        Task<long> DeleteObjectsAsync(Expression<Func<AppExceptionLog, bool>> filter);

        Task<long> DeleteObjectsAsync(Expression<Func<CityUploadOperateLog, bool>> filter);

        Task<long> DeleteObjectsAsync(Expression<Func<CityOpenApiOperateLog, bool>> filter);
    }
}
