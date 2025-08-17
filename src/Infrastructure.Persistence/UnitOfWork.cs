using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
	private readonly AppDbContext _dbContext;

	public UnitOfWork(AppDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
		=> _dbContext.Database.BeginTransactionAsync(cancellationToken);

	public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
		=> _dbContext.SaveChangesAsync(cancellationToken);
}