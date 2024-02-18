using System.Linq.Expressions;
using DataAccess.Data;
using DataAccess.UnitOfWork.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DataAccess.UnitOfWork.Classes
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        internal ApplicationDbContext _context;
        internal DbSet<TEntity> dbSet;
        private ISortHelper<TEntity> _sortHelper;

        public GenericRepository(ApplicationDbContext _context, ISortHelper<TEntity> sortHelper)
        {
            this._context = _context;
            this.dbSet = _context.Set<TEntity>();
            _sortHelper = sortHelper;
        }
        /// <summary>
        /// Get Paged List from TEntity.
        /// Ex for QueryString localhost:5001/api/events?pageNumber=2&pageSize=2
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="parameters"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public async Task<PagedList<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            //Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Parameters parameters = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;
            parameters ??= new Parameters();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            //if (orderBy != null)
            //{
            //    query = orderBy(query);
            //}

            if (!parameters.OrderBy.IsNullOrEmpty())
            {
                query = _sortHelper.ApplySort(query, parameters.OrderBy);
            }

            return await PagedList<TEntity>.ToPagedList(query, parameters.PageNumber, parameters.PageSize);
        }

        /// <summary>
        /// Get a single record from TEntity.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TEntity> GetByIdAsync(object id)
        {
            return await dbSet.FindAsync(id);
        }

        /// <summary>
        /// Add new record.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task InsertAsync(TEntity entity)
        {
            await dbSet.AddAsync(entity);
        }

        /// <summary>
        /// Update a single record by entity.
        /// </summary>
        /// <param name="entityToUpdate"></param>
        /// <returns></returns>
        public Task UpdateAsync(TEntity entityToUpdate)
        {
            dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
            return Task.CompletedTask;
        }

        /// <summary>
        /// Delete a single record by Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(object id)
        {
            TEntity entityToDelete = await dbSet.FindAsync(id);
            await DeleteAsync(entityToDelete);
        }

        /// <summary>
        /// Delete a single record by entity.
        /// </summary>
        /// <param name="entityToDelete"></param>
        /// <returns></returns>
        public Task DeleteAsync(TEntity entityToDelete)
        {
            if (entityToDelete != null)
            {
                if (_context.Entry(entityToDelete).State == EntityState.Detached)
                {
                    dbSet.Attach(entityToDelete);
                }
                dbSet.Remove(entityToDelete);
                return Task.CompletedTask;
            }
            return Task.FromCanceled(new CancellationToken());
        }


    }
}
