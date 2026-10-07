using HelpDesk.Application.Abstractions;
using Isopoh.Cryptography.Argon2;

namespace HelpDesk.Infrastructure.Security;

public sealed class Argon2PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return Argon2.Hash(password);
    }

    public bool Verify(
        string password,
        string passwordHash)
    {
        return Argon2.Verify(
            passwordHash,
            password);
    }
}