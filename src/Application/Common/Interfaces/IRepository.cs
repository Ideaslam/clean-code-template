using System.Linq.Expressions;

namespace Application.Common.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
	Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
	Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken cancellationToken = default);
	Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
	Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
	void Update(TEntity entity);
	void Remove(TEntity entity);
}