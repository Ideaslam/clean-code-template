using Microsoft.EntityFrameworkCore.Storage;

namespace Application.Common.Interfaces;

public interface IUnitOfWork
{
	Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}