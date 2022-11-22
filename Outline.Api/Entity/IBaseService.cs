using Outline.Api.Extensions;
using System;
using System.Threading.Tasks;

namespace Outline.Api.Entity
{
    public interface IBaseService<TKey, in TUpdate, in TInsert, TOut, TListOut, in TFilter>
        where TKey : IEquatable<TKey>
        where TOut : EntityDto<TKey>
        where TFilter : PaginationModelInput

    {
        Task Delete(TKey id);

        Task SoftDelete(TKey id);

        Task<Pagination<TListOut>> GetAllAsync(TFilter paging, params string[] include);

        Task<TOut> GetById(TKey id, params string[] include);

        Task<TKey> InsertGetIdAsync(TInsert input);

        Task InsertAsync(TInsert input);

        Task UpdateAsync(TKey id, TUpdate input, params string[] include);
    }
}