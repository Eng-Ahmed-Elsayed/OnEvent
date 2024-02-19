using System.Linq.Expressions;
using DataAccess.UnitOfWork.Classes;

namespace DataAccess.UnitOfWork.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<PagedList<TEntity>> GetListAsync(
            Expression<Func<TEntity, bool>> filter = null,
            //Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Parameters parameters = null,
            string includeProperties = "");

        Task<TEntity> GetByIdAsync(object id);

        Task<TEntity> GetAsync(
            Expression<Func<TEntity, bool>> filter = null,
            string includeProperties = "");

        Task InsertAsync(TEntity entity);

        Task UpdateAsync(TEntity entity);

        Task DeleteAsync(object id);

    }
}
