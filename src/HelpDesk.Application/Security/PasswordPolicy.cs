using HelpDesk.Application.Abstractions;
using HelpDesk.Domain.Exceptions;

namespace HelpDesk.Application.Security;

public sealed class PasswordPolicy : IPasswordPolicy
{
    public void Validate(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new DomainException("Password is required");
        }

        if (password.Length < 8)
        {
            throw new DomainException("Password must be at least 8 characters long.");
        }

        if (!password.Any(char.IsUpper))
        {
            throw new DomainException("Password must contain at least one uppercase letter.");
        }
        
        if (!password.Any(char.IsDigit))
        {
            throw new DomainException("Password must contain at least one digit.");
        }
        
        if (password.All(char.IsLetterOrDigit))
        {
            throw new DomainException("Password must contain at least one special character.");
        }
    }
}