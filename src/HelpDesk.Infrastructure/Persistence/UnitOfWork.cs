using HelpDesk.Application.Abstractions;
using HelpDesk.Infrastructure.Persistence.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Persistence;

public sealed class UnitOfWork(AppDbContext dbContext)
    : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception)
        {
            if (DatabaseExceptionTranslator.TryTranslate(exception, out var translatedException))
            {
                throw translatedException!;
            }

            throw;
        }
    }
}