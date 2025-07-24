using Application.Interfaces.Common;
using System.Text.Json;

namespace Application.Services.Common
{
    public abstract class BaseServiceMongo<TDto, TEntity> : IBaseServiceMongo<TDto>
        where TEntity : class
    {
        protected readonly IMongoBaseRepository<TEntity> _repository;

        protected BaseServiceMongo(IMongoBaseRepository<TEntity> repository)
        {
            _repository = repository;
        }

        public virtual async Task<IEnumerable<TDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(MapToDto);
        }

        public virtual async Task<TDto?> GetByIdAsync(string id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? default : MapToDto(entity);
        }

        public virtual async Task<string> InsertAsync(TDto dto)
        {
            var entity = MapToEntity(dto);
            await _repository.InsertAsync(entity);
            return GetDtoId(dto); // trả lại Id dạng string (Mongo _id)
        }

        public virtual async Task UpdateAsync(TDto dto)
        {
            var id = GetDtoId(dto);
            var entity = MapToEntity(dto);
            await _repository.UpdateAsync(id, entity);
        }

        public virtual async Task DeleteAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }

        // 🔹 Hook method (override nếu cần)
        protected virtual Task ValidateBeforeUpdate(TDto dto) => Task.CompletedTask;
        protected virtual Task ValidateBeforeDelete(string id) => Task.CompletedTask;

        // 🔹 Mapping abstract
        protected abstract TDto MapToDto(TEntity entity);
        protected abstract TEntity MapToEntity(TDto dto);
        protected abstract string GetDtoId(TDto dto);

        // 🔹 JSON serialize để audit
        protected string SerializeForAudit(object obj) => JsonSerializer.Serialize(obj, _auditOptions);
        protected string SerializePasswordForAudit(string label, string hash) => $"{label}: {hash}";

        private static readonly JsonSerializerOptions _auditOptions = new()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };
    }
}
