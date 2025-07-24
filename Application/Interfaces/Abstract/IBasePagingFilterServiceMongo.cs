using Application.Interfaces.Common;

namespace Application.Interfaces.Abstract
{
    public interface IBasePagingFilterServiceMongo<TDto, TFilter> : IBaseServiceMongo<TDto>
    {
        Task<(IEnumerable<TDto> Data, int TotalRecords)> GetPagingAsync(TFilter filter);
        Task<IEnumerable<TDto>> SelectSkipAndTakeWhereDynamicAsync(TFilter filter);
        Task<int> GetRecordCountWhereDynamicAsync(TFilter filter);
    }
}
