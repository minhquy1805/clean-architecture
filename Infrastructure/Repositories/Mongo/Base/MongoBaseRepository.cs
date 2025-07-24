using Application.Interfaces.Common;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Infrastructure.Repositories.Mongo.Base
{
    public abstract class MongoBaseRepository<T> : IMongoBaseRepository<T> where T : class
    {
        protected readonly IMongoCollection<T> _collection;

        protected MongoBaseRepository(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<T>(collectionName);
        }

        // ✅ Lấy tất cả
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(Builders<T>.Filter.Empty).ToListAsync();
        }

        // ✅ Lấy theo _id (string)
        public async Task<T?> GetByIdAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        // ✅ Lấy theo điều kiện filter
        public async Task<T?> GetOneAsync(Expression<Func<T, bool>> filter)
        {
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        // ✅ Thêm mới
        public async Task InsertAsync(T entity)
        {
            Console.WriteLine($"📥 Mongo Insert Called");
            Console.WriteLine($"📥 Mongo Insert Called");
            await _collection.InsertOneAsync(entity);
        }

        // ✅ Cập nhật theo _id
        public async Task UpdateAsync(string id, T entity)
        {
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
            await _collection.ReplaceOneAsync(filter, entity);
        }

        // ✅ Cập nhật theo điều kiện
        public async Task UpdateAsync(Expression<Func<T, bool>> filter, T entity)
        {
            await _collection.ReplaceOneAsync(filter, entity);
        }

        // ✅ Xoá theo _id
        public async Task DeleteAsync(string id)
        {
            var filter = Builders<T>.Filter.Eq("_id", ObjectId.Parse(id));
            await _collection.DeleteOneAsync(filter);
        }

        // ✅ Xoá theo điều kiện
        public async Task DeleteAsync(Expression<Func<T, bool>> filter)
        {
            await _collection.DeleteOneAsync(filter);
        }
    }
}
