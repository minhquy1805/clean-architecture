using Application.Common.Helpers;
using Application.DTOs.Abstract;
using Application.Interfaces.Abstract;
using Application.Interfaces.Common;
using Shared.Helpers;
using System.Text.Json;

namespace Application.Services.Abstract
{
    public abstract class BasePagingFilterServiceMongo<TDto, TEntity, TFilter>
         : IBasePagingFilterServiceMongo<TDto, TFilter>
         where TEntity : class
         where TFilter : BasePagingFilterDto
    {
        protected readonly IMongoBaseRepository<TEntity> _repository;

        protected BasePagingFilterServiceMongo(IMongoBaseRepository<TEntity> repository)
        {
            _repository = repository;
        }

        // 🔹 CRUD
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
            return GetDtoId(dto);
        }

        public virtual async Task UpdateAsync(TDto dto)
        {
            await ValidateBeforeUpdate(dto);
            var entity = MapToEntity(dto);
            await _repository.UpdateAsync(GetDtoId(dto), entity);
        }

        public virtual async Task DeleteAsync(string id)
        {
            await ValidateBeforeDelete(id);
            await _repository.DeleteAsync(id);
        }


        // 🔹 Filter & Paging
        public abstract Task<IEnumerable<TDto>> SelectSkipAndTakeWhereDynamicAsync(TFilter filter);
        public abstract Task<int> GetRecordCountWhereDynamicAsync(TFilter filter);

        public async Task<(IEnumerable<TDto> Data, int TotalRecords)> GetPagingAsync(TFilter filter)
        {
            filter.SortBy = SortFieldValidator.Validate(filter.SortBy, GetAllowedSortFields(), "CreatedAt");
            filter.SortDirection = SortFieldValidator.ValidateDirection(filter.SortDirection);

            var totalRecords = await GetRecordCountWhereDynamicAsync(filter);
            var (start, _) = PaginationHelper.GetStartRowIndexAndTotalPages(filter.CurrentPage, filter.NumberOfRows, totalRecords);
            filter.Start = start;

            var data = await SelectSkipAndTakeWhereDynamicAsync(filter);
            return (data, totalRecords);
        }

        // 🔹 Hook logic
        protected virtual Task ValidateBeforeUpdate(TDto dto) => Task.CompletedTask;
        protected virtual Task ValidateBeforeDelete(string id) => Task.CompletedTask;
        protected virtual Task LogAuditAsync(string userId, string action, string? oldValue, string? newValue) => Task.CompletedTask;

        // 🔹 Mapping logic
        protected abstract string GetDtoId(TDto dto);
        protected abstract string[] GetAllowedSortFields();
        protected abstract TDto MapToDto(TEntity entity);
        protected abstract TEntity MapToEntity(TDto dto);

        // 🔹 Serialize helper
        protected string SerializeForAudit(object obj)
             => JsonSerializer.Serialize(obj, _auditOptions);

        protected string SerializePasswordForAudit(string label, string hash)
            => $"{label}: {hash}";

        private static readonly JsonSerializerOptions _auditOptions = new()
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = true
        };
    }
}
