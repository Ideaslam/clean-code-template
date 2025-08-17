using System.Linq.Expressions;
using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
	private readonly AppDbContext _dbContext;
	private readonly DbSet<TEntity> _set;

	public GenericRepository(AppDbContext dbContext)
	{
		_dbContext = dbContext;
		_set = _dbContext.Set<TEntity>();
	}

	public async Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
		=> await _set.FindAsync([id], cancellationToken);

	public async Task<IReadOnlyList<TEntity>> ListAsync(CancellationToken cancellationToken = default)
		=> await _set.AsNoTracking().ToListAsync(cancellationToken);

	public async Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
		=> await _set.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

	public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
		=> await _set.AddAsync(entity, cancellationToken);

	public void Update(TEntity entity) => _set.Update(entity);

	public void Remove(TEntity entity) => _set.Remove(entity);
}