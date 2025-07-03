using Application.Interfaces.Common;

namespace Application.Services.Common
{
    public abstract class BaseService<TDto, TEntity> : IBaseService<TDto>
    {
        protected readonly IBaseRepository<TEntity> _repository;

        protected BaseService(IBaseRepository<TEntity> repository)
        {
            _repository = repository;
        }

        public virtual async Task<IEnumerable<TDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(MapToDto);
        }

        public virtual async Task<TDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? default! : MapToDto(entity);
        }

        public virtual async Task<int> InsertAsync(TDto dto)
        {
            var entity = MapToEntity(dto);
            return await _repository.InsertAsync(entity);
        }

        public virtual async Task UpdateAsync(TDto dto)
        {
            await ValidateBeforeUpdate(dto);
            var entity = MapToEntity(dto);
            await _repository.UpdateAsync(entity);
        }

        public virtual async Task DeleteAsync(int id)
        {
            await ValidateBeforeDelete(id);
            await _repository.DeleteAsync(id);
        }

        // 🧩 Hook method: override ở lớp con nếu cần
        protected virtual Task ValidateBeforeUpdate(TDto dto) => Task.CompletedTask;
        protected virtual Task ValidateBeforeDelete(int id) => Task.CompletedTask;

        // 🧩 Audit Hook: override ở lớp con để ghi log
        protected virtual Task LogAuditAsync(int userId, string action, string oldValue, string newValue)
        {
            // Mặc định không log (optional)
            return Task.CompletedTask;
        }

        // 🧩 Extract Id từ DTO
        protected abstract int GetDtoId(TDto dto);


        // Abstract mapping logic
        protected abstract TDto MapToDto(TEntity entity);
        protected abstract TEntity MapToEntity(TDto dto);
    }
}
