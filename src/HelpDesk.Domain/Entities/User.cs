using HelpDesk.Domain.Common;
using HelpDesk.Domain.Enums;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.ValueObjects;

namespace HelpDesk.Domain.Entities;

public class User : BaseEntity
{
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public FullName FullName { get; private set; } = null!;
    public UserRole Role { get; private set; }
    public bool IsBlocked { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    
    private  User()
    {
    }

    private User(Email email,
        string passwordHash,
        FullName fullName,
        DateTime createdAtUtc)
    {
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
        Role = UserRole.Client;
        IsBlocked = false;
        CreatedAtUtc = createdAtUtc;
    }

    public static User Create(Email email,
        string passwordHash,
        FullName fullName,
        DateTime createdAtUtc
    )
    {
        Validate(email, fullName, passwordHash, createdAtUtc);
        
        return new User(email, passwordHash, fullName, createdAtUtc);
    }

    private static void Validate(Email email, 
        FullName fullName, string passwordHash,
        DateTime createdAtUtc)
    {
        if (email is null)
        {
            throw new DomainException("Email is required.");
        }
        
        if (fullName is null)
        {
            throw new DomainException("FullName is required.");
        }
        
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainException("Password hash is required.");
        }
        
        if (createdAtUtc == default)
        {
            throw new DomainException("CreatedAtUtc is required.");
        }
    }
    
    public void ChangeFullName(FullName newFullName)
    {
        if (newFullName is null)
        {
            throw new DomainException("Full name is required.");
        }

        if (newFullName == FullName)
        {
            throw new DomainException(
                "The new full name must differ from the current full name.");
        }

        FullName = newFullName;
    }

    public void ChangeEmail(Email email)
    {
        if (email is null)
        {
            throw new DomainException("Email is required.");
        }

        if (email == Email)
        {
            throw new DomainException("The new email must be different from the current email.");
        }
        
        Email = email;
    }

    public void ChangePasswordHash(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
        {
            throw new DomainException("Password hash is required.");
        }

        if (newPasswordHash == PasswordHash)
        {
            throw new DomainException("The new password hash must differ from the current password hash.");
        }
        
        PasswordHash = newPasswordHash;
    }

    public void AssignRole(UserRole newRole)
    {
        var isValid = Enum.IsDefined(newRole);

        if (!isValid)
        {
            throw new DomainException("Role is not valid.");
        }

        if (newRole == Role)
        {
            throw new DomainException("Cannot assign the same role.");
        }

        Role = newRole;
    }

    public void Block()
    {
        if (IsBlocked)
        {
            throw new DomainException("User is already blocked.");
        }
        
        IsBlocked = true;
    }

    public void Unblock()
    {
        if (!IsBlocked)
        {
            throw new DomainException("Cannot unblock a user who is not blocked.");
        }
        
        IsBlocked = false;
    }
}