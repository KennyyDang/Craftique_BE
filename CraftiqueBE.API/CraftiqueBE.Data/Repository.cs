using CraftiqueBE.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CraftiqueBE.Data
{
	public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IBaseEntity
	{
		private readonly CraftiqueDBContext _dbContext;

		public Repository(CraftiqueDBContext dbContext)
		{
			_dbContext = dbContext;
		}

		public async Task<TEntity> AddAsync(TEntity entity)
		{
			try
			{
				var result = await _dbContext.Set<TEntity>().AddAsync(entity);
				return result.Entity;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task AddRangeAsync(List<TEntity> entities)
		{
			try
			{
				await _dbContext.Set<TEntity>().AddRangeAsync(entities);
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate = null, params Expression<Func<TEntity, object>>[] includes)
		{
			IQueryable<TEntity> query = _dbContext.Set<TEntity>();

			if (predicate != null)
			{
				query = query.Where(predicate);
			}

			if (includes != null)
			{
				foreach (var include in includes)
				{
					query = query.Include(include);
				}
			}

			// Add OrderBy for entities that have DisplayIndex property
			if (typeof(TEntity).GetProperty("DisplayIndex") != null)
			{
				var parameter = Expression.Parameter(typeof(TEntity), "x");
				var property = Expression.Property(parameter, "DisplayIndex");
				var lambda = Expression.Lambda<Func<TEntity, int>>(property, parameter);
				query = query.OrderBy(lambda);
			}

		return await query.ToListAsync();
		}

		public IQueryable<TEntity> GetAllQueryable()
		{
			return _dbContext.Set<TEntity>().AsQueryable();
		}


		public async Task<TEntity?> GetByIdAsync(int id, params Expression<Func<TEntity, object>>[] includes)
		{
			IQueryable<TEntity> query = _dbContext.Set<TEntity>();

			if (includes != null)
			{
				foreach (var include in includes)
				{
					query = query.Include(include);
				}
			}

			// Tìm thuộc tính chứa "Id" (ví dụ: Id, ProductId, UserId, ...)
			var keyProperty = typeof(TEntity)
				.GetProperties()
				.FirstOrDefault(p => p.Name.EndsWith("ID", StringComparison.OrdinalIgnoreCase));

			if (keyProperty == null)
			{
				throw new InvalidOperationException($"Entity {typeof(TEntity).Name} không có khóa chính hợp lệ.");
			}

			// Tạo biểu thức động: x => x.{KeyProperty} == id
			var parameter = Expression.Parameter(typeof(TEntity), "x");
			var propertyAccess = Expression.Property(parameter, keyProperty);
			var constant = Expression.Constant(id);
			var equalExpression = Expression.Equal(propertyAccess, constant);
			var lambda = Expression.Lambda<Func<TEntity, bool>>(equalExpression, parameter);

			return await query.FirstOrDefaultAsync(lambda);
		}

		public async Task<TEntity> GetSingleByConditionAsynce(Expression<Func<TEntity, bool>> predicate = null, params Expression<Func<TEntity, object>>[] includes)
		{
			IQueryable<TEntity> query = _dbContext.Set<TEntity>();

			if (includes != null)
			{
				foreach (var include in includes)
				{
					query = query.Include(include);
				}
			}

			return await query.FirstOrDefaultAsync(predicate);
		}

		public async Task<bool> HardRemove(Expression<Func<TEntity, bool>> predicate)
		{
			try
			{
				var entities = await _dbContext.Set<TEntity>().Where(predicate).ToListAsync();
				if (entities.Any())
				{
					_dbContext.Set<TEntity>().RemoveRange(entities);
					return true;
				}
				return false; // Không có gì để xóa
			}
			catch (Exception ex)
			{
				throw new Exception($"Error while performing hard remove: {ex.Message}");
			}
		}

		public async Task<bool> HardRemoveRange(List<TEntity> entities)
		{
			try
			{
				if (entities.Any())
				{
					_dbContext.Set<TEntity>().RemoveRange(entities);
					return true;
				}
				return false; // Không có gì để xóa
			}
			catch (Exception ex)
			{
				throw new Exception($"Error while performing hard remove range: {ex.Message}");
			}
		}

		public async Task<bool> SoftDelete(TEntity entity)
		{
			entity.IsDeleted = true;
			_dbContext.Set<TEntity>().Update(entity);
			return true;
		}

		public async Task<bool> SoftDeleteRange(List<TEntity> entities)
		{
			foreach (var entity in entities)
			{
				entity.IsDeleted = true;
			}
			_dbContext.Set<TEntity>().UpdateRange(entities);
			//  await _dbContext.SaveChangesAsync();
			return true;
		}

		public async Task<bool> Update(TEntity entity)
		{
			_dbContext.Set<TEntity>().Update(entity);
			return true;
		}

		public async Task<bool> UpdateRange(List<TEntity> entities)
		{
			_dbContext.Set<TEntity>().UpdateRange(entities);
			return true;
		}

		public async Task CommitAsync()
		{
			await _dbContext.SaveChangesAsync();
		}
	}
}
