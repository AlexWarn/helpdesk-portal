using HelpDesk.Infrastructure.Security;

namespace HelpDesk.UnitTests.Infrastructure.Security;

public class Argon2PasswordHasherTests
{
    [Fact]
    public void Hash_ShouldReturnHashDifferentFromPassword()
    {
        // Arrange
        const string password = "Password1!";
        var hasher = new Argon2PasswordHasher();

        // Act
        var hash = hasher.Hash(password);

        // Assert
        Assert.NotEqual(password, hash);
    }

    [Fact]
    public void Hash_CalledTwiceForSamePassword_ShouldReturnDifferentHashes()
    {
        // Arrange
        const string password = "Password1!";
        var hasher = new Argon2PasswordHasher();

        // Act
        var hash1 = hasher.Hash(password);
        var hash2 = hasher.Hash(password);

        // Assert
        Assert.NotEqual(hash1, hash2);
    }

    [Fact]
    public void Verify_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        const string password = "Password1!";
        var hasher = new Argon2PasswordHasher();

        var hash = hasher.Hash(password);

        // Act
        var result = hasher.Verify(password, hash);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Verify_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        const string password = "Password1!";
        const string incorrectPassword = "WrongPassword1!";

        var hasher = new Argon2PasswordHasher();

        var hash = hasher.Hash(password);

        // Act
        var result = hasher.Verify(incorrectPassword, hash);

        // Assert
        Assert.False(result);
    }
}