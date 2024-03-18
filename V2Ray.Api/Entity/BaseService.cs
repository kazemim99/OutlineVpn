using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using V2Ray.Api.Database;
using V2Ray.Api.Extensions;

namespace V2Ray.Api.Entity
{
    public abstract class BaseService<TEntity, TKey, TUpdate, TInsert, TOut, TListOut, TFilter> :

        IBaseService<TKey, TUpdate, TInsert, TOut, TListOut, TFilter> where TEntity : Entity<TKey>
        where TFilter : PaginationModelInput
        where TKey : IEquatable<TKey>
        where TOut : EntityDto<TKey>
    {
        protected readonly IMapper _mapper;

        protected readonly DB _db;

        protected BaseService(IMapper mapper, DB db)
        {
            _mapper = mapper;
            _db = db;
        }

        public virtual async Task InsertAsync(TInsert input)
        {
            try
            {
                var entity = _mapper.Map<TEntity>(input);
                await _db.Set<TEntity>().AddAsync(entity);
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public virtual async Task<TKey> InsertGetIdAsync(TInsert input)
        {
            try
            {
                var entity = _mapper.Map<TEntity>(input);
                var model = await _db.Set<TEntity>().AddAsync(entity);

                await _db.SaveChangesAsync();

                return model.Entity.Id;
            }
            catch (Exception ex)
            {

                throw;
            }
         
        }

        public virtual async Task UpdateAsync(TKey id, TUpdate input, params string[] include)
        {
            try
            {

            var map = _mapper.Map<TUpdate, TEntity>(input);
            map.Id = id;
            _db.Entry(map).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task SoftDelete(TKey id)
        {
            var query = _db.Set<TEntity>().AsQueryable();
            var entity = await query.FirstAsync(o => o.Id.Equals(id)) as ISoftDelete;
            entity.IsDeleted = true;
            _db.Entry(entity).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public virtual async Task<Pagination<TListOut>> GetAllAsync(TFilter paging, params string[] include)
        {
            var model = Filter(paging);
            var result = await model.Include(include)
                .GetPagination<TEntity, TListOut, TFilter>(paging, _mapper);
            return result;
        }

        public virtual async Task<Pagination<TOut>> GetAllAsync(Expression<Func<TEntity, bool>> predicate, TFilter paging, params string[] include)
        {
            var model = await SearchFor(predicate, include).GetPagination<TEntity, TOut, TFilter>(paging, _mapper);
            return model;
        }

        public virtual IQueryable<TEntity> Filter(TFilter filter)
        {
            return _db.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> SearchFor(Expression<Func<TEntity, bool>> predicate, params string[] include)
        {
            return _db.Set<TEntity>().Include(include).Where(predicate);
        }

        public async virtual Task<TEntity> GetEntityByIdAsync(TKey id, params string[] include)
        {
            var entity = await _db.Set<TEntity>().Include(include).SingleOrDefaultAsync(o => o.Id.Equals(id));

            return entity;
        }

        public virtual async Task<TOut> GetById(TKey id, params string[] include)
        {
            var q = _db.Set<TEntity>().AsQueryable();

            q = q.Include(include);

            var entity = await q.FirstOrDefaultAsync(o => o.Id.Equals(id));

            return _mapper.Map<TOut>(entity);
        }

        public async virtual Task Delete(TKey id)
        {
            var model = await _db.Set<TEntity>().SingleOrDefaultAsync(o => o.Id.Equals(id));
            if (model == null)
                throw new Exception("Entity Not Found");

            _db.Entry(model).State = EntityState.Deleted;

            await _db.SaveChangesAsync();
        }
    }
}