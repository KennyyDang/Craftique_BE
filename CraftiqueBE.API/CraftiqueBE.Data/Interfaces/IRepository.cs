using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data.Interfaces
{
	public interface IRepository<TEntity> where TEntity : class
	{
		Task CommitAsync();
		Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null, params Expression<Func<TEntity, object>>[] includes);
		Task<TEntity?> GetByIdAsync(int id, params Expression<Func<TEntity, object>>[] includes);
		Task<TEntity> GetSingleByConditionAsynce(Expression<Func<TEntity, bool>> predicate = null, params Expression<Func<TEntity, object>>[] includes);
		Task<TEntity> AddAsync(TEntity entity);
		Task AddRangeAsync(List<TEntity> entities);
		Task<bool> Update(TEntity entity);
		Task<bool> UpdateRange(List<TEntity> entities);
		Task<bool> SoftDelete(TEntity entity);
		Task<bool> SoftDeleteRange(List<TEntity> entities);
		Task<bool> HardRemove(Expression<Func<TEntity, bool>> predicate);
		Task<bool> HardRemoveRange(List<TEntity> entities);
		IQueryable<TEntity> GetAllQueryable();
	}
}
