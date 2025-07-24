using Application.DTOs.LoginHistories;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Helpers.Mongo;
using Infrastructure.Repositories.Mongo.Base;
using MongoDB.Driver;

namespace Infrastructure.Repositories.Mongo.Users
{
    public class MongoLoginHistoryRepository : MongoBaseRepository<LoginHistory>, ILoginHistoryRepository
    {
        public MongoLoginHistoryRepository(IMongoDatabase database)
            : base(database, "LoginHistory") { }


        public async Task<IEnumerable<LoginHistory>> GetByUserIdAsync(int userId)
        {
            var filter = Builders<LoginHistory>.Filter.Eq(x => x.UserId, userId);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<LoginHistory?> GetLastLoginAsync(int userId)
        {
            var filter = Builders<LoginHistory>.Filter.Eq(x => x.UserId, userId);
            return await _collection.Find(filter)
                                    .SortByDescending(x => x.CreatedAt)
                                    .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<LoginHistory>> SelectSkipAndTakeWhereDynamicAsync(LoginHistoryFilterDto filterDto)
        {
            var filter = BuildFilter(filterDto);

            return await _collection.Find(filter)
                                    .SortByDescending(x => x.CreatedAt)
                                    .Skip(filterDto.Skip)
                                    .Limit(filterDto.Take)
                                    .ToListAsync();
        }

        public async Task<int> GetRecordCountWhereDynamicAsync(LoginHistoryFilterDto filterDto)
        {
            var filter = BuildFilter(filterDto);
            return (int)await _collection.CountDocumentsAsync(filter);
        }

        private FilterDefinition<LoginHistory> BuildFilter(LoginHistoryFilterDto filterDto)
        {
            return MongoFilterBuilder.BuildFilter<LoginHistory>(filterDto);
        }

    }
}
