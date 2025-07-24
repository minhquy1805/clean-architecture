using Application.DTOs.AuditLogs;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Helpers.Mongo;
using Infrastructure.Repositories.Mongo.Base;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;

namespace Infrastructure.Repositories.Mongo.Users
{
    public class MongoUserAuditRepository : MongoBaseRepository<UserAudit>, IUserAuditRepository
    {
        public MongoUserAuditRepository(IMongoDatabase database)
            : base(database, "UserAudit")
        {
        }

        public async Task<IEnumerable<UserAudit>> GetByUserIdAsync(int userId)
        {
            var filter = Builders<UserAudit>.Filter.Eq(a => a.UserId, userId);
            return await _collection.Find(filter).ToListAsync();
        }

        public async Task<int> GetRecordCountWhereDynamicAsync(AuditLogFilterDto filterDto)
        {
            var filter = BuildFilter(filterDto);
            return (int)await _collection.CountDocumentsAsync(filter);
        }

        public async Task<IEnumerable<UserAudit>> SelectSkipAndTakeWhereDynamicAsync(AuditLogFilterDto filterDto)
        {
            var filter = BuildFilter(filterDto);
            var sort = Builders<UserAudit>.Sort.Descending(a => a.CreatedAt);

            return await _collection.Find(filter)
                .Sort(sort)
                .Skip(filterDto.Skip)
                .Limit(filterDto.Take)
                .ToListAsync();
        }

        public async Task LogAuditAsync(int userId, string action, object? oldValue, object? newValue, string? ip = null)
        {
            var audit = new UserAudit
            {
                Id = ObjectId.GenerateNewId().ToString(),
                UserId = userId,
                Action = action,
                OldValue = oldValue != null ? JsonSerializer.Serialize(oldValue) : null,
                NewValue = newValue != null ? JsonSerializer.Serialize(newValue) : null,
                IpAddress = ip,
                CreatedAt = DateTime.UtcNow,
                Flag = "T"
            };

            await _collection.InsertOneAsync(audit);
        }

        

        private FilterDefinition<UserAudit> BuildFilter(AuditLogFilterDto dto)
        {
            return MongoFilterBuilder.BuildFilter<UserAudit>(dto);
        }

    }
}
