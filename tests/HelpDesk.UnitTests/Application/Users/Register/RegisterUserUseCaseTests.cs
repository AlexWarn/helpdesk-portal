using HelpDesk.Application.Abstractions;
using HelpDesk.Application.Users.Register;
using HelpDesk.Domain.Entities;
using HelpDesk.Domain.Exceptions;
using HelpDesk.Domain.ValueObjects;
using Moq;

namespace HelpDesk.UnitTests.Application.Users.Register;

public class RegisterUserUseCaseTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IPasswordPolicy> _passwordPolicyMock = new();
    private readonly Mock<IClock> _clockMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

    [Fact]
    public async Task ExecuteAsync_WithValidCommand_ShouldRegisterUser()
    {
        // Arrange
        const string passwordHash = "password-hash";
        var createdAt = new DateTime(
            2026, 09, 12, 10, 20, 00,
            DateTimeKind.Utc);

        var command = CreateCommand();
        var handler = CreateHandler();
        
        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(
                It.Is<Email>(e => e.Value == command.Email),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        _passwordHasherMock.Setup(x => x.Hash(command.Password))
            .Returns(passwordHash);
        
        _clockMock.Setup(x => x.UtcNow)
            .Returns(createdAt);
        
        _userRepositoryMock.Setup(x => x.AddAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);
        
        // Act
        var result = await handler
            .ExecuteAsync(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(command.Email, result.Email);
        Assert.Equal("Warn Alex Michailovich", result.FullName);
        
        _passwordPolicyMock.Verify(
            x => x.Validate(command.Password),
            Times.Once);
        
        _passwordHasherMock.Verify(
            x => x.Hash(command.Password),
            Times.Once);
        
        _userRepositoryMock.Verify(
            x => x.ExistsByEmailAsync(
                It.Is<Email>(e => e.Value == command.Email),
                It.IsAny<CancellationToken>()),
            Times.Once);

        _userRepositoryMock.Verify(
            x => x.AddAsync(
                It.Is<User>(u =>
                    u.Email.Value == command.Email &&
                    u.PasswordHash == passwordHash &&
                    u.FullName.Value == "Warn Alex Michailovich" &&
                    u.CreatedAtUtc == createdAt),
                It.IsAny<CancellationToken>()),
            Times.Once);
        
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmailAlreadyExists_ShouldThrowDomainException()
    {
        var command = CreateCommand();
        var handler = CreateHandler();
        
        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(
                It.Is<Email>(e => e.Value == command.Email),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
         
        // Act
        var action = () => handler.ExecuteAsync(command,
            CancellationToken.None);
        
        // Assert
        await Assert.ThrowsAsync<DomainException>(action);
        
        _userRepositoryMock.Verify(
            x => x.ExistsByEmailAsync(
                It.Is<Email>(e => e.Value == command.Email),
                It.IsAny<CancellationToken>()),
            Times.Once);
        
        _passwordPolicyMock.Verify(
            x => x.Validate(It.IsAny<string>()),
            Times.Never);
        
        _passwordHasherMock.Verify(
            x => x.Hash(It.IsAny<string>()),
            Times.Never);
        
        _clockMock.Verify(
            x => x.UtcNow,
            Times.Never);
        
        _userRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
        
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenPasswordIsInvalid_ShouldThrowDomainException()
    {
        var command = CreateCommand();
        var handler = CreateHandler();
        
        _userRepositoryMock.Setup(x => x.ExistsByEmailAsync(
                It.Is<Email>(e => e.Value == command.Email),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _passwordPolicyMock.Setup(x => x.Validate(command.Password))
            .Throws(new DomainException("Invalid password."));
        
        // Act
        var action = () => handler.ExecuteAsync(command,
            CancellationToken.None);
        
        // Assert
        await Assert.ThrowsAsync<DomainException>(action);
        
        _userRepositoryMock.Verify(
            x => x.ExistsByEmailAsync(
                It.Is<Email>(e => e.Value == command.Email),
                It.IsAny<CancellationToken>()),
            Times.Once);
        
        _passwordPolicyMock.Verify(
            x => x.Validate(command.Password),
            Times.Once);
        
        _passwordHasherMock.Verify(
            x => x.Hash(It.IsAny<string>()),
            Times.Never);
        
        _clockMock.Verify(
            x => x.UtcNow,
            Times.Never);
        
        _userRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
        
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_WhenEmailIsInvalid_ShouldThrowDomainException()
    {
        var command = CreateCommand(email: string.Empty);
        var handler = CreateHandler();
        
        // Act
        var action = () => handler.ExecuteAsync(command,
            CancellationToken.None);
        
        // Assert
        await Assert.ThrowsAsync<DomainException>(action);
        
        _userRepositoryMock.Verify(
            x => x.ExistsByEmailAsync(
                It.IsAny<Email>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
        
        _passwordPolicyMock.Verify(
            x => x.Validate(It.IsAny<string>()),
            Times.Never);
        
        _passwordHasherMock.Verify(
            x => x.Hash(It.IsAny<string>()),
            Times.Never);
        
        _clockMock.Verify(
            x => x.UtcNow,
            Times.Never);
        
        _userRepositoryMock.Verify(
            x => x.AddAsync(
                It.IsAny<User>(),
                It.IsAny<CancellationToken>()),
            Times.Never);
        
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(
                It.IsAny<CancellationToken>()),
            Times.Never);
    }
    
    private RegisterUserUseCase CreateHandler()
    {
        return new RegisterUserUseCase(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _passwordPolicyMock.Object,
            _clockMock.Object,
            _unitOfWorkMock.Object);
    }

    private static RegisterUserCommand CreateCommand(
        string email = "same@test.com",
        string firstName = "Alex",
        string lastName = "Warn",
        string middleName = "Michailovich",
        string password = "password")
    {
        return new RegisterUserCommand(
            email,
            firstName,
            lastName,
            middleName,
            password);
    }
}