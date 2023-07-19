using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model;
using XHS.Build.Model.Models;
using XHS.Build.Model.MongoModels;
using XHS.Build.Repository.Base;

namespace XHS.Build.Services.TestMongoDBService
{
    public class LogService : ILogService
    {
        private readonly IMongoDBRepository<AppExceptionLog> _mongoDBRepository;
        private readonly IMongoDBRepository<OpenApiOperateLog> _mongoDBOpenApiOperateRepository;
        private readonly IMongoDBRepository<MongoOperateLog> _mongoOperateRepository;
        private readonly IMongoDBRepository<AppExceptionLog> _mongoAppExceptionLogRepository;
        private readonly IMongoDBRepository<CityUploadOperateLog> _mongoCityUploadOperateLogRepository;
        private readonly IMongoDBRepository<CityOpenApiOperateLog> _mongoCityOpenApiOperateLogRepository;
        public LogService(IMongoDBRepository<AppExceptionLog> mongoDBRepository,
            IMongoDBRepository<OpenApiOperateLog> mongoDBOpenApiOperateRepository,
            IMongoDBRepository<MongoOperateLog> mongoOperateRepository,
            IMongoDBRepository<AppExceptionLog> mongoAppExceptionLogRepository, 
            IMongoDBRepository<CityUploadOperateLog> mongoCityUploadOperateLogRepository,
            IMongoDBRepository<CityOpenApiOperateLog> mongoCityOpenApiOperateLogRepository
            )
        {
            _mongoDBRepository = mongoDBRepository;
            _mongoDBOpenApiOperateRepository = mongoDBOpenApiOperateRepository;
            _mongoOperateRepository = mongoOperateRepository;
            _mongoAppExceptionLogRepository = mongoAppExceptionLogRepository;
            _mongoCityUploadOperateLogRepository = mongoCityUploadOperateLogRepository;
            _mongoCityOpenApiOperateLogRepository = mongoCityOpenApiOperateLogRepository;
        }

        public async Task<bool> Delete(string id)
        {
            AppExceptionLog testEntity = _mongoDBRepository.GetById(id);
            return await _mongoDBRepository.DeleteAsync(testEntity) != null;
        }

        public async Task<List<BsonDocument>> GetTestEntities()
        {
            var command = new BsonDocumentCommand<BsonDocument>(new BsonDocument() { { "addNumbers",1 } });
            var result=  await _mongoDBRepository.Database.RunCommandAsync<BsonDocument>((Command<BsonDocument>)"{ping:1}");


            return await _mongoDBRepository.DocCollection.Find(new BsonDocument()).ToListAsync();
            //return _mongoDBRepository.Table.ToListAsync();
        }

        public async Task<List<BsonDocument>> GetTestEntityByName(string name)
        {
            Expression<Func<BsonDocument, bool>> whereExpression = e => e["name"] == name;
            var filter = Builders<BsonDocument>.Filter.Where(whereExpression); //声明过滤条件
            return await _mongoDBRepository.DocCollection.Find(filter).As<BsonDocument>().ToListAsync();  //查询数据
            //return await _mongoDBRepository.Table.Where(t => t.name == name).ToListAsync());
        }

        public async Task<AppExceptionLog> InsertAsync(AppExceptionLog testEntity)
        {
            return await _mongoDBRepository.InsertAsync(testEntity);
        }

        public async Task InsertObjectAsync(string json)
        {
            BsonDocument document = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<BsonDocument>(json);
            document.Add(new BsonElement("reportDate", DateTime.Now));
            await _mongoDBRepository.DocCollection.InsertOneAsync(document);
        }

        public async Task InsertObjectsAsync(string json)
        {
            List<BsonDocument> arr = MongoDB.Bson.Serialization.BsonSerializer.Deserialize<List<BsonDocument>>(json);
            await _mongoDBRepository.DocCollection.InsertManyAsync(arr);
        }

        public async Task<IPagedList<BsonDocument>> Pages(string name = "", int pageIndex = 1, int pageSize = int.MaxValue)
        {
            var query = from q in _mongoDBRepository.DocCollection.AsQueryable()
                        select q;
            if (!String.IsNullOrWhiteSpace(name))
            {
                query.Where(q => q["name"] == name);
            }
            query.OrderByDescending(q=>q["_id"]);

            return await PagedList<BsonDocument>.Create(query, pageIndex - 1, pageSize);
        }

        public async Task<bool> Update(string testEntity)
        {
            return await _mongoDBRepository.UpdateAsync(testEntity);
        }

        public async Task<long> DeleteObjectsAsync(Expression<Func<OpenApiOperateLog, bool>> filter)
        {
            DeleteResult result= await _mongoDBOpenApiOperateRepository.DeleteManyAsync(filter);
            return result.DeletedCount;
        }

        public async Task<long> DeleteObjectsAsync(Expression<Func<MongoOperateLog, bool>> filter)
        {
            DeleteResult result = await _mongoOperateRepository.DeleteManyAsync(filter);
            return result.DeletedCount;
        }

        public async Task<long> DeleteObjectsAsync(Expression<Func<AppExceptionLog, bool>> filter)
        {
            DeleteResult result = await _mongoAppExceptionLogRepository.DeleteManyAsync(filter);
            return result.DeletedCount;
        }

        public async Task<long> DeleteObjectsAsync(Expression<Func<CityUploadOperateLog, bool>> filter)
        {
            DeleteResult result = await _mongoCityUploadOperateLogRepository.DeleteManyAsync(filter);
            return result.DeletedCount;
        }

        public async Task<long> DeleteObjectsAsync(Expression<Func<CityOpenApiOperateLog, bool>> filter)
        {
            DeleteResult result = await _mongoCityOpenApiOperateLogRepository.DeleteManyAsync(filter);
            return result.DeletedCount;
        }
    }
}
