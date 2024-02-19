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
        public async Task<PagedList<TEntity>> GetListAsync(
            Expression<Func<TEntity, bool>> filter = null,
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

            if (!parameters.OrderBy.IsNullOrEmpty())
            {
                query = _sortHelper.ApplySort(query, parameters.OrderBy);
            }

            return await PagedList<TEntity>.ToPagedList(query, parameters.PageNumber, parameters.PageSize);
        }

        public async Task<TEntity> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return await query.FirstOrDefaultAsync();
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
        public async Task<bool> DeleteAsync(object id)
        {
            TEntity entityToDelete = await dbSet.FindAsync(id);
            if (entityToDelete != null)
            {
                AttachEntity(entityToDelete);
                if (typeof(TEntity).GetProperty("IsDeleted") != null)
                {
                    await VirtualDeleteAsync(entityToDelete);
                }
                else { await PhysicalDeleteAsync(entityToDelete); }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Physical delete a single record by entity.
        /// </summary>
        /// <param name="entityToDelete"></param>
        /// <returns></returns>
        private Task PhysicalDeleteAsync(TEntity entityToDelete)
        {
            dbSet.Remove(entityToDelete);
            return Task.CompletedTask;
        }



        /// <summary>
        /// Virtual delete a single record by entity.
        /// </summary>
        /// <param name="entityToDelete"></param>
        /// <returns></returns>
        private Task VirtualDeleteAsync(TEntity entityToDelete)
        {
            var result = IsVirtualDeletableMarkIt(entityToDelete);
            return result ? Task.CompletedTask : Task.FromCanceled(new CancellationToken());

        }

        /// <summary>
        /// Return true: if entity has a IsDeleted attribute mark it as true.
        /// Else return false
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private bool IsVirtualDeletableMarkIt(TEntity entity)
        {
            try
            {
                var isDeletedProperty = typeof(TEntity).GetProperty("IsDeleted");
                if (isDeletedProperty != null && isDeletedProperty.PropertyType == typeof(bool))
                {
                    isDeletedProperty.SetValue(entity, true);
                    return true;
                }
                return false;
            }
            catch { return false; }
        }

        /// <summary>
        /// The entry provides access to change tracking information 
        /// and operations for the entity.
        /// </summary>
        /// <param name="entity"></param>
        private void AttachEntity(TEntity entity)
        {
            if (entity != null)
            {
                if (_context.Entry(entity).State == EntityState.Detached)
                {
                    dbSet.Attach(entity);
                }
            }
        }

    }
}
