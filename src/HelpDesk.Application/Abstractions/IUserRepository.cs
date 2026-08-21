using HelpDesk.Domain.Entities;
using HelpDesk.Domain.ValueObjects;

namespace HelpDesk.Application.Abstractions;

public interface IUserRepository
{
    Task<bool> ExistsByEmailAsync(
        Email email,
        CancellationToken cancellationToken);

    Task AddAsync(
        User user,
        CancellationToken cancellationToken);
}