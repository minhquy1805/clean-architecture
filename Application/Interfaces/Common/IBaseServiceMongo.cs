namespace Application.Interfaces.Common
{
    public interface IBaseServiceMongo<TDto>
    {
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto?> GetByIdAsync(string id);
        Task<string> InsertAsync(TDto dto);
        Task UpdateAsync(TDto dto);
        Task DeleteAsync(string id);
    }
}
