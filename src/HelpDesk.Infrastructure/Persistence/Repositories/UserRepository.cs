using HelpDesk.Application.Abstractions;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Persistence.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<bool> ExistsByEmailAsync(
        Email email,
        CancellationToken cancellationToken)
    {
        return _dbContext.Users.AnyAsync(
            x => x.Email == email,
            cancellationToken);
    }

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken)
    {
        await _dbContext.Users.AddAsync(
            user,
            cancellationToken);
    }
}