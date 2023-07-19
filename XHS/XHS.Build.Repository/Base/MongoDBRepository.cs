using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using XHS.Build.Model.MongoBase;

namespace XHS.Build.Repository.Base
{
    public class MongoDBRepository<T> : IMongoDBRepository<T> where T : BaseEntity
    {
        /// <summary>
        /// Gets the collection
        /// </summary>
        protected IMongoCollection<T> _collection;
        public IMongoCollection<T> Collection
        {
            get
            {
                return _collection;
            }
        }

        /// <summary>
        /// Mongo Database
        /// </summary>
        protected IMongoDatabase _database;
        public IMongoDatabase Database
        {
            get
            {
                return _database;
            }
        }

        protected IMongoCollection<BsonDocument> _docCollection;
        public IMongoCollection<BsonDocument> DocCollection
        {
            get
            {
                return _docCollection;
            }
        }


        public MongoDBRepository(string connectionString)
        {
            var client = new MongoClient(connectionString);
            var databaseName = new MongoUrl(connectionString).DatabaseName;
            _database = client.GetDatabase(databaseName);
            _collection = _database.GetCollection<T>(typeof(T).Name);
            _docCollection = _database.GetCollection<BsonDocument>(typeof(T).Name);
        }

        public MongoDBRepository(IMongoDatabase database)
        {
            _database = database;
            _collection = _database.GetCollection<T>(typeof(T).Name);
            _docCollection = _database.GetCollection<BsonDocument>(typeof(T).Name);
        }

        #region Methods

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public virtual T GetById(string id)
        {
            return _collection.Find(e => e.Id == id).FirstOrDefault();
        }

        /// <summary>
        /// Get entity by identifier async
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public virtual Task<T> GetByIdAsync(string id)
        {
            return _collection.Find(e => e.Id == id).FirstOrDefaultAsync();
        }
        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual T Insert(T entity)
        {
            _collection.InsertOne(entity);
            return entity;
        }

        /// <summary>
        /// Async Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task<T> InsertAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }


        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Insert(IEnumerable<T> entities)
        {
            _collection.InsertMany(entities);
        }

        /// <summary>
        /// Async Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual async Task<IEnumerable<T>> InsertAsync(IEnumerable<T> entities)
        {
            await _collection.InsertManyAsync(entities);
            return entities;
        }


        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual T Update(T entity)
        {
            _collection.ReplaceOne(x => x.Id == entity.Id, entity, new ReplaceOptions() { IsUpsert = false });
            return entity;

        }

        /// <summary>
        /// Async Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task<T> UpdateAsync(T entity)
        {
            await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity, new ReplaceOptions() { IsUpsert = false });
            return entity;
        }


        /// <summary>
        /// Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Update(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                Update(entity);
            }
        }


        public async Task<bool> UpdateAsync(string entity)
        {
            var jobj = MongoDB.Bson.Serialization.BsonSerializer.Deserialize(entity,typeof(object));// JsonConvert.DeserializeObject<object>(entity);
            var doc = jobj.ToBsonDocument();
            var filter = Builders<BsonDocument>.Filter.Eq(f => f["_id"], doc["_id"]);
            var update = Builders<BsonDocument>.Update.Combine(BuildUpdateDefinition(doc, null));
            var updateOptions = new UpdateOptions { IsUpsert = true };
            await _database.GetCollection<BsonDocument>(typeof(T).Name).UpdateOneAsync(filter, update, updateOptions);
            return true;
        }

        /// <summary>
        /// Async Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual async Task<IEnumerable<T>> UpdateAsync(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                await UpdateAsync(entity);
            }
            return entities;
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Delete(T entity)
        {
            _collection.FindOneAndDelete(e => e.Id == entity.Id);
        }

        /// <summary>
        /// Async Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual async Task<T> DeleteAsync(T entity)
        {
            await _collection.DeleteOneAsync(e => e.Id == entity.Id);
            return entity;
        }

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Delete(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                _collection.FindOneAndDeleteAsync(e => e.Id == entity.Id);
            }
        }

        /// <summary>
        /// Async Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual async Task<IEnumerable<T>> DeleteAsync(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                await DeleteAsync(entity);
            }
            return entities;
        }

        public virtual  DeleteResult DeleteMany(Expression<Func<T, bool>> filter)
        {
            return _collection.DeleteMany(filter);
        }

        public virtual async Task<DeleteResult> DeleteManyAsync(Expression<Func<T, bool>> filter)
        {
            return await  _collection.DeleteManyAsync(filter);
        }

        public virtual DeleteResult DeleteMany(FilterDefinition<BsonDocument> filter)
        {
            return _docCollection.DeleteMany(filter);
        }

        public virtual async Task<DeleteResult> DeleteManyAsync(FilterDefinition<BsonDocument> filter)
        {
            return await _docCollection.DeleteManyAsync(filter);
        }
        #endregion


        #region Methods

        /// <summary>
        /// Determines whether a list contains any elements
        /// </summary>
        /// <returns></returns>
        public virtual bool Any()
        {
            return _collection.AsQueryable().Any();
        }

        /// <summary>
        /// Determines whether any element of a list satisfies a condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual bool Any(Expression<Func<T, bool>> where)
        {
            return _collection.Find(where).Any();
        }

        /// <summary>
        /// Async determines whether a list contains any elements
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> AnyAsync()
        {
            return await _collection.AsQueryable().AnyAsync();
        }

        /// <summary>
        /// Async determines whether any element of a list satisfies a condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> where)
        {
            return await _collection.Find(where).AnyAsync();
        }

        /// <summary>
        /// Returns the number of elements in the specified sequence.
        /// </summary>
        /// <returns></returns>
        public virtual long Count()
        {
            return _collection.CountDocuments(new BsonDocument());
        }

        /// <summary>
        /// Returns the number of elements in the specified sequence that satisfies a condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual long Count(Expression<Func<T, bool>> where)
        {
            return _collection.CountDocuments(where);
        }

        /// <summary>
        /// Async returns the number of elements in the specified sequence
        /// </summary>
        /// <returns></returns>
        public virtual async Task<long> CountAsync()
        {
            return await _collection.CountDocumentsAsync(new BsonDocument());
        }

        /// <summary>
        /// Async returns the number of elements in the specified sequence that satisfies a condition.
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public virtual async Task<long> CountAsync(Expression<Func<T, bool>> where)
        {
            return await _collection.CountDocumentsAsync(where);
        }


        #endregion

        #region Properties

        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IMongoQueryable<T> Table
        {
            get { return _collection.AsQueryable(); }
        }

        /// <summary>
        /// Get collection by filter definitions
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public virtual IList<T> FindByFilterDefinition(FilterDefinition<T> query, SortDefinition<T> sort = null)
        {
            return _collection.Find(query).Sort(sort).ToList();
        }

        public virtual IList<T> FindByFilterWithPage(FilterDefinition<T> query,int pageIndex, int pageSize = 20, SortDefinition<T> sort = null)
        {
            return _collection.Find(query).Sort(sort).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();
        }

        public virtual IList<T> FindByFilterWithPageTotal(FilterDefinition<T> query, int pageIndex, ref long total, int pageSize = 20, SortDefinition<T> sort = null)
        {
            total = _collection.CountDocuments(query);
            return _collection.Find(query).Sort(sort).Skip((pageIndex - 1) * pageSize).Limit(pageSize).ToList();
        }

        [Obsolete]
        public virtual long FindByFilterTotalcount(FilterDefinition<T> query, SortDefinition<T> sort = null)
        {
            return _collection.Find(query).Sort(sort).Count();
        }
        #endregion


        /// <summary>
        /// 构建更新操作定义 
        /// </summary>
        /// <param name="bc">BsonDocument</param>
        /// <param name="parent">父级</param>
        /// <returns></returns>
        private List<UpdateDefinition<BsonDocument>> BuildUpdateDefinition(BsonDocument bc, string parent)
        {
            var updates = new List<UpdateDefinition<BsonDocument>>();
            foreach (var element in bc.Elements)
            {
                var key = parent == null ? element.Name : $"{parent}.{element.Name}";
                var subUpdates = new List<UpdateDefinition<BsonDocument>>();
                //子元素是对象
                if (element.Value.IsBsonDocument)
                {
                    updates.AddRange(BuildUpdateDefinition(element.Value.ToBsonDocument(), key));
                }
                //子元素是对象数组
                else if (element.Value.IsBsonArray)
                {
                    var arrayDocs = element.Value.AsBsonArray;
                    var i = 0;
                    foreach (var doc in arrayDocs)
                    {
                        if (doc.IsBsonDocument)
                        {
                            updates.AddRange(BuildUpdateDefinition(doc.ToBsonDocument(), key + $".{i}"));
                        }
                        else
                        {
                            updates.Add(Builders<BsonDocument>.Update.Set(f => f[key], element.Value));
                            continue;
                        }
                        i++;
                    }
                }
                //子元素是其他
                else
                {
                    updates.Add(Builders<BsonDocument>.Update.Set(f => f[key], element.Value));
                }
            }
            return updates;
        }


    }
}
