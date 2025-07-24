using System.Linq.Expressions;

namespace Application.Interfaces.Common
{
    public interface IMongoBaseRepository<T> where T : class
    {
        // ✅ Lấy tất cả
        Task<IEnumerable<T>> GetAllAsync();

        // ✅ Lấy theo Id (ObjectId dạng string)
        Task<T?> GetByIdAsync(string id);

        // ✅ Lấy theo điều kiện
        Task<T?> GetOneAsync(Expression<Func<T, bool>> filter);

        // ✅ Thêm mới
        Task InsertAsync(T entity);

        // ✅ Cập nhật theo Id
        Task UpdateAsync(string id, T entity);

        // ✅ Cập nhật theo điều kiện
        Task UpdateAsync(Expression<Func<T, bool>> filter, T entity);

        // ✅ Xoá theo Id
        Task DeleteAsync(string id);

        // ✅ Xoá theo điều kiện
        Task DeleteAsync(Expression<Func<T, bool>> filter);
    }
}
